using System.Net;
using System.Text.RegularExpressions;
using BiltekPdks.Domain.Entities;
using BiltekPdks.Infrastructure.Data;
using BiltekPdks.Infrastructure.Firmalar;
using BiltekPdks.Infrastructure.Security;
using BiltekPdks.Infrastructure.Seed;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BiltekPdks.Tests;

/// <summary>Her firmanın ayrı veritabanında çalıştığı (bulut) kurulum.</summary>
public sealed class FirmaBasinaFabrikasi : WebApplicationFactory<Program>
{
    public const string Sifre = "TestSifre123";
    public readonly string Klasor = Path.Combine(Path.GetTempPath(), $"pdks-firmalar-{Guid.NewGuid():N}");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.UseSetting("Veritabani:Saglayici", "Sqlite");
        builder.UseSetting("Veritabani:Mod", "FirmaBasina");
        builder.UseSetting("ConnectionStrings:Katalog", $"Data Source={Path.Combine(Klasor, "katalog.db")}");
        builder.UseSetting("ConnectionStrings:FirmaSablonu", $"Data Source={Path.Combine(Klasor, "{kod}", "pdks.db")}");
        builder.UseSetting("Kurulum:FirmaKodu", "ALFA");
        builder.UseSetting("Kurulum:FirmaAdi", "Alfa A.Ş.");
        builder.UseSetting("Kurulum:YoneticiSifresi", Sifre);
        builder.UseSetting("Sistem:YonetimFirmasi", "ALFA");
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
        try { Directory.Delete(Klasor, true); } catch (IOException) { }
    }
}

public class FirmaBasinaVeritabaniTestleri : IClassFixture<FirmaBasinaFabrikasi>
{
    private readonly FirmaBasinaFabrikasi _fabrika;
    public FirmaBasinaVeritabaniTestleri(FirmaBasinaFabrikasi fabrika) => _fabrika = fabrika;

    private HttpClient Istemci() => _fabrika.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

    private static async Task<HttpResponseMessage> GirisYapAsync(HttpClient c, string firma, string sifre)
    {
        var html = await c.GetStringAsync("/hesap/giris");
        var m = Regex.Match(html, "name=\"__RequestVerificationToken\"[^>]*value=\"([^\"]+)\"");
        if (!m.Success) m = Regex.Match(html, "value=\"([^\"]+)\"[^>]*name=\"__RequestVerificationToken\"");
        Assert.True(m.Success);
        return await c.PostAsync("/hesap/giris", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = WebUtility.HtmlDecode(m.Groups[1].Value),
            ["firmaKodu"] = firma,
            ["kullaniciAdi"] = "admin",
            ["sifre"] = sifre,
            ["donus"] = "/personeller",
        }));
    }

    private static async Task PersonelEkleAsync(IDbContextFactory<PdksDbContext> fabrika, string ad)
    {
        await using var db = await fabrika.CreateDbContextAsync();
        db.FirmaFiltresiKapali = true;
        var firmaId = await db.Firmalar.Select(f => f.Id).SingleAsync();
        db.FirmaFiltresiKapali = false;
        db.AktifFirmaId = firmaId;
        var grup = await db.CalismaGruplari.FirstAsync();
        db.Personeller.Add(new Personel { KartNo = "K" + Guid.NewGuid().ToString("N")[..8], Ad = ad, Soyad = "Test", IseGirisTarihi = new DateOnly(2026, 1, 1), CalismaGrubuId = grup.Id });
        await db.SaveChangesAsync();
    }

    [Fact]
    public async Task Her_firma_kendi_klasorunde_kendi_verisini_gorur()
    {
        _ = _fabrika.Server; // başlangıç kurulumu (ALFA) çalışsın
        var fv = _fabrika.Services.GetRequiredService<IFirmaVeritabanlari>();
        Assert.True(fv.AyriVeritabani);

        // İkinci firma: ayrı veritabanı açılır, kurulum yapılır.
        await fv.FirmaEkleAsync("beta", "Beta Ltd.");
        var sifre = _fabrika.Services.GetRequiredService<SifreServisi>();
        await new KurulumServisi(fv.Fabrika("BETA"), sifre).CalistirAsync("BETA", "Beta Ltd.", FirmaBasinaFabrikasi.Sifre);

        Assert.True(File.Exists(Path.Combine(_fabrika.Klasor, "alfa", "pdks.db")));
        Assert.True(File.Exists(Path.Combine(_fabrika.Klasor, "beta", "pdks.db")));

        await PersonelEkleAsync(fv.Fabrika("ALFA"), "AlfaKisisi");
        await PersonelEkleAsync(fv.Fabrika("BETA"), "BetaKisisi");

        // Veri gerçekten ayrı dosyalarda.
        await using (var a = await fv.Fabrika("ALFA").CreateDbContextAsync())
        {
            a.FirmaFiltresiKapali = true;
            Assert.False(await a.Personeller.AnyAsync(p => p.Ad == "BetaKisisi"));
        }

        var ca = Istemci();
        Assert.Equal("/personeller", (await GirisYapAsync(ca, "alfa", FirmaBasinaFabrikasi.Sifre)).Headers.Location!.ToString());
        var alfa = WebUtility.HtmlDecode(await ca.GetStringAsync("/personeller"));
        Assert.Contains("AlfaKisisi", alfa);
        Assert.DoesNotContain("BetaKisisi", alfa);

        var cb = Istemci();
        Assert.Equal("/personeller", (await GirisYapAsync(cb, "BETA", FirmaBasinaFabrikasi.Sifre)).Headers.Location!.ToString());
        var beta = WebUtility.HtmlDecode(await cb.GetStringAsync("/personeller"));
        Assert.Contains("BetaKisisi", beta);
        Assert.DoesNotContain("AlfaKisisi", beta);

        // Rapor ve yedek de yalnızca oturumdaki firmanın veritabanından gelir.
        var r = await cb.GetAsync($"/raporlar/toplu-puantaj.xlsx?yil=2026&ay=1");
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        var yedek = await cb.GetAsync("/araclar/yedek");
        Assert.Equal(HttpStatusCode.OK, yedek.StatusCode);
    }

    [Fact]
    public async Task Olmayan_firma_kodu_ile_giris_reddedilir()
    {
        var c = Istemci();
        var r = await GirisYapAsync(c, "YOKFIRMA", FirmaBasinaFabrikasi.Sifre);
        Assert.Contains("hata=", r.Headers.Location!.ToString());
        var r2 = await GirisYapAsync(c, "../x", FirmaBasinaFabrikasi.Sifre);
        Assert.Contains("hata=", r2.Headers.Location!.ToString());
    }

    [Fact]
    public async Task Masaustu_veritabaniyla_firma_acilir_pasif_firma_giremez_firma_ekrani_yonetime_acik()
    {
        _ = _fabrika.Server;
        var fv = _fabrika.Services.GetRequiredService<IFirmaVeritabanlari>();

        // Masaüstü sürümünün pdks.db'si gibi ayrı bir veritabanı hazırla.
        var kaynak = Path.Combine(_fabrika.Klasor, "masaustu.db");
        using (var tv = new TestVeritabani(kaynak))
        {
            await new KurulumServisi(tv, new SifreServisi()).CalistirAsync("ESKI", "Eski Firma", "Masaustu123");
        }
        Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
        await using (var akis = File.OpenRead(kaynak))
            await fv.FirmaEkleAsync("gama", "Gama A.Ş.", akis);
        await using (var bozuk = new MemoryStream(System.Text.Encoding.ASCII.GetBytes("bu bir veritabani degil")))
            await Assert.ThrowsAsync<ArgumentException>(() => fv.FirmaEkleAsync("delta", "Delta", bozuk));

        var c = Istemci();
        Assert.Equal("/personeller", (await GirisYapAsync(c, "GAMA", "Masaustu123")).Headers.Location!.ToString());

        await fv.AktiflikAsync("GAMA", false);
        Assert.Contains("hata=", (await GirisYapAsync(Istemci(), "GAMA", "Masaustu123")).Headers.Location!.ToString());
        await fv.AktiflikAsync("GAMA", true);

        // Firma işlemleri ekranı yalnız yönetim firmasının (ALFA) yöneticisine açık.
        var alfa = Istemci();
        await GirisYapAsync(alfa, "ALFA", FirmaBasinaFabrikasi.Sifre);
        Assert.Equal(HttpStatusCode.OK, (await alfa.GetAsync("/sistem/firmalar")).StatusCode);
        Assert.NotEqual(HttpStatusCode.OK, (await c.GetAsync("/sistem/firmalar")).StatusCode);
    }

    [Fact]
    public void Firma_kodu_normallestirilir_ve_tehlikeli_karakter_reddedilir()
    {
        Assert.Equal("ISIK_01", FirmaKodu.Normallestir(" ışık_01 "));
        Assert.Throws<ArgumentException>(() => FirmaKodu.Normallestir("a/b"));
        Assert.Throws<ArgumentException>(() => FirmaKodu.Normallestir("x"));
        Assert.Throws<ArgumentException>(() => FirmaKodu.Normallestir("abc;drop"));
    }
}
