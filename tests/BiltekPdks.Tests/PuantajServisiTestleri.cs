using BiltekPdks.Domain.Entities;
using BiltekPdks.Domain.Puantaj;
using BiltekPdks.Infrastructure.Puantaj;
using BiltekPdks.Infrastructure.Security;
using BiltekPdks.Infrastructure.Seed;

namespace BiltekPdks.Tests;

public class PuantajServisiTestleri
{
    private static DateTime S(int gun, int saat, int dakika) => new(2026, 10, gun, saat, dakika, 0);

    [Fact]
    public async Task Ekim_ayi_varsayilan_planla_dogru_ozetlenir()
    {
        using var tv = new TestVeritabani();
        await new KurulumServisi(tv, new SifreServisi()).CalistirAsync("demo", "Demo", "Gizli1234");

        int firmaId, personelId;
        using (var db = tv.Sistem())
        {
            firmaId = db.Firmalar.Single().Id;
        }

        using (var db = tv.Db(firmaId))
        {
            var grup = db.CalismaGruplari.Single();
            var p = new Personel { KartNo = "1", Ad = "Ayşe", Soyad = "Yılmaz", IseGirisTarihi = new DateOnly(2026, 10, 5), CalismaGrubuId = grup.Id };
            db.Personeller.Add(p);
            db.SaveChanges();
            personelId = p.Id;

            // 5 Ekim Pzt: tam gün; 6 Ekim Sal: 20 dk geç; 7 Ekim Çar: gelmedi; 8 Ekim Per: resmi tatil (varsayım);
            // 9 Ekim Cum: ücretsiz izin; 10 Ekim Cmt: 4 saat çalıştı (tatil mesaisi); 12 Ekim Pzt: 2 saat fazla mesai.
            db.Hareketler.AddRange(
                new Hareket { PersonelId = p.Id, Zaman = S(5, 7, 58) }, new Hareket { PersonelId = p.Id, Zaman = S(5, 17, 2) },
                new Hareket { PersonelId = p.Id, Zaman = S(6, 8, 20) }, new Hareket { PersonelId = p.Id, Zaman = S(6, 17, 0) },
                new Hareket { PersonelId = p.Id, Zaman = S(10, 9, 0) }, new Hareket { PersonelId = p.Id, Zaman = S(10, 13, 0) },
                new Hareket { PersonelId = p.Id, Zaman = S(12, 8, 0) }, new Hareket { PersonelId = p.Id, Zaman = S(12, 19, 0) },
                new Hareket { PersonelId = p.Id, Zaman = S(13, 8, 0), Iptal = true });
            db.ResmiTatiller.Add(new ResmiTatil { Tarih = new DateOnly(2026, 10, 8), Ad = "Test tatili" });
            var ucretsiz = db.IzinTurleri.Single(i => i.Kod == "UI");
            db.Izinler.Add(new Izin { PersonelId = p.Id, IzinTuruId = ucretsiz.Id, Baslangic = new DateOnly(2026, 10, 9), Bitis = new DateOnly(2026, 10, 9) });
            db.SaveChanges();
        }

        using var oku = tv.Db(firmaId);
        var saat = new SabitSaat { Simdi = new DateTimeOffset(2026, 10, 31, 12, 0, 0, TimeSpan.Zero) };
        var ay = await new PuantajServisi(saat).AylikAsync(oku, personelId, 2026, 10);
        Assert.NotNull(ay);
        Assert.Equal(31, ay!.Gunler.Count);

        PuantajDurumu Durum(int gun) => ay.Gunler[gun - 1].Durum;
        Assert.Equal(PuantajDurumu.KapsamDisi, Durum(1));
        Assert.Equal(PuantajDurumu.Calisti, Durum(5));
        Assert.Equal(20, ay.Gunler[5].GecKalmaDakika);
        Assert.Equal(PuantajDurumu.Devamsiz, Durum(7));
        Assert.Equal(PuantajDurumu.ResmiTatil, Durum(8));
        Assert.Equal(PuantajDurumu.Izinli, Durum(9));
        Assert.Equal(PuantajDurumu.HaftaTatili, Durum(10));
        Assert.Equal(225, ay.Gunler[9].TatilMesaisiDakika); // 240 - 15
        Assert.Equal(120, ay.Gunler[11].FazlaMesaiDakika);
        // 13 Ekim'deki hareket iptal edildiği için o gün devamsız.
        Assert.Equal(PuantajDurumu.Devamsiz, Durum(13));

        Assert.Equal(1, ay.Toplam.UcretsizIzinGun);
        Assert.Equal(120, ay.Toplam.FazlaMesaiDakika);
        Assert.Equal(225, ay.Toplam.TatilMesaisiDakika);
    }

    [Fact]
    public void Bugun_okutma_yoksa_devamsiz_yazilmaz_ama_dun_yazilir()
    {
        var gunduz = new Vardiya { Id = 1, Baslangic = new TimeOnly(8, 0), Bitis = new TimeOnly(17, 0) };
        var personel = new Personel { Id = 1, IseGirisTarihi = new DateOnly(2026, 1, 1) };
        var bugun = new DateOnly(2026, 10, 13);
        var gunler = PuantajServisi.Hesapla(personel, bugun.AddDays(-1), bugun,
            new Dictionary<int, Vardiya> { [1] = gunduz },
            Enum.GetValues<DayOfWeek>().ToDictionary(g => g, _ => (int?)1),
            new Dictionary<DateOnly, int?>(), new HashSet<DateOnly>(), Array.Empty<Izin>(), Array.Empty<DateTime>(), bugun);
        Assert.Equal(PuantajDurumu.Devamsiz, gunler[0].Durum);
        Assert.Equal(PuantajDurumu.KapsamDisi, gunler[1].Durum);
    }

    [Fact]
    public void Ay_basina_sarkan_gece_vardiyasi_cikisi_1ine_yazilmaz()
    {
        var gece = new Vardiya { Id = 1, Baslangic = new TimeOnly(22, 0), Bitis = new TimeOnly(6, 0), MolaDakika = 30 };
        var gunduz = new Vardiya { Id = 2, Baslangic = new TimeOnly(8, 0), Bitis = new TimeOnly(17, 0), MolaDakika = 60 };
        var personel = new Personel { Id = 1, IseGirisTarihi = new DateOnly(2026, 1, 1) };
        var ilk = new DateOnly(2026, 10, 1);

        var gunler = PuantajServisi.Hesapla(
            personel, ilk, ilk,
            new Dictionary<int, Vardiya> { [1] = gece, [2] = gunduz },
            new Dictionary<DayOfWeek, int?>(),
            new Dictionary<DateOnly, int?> { [ilk.AddDays(-1)] = 1, [ilk] = 2 },
            new HashSet<DateOnly>(),
            Array.Empty<Izin>(),
            new[] { new DateTime(2026, 9, 30, 21, 58, 0), new DateTime(2026, 10, 1, 6, 5, 0), new DateTime(2026, 10, 1, 7, 59, 0), new DateTime(2026, 10, 1, 17, 1, 0) });

        var bir = Assert.Single(gunler);
        Assert.Equal(PuantajDurumu.Calisti, bir.Durum);
        Assert.Equal(new DateTime(2026, 10, 1, 7, 59, 0), bir.Giris);
        Assert.Equal(0, bir.EksikDakika);
    }
}
