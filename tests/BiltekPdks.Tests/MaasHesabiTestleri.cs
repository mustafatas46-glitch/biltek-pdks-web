using BiltekPdks.Domain.Entities;
using BiltekPdks.Infrastructure.Puantaj;
using BiltekPdks.Infrastructure.Raporlar;
using BiltekPdks.Infrastructure.Security;
using BiltekPdks.Infrastructure.Seed;

namespace BiltekPdks.Tests;

public class MaasHesabiTestleri
{
    private static DateTime Z(int gun, int saat, int dakika) => new(2026, 10, gun, saat, dakika, 0);

    [Fact]
    public async Task Maas_pusulasi_puantaj_ek_ucret_kazanc_kesinti_ve_cezadan_hesaplanir()
    {
        using var tv = new TestVeritabani();
        await new KurulumServisi(tv, new SifreServisi()).CalistirAsync("demo", "Demo", "Gizli1234");
        int firmaId;
        using (var db = tv.Sistem()) firmaId = db.Firmalar.Single().Id;

        using (var db = tv.Db(firmaId))
        {
            var grup = db.CalismaGruplari.Single();
            var p = new Personel
            {
                KartNo = "1", Ad = "Ali", Soyad = "Usta", IseGirisTarihi = new DateOnly(2026, 10, 1), CalismaGrubuId = grup.Id,
                Ucret = 30000m, UcretTipi = UcretTipi.Maas, PrimUcreti = 100m, PrimGunluk = true,
            };
            db.Personeller.Add(p);
            db.SaveChanges();
            db.Hareketler.AddRange(
                new Hareket { PersonelId = p.Id, Zaman = Z(1, 8, 0) }, new Hareket { PersonelId = p.Id, Zaman = Z(1, 17, 0) },
                new Hareket { PersonelId = p.Id, Zaman = Z(2, 8, 30) }, new Hareket { PersonelId = p.Id, Zaman = Z(2, 17, 0) }, // 30 dk geç
                new Hareket { PersonelId = p.Id, Zaman = Z(5, 8, 0) }, new Hareket { PersonelId = p.Id, Zaman = Z(5, 20, 0) }); // 3 saat FM
            db.KazancKesintiler.Add(new KazancKesinti { PersonelId = p.Id, KazancKesintiTuruId = db.KazancKesintiTurleri.Single(t => t.Kod == "PRIM").Id, Tarih = new DateOnly(2026, 10, 10), Tutar = 500m });
            db.KazancKesintiler.Add(new KazancKesinti { PersonelId = p.Id, KazancKesintiTuruId = db.KazancKesintiTurleri.Single(t => t.Kod == "AVANS").Id, Tarih = new DateOnly(2026, 10, 15), Tutar = 2000m });
            db.SaveChanges();
        }

        using var oku = tv.Db(firmaId);
        var saat = new SabitSaat { Simdi = new DateTimeOffset(2026, 10, 5, 21, 0, 0, TimeSpan.Zero) };
        var pusula = Assert.Single(await MaasHesabi.HesaplaAsync(oku, new PuantajServisi(saat), 2026, 10));

        decimal Kazanc(string ad) => pusula.Kazanclar.Single(k => k.Ad == ad).Tutar;
        Assert.Equal(30000m, Kazanc("Normal çalışma"));      // 30 gün × 1.000
        Assert.Equal(600m, Kazanc("Fazla mesai %50"));       // 3 saat × 133,33 × 1,5
        Assert.Equal(300m, Kazanc("Prim (sicil)"));                  // 3 gün × 100 (sicil kartı, günlük) — ayrıca 500 prim kalemi
        Assert.Equal(500m, Kazanc("Prim"));                  // ay içindeki prim kalemi
        Assert.Equal(31400m, pusula.ToplamKazanc);
        Assert.Equal(66.67m, pusula.Kesintiler.Single(k => k.Ad.Contains("cezası")).Tutar); // 30 dk geç
        Assert.Equal(2000m, pusula.Kesintiler.Single(k => k.Ad == "Avans").Tutar);
        Assert.Equal(29333.33m, pusula.NetOdenecek);

        Assert.NotEmpty(MaasListesiExcel.Olustur(new[] { pusula }, 2026, 10, "Demo"));
    }
}
