using BiltekPdks.Domain.Entities;
using BiltekPdks.Infrastructure.Firmalar;
using BiltekPdks.Infrastructure.Security;
using BiltekPdks.Infrastructure.Seed;

namespace BiltekPdks.Tests;

public class KullaniciTestleri
{
    [Fact]
    public async Task Ozel_yetki_girisle_gelir_son_yonetici_korunur()
    {
        using var tv = new TestVeritabani();
        var sifre = new SifreServisi();
        await new KurulumServisi(tv, sifre).CalistirAsync("demo", "Demo", "Gizli1234");
        int firmaId, adminId;
        using (var db = tv.Sistem())
        {
            firmaId = db.Firmalar.Single().Id;
            adminId = db.Kullanicilar.Single().Id;
        }

        using (var db = tv.Db(firmaId))
        {
            var b = new KullaniciBilgisi
            {
                KullaniciAdi = "Ayse.IK", AdSoyad = "Ayşe", Rol = KullaniciRolu.InsanKaynaklari, OzelYetki = true,
                Yetkiler = new() { Yetkiler.PuantajGor, Yetkiler.IzinDuzenle }, YeniSifre = "Sifre12345",
            };
            Assert.Null(await KullaniciServisi.KaydetAsync(db, b, sifre, adminId));
            Assert.NotNull(await KullaniciServisi.KaydetAsync(db, new KullaniciBilgisi { KullaniciAdi = "ayse.ik", AdSoyad = "x", YeniSifre = "Sifre12345" }, sifre, adminId)); // aynı ad
            Assert.NotNull(await KullaniciServisi.KaydetAsync(db, new KullaniciBilgisi { KullaniciAdi = "şükrü", AdSoyad = "x", YeniSifre = "Sifre12345" }, sifre, adminId)); // Türkçe karakter
        }

        var giris = new GirisServisi(new TekVeritabani(tv), sifre, TimeProvider.System);
        var s = await giris.DogrulaAsync(null, "ayse.ik", "Sifre12345");
        Assert.True(s.Basarili);
        Assert.Equal(new[] { Yetkiler.IzinDuzenle, Yetkiler.PuantajGor }, s.Yetkiler!.OrderBy(x => x).ToArray());

        var admin = await giris.DogrulaAsync(null, "admin", "Gizli1234");
        Assert.Contains(Yetkiler.KullaniciYonetimi, admin.Yetkiler!);

        // Tek yönetici kendini pasif yapamaz / rolünü düşüremez.
        using (var db = tv.Db(firmaId))
        {
            var liste = await KullaniciServisi.ListeleAsync(db);
            var a = liste.Single(x => x.KullaniciAdi == "admin");
            a.Rol = KullaniciRolu.InsanKaynaklari;
            Assert.NotNull(await KullaniciServisi.KaydetAsync(db, a, sifre, adminId));
            a.Rol = KullaniciRolu.Yonetici;
            a.Aktif = false;
            Assert.NotNull(await KullaniciServisi.KaydetAsync(db, a, sifre, 999));
        }
    }
}

public class BolumKisitiTestleri
{
    [Fact]
    public async Task Bolum_kisitli_kullanici_yalniz_kendi_bolumunu_gorur_ve_baska_bolume_kaydedemez()
    {
        using var tv = new TestVeritabani();
        await new KurulumServisi(tv, new SifreServisi()).CalistirAsync("demo", "Demo", "Gizli1234");
        int firmaId, uretim, depo;
        using (var db = tv.Sistem()) firmaId = db.Firmalar.Single().Id;
        using (var db = tv.Db(firmaId))
        {
            var u = new Bolum { Ad = "Üretim" };
            var d = new Bolum { Ad = "Depo" };
            db.Bolumler.AddRange(u, d);
            db.SaveChanges();
            (uretim, depo) = (u.Id, d.Id);
            var a = new Personel { KartNo = "1", Ad = "Ali", Soyad = "U", IseGirisTarihi = new DateOnly(2026, 1, 1), BolumId = uretim };
            var b = new Personel { KartNo = "2", Ad = "Veli", Soyad = "D", IseGirisTarihi = new DateOnly(2026, 1, 1), BolumId = depo };
            db.Personeller.AddRange(a, b);
            db.SaveChanges();
            db.Hareketler.AddRange(new Hareket { PersonelId = a.Id, Zaman = new DateTime(2026, 10, 1, 8, 0, 0) },
                new Hareket { PersonelId = b.Id, Zaman = new DateTime(2026, 10, 1, 8, 0, 0) });
            db.SaveChanges();
        }

        using var kisitli = tv.Db(firmaId);
        kisitli.IzinliBolumler = new List<int> { uretim };
        Assert.Equal(new[] { "Ali" }, kisitli.Personeller.Select(p => p.Ad).ToArray());
        Assert.Single(kisitli.Hareketler.ToList());

        kisitli.Personeller.Add(new Personel { KartNo = "3", Ad = "Yeni", Soyad = "D", IseGirisTarihi = new DateOnly(2026, 1, 1), BolumId = depo });
        Assert.Throws<BiltekPdks.Infrastructure.Data.FirmaIzolasyonHatasi>(() => kisitli.SaveChanges());

        using var tam = tv.Db(firmaId);
        Assert.Equal(2, tam.Personeller.Count());
    }
}
