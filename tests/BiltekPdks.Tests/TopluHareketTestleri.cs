using BiltekPdks.Domain.Entities;
using BiltekPdks.Infrastructure.Puantaj;
using BiltekPdks.Infrastructure.Raporlar;
using BiltekPdks.Infrastructure.Security;
using BiltekPdks.Infrastructure.Seed;
using ClosedXML.Excel;

namespace BiltekPdks.Tests;

public class TopluHareketTestleri
{
    private static async Task<(TestVeritabani tv, int firmaId, int personelId)> HazirlaAsync()
    {
        var tv = new TestVeritabani();
        await new KurulumServisi(tv, new SifreServisi()).CalistirAsync("demo", "Demo", "Gizli1234");
        int firmaId;
        using (var db = tv.Sistem()) firmaId = db.Firmalar.Single().Id;
        using var d = tv.Db(firmaId);
        var p = new Personel { KartNo = "1", Ad = "Ali", Soyad = "Kaya", IseGirisTarihi = new DateOnly(2026, 1, 1), CalismaGrubuId = d.CalismaGruplari.Single().Id };
        d.Personeller.Add(p);
        d.SaveChanges();
        return (tv, firmaId, p.Id);
    }

    [Fact]
    public async Task Bir_ay_icin_calisma_gunlerine_giris_cikis_ekler_tatil_izin_ve_dolu_gunleri_atlar()
    {
        var (tv, firmaId, pid) = await HazirlaAsync();
        using (tv)
        {
            using (var d = tv.Db(firmaId))
            {
                d.ResmiTatiller.Add(new ResmiTatil { Tarih = new DateOnly(2026, 10, 29), Ad = "Cumhuriyet Bayramı" });
                d.Izinler.Add(new Izin { PersonelId = pid, IzinTuruId = d.IzinTurleri.First().Id, Baslangic = new DateOnly(2026, 10, 19), Bitis = new DateOnly(2026, 10, 20) });
                d.Hareketler.Add(new Hareket { PersonelId = pid, Zaman = new DateTime(2026, 10, 5, 7, 55, 0), Kaynak = HareketKaynagi.Terminal });
                d.SaveChanges();
            }

            var istek = new TopluHareketIstegi(pid, new DateOnly(2026, 10, 1), new DateOnly(2026, 10, 31), new TimeOnly(8, 0), new TimeOnly(17, 0), "Toplu");
            using (var d = tv.Db(firmaId))
            {
                var on = await TopluHareketServisi.EkleAsync(d, istek, 7, deneme: true);
                Assert.Equal(0, d.Hareketler.Count(h => h.Kaynak == HareketKaynagi.Elle)); // önizleme bir şey yazmaz

                var s = await TopluHareketServisi.EkleAsync(d, istek, 7);
                Assert.Equal(on, s);
                // Ekim 2026: 22 iş günü − 1 resmi tatil − 2 izin − 1 dolu = 18
                Assert.Equal(18, s.EklenenGun);
                Assert.Equal(9, s.CalismaGunuDegil);
                Assert.Equal(1, s.ResmiTatil);
                Assert.Equal(2, s.Izinli);
                Assert.Equal(1, s.HareketiOlan);
            }

            using var oku = tv.Db(firmaId);
            var eklenen = oku.Hareketler.Where(h => h.Kaynak == HareketKaynagi.Elle).ToList();
            Assert.Equal(36, eklenen.Count);
            Assert.All(eklenen, h => { Assert.Equal("Toplu", h.Aciklama); Assert.Equal(7, h.KaydedenKullaniciId); });
            Assert.Contains(eklenen, h => h.Zaman == new DateTime(2026, 10, 1, 8, 0, 0));
            Assert.Contains(eklenen, h => h.Zaman == new DateTime(2026, 10, 1, 17, 0, 0));

            // Aynı işlem tekrarlanırsa hiçbir gün iki kez eklenmez.
            var tekrar = await TopluHareketServisi.EkleAsync(oku, istek, 7);
            Assert.Equal(0, tekrar.EklenenGun);

            // Puantaj artık bu günleri "çalıştı" sayar.
            var saat = new SabitSaat { Simdi = new DateTimeOffset(2026, 11, 2, 12, 0, 0, TimeSpan.Zero) };
            var ay = await new PuantajServisi(saat).AylikAsync(oku, pid, 2026, 10);
            Assert.Equal(18, ay!.Toplam.CalisilanGun); // 5 Ekim tek okutma: eksik hareket
            Assert.Equal(1, ay.Toplam.EksikHareketGun);
        }
    }

    [Fact]
    public async Task Gece_vardiyasinda_cikis_ertesi_gune_yazilir_ve_hatali_istek_reddedilir()
    {
        var (tv, firmaId, pid) = await HazirlaAsync();
        using (tv)
        {
            using var d = tv.Db(firmaId);
            var s = await TopluHareketServisi.EkleAsync(d,
                new TopluHareketIstegi(pid, new DateOnly(2026, 10, 3), new DateOnly(2026, 10, 4), new TimeOnly(22, 0), new TimeOnly(6, 0), "Gece",
                    YalnizCalismaGunleri: false), null);
            Assert.Equal(2, s.EklenenGun);
            Assert.Contains(d.Hareketler, h => h.Zaman == new DateTime(2026, 10, 5, 6, 0, 0));

            await Assert.ThrowsAsync<ArgumentException>(() => TopluHareketServisi.EkleAsync(d,
                new TopluHareketIstegi(pid, new DateOnly(2026, 10, 5), new DateOnly(2026, 10, 1), new TimeOnly(8, 0), new TimeOnly(17, 0), "x"), null));
            await Assert.ThrowsAsync<ArgumentException>(() => TopluHareketServisi.EkleAsync(d,
                new TopluHareketIstegi(pid, new DateOnly(2026, 10, 1), new DateOnly(2026, 10, 2), new TimeOnly(8, 0), new TimeOnly(17, 0), " "), null));
            await Assert.ThrowsAsync<ArgumentException>(() => TopluHareketServisi.EkleAsync(d,
                new TopluHareketIstegi(pid, new DateOnly(2025, 1, 1), new DateOnly(2026, 12, 31), new TimeOnly(8, 0), new TimeOnly(17, 0), "x"), null));
        }
    }

    [Fact]
    public async Task Kisisel_puantaj_excel_raporu_olusur()
    {
        var (tv, firmaId, pid) = await HazirlaAsync();
        using (tv)
        {
            using var d = tv.Db(firmaId);
            await TopluHareketServisi.EkleAsync(d, new TopluHareketIstegi(pid, new DateOnly(2026, 10, 1), new DateOnly(2026, 10, 2), new TimeOnly(8, 0), new TimeOnly(17, 0), "x"), null);
            var saat = new SabitSaat { Simdi = new DateTimeOffset(2026, 11, 2, 12, 0, 0, TimeSpan.Zero) };
            var ay = await new PuantajServisi(saat).AylikAsync(d, pid, 2026, 10);
            using var kitap = new XLWorkbook(new MemoryStream(KisiselPuantajExcel.Olustur(ay!, "Demo")));
            var s = kitap.Worksheet(1);
            Assert.Equal("Demo — Ekim 2026 kişisel puantaj", s.Cell(1, 1).GetString());
            Assert.Equal("Ali Kaya", s.Cell(2, 1).GetString());
            Assert.Equal("08:00", s.Cell(6, 4).GetString());
            Assert.Equal("Çalıştı", s.Cell(6, 3).GetString());
        }
    }
}
