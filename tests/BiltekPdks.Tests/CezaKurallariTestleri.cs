using BiltekPdks.Domain.Puantaj;

namespace BiltekPdks.Tests;

public class CezaKurallariTestleri
{
    private static readonly DateOnly Gun = new(2026, 10, 14); // Çarşamba
    private static DateTime S(int saat, int dakika) => Gun.ToDateTime(new TimeOnly(saat, dakika));

    private static readonly VardiyaKurali Normal = new(new TimeOnly(8, 0), new TimeOnly(17, 0), 60, 5, 5, 30);

    private static GunlukPuantaj Hesapla(VardiyaKurali v, params DateTime[] h) =>
        PuantajMotoru.Hesapla(new GunGirdisi(Gun, v, h));

    [Fact]
    public void Katsayi_yoksa_ceza_eksik_sure_kadardir()
    {
        var g = Hesapla(Normal, S(8, 20), S(17, 0));
        Assert.Equal(20, g.GecKalmaDakika);
        Assert.Equal(20, g.EksikDakika);
        Assert.Equal(20, g.CezaDakika);
    }

    [Fact]
    public void Gec_kalma_katsayisi_ile_carpilir_diger_eksik_kendi_katsayisiyla()
    {
        var v = Normal with { GecCezaKatsayisi = 2, ErkenCezaKatsayisi = 3, EksikCezaKatsayisi = 1.5m };
        var g = Hesapla(v, S(8, 20), S(16, 30));
        Assert.Equal(20, g.GecKalmaDakika);
        Assert.Equal(30, g.ErkenCikmaDakika);
        Assert.Equal(50, g.EksikDakika);
        Assert.Equal(20 * 2 + 30 * 3, g.CezaDakika);
    }

    [Fact]
    public void Devamsizlik_cezasi_tanimliysa_o_sure_kesilir()
    {
        Assert.Equal(480, Hesapla(Normal).CezaDakika);
        Assert.Equal(960, Hesapla(Normal with { DevamsizlikCezaDakika = 960 }).CezaDakika);
    }

    [Fact]
    public void Normal_calisma_tamamlanirsa_gec_iptal_olur_fazlasi_mesai()
    {
        var v = Normal with { NormalTamamlaSonraFazlaMesai = true };
        var g = Hesapla(v, S(8, 20), S(18, 30)); // 10:10 brüt − 1 sa mola = 9:10 net
        Assert.Equal(0, g.GecKalmaDakika);
        Assert.Equal(0, g.EksikDakika);
        Assert.Equal(0, g.CezaDakika);
        Assert.Equal(480, g.NormalCalismaDakika);
        Assert.Equal(70, g.FazlaMesaiDakika);

        var kural_yok = Hesapla(Normal, S(8, 20), S(18, 30));
        Assert.Equal(20, kural_yok.GecKalmaDakika);
    }

    [Fact]
    public void Coklu_okutmada_eksik_yazilmayabilir()
    {
        var v = Normal with { CokluGiriseEksikYazma = true };
        var g = Hesapla(v, S(8, 30), S(12, 0), S(13, 0), S(17, 0));
        Assert.Equal(0, g.EksikDakika);
        Assert.Equal(0, g.CezaDakika);
        Assert.Equal(30, Hesapla(Normal, S(8, 30), S(12, 0), S(13, 0), S(17, 0)).EksikDakika);
    }

    [Fact]
    public void Max_tolerans_icindeki_giris_vardiya_baslangici_sayilir()
    {
        var v = Normal with { MaxToleransDakika = 15, GecKalmaToleransi = 0 };
        var gec = Hesapla(v, S(8, 10), S(17, 0));
        Assert.Equal(new DateTime(2026, 10, 14, 8, 0, 0), gec.Giris);
        Assert.Equal(0, gec.GecKalmaDakika);
        Assert.Equal(20, Hesapla(v, S(8, 20), S(17, 0)).GecKalmaDakika);
    }
}
