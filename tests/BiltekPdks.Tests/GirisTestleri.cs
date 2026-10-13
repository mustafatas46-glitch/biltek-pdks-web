using BiltekPdks.Domain.Entities;
using BiltekPdks.Infrastructure.Security;
using BiltekPdks.Infrastructure.Seed;

namespace BiltekPdks.Tests;

public class GirisTestleri
{
    private const string Sifre = "Gizli1234";

    private static (TestVeritabani tv, GirisServisi giris, SabitSaat saat) Hazirla()
    {
        var tv = new TestVeritabani();
        var sifre = new SifreServisi();
        var kurulum = new KurulumServisi(tv, sifre);
        var sonuc = kurulum.CalistirAsync("demo", "Demo Ltd", Sifre).GetAwaiter().GetResult();
        Assert.True(sonuc.IlkKurulum);
        var saat = new SabitSaat();
        return (tv, new GirisServisi(tv, sifre, saat), saat);
    }

    [Fact]
    public async Task Dogru_bilgilerle_giris_basarili()
    {
        var (tv, giris, _) = Hazirla();
        using var _tv = tv;
        var s = await giris.DogrulaAsync("DEMO", "admin", Sifre);
        Assert.True(s.Basarili);
        Assert.Equal(KullaniciRolu.Yonetici, s.Kullanici!.Rol);
    }

    [Fact]
    public async Task Tek_firmada_firma_kodu_bos_birakilabilir_ve_kullanici_adi_buyuk_kucuk_harf_duyarsiz()
    {
        var (tv, giris, _) = Hazirla();
        using var _tv = tv;
        Assert.True((await giris.DogrulaAsync(null, "ADMIN", Sifre)).Basarili);
    }

    [Fact]
    public async Task Yanlis_sifre_reddedilir_ve_sifre_duz_metin_saklanmaz()
    {
        var (tv, giris, _) = Hazirla();
        using var _tv = tv;
        Assert.False((await giris.DogrulaAsync("DEMO", "admin", "yanlis999")).Basarili);

        using var db = tv.Sistem();
        var k = db.Kullanicilar.Single();
        Assert.NotEqual(Sifre, k.SifreHash);
        Assert.DoesNotContain(Sifre, k.SifreHash);
    }

    [Fact]
    public async Task Bes_hatali_denemeden_sonra_hesap_kilitlenir_ve_sure_dolunca_acilir()
    {
        var (tv, giris, saat) = Hazirla();
        using var _tv = tv;
        for (var i = 0; i < GirisServisi.AzamiHataliDeneme; i++)
            await giris.DogrulaAsync("DEMO", "admin", "yanlis999");

        var kilitli = await giris.DogrulaAsync("DEMO", "admin", Sifre);
        Assert.False(kilitli.Basarili);
        Assert.Contains("kilit", kilitli.Hata!);

        saat.Simdi = saat.Simdi.Add(GirisServisi.KilitSuresi).AddSeconds(1);
        Assert.True((await giris.DogrulaAsync("DEMO", "admin", Sifre)).Basarili);
    }

    [Fact]
    public async Task Kurulum_ikinci_kez_calistiginda_bir_sey_degistirmez()
    {
        var (tv, _, _) = Hazirla();
        using var _tv = tv;
        var tekrar = await new KurulumServisi(tv, new SifreServisi()).CalistirAsync("X", "X", Sifre);
        Assert.False(tekrar.IlkKurulum);
        using var db = tv.Sistem();
        Assert.Equal(1, db.Firmalar.Count());
    }

    [Fact]
    public async Task Zayif_sifre_verilirse_guclu_gecici_sifre_uretilir()
    {
        using var tv = new TestVeritabani();
        var sifre = new SifreServisi();
        var sonuc = await new KurulumServisi(tv, sifre).CalistirAsync("demo", "Demo", "123");
        Assert.NotNull(sonuc.UretilenSifre);
        Assert.Null(SifreServisi.KuralHatasi(sonuc.UretilenSifre));
        var giris = new GirisServisi(tv, sifre, new SabitSaat());
        Assert.True((await giris.DogrulaAsync("DEMO", "admin", sonuc.UretilenSifre!)).Basarili);
    }

    [Fact]
    public async Task Sifre_degistirilince_eski_sifre_gecersiz_yeni_gecerli_olur()
    {
        var (tv, giris, _) = Hazirla();
        using var _tv = tv;
        var id = (await giris.DogrulaAsync("DEMO", "admin", Sifre)).Kullanici!.Id;

        Assert.Equal("Mevcut şifre hatalı.", await giris.SifreDegistirAsync(id, "yanlis999", "YeniSifre42", "YeniSifre42"));
        Assert.Equal("Yeni şifreler birbiriyle aynı değil.", await giris.SifreDegistirAsync(id, Sifre, "YeniSifre42", "YeniSifre43"));
        Assert.NotNull(await giris.SifreDegistirAsync(id, Sifre, "kisa1", "kisa1"));

        Assert.Null(await giris.SifreDegistirAsync(id, Sifre, "YeniSifre42", "YeniSifre42"));
        Assert.False((await giris.DogrulaAsync("DEMO", "admin", Sifre)).Basarili);
        Assert.True((await giris.DogrulaAsync("DEMO", "admin", "YeniSifre42")).Basarili);
    }
}
