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
    public void Ayni_kart_no_farkli_firmalarda_kullanilabilir_ayni_firmada_kullanilamaz()
    {
        using var tv = new TestVeritabani();
        var a = tv.FirmaEkle("A");
        var b = tv.FirmaEkle("B");

        using (var db = tv.Db(a)) { db.Personeller.Add(Personel("100", "Ali")); db.SaveChanges(); }
        using (var db = tv.Db(b)) { db.Personeller.Add(Personel("100", "Berk")); db.SaveChanges(); }

        using var db2 = tv.Db(a);
        db2.Personeller.Add(Personel("100", "Cem"));
        Assert.Throws<DbUpdateException>(() => db2.SaveChanges());
    }
}
