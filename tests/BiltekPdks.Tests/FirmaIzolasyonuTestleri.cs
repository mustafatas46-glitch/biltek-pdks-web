using BiltekPdks.Domain.Entities;
using BiltekPdks.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BiltekPdks.Tests;

public class FirmaIzolasyonuTestleri
{
    private static Personel Personel(string kart, string ad) =>
        new() { KartNo = kart, Ad = ad, Soyad = "Test", IseGirisTarihi = new DateOnly(2024, 1, 1) };

    [Fact]
    public void Sorgular_yalnizca_aktif_firmanin_kayitlarini_dondurur()
    {
        using var tv = new TestVeritabani();
        var a = tv.FirmaEkle("A");
        var b = tv.FirmaEkle("B");

        using (var db = tv.Db(a)) { db.Personeller.Add(Personel("1", "Ali")); db.SaveChanges(); }
        using (var db = tv.Db(b)) { db.Personeller.Add(Personel("1", "Berk")); db.SaveChanges(); }

        using var dbA = tv.Db(a);
        var liste = dbA.Personeller.ToList();
        Assert.Single(liste);
        Assert.Equal("Ali", liste[0].Ad);
    }

    [Fact]
    public void Firma_secilmemisse_hicbir_kayit_gorunmez()
    {
        using var tv = new TestVeritabani();
        var a = tv.FirmaEkle("A");
        using (var db = tv.Db(a)) { db.Personeller.Add(Personel("1", "Ali")); db.SaveChanges(); }

        using var bos = tv.CreateDbContext();
        Assert.Empty(bos.Personeller.ToList());
    }

    [Fact]
    public void Yeni_kayda_FirmaId_otomatik_yazilir()
    {
        using var tv = new TestVeritabani();
        var a = tv.FirmaEkle("A");
        using var db = tv.Db(a);
        var bolum = new Bolum { Ad = "Üretim" };
        db.Bolumler.Add(bolum);
        db.SaveChanges();
        Assert.Equal(a, bolum.FirmaId);
    }

    [Fact]
    public void Baska_firmaya_kayit_yazmak_engellenir()
    {
        using var tv = new TestVeritabani();
        var a = tv.FirmaEkle("A");
        var b = tv.FirmaEkle("B");
        using var db = tv.Db(a);
        db.Bolumler.Add(new Bolum { Ad = "Sızma", FirmaId = b });
        Assert.Throws<FirmaIzolasyonHatasi>(() => db.SaveChanges());
    }

    [Fact]
    public void Firma_secilmeden_kayit_eklenemez()
    {
        using var tv = new TestVeritabani();
        tv.FirmaEkle("A");
        using var db = tv.CreateDbContext();
        db.Bolumler.Add(new Bolum { Ad = "Sahipsiz" });
        Assert.Throws<FirmaIzolasyonHatasi>(() => db.SaveChanges());
    }

    [Fact]
    public void Ayni_kart_no_farkli_firmalarda_kullanilabilir()
    {
        using var tv = new TestVeritabani();
        var a = tv.FirmaEkle("A");
        var b = tv.FirmaEkle("B");

        using (var db = tv.Db(a)) { db.Personeller.Add(Personel("100", "Ali")); db.SaveChanges(); }
        using (var db = tv.Db(b)) { db.Personeller.Add(Personel("100", "Berk")); db.SaveChanges(); }

        using var oku = tv.Sistem();
        Assert.Equal(2, oku.Personeller.Count(p => p.KartNo == "100"));
    }
}

public class IslemKaydiTestleri
{
    [Fact]
    public void Kullanici_islemleri_kaydedilir_sistem_islemleri_kaydedilmez()
    {
        using var tv = new TestVeritabani();
        var a = tv.FirmaEkle("A");

        using (var db = tv.Db(a))
        {
            db.IslemYapanKullaniciId = 7;
            var p = new Personel { KartNo = "1", Ad = "Ali", Soyad = "Can", IseGirisTarihi = new DateOnly(2024, 1, 1) };
            db.Personeller.Add(p);
            db.SaveChanges();
            p.Ad = "Veli";
            db.SaveChanges();
        }
        using (var db = tv.Db(a))
        {
            // Kullanıcı yoksa (aktarım vb.) kayıt düşülmez.
            db.Bolumler.Add(new Bolum { Ad = "Depo" });
            db.SaveChanges();
        }

        using var oku = tv.Db(a);
        var kayitlar = oku.IslemKayitlari.OrderBy(k => k.Id).ToList();
        Assert.Equal(2, kayitlar.Count);
        Assert.All(kayitlar, k => Assert.Equal("personel", k.Tablo));
        Assert.Equal("Ekle", kayitlar[0].Islem);
        Assert.True(kayitlar[0].KayitId > 0);
        Assert.Equal("Değiştir", kayitlar[1].Islem);
        Assert.Contains("Veli", kayitlar[1].Degisiklikler);
        Assert.Contains("Ali", kayitlar[1].Degisiklikler);
        Assert.All(kayitlar, k => Assert.Equal(7, k.KullaniciId));
    }
}
