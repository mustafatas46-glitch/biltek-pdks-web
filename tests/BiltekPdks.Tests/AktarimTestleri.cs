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
