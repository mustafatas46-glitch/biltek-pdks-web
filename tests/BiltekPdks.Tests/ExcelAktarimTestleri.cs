using BiltekPdks.Domain.Entities;
using BiltekPdks.Infrastructure.Aktarim;
using BiltekPdks.Infrastructure.Puantaj;
using BiltekPdks.Infrastructure.Raporlar;
using BiltekPdks.Infrastructure.Security;
using BiltekPdks.Infrastructure.Seed;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;

namespace BiltekPdks.Tests;

public class ExcelAktarimTestleri
{
    /// <summary>İlk satır başlık, sonrakiler veri olan bir Excel dosyası üretir.</summary>
    private static ExcelTablo Tablo(params object?[][] satirlar)
    {
        using var kitap = new XLWorkbook();
        var s = kitap.Worksheets.Add("Sayfa1");
        for (var r = 0; r < satirlar.Length; r++)
            for (var c = 0; c < satirlar[r].Length; c++)
            {
                var hucre = s.Cell(r + 1, c + 1);
                switch (satirlar[r][c])
                {
                    case null: break;
                    case DateTime d: hucre.Value = d; break;
                    case double x: hucre.Value = x; break;
                    case int i: hucre.Value = i; break;
                    case TimeSpan t: hucre.Value = t; break;
                    default: hucre.Value = satirlar[r][c]!.ToString(); break;
                }
            }
        using var ms = new MemoryStream();
        kitap.SaveAs(ms);
        ms.Position = 0;
        return ExcelTablo.Oku(ms);
    }

    private static async Task<(TestVeritabani Tv, int FirmaId)> KurAsync()
    {
        var tv = new TestVeritabani();
        await new KurulumServisi(tv, new SifreServisi()).CalistirAsync("demo", "Demo", "Gizli1234");
        using var db = tv.Sistem();
        return (tv, db.Firmalar.Single().Id);
    }

    [Fact]
    public void Turkce_tarih_saat_ve_sayi_bicimleri_cozulur()
    {
        Assert.Equal(new DateOnly(2026, 10, 5), DegerCozucu.Tarih(new Hucre("05.10.2026")));
        Assert.Equal(new DateOnly(2026, 10, 5), DegerCozucu.Tarih(new Hucre("5/10/2026")));
        Assert.Equal(new TimeOnly(8, 5), DegerCozucu.Saat(new Hucre("08:05")));
        Assert.Equal(new TimeOnly(17, 30), DegerCozucu.Saat(new Hucre("", Sayi: 17.5 / 24)));
        Assert.Equal(1234.56m, DegerCozucu.Sayi(new Hucre("1.234,56")));
        Assert.Equal(1234.56m, DegerCozucu.Sayi(new Hucre("1234.56")));
        Assert.Equal(17002m, DegerCozucu.Sayi(new Hucre("17.002 TL")));
        Assert.Equal(new DateOnly(2026, 9, 1), DegerCozucu.AyBasi(new Hucre("09.2026")));
        Assert.Equal("1234", DegerCozucu.Kod(new Hucre("1234", Sayi: 1234)));
    }

    [Fact]
    public void Basliklar_otomatik_eslesir()
    {
        var t = Tablo(new object?[] { "Sicil No", "Adı Soyadı", "Tarih", "Giriş", "Çıkış" });
        var e = ExcelAktarimi.OtomatikEslestir(AktarimTuru.Hareket, t.Basliklar);
        Assert.Equal("Sicil No", e["SicilNo"]);
        Assert.Equal("Adı Soyadı", e["AdSoyad"]);
        Assert.Equal("Giriş", e["Giris"]);
        Assert.Equal("Çıkış", e["Cikis"]);
        Assert.Empty(ExcelAktarimi.EslesmeHatalari(AktarimTuru.Hareket, t, e));
    }

    [Fact]
    public async Task Personel_aktarimi_onizler_ekler_gunceller_ve_tanim_acar()
    {
        var (tv, firmaId) = await KurAsync();
        using var _ = tv;
        using (var d = tv.Db(firmaId))
        {
            d.Personeller.Add(new Personel { KartNo = "0100", Ad = "Ali", Soyad = "Eski", IseGirisTarihi = new DateOnly(2025, 1, 1) });
            d.SaveChanges();
        }

        var t = Tablo(
            new object?[] { "Kart No", "Adı", "Soyadı", "İşe Giriş Tarihi", "Bölüm", "TC" },
            new object?[] { 100, "Ali", "Yeni", "01.01.2025", "Üretim", null },
            new object?[] { "200", "Veli", "Kaya", new DateTime(2026, 3, 1), "Üretim", null },
            new object?[] { "300", "Can", null, "01.02.2026", null, null },
            new object?[] { "400", "Ece", "Ak", "01.02.2026", null, "12345678901" });
        var e = ExcelAktarimi.OtomatikEslestir(AktarimTuru.Personel, t.Basliklar);
        Assert.Equal("TC", e["TcKimlikNo"]);

        using (var db = tv.Db(firmaId))
        {
            var on = await ExcelAktarimi.IsleAsync(db, AktarimTuru.Personel, t, e, false, "musteri.xlsx", null);
            Assert.Equal((1, 1, 2), (on.Eklenen, on.Guncellenen, on.Hatali));
            Assert.Equal(1, db.Personeller.Count()); // önizleme hiçbir şey yazmaz
            Assert.False(db.Bolumler.Any(b => b.Ad == "Üretim"));
        }

        using (var db = tv.Db(firmaId))
        {
            var s = await ExcelAktarimi.IsleAsync(db, AktarimTuru.Personel, t, e, true, "musteri.xlsx", 1);
            Assert.NotNull(s.AktarimId);
        }

        using var kontrol = tv.Db(firmaId);
        var ali = kontrol.Personeller.Include(p => p.Bolum).Single(p => p.Ad == "Ali");
        Assert.Equal("Yeni", ali.Soyad);           // baştaki sıfır kaybolmuş kart (100 = 0100) eşleşti
        Assert.Equal("Üretim", ali.Bolum!.Ad);     // bölüm tanımı açıldı
        var veli = kontrol.Personeller.Single(p => p.Ad == "Veli");
        Assert.Equal(new DateOnly(2026, 3, 1), veli.IseGirisTarihi);
        Assert.Equal(ali.BolumId, veli.BolumId);
        Assert.NotNull(veli.AktarimId);
        Assert.NotNull(veli.CalismaGrubuId);
        Assert.Equal(1, kontrol.Bolumler.Count(b => b.Ad == "Üretim"));
        var kayit = kontrol.AktarimKayitlari.Single();
        Assert.Equal(("musteri.xlsx", 1, 1, 2), (kayit.DosyaAdi, kayit.Eklenen, kayit.Guncellenen, kayit.Hatali));
    }

    [Fact]
    public async Task Hareket_aktarimi_kaynagi_isaretler_tekrari_atlar_ve_geri_alinir()
    {
        var (tv, firmaId) = await KurAsync();
        using var _ = tv;
        int pid;
        using (var d = tv.Db(firmaId))
        {
            var p = new Personel { KartNo = "7", SicilNo = "S7", Ad = "Ayşe", Soyad = "Gece", IseGirisTarihi = new DateOnly(2026, 1, 1) };
            d.Personeller.Add(p);
            d.SaveChanges();
            pid = p.Id;
            d.Hareketler.Add(new Hareket { PersonelId = pid, Zaman = new DateTime(2026, 10, 1, 8, 0, 0), Kaynak = HareketKaynagi.Terminal });
            d.SaveChanges();
        }

        var t = Tablo(
            new object?[] { "Sicil", "Tarih", "Giriş", "Çıkış" },
            new object?[] { "S7", "01.10.2026", "08:00", "17:00" },     // giriş zaten var
            new object?[] { "S7", "02.10.2026", "22:00", "06:00" },     // gece: çıkış ertesi gün
            new object?[] { "S99", "02.10.2026", "08:00", "17:00" },    // personel yok
            new object?[] { "S7", "31.12.2025", "08:00", null });       // işe girmeden önce
        var e = ExcelAktarimi.OtomatikEslestir(AktarimTuru.Hareket, t.Basliklar);

        int aktarimId;
        using (var db = tv.Db(firmaId))
        {
            var s = await ExcelAktarimi.IsleAsync(db, AktarimTuru.Hareket, t, e, true, "hareket.xlsx", 1);
            Assert.Equal(3, s.Eklenen);
            Assert.Equal(2, s.Atlanan);
            Assert.Equal(1, s.Hatali);
            aktarimId = s.AktarimId!.Value;
        }

        using (var db = tv.Db(firmaId))
        {
            var aktarilan = db.Hareketler.Where(h => h.AktarimId == aktarimId).OrderBy(h => h.Zaman).ToList();
            Assert.Equal(3, aktarilan.Count);
            Assert.All(aktarilan, h => Assert.Equal(HareketKaynagi.Aktarim, h.Kaynak));
            Assert.All(aktarilan, h => Assert.Contains("hareket.xlsx", h.Aciklama));
            Assert.Equal(new DateTime(2026, 10, 3, 6, 0, 0), aktarilan[^1].Zaman);

            // Aynı dosya ikinci kez yüklenirse hiçbir hareket tekrar eklenmez.
            var ikinci = await ExcelAktarimi.IsleAsync(db, AktarimTuru.Hareket, t, e, false, "hareket.xlsx", 1);
            Assert.Equal(0, ikinci.Eklenen);

            Assert.Equal(3, await ExcelAktarimi.GeriAlAsync(db, aktarimId));
            await Assert.ThrowsAsync<ArgumentException>(() => ExcelAktarimi.GeriAlAsync(db, aktarimId));
        }

        using var kontrol = tv.Db(firmaId);
        Assert.Equal(1, kontrol.Hareketler.Count(h => !h.Iptal)); // yalnız terminal hareketi kaldı
        Assert.True(kontrol.AktarimKayitlari.Single().GeriAlindi);
    }

    [Fact]
    public async Task Izin_kazanc_kesinti_ve_ucret_aktarilir()
    {
        var (tv, firmaId) = await KurAsync();
        using var _ = tv;
        using (var d = tv.Db(firmaId))
        {
            d.Personeller.Add(new Personel { KartNo = "1", Ad = "Ali", Soyad = "A", IseGirisTarihi = new DateOnly(2026, 1, 1), Ucret = 20000m });
            d.SaveChanges();
        }

        using var db = tv.Db(firmaId);
        var izin = Tablo(
            new object?[] { "Kart", "İzin Türü", "Başlangıç", "Gün" },
            new object?[] { "1", "Yıllık izin", "05.10.2026", 3 },
            new object?[] { "1", "YI", "06.10.2026", 1 },          // çakışıyor
            new object?[] { "1", "Doğum izni", "20.10.2026", 1 }); // tanımsız tür
        var si = await ExcelAktarimi.IsleAsync(db, AktarimTuru.Izin, izin, ExcelAktarimi.OtomatikEslestir(AktarimTuru.Izin, izin.Basliklar), true, "izin.xlsx", 1);
        Assert.Equal((1, 1, 1), (si.Eklenen, si.Atlanan, si.Hatali));
        Assert.Equal(new DateOnly(2026, 10, 7), db.Izinler.Single().Bitis);

        var kk = Tablo(
            new object?[] { "Kart", "Kalem", "Tutar" },
            new object?[] { "1", "Prim", "1.500,00" },
            new object?[] { "1", "Avans", 750 });
        var ekk = ExcelAktarimi.OtomatikEslestir(AktarimTuru.KazancKesinti, kk.Basliklar);
        await Assert.ThrowsAsync<ArgumentException>(() => ExcelAktarimi.IsleAsync(db, AktarimTuru.KazancKesinti, kk, ekk, false, "kk.xlsx", 1)); // ay yok
        var sk = await ExcelAktarimi.IsleAsync(db, AktarimTuru.KazancKesinti, kk, ekk, true, "kk.xlsx", 1, new AktarimSecenekleri(new DateOnly(2026, 9, 1)));
        Assert.Equal(2, sk.Eklenen);
        Assert.Equal(1500m, db.KazancKesintiler.ToList().Single(k => k.Tutar > 1000).Tutar);
        var tekrar = await ExcelAktarimi.IsleAsync(db, AktarimTuru.KazancKesinti, kk, ekk, false, "kk.xlsx", 1, new AktarimSecenekleri(new DateOnly(2026, 9, 1)));
        Assert.Equal(2, tekrar.Atlanan);

        var ucret = Tablo(new object?[] { "Kart No", "Maaş" }, new object?[] { "1", "22.500,50" });
        var su = await ExcelAktarimi.IsleAsync(db, AktarimTuru.Ucret, ucret, ExcelAktarimi.OtomatikEslestir(AktarimTuru.Ucret, ucret.Basliklar), true, "ucret.xlsx", 1);
        Assert.Equal(1, su.Guncellenen);
        using var kontrol = tv.Db(firmaId);
        var p = kontrol.Personeller.Single();
        Assert.Equal((22500.50m, 20000m), (p.Ucret!.Value, p.EskiUcret!.Value));
        await Assert.ThrowsAsync<ArgumentException>(() => ExcelAktarimi.GeriAlAsync(kontrol, su.AktarimId!.Value)); // ücret geri alınmaz
    }

    [Fact]
    public async Task Bordro_aktarilir_ve_uyum_raporu_farki_gosterir()
    {
        var (tv, firmaId) = await KurAsync();
        using var _ = tv;
        using (var d = tv.Db(firmaId))
        {
            var grup = d.CalismaGruplari.First();
            d.Personeller.Add(new Personel { KartNo = "1", Ad = "Ali", Soyad = "A", IseGirisTarihi = new DateOnly(2026, 1, 1), CalismaGrubuId = grup.Id });
            d.Personeller.Add(new Personel { KartNo = "2", Ad = "Ayşe", Soyad = "B", IseGirisTarihi = new DateOnly(2026, 1, 1), CalismaGrubuId = grup.Id });
            d.SaveChanges();
        }

        using var db = tv.Db(firmaId);
        var puantaj = new PuantajServisi(TimeProvider.System);
        var pdks = await puantaj.TopluAsync(db, 2026, 9);
        var aliGun = pdks.Single(x => x.Personel.Ad == "Ali").OdenecekGun;

        var bordro = Tablo(
            new object?[] { "Dönem", "Kart", "Prim Günü", "Net Ücret" },
            new object?[] { "Eylül 2026", "1", aliGun, 25000 },
            new object?[] { "09.2026", "2", 30, 25000 });
        var e = ExcelAktarimi.OtomatikEslestir(AktarimTuru.Bordro, bordro.Basliklar);
        Assert.Equal("Prim Günü", e["CalisilanGun"]);
        var s = await ExcelAktarimi.IsleAsync(db, AktarimTuru.Bordro, bordro, e, true, "bordro.xlsx", 1);
        Assert.Equal(2, s.Eklenen);

        // Aynı ay yeniden aktarılırsa satırlar güncellenir, çoğalmaz.
        var s2 = await ExcelAktarimi.IsleAsync(db, AktarimTuru.Bordro, bordro, e, true, "bordro-duzeltilmis.xlsx", 1);
        Assert.Equal(2, s2.Guncellenen);
        Assert.Equal(2, db.BordroSatirlari.Count());

        var uyum = await BordroUyumu.HesaplaAsync(db, puantaj, 2026, 9);
        var ali = uyum.Single(x => x.Personel.Ad == "Ali");
        Assert.Equal(0m, ali.GunFarki);
        var ayse = uyum.Single(x => x.Personel.Ad == "Ayşe");
        Assert.NotNull(ayse.Bordro);
        Assert.Equal(30m - ayse.PdksGun, ayse.GunFarki);
        Assert.NotEmpty(BordroUyumu.Excel(uyum, 2026, 9, "Demo"));
    }
}
