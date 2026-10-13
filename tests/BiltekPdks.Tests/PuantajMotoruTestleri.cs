using BiltekPdks.Domain.Puantaj;

namespace BiltekPdks.Tests;

public class PuantajMotoruTestleri
{
    // 08:00-17:00, 60 dk mola => 480 dk plan. Geç kalma toleransı 10, erken çıkma 5, fazla mesai eşiği 30.
    private static readonly VardiyaKurali Gunduz = new(new TimeOnly(8, 0), new TimeOnly(17, 0), 60, 10, 5, 30);
    private static readonly DateOnly Pazartesi = new(2026, 10, 12);

    private static DateTime S(DateOnly gun, int saat, int dakika) => gun.ToDateTime(new TimeOnly(saat, dakika));

    private static GunlukPuantaj Hesapla(VardiyaKurali? v, params DateTime[] h) =>
        PuantajMotoru.Hesapla(new GunGirdisi(Pazartesi, v, h));

    [Fact]
    public void Tam_gun_calisma_eksiksiz_ve_fazla_mesaisiz()
    {
        var p = Hesapla(Gunduz, S(Pazartesi, 7, 55), S(Pazartesi, 17, 5));
        Assert.Equal(PuantajDurumu.Calisti, p.Durum);
        Assert.Equal(480, p.PlanlananDakika);
        Assert.Equal(490, p.CalismaDakika);
        Assert.Equal(480, p.NormalCalismaDakika);
        Assert.Equal(0, p.GecKalmaDakika);
        Assert.Equal(0, p.EksikDakika);
        Assert.Equal(0, p.FazlaMesaiDakika); // 10 dk fazla, eşik 30
    }

    [Fact]
    public void Tolerans_ustu_gec_kalma_eksik_sureye_yazilir()
    {
        var p = Hesapla(Gunduz, S(Pazartesi, 8, 20), S(Pazartesi, 17, 0));
        Assert.Equal(20, p.GecKalmaDakika);
        Assert.Equal(460, p.CalismaDakika);
        Assert.Equal(460, p.NormalCalismaDakika);
        Assert.Equal(20, p.EksikDakika);
    }

    [Fact]
    public void Tolerans_icindeki_gec_kalma_affedilir()
    {
        var p = Hesapla(Gunduz, S(Pazartesi, 8, 7), S(Pazartesi, 17, 0));
        Assert.Equal(0, p.GecKalmaDakika);
        Assert.Equal(0, p.EksikDakika);
        Assert.Equal(480, p.NormalCalismaDakika);
    }

    [Fact]
    public void Erken_cikma_tolerans_ustunde_yazilir()
    {
        var p = Hesapla(Gunduz, S(Pazartesi, 8, 0), S(Pazartesi, 16, 30));
        Assert.Equal(30, p.ErkenCikmaDakika);
        Assert.Equal(30, p.EksikDakika);
    }

    [Fact]
    public void Esik_ustu_fazla_mesai_yazilir()
    {
        var p = Hesapla(Gunduz, S(Pazartesi, 8, 0), S(Pazartesi, 19, 0));
        Assert.Equal(600, p.CalismaDakika);
        Assert.Equal(480, p.NormalCalismaDakika);
        Assert.Equal(120, p.FazlaMesaiDakika);
    }

    [Fact]
    public void Hareket_yoksa_devamsiz_ve_tum_gun_eksik()
    {
        var p = Hesapla(Gunduz);
        Assert.Equal(PuantajDurumu.Devamsiz, p.Durum);
        Assert.Equal(480, p.EksikDakika);
    }

    [Fact]
    public void Tek_hareket_eksik_hareket_olarak_isaretlenir()
    {
        var p = Hesapla(Gunduz, S(Pazartesi, 8, 0));
        Assert.Equal(PuantajDurumu.EksikHareket, p.Durum);
        Assert.Equal(S(Pazartesi, 8, 0), p.Giris);
        Assert.Null(p.Cikis);
    }

    [Fact]
    public void Mukerrer_okutma_tek_hareket_sayilir()
    {
        var p = Hesapla(Gunduz, S(Pazartesi, 8, 0), S(Pazartesi, 8, 0).AddSeconds(30));
        Assert.Equal(PuantajDurumu.EksikHareket, p.Durum);
    }

    [Fact]
    public void Hafta_tatilinde_calisma_tatil_mesaisi_olur_ve_yasal_ara_duser()
    {
        var p = Hesapla(null, S(Pazartesi, 9, 0), S(Pazartesi, 14, 0));
        Assert.Equal(PuantajDurumu.HaftaTatili, p.Durum);
        Assert.Equal(270, p.TatilMesaisiDakika); // 300 - 30
        Assert.Equal(0, p.EksikDakika);
    }

    [Fact]
    public void Calisilmayan_hafta_tatili_eksik_yazmaz()
    {
        var p = Hesapla(null);
        Assert.Equal(PuantajDurumu.HaftaTatili, p.Durum);
        Assert.Equal(0, p.EksikDakika);
    }

    [Fact]
    public void Resmi_tatilde_gelmeyen_devamsiz_sayilmaz()
    {
        var p = PuantajMotoru.Hesapla(new GunGirdisi(Pazartesi, Gunduz, Array.Empty<DateTime>(), ResmiTatil: true));
        Assert.Equal(PuantajDurumu.ResmiTatil, p.Durum);
        Assert.Equal(0, p.EksikDakika);
    }

    [Fact]
    public void Izinli_gunde_eksik_yazilmaz_ve_ucret_durumu_tasinir()
    {
        var p = PuantajMotoru.Hesapla(new GunGirdisi(Pazartesi, Gunduz, Array.Empty<DateTime>(), Izinli: true, IzinUcretli: false));
        Assert.Equal(PuantajDurumu.Izinli, p.Durum);
        Assert.Equal(0, p.EksikDakika);
        Assert.False(p.IzinUcretli);
    }

    [Fact]
    public void Gece_vardiyasi_ertesi_gune_tasan_cikisi_dogru_hesaplar()
    {
        var gece = new VardiyaKurali(new TimeOnly(22, 0), new TimeOnly(6, 0), 30, 10, 5, 30);
        Assert.True(gece.GeceyiAsar);
        Assert.Equal(450, gece.PlanlananDakika(Pazartesi));

        var p = Hesapla(gece, S(Pazartesi, 21, 55), S(Pazartesi.AddDays(1), 6, 10));
        Assert.Equal(PuantajDurumu.Calisti, p.Durum);
        Assert.Equal(465, p.CalismaDakika); // 495 - 30
        Assert.Equal(450, p.NormalCalismaDakika);
        Assert.Equal(0, p.FazlaMesaiDakika); // 15 dk < eşik
        Assert.Equal(0, p.EksikDakika);
    }

    [Fact]
    public void Kapsam_disi_gun_hesaplanmaz()
    {
        var p = PuantajMotoru.Hesapla(new GunGirdisi(Pazartesi, Gunduz, Array.Empty<DateTime>(), KapsamDisi: true));
        Assert.Equal(PuantajDurumu.KapsamDisi, p.Durum);
        Assert.Equal(0, p.EksikDakika);
    }

    [Theory]
    [InlineData(0, "")]
    [InlineData(45, "0:45")]
    [InlineData(480, "8:00")]
    [InlineData(605, "10:05")]
    public void Sure_bicimi(int dakika, string beklenen) => Assert.Equal(beklenen, PuantajMotoru.SaatDakika(dakika));
}
