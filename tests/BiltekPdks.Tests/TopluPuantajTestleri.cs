using BiltekPdks.Domain.Entities;
using BiltekPdks.Domain.Puantaj;
using BiltekPdks.Infrastructure.Puantaj;
using BiltekPdks.Infrastructure.Raporlar;
using BiltekPdks.Infrastructure.Security;
using BiltekPdks.Infrastructure.Seed;
using ClosedXML.Excel;

namespace BiltekPdks.Tests;

public class TopluPuantajTestleri
{
    private static readonly DateOnly Ocak1 = new(2020, 1, 1);

    [Theory]
    [InlineData(2026, 2, 0, 30)]   // şubat 28 gün çeker ama 30 sayılır
    [InlineData(2028, 2, 0, 30)]   // artık yıl şubatı
    [InlineData(2026, 10, 0, 30)]  // 31 çeken ay
    [InlineData(2026, 4, 0, 30)]   // 30 çeken ay
    [InlineData(2026, 10, 2, 28)]  // 2 eksik gün düşülür
    [InlineData(2026, 2, 3, 27)]
    public void Ayin_tamaminda_calisan_her_ay_30_gun_alir(int yil, int ay, int eksik, int beklenen) =>
        Assert.Equal(beklenen, AylikPuantaj.OtuzGunHesabi(Ocak1, null, yil, ay, eksik));

    [Fact]
    public void Ay_icinde_giren_ve_cikan_fiili_gun_alir()
    {
        Assert.Equal(17, AylikPuantaj.OtuzGunHesabi(new DateOnly(2026, 10, 15), null, 2026, 10, 0)); // 15–31 Ekim
        Assert.Equal(30, AylikPuantaj.OtuzGunHesabi(new DateOnly(2026, 10, 2), null, 2026, 10, 0));  // 2–31 Ekim = 30
        Assert.Equal(14, AylikPuantaj.OtuzGunHesabi(new DateOnly(2026, 2, 15), null, 2026, 2, 0));   // 15–28 Şubat
        Assert.Equal(10, AylikPuantaj.OtuzGunHesabi(Ocak1, new DateOnly(2026, 10, 10), 2026, 10, 0)); // 1–10 Ekim
        Assert.Equal(8, AylikPuantaj.OtuzGunHesabi(Ocak1, new DateOnly(2026, 10, 10), 2026, 10, 2));
        Assert.Equal(0, AylikPuantaj.OtuzGunHesabi(new DateOnly(2026, 11, 1), null, 2026, 10, 0));    // henüz girmemiş
        Assert.Equal(0, AylikPuantaj.OtuzGunHesabi(Ocak1, new DateOnly(2026, 9, 30), 2026, 10, 0));    // ayrılmış
        Assert.Equal(0, AylikPuantaj.OtuzGunHesabi(Ocak1, null, 2026, 10, 40));                        // eksiye inmez
    }

    [Fact]
    public async Task Toplu_puantaj_tum_personeli_ve_filtreleri_hesaplar()
    {
        using var tv = new TestVeritabani();
        await new KurulumServisi(tv, new SifreServisi()).CalistirAsync("demo", "Demo", "Gizli1234");
        int firmaId;
        using (var db = tv.Sistem()) firmaId = db.Firmalar.Single().Id;

        int uretimId;
        using (var db = tv.Db(firmaId))
        {
            var grup = db.CalismaGruplari.Single();
            var uretim = new Bolum { Kod = "U", Ad = "Üretim" };
            var idari = new Bolum { Kod = "I", Ad = "İdari" };
            db.AddRange(uretim, idari);
            db.SaveChanges();
            uretimId = uretim.Id;

            var ali = new Personel { KartNo = "1", Ad = "Ali", Soyad = "Kaya", IseGirisTarihi = Ocak1, CalismaGrubuId = grup.Id, BolumId = uretim.Id };
            var ayse = new Personel { KartNo = "2", Ad = "Ayşe", Soyad = "Demir", IseGirisTarihi = Ocak1, CalismaGrubuId = grup.Id, BolumId = idari.Id };
            var ayrilan = new Personel { KartNo = "3", Ad = "Veli", Soyad = "Eski", IseGirisTarihi = Ocak1, IstenCikisTarihi = new DateOnly(2026, 9, 30), CalismaGrubuId = grup.Id };
            db.Personeller.AddRange(ali, ayse, ayrilan);
            db.SaveChanges();

            // Ali Ekim'de her iş günü 08:00–17:00 çalıştı; Ayşe hiç gelmedi.
            for (var t = new DateOnly(2026, 10, 1); t.Month == 10; t = t.AddDays(1))
            {
                if (t.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday) continue;
                db.Hareketler.Add(new Hareket { PersonelId = ali.Id, Zaman = t.ToDateTime(new TimeOnly(8, 0)) });
                db.Hareketler.Add(new Hareket { PersonelId = ali.Id, Zaman = t.ToDateTime(new TimeOnly(17, 0)) });
            }
            db.SaveChanges();
        }

        var saat = new SabitSaat { Simdi = new DateTimeOffset(2026, 11, 5, 12, 0, 0, TimeSpan.Zero) };
        var servis = new PuantajServisi(saat);
        using var oku = tv.Db(firmaId);

        var hepsi = await servis.TopluAsync(oku, 2026, 10);
        Assert.Equal(new[] { "Ali", "Ayşe" }, hepsi.Select(x => x.Personel.Ad)); // ayrılan listelenmez, ada göre sıralı

        var ali2 = hepsi[0];
        Assert.Equal(22, ali2.Toplam.CalisilanGun);
        Assert.Equal(0, ali2.Toplam.DevamsizGun);
        Assert.Equal(30, ali2.OdenecekGun);
        Assert.Equal("Üretim", ali2.Personel.Bolum?.Ad);

        var ayse2 = hepsi[1];
        Assert.Equal(22, ayse2.Toplam.DevamsizGun);
        Assert.Equal(8, ayse2.OdenecekGun); // 30 − 22
        Assert.Equal("D", PuantajKodlari.Kod(ayse2.Gunler[0]));
        Assert.Equal("HT", PuantajKodlari.Kod(ayse2.Gunler[3])); // 4 Ekim Pazar

        var uretim2 = await servis.TopluAsync(oku, 2026, 10, bolumId: uretimId);
        Assert.Equal("Ali", Assert.Single(uretim2).Personel.Ad);

        // Tekil puantaj, toplu hesapla aynı sonucu verir.
        var tekil = await servis.AylikAsync(oku, ali2.Personel.Id, 2026, 10);
        Assert.Equal(ali2.Toplam, tekil!.Toplam);

        var dosya = TopluPuantajExcel.Olustur(hepsi, 2026, 10, "Demo");
        using var kitap = new XLWorkbook(new MemoryStream(dosya));
        var s = kitap.Worksheet(1);
        Assert.Equal("Demo — Ekim 2026 toplu puantaj", s.Cell(1, 1).GetString());
        Assert.Equal("Ali Kaya", s.Cell(4, 3).GetString());
        Assert.Equal("Gün (30)", s.Cell(3, 4 + 31 + 1).GetString());
        Assert.Equal(30, s.Cell(4, 4 + 31 + 1).GetValue<int>());
        Assert.Equal(8, s.Cell(5, 4 + 31 + 1).GetValue<int>());
        Assert.Equal("TOPLAM", s.Cell(6, 3).GetString());
    }
}
