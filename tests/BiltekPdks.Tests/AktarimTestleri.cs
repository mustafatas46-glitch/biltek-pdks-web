using Microsoft.EntityFrameworkCore;
using BiltekPdks.Domain.Entities;
using BiltekPdks.Infrastructure.Aktarim;
using BiltekPdks.Infrastructure.Security;
using BiltekPdks.Infrastructure.Seed;

namespace BiltekPdks.Tests;

public class AktarimTestleri
{
    private static EskiVeri OrnekVeri()
    {
        var v = new EskiVeri();
        v.Subeler.Add(new(1, "M", "Merkez"));
        v.Bolumler.Add(new(10, "U", "Üretim"));
        v.Bolumler.Add(new(11, null, "0")); // eski sistemdeki boş kayıt
        v.Gorevler.Add(new(20, null, "Operatör"));
        v.Gruplar.Add(new(5, "Vardiyalı", "07:30", "45:00"));
        v.IzinTurleri.Add(new(3, "1", "YI", "Yıllık İzin", "ucretli"));
        v.IzinTurleri.Add(new(4, "2", "UI", "Ücretsiz İzin", "ucretsiz"));

        v.Personeller.Add(new(100, "0001", "Şükrü", "Güneş", "S1", 1, 5, 10, 20, 1,
            new DateTime(2020, 3, 1), null, null, "10000000146", new DateTime(1990, 1, 1), "Bay", "555", "s@x.com", "İzmir"));
        v.Personeller.Add(new(101, "0002", "Ayşe", "Işık", null, 1, null, null, null, null,
            null, new DateTime(2024, 5, 31), "İstifa", "12345", null, "Bayan", null, null, null));
        v.Personeller.Add(new(102, "0001", "Çift", "Kart", null, 1, null, null, null, null,
            new DateTime(2021, 1, 1), null, null, null, null, null, null, null, null));
        v.Personeller.Add(new(103, "", "Kartsız", "Kişi", null, 1, null, null, null, null,
            new DateTime(2021, 1, 1), null, null, null, null, null, null, null, null));

        v.Tatiller.Add(new(new DateTime(2026, 10, 29), "Cumhuriyet Bayramı"));
        v.Tatiller.Add(new(new DateTime(2026, 10, 29), "Cumhuriyet Bayramı")); // mükerrer

        v.Izinler.Add(new(1, 100, new DateTime(2026, 10, 5), null, new DateTime(2026, 10, 9), null, 3, "Tatil"));
        v.Izinler.Add(new(2, 100, new DateTime(2026, 10, 12), new TimeSpan(10, 0, 0), new DateTime(2026, 10, 12), new TimeSpan(12, 0, 0), 3, "Saatlik"));
        v.Izinler.Add(new(3, 999, new DateTime(2026, 10, 12), null, null, null, 3, "Personel yok"));

        v.GirisCikislar.Add(new(1, 100, new DateTime(2026, 10, 1), new TimeSpan(7, 58, 0), null, "01",
            new DateTime(2026, 10, 1), new TimeSpan(17, 3, 0), "E", "02"));
        v.GirisCikislar.Add(new(2, 100, new DateTime(2026, 10, 2), new TimeSpan(22, 0, 0), null, "01",
            new DateTime(2026, 10, 3), new TimeSpan(6, 5, 0), null, "02"));
        v.GirisCikislar.Add(new(3, 100, new DateTime(2026, 10, 4), null, null, null, null, null, null, null));
        return v;
    }

    private static async Task<(TestVeritabani tv, int firmaId)> HedefAsync()
    {
        var tv = new TestVeritabani();
        await new KurulumServisi(tv, new SifreServisi()).CalistirAsync("demo", "Demo", "Gizli1234");
        using var s = tv.Sistem();
        return (tv, s.Firmalar.Single().Id);
    }

    [Fact]
    public async Task Eski_veri_eslenerek_aktarilir()
    {
        var (tv, firmaId) = await HedefAsync();
        using var _ = tv;

        using (var db = tv.Db(firmaId))
        {
            var r = await EskiSistemAktarimi.AktarAsync(db, firmaId, OrnekVeri());
            Assert.Equal(2, r.Eklenen["Personel"]);
            Assert.Equal(1, r.Eklenen["Resmi tatil"]);
            Assert.Equal(1, r.Eklenen["İzin"]);
            Assert.Equal(4, r.Eklenen["Hareket"]);
            Assert.Equal(1, r.Atlanan["Personel: kart no başka personelde var"]);
            Assert.Equal(1, r.Atlanan["Personel: kart no boş"]);
            Assert.Equal(1, r.Atlanan["İzin: saatlik izin (sonraki sürümde)"]);
            Assert.Equal(1, r.Atlanan["Bölüm: adı boş"]);
            Assert.Contains(r.Uyarilar, u => u.Contains("T.C. kimlik no geçersiz"));
        }

        using var oku = tv.Db(firmaId);
        var sukru = oku.Personeller.Single(p => p.KartNo == "0001");
        Assert.Equal("Şükrü", sukru.Ad);
        Assert.Equal(Cinsiyet.Erkek, sukru.Cinsiyet);
        Assert.Equal("10000000146", sukru.TcKimlikNo);
        Assert.Equal(new DateOnly(2020, 3, 1), sukru.IseGirisTarihi);
        Assert.Equal("Üretim", oku.Bolumler.Single(b => b.Id == sukru.BolumId).Ad);
        var grup = oku.CalismaGruplari.Single(g => g.Id == sukru.CalismaGrubuId);
        Assert.Equal(450, grup.GunlukCalismaDakika);
        Assert.Equal(7, oku.CalismaGrubuGunleri.Count(g => g.CalismaGrubuId == grup.Id));

        var ayse = oku.Personeller.Single(p => p.KartNo == "0002");
        Assert.Equal(Cinsiyet.Kadin, ayse.Cinsiyet);
        Assert.Null(ayse.TcKimlikNo);
        Assert.Equal(new DateOnly(2000, 1, 1), ayse.IseGirisTarihi);
        Assert.Equal(new DateOnly(2024, 5, 31), ayse.IstenCikisTarihi);

        Assert.False(oku.IzinTurleri.Single(t => t.EskiId == 4).Ucretli);
        Assert.True(oku.IzinTurleri.Single(t => t.EskiId == 3).YillikIzindenDuser);

        var hareketler = oku.Hareketler.Where(h => h.PersonelId == sukru.Id).OrderBy(h => h.Zaman).ToList();
        Assert.Equal(new DateTime(2026, 10, 3, 6, 5, 0), hareketler[3].Zaman); // gece çıkışı ertesi gün
        Assert.Equal(HareketKaynagi.Elle, hareketler[1].Kaynak);
        Assert.Equal(HareketKaynagi.Aktarim, hareketler[0].Kaynak);
    }

    [Fact]
    public async Task Ikinci_kez_calistirmak_kayit_cogaltmaz()
    {
        var (tv, firmaId) = await HedefAsync();
        using var _ = tv;
        using (var db = tv.Db(firmaId)) await EskiSistemAktarimi.AktarAsync(db, firmaId, OrnekVeri());

        AktarimRaporu ikinci;
        using (var db = tv.Db(firmaId)) ikinci = await EskiSistemAktarimi.AktarAsync(db, firmaId, OrnekVeri());

        Assert.False(ikinci.Eklenen.ContainsKey("Personel"));
        Assert.False(ikinci.Eklenen.ContainsKey("Hareket"));
        Assert.False(ikinci.Eklenen.ContainsKey("İzin"));
        using var oku = tv.Db(firmaId);
        Assert.Equal(2, oku.Personeller.Count());
        Assert.Equal(4, oku.Hareketler.Count());
    }

    [Fact]
    public async Task Gunluk_calisma_saatleri_alt_gruplar_planlar_ve_donemler_aktarilir()
    {
        var v = new EskiVeri();
        v.Gruplar.Add(new(5, "Vardiyalı", "07:30", "225:00"));
        v.PuantajTanimlari.Add(new(1, "Sabah", "07:30"));
        v.PuantajTanimlari.Add(new(2, "Akşam", "07:30"));
        v.PuantajTanimlari.Add(new(4, "Bozuk", null));
        v.CalismaParametreleri.Add(new(1, 1, "Normal Çalışma", "08:00", "16:00"));
        v.CalismaParametreleri.Add(new(2, 1, "Fazla Mesai %50", "16:00", "20:00"));
        v.CalismaParametreleri.Add(new(3, 2, "Normal", "16:00", "24:00"));
        v.Molalar.Add(new(1, "Yemek", "12:00", "12:30", "ucretsiz"));
        v.AltGruplar.Add(new(10, 5, "08-16", "07:30"));
        v.AltGruplar.Add(new(11, 5, "16-24", null));
        v.AltGruplar.Add(new(12, 99, "Yetim", null));
        v.GunParametreleri.Add(new(5, 10, new int?[] { 1, 1, 1, 1, 1, null, null }));
        v.GunParametreleri.Add(new(5, 11, new int?[] { 2, 2, 2, 2, 2, 2, 4 }));    // 4: aktarılamayan plan
        v.GunParametreleri.Add(new(5, 0, new int?[] { 1, 1, 1, 1, 1, 1, null }));    // grubun kendi planı
        v.Donemler.Add(new(5, null, new DateTime(2026, 9, 26), new DateTime(2026, 10, 25), new TimeSpan(8, 0, 0), new TimeSpan(16, 0, 0), 0, "0:00", 0, "0:00", 30));
        v.Donemler.Add(new(5, 10, new DateTime(2026, 2, 1), new DateTime(2026, 2, 28), null, null, null, null, null, null, null));

        var (tv, firmaId) = await HedefAsync();
        using var _ = tv;
        using (var db = tv.Db(firmaId))
        {
            var r = await EskiSistemAktarimi.AktarAsync(db, firmaId, v);
            Assert.Equal(2, r.Eklenen["Günlük çalışma saati"]);
            Assert.Equal(2, r.Eklenen["Alt grup"]);
            Assert.Equal(2, r.Eklenen["Dönem"]);
            Assert.Equal(1, r.Eklenen["Grup çalışma planı"]);
            Assert.Equal(1, r.Atlanan["Günlük çalışma saati: giriş/çıkış saati okunamadı"]);
            Assert.Equal(1, r.Atlanan["Alt grup: grubu aktarılmamış"]);
            Assert.Contains(r.Uyarilar, u => u.Contains("varsayılan plana bağlandı"));
        }

        using (var oku = tv.Db(firmaId))
        {
            var sabah = oku.Vardiyalar.Include(x => x.Molalar).Single(x => x.EskiId == 1);
            Assert.Equal((new TimeOnly(8, 0), new TimeOnly(16, 0), 30), (sabah.Baslangic, sabah.Bitis, sabah.MolaDakika));
            Assert.Single(sabah.Molalar);
            var aksam = oku.Vardiyalar.Single(x => x.EskiId == 2);
            Assert.Equal((new TimeOnly(16, 0), new TimeOnly(0, 0), 30), (aksam.Baslangic, aksam.Bitis, aksam.MolaDakika)); // 8 saat − 7:30

            var grup = oku.CalismaGruplari.Single(g => g.EskiId == 5);
            Assert.Equal(sabah.Id, oku.CalismaGrubuGunleri.Single(x => x.CalismaGrubuId == grup.Id && x.Gun == DayOfWeek.Saturday).VardiyaId);

            var a816 = oku.AltGruplar.Single(a => a.EskiId == 10);
            var a1624 = oku.AltGruplar.Single(a => a.EskiId == 11);
            Assert.Equal(grup.Id, a816.CalismaGrubuId);
            Assert.Equal(450, a1624.GunlukCalismaDakika);
            Assert.Equal(sabah.Id, oku.AltGrupGunleri.Single(x => x.AltGrupId == a816.Id && x.Gun == DayOfWeek.Monday).VardiyaId);
            Assert.Null(oku.AltGrupGunleri.Single(x => x.AltGrupId == a816.Id && x.Gun == DayOfWeek.Saturday).VardiyaId);
            Assert.Equal(aksam.Id, oku.AltGrupGunleri.Single(x => x.AltGrupId == a1624.Id && x.Gun == DayOfWeek.Saturday).VardiyaId);
            Assert.NotNull(oku.AltGrupGunleri.Single(x => x.AltGrupId == a1624.Id && x.Gun == DayOfWeek.Sunday).VardiyaId); // varsayılana bağlandı

            var subat = oku.DonemTanimlari.Single(d => d.AltGrupId == a816.Id);
            Assert.Equal((28, 2, 900), (subat.GunFarki, subat.EksikGun, subat.EksikDakika));
            Assert.Equal(new TimeOnly(8, 0), oku.DonemTanimlari.Single(d => d.AltGrupId == null).GirisSaati);
        }

        // İkinci çalıştırma hiçbir şeyi çoğaltmaz.
        using (var db = tv.Db(firmaId))
        {
            var r2 = await EskiSistemAktarimi.AktarAsync(db, firmaId, v);
            Assert.False(r2.Eklenen.ContainsKey("Alt grup"));
            Assert.False(r2.Eklenen.ContainsKey("Dönem"));
            Assert.False(r2.Eklenen.ContainsKey("Günlük çalışma saati"));
            Assert.False(r2.Eklenen.ContainsKey("Grup çalışma planı"));
            Assert.False(r2.Atlanan.ContainsKey("Grup çalışma planı: yeni sistemde değiştirilmiş, dokunulmadı"));
        }
    }

    [Theory]
    [InlineData("08:00", 8, 0)]
    [InlineData("8:30", 8, 30)]
    [InlineData("24:00", 0, 0)]
    public void Eski_saat_metni_cozulur(string metin, int saat, int dakika) =>
        Assert.Equal(new TimeOnly(saat, dakika), EskiSistemAktarimi.EskiSaat(metin));

    [Theory]
    [InlineData("07:30", 450)]
    [InlineData("45:00", 2700)]
    [InlineData("7.5", 450)]
    [InlineData("7,5", 450)]
    [InlineData("", null)]
    [InlineData("abc", null)]
    public void Eski_sure_metni_dakikaya_cevrilir(string metin, int? beklenen) =>
        Assert.Equal(beklenen, EskiSistemAktarimi.SureDakika(metin));
}
