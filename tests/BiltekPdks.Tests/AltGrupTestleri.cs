using BiltekPdks.Domain.Entities;
using BiltekPdks.Domain.Puantaj;
using BiltekPdks.Infrastructure.Puantaj;
using BiltekPdks.Infrastructure.Security;
using BiltekPdks.Infrastructure.Seed;

namespace BiltekPdks.Tests;

public class AltGrupTestleri
{
    private static DateTime Z(int gun, int saat, int dakika) => new(2026, 10, gun, saat, dakika, 0);

    private static readonly Dictionary<int, Vardiya> Vardiyalar = new()
    {
        [1] = new Vardiya { Id = 1, Ad = "08-16", Baslangic = new TimeOnly(8, 0), Bitis = new TimeOnly(16, 0), MolaDakika = 30 },
        [2] = new Vardiya { Id = 2, Ad = "16-24", Baslangic = new TimeOnly(16, 0), Bitis = new TimeOnly(0, 0), MolaDakika = 30 },
        [3] = new Vardiya { Id = 3, Ad = "24-08", Baslangic = new TimeOnly(0, 0), Bitis = new TimeOnly(8, 0), MolaDakika = 30 },
    };

    private static IReadOnlyDictionary<DayOfWeek, int?> HerGun(int vardiyaId) =>
        Enum.GetValues<DayOfWeek>().ToDictionary(g => g, _ => (int?)vardiyaId);

    [Fact]
    public void Alt_grup_sabit_degilse_okutmaya_gore_secilir()
    {
        var personel = new Personel { Id = 1, IseGirisTarihi = new DateOnly(2026, 1, 1) };
        var altPlanlar = new List<IReadOnlyDictionary<DayOfWeek, int?>> { HerGun(1), HerGun(2), HerGun(3) };
        var hareketler = new[]
        {
            Z(5, 7, 58), Z(5, 16, 2),     // 5 Ekim: 08-16
            Z(6, 15, 55), Z(7, 0, 3),     // 6 Ekim: 16-24 (çıkış gece yarısından sonra)
                                          // 7 Ekim: gelmedi
            Z(7, 23, 57), Z(8, 8, 1),     // 8 Ekim: 24-08 (giriş bir önceki gün 23:57)
        };

        var gunler = PuantajServisi.Hesapla(personel, new DateOnly(2026, 10, 5), new DateOnly(2026, 10, 8), Vardiyalar,
            new Dictionary<DayOfWeek, int?>(), new Dictionary<DateOnly, int?>(), new HashSet<DateOnly>(), Array.Empty<Izin>(),
            hareketler, null, altPlanlar);

        Assert.Equal(PuantajDurumu.Calisti, gunler[0].Durum);
        Assert.Equal(Z(5, 7, 58), gunler[0].Giris);
        Assert.Equal(PuantajDurumu.Calisti, gunler[1].Durum);
        Assert.Equal((Z(6, 15, 55), Z(7, 0, 3)), (gunler[1].Giris, gunler[1].Cikis));
        Assert.Equal(PuantajDurumu.Devamsiz, gunler[2].Durum);
        Assert.Equal(PuantajDurumu.Calisti, gunler[3].Durum);
        Assert.Equal((Z(7, 23, 57), Z(8, 8, 1)), (gunler[3].Giris, gunler[3].Cikis));
        Assert.All(new[] { gunler[0], gunler[1], gunler[3] }, g => Assert.Equal(0, g.GecKalmaDakika));
    }

    [Fact]
    public async Task Sabit_alt_grubun_plani_kullanilir()
    {
        using var tv = new TestVeritabani();
        await new KurulumServisi(tv, new SifreServisi()).CalistirAsync("demo", "Demo", "Gizli1234");
        int firmaId, personelId;
        using (var db = tv.Sistem()) firmaId = db.Firmalar.Single().Id;

        using (var db = tv.Db(firmaId))
        {
            var grup = db.CalismaGruplari.First();
            var aksam = new Vardiya { Ad = "16-24", Baslangic = new TimeOnly(16, 0), Bitis = new TimeOnly(0, 0), MolaDakika = 30 };
            db.Vardiyalar.Add(aksam);
            var alt = new AltGrup { CalismaGrubuId = grup.Id, Ad = "16-24", GunlukCalismaDakika = 450 };
            db.AltGruplar.Add(alt);
            db.SaveChanges();
            foreach (var g in Enum.GetValues<DayOfWeek>())
                db.AltGrupGunleri.Add(new AltGrupGunu { AltGrupId = alt.Id, Gun = g, VardiyaId = g == DayOfWeek.Sunday ? null : aksam.Id });
            var p = new Personel { KartNo = "1", Ad = "Ali", Soyad = "Gece", IseGirisTarihi = new DateOnly(2026, 1, 1), CalismaGrubuId = grup.Id, AltGrupId = alt.Id };
            db.Personeller.Add(p);
            db.SaveChanges();
            personelId = p.Id;
            db.Hareketler.AddRange(new Hareket { PersonelId = p.Id, Zaman = Z(5, 16, 10) }, new Hareket { PersonelId = p.Id, Zaman = Z(6, 0, 0) });
            db.SaveChanges();
        }

        using var oku = tv.Db(firmaId);
        var saat = new SabitSaat { Simdi = new DateTimeOffset(2026, 10, 31, 12, 0, 0, TimeSpan.Zero) };
        var ay = await new PuantajServisi(saat).AylikAsync(oku, personelId, 2026, 10);
        var bes = ay!.Gunler[4];
        Assert.Equal(PuantajDurumu.Calisti, bes.Durum);
        Assert.Equal(10, bes.GecKalmaDakika);                         // 16-24 planına göre 10 dk geç
        Assert.Equal(PuantajDurumu.HaftaTatili, ay.Gunler[3].Durum);  // 4 Ekim pazar: alt grup planında boş
    }

    [Fact]
    public void Donem_gun_hesabi_30_gune_gore_eksik_ve_fazlayi_bulur()
    {
        var subat = new DonemTanimi { Baslangic = new DateOnly(2026, 2, 1), Bitis = new DateOnly(2026, 2, 28) };
        subat.GunHesapla(450);
        Assert.Equal((28, 2, 900, 0, 0), (subat.GunFarki, subat.EksikGun, subat.EksikDakika, subat.FazlaGun, subat.FazlaDakika));

        var ozel = new DonemTanimi { Baslangic = new DateOnly(2026, 9, 26), Bitis = new DateOnly(2026, 10, 25) };
        ozel.GunHesapla(450);
        Assert.Equal((30, 0, 0), (ozel.GunFarki, ozel.EksikGun, ozel.FazlaGun));

        var ocak = new DonemTanimi { Baslangic = new DateOnly(2026, 1, 1), Bitis = new DateOnly(2026, 1, 31) };
        ocak.GunHesapla(480);
        Assert.Equal((31, 1, 480), (ocak.GunFarki, ocak.FazlaGun, ocak.FazlaDakika));
    }
}
