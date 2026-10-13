using System.Net;
using System.Text.RegularExpressions;
using BiltekPdks.Domain.Entities;
using BiltekPdks.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BiltekPdks.Tests;

/// <summary>Uygulamayı gerçekten ayağa kaldırır: giriş yapar ve tüm sayfaları açar.</summary>
public sealed class UygulamaFabrikasi : WebApplicationFactory<Program>
{
    public const string Sifre = "TestSifre123";
    private readonly string _dosya = Path.Combine(Path.GetTempPath(), $"pdks-test-{Guid.NewGuid():N}.db");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.UseSetting("ConnectionStrings:Pdks", $"Data Source={_dosya}");
        builder.UseSetting("Veritabani:Saglayici", "Sqlite");
        builder.UseSetting("Kurulum:FirmaKodu", "TEST");
        builder.UseSetting("Kurulum:FirmaAdi", "Test A.Ş.");
        builder.UseSetting("Kurulum:YoneticiSifresi", Sifre);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
        try { File.Delete(_dosya); } catch (IOException) { }
    }
}

public class UygulamaTestleri : IClassFixture<UygulamaFabrikasi>
{
    private readonly UygulamaFabrikasi _fabrika;

    public UygulamaTestleri(UygulamaFabrikasi fabrika) => _fabrika = fabrika;

    private HttpClient Istemci() => _fabrika.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

    private static async Task<string> TokenAsync(HttpClient c)
    {
        var html = await c.GetStringAsync("/hesap/giris");
        var m = Regex.Match(html, "name=\"__RequestVerificationToken\"[^>]*value=\"([^\"]+)\"");
        if (!m.Success) m = Regex.Match(html, "value=\"([^\"]+)\"[^>]*name=\"__RequestVerificationToken\"");
        Assert.True(m.Success, "Giriş sayfasında antiforgery token bulunamadı.");
        return WebUtility.HtmlDecode(m.Groups[1].Value);
    }

    private static async Task<HttpResponseMessage> GirisYapAsync(HttpClient c, string sifre)
    {
        var token = await TokenAsync(c);
        return await c.PostAsync("/hesap/giris", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["firmaKodu"] = "",
            ["kullaniciAdi"] = "admin",
            ["sifre"] = sifre,
            ["donus"] = "/personeller",
        }));
    }

    [Fact]
    public async Task Oturum_acmadan_sayfalar_giris_ekranina_yonlendirir()
    {
        var c = Istemci();
        var r = await c.GetAsync("/personeller");
        Assert.Equal(HttpStatusCode.Redirect, r.StatusCode);
        Assert.Contains("/hesap/giris", r.Headers.Location!.ToString());
    }

    [Fact]
    public async Task Yanlis_sifre_ile_giris_reddedilir()
    {
        var c = Istemci();
        var r = await GirisYapAsync(c, "yanlis-sifre-1");
        Assert.Equal(HttpStatusCode.Redirect, r.StatusCode);
        Assert.Contains("hata=", r.Headers.Location!.ToString());
    }

    [Fact]
    public async Task Giris_yapan_kullanici_tum_sayfalari_hatasiz_acar()
    {
        int personelId;
        using (var scope = _fabrika.Services.CreateScope())
        {
            var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<PdksDbContext>>();
            await using var sistem = await factory.CreateDbContextAsync();
            sistem.FirmaFiltresiKapali = true;
            var firmaId = await sistem.Firmalar.Select(f => f.Id).SingleAsync();

            await using var db = await factory.CreateDbContextAsync();
            db.AktifFirmaId = firmaId;
            var grup = await db.CalismaGruplari.FirstAsync();
            var p = new Personel { KartNo = "T" + Guid.NewGuid().ToString("N")[..8], Ad = "Deneme", Soyad = "Personel", IseGirisTarihi = new DateOnly(2026, 1, 1), CalismaGrubuId = grup.Id };
            db.Personeller.Add(p);
            await db.SaveChangesAsync();
            db.Hareketler.Add(new Hareket { PersonelId = p.Id, Zaman = DateTime.Today.AddDays(-1).AddHours(8) });
            await db.SaveChangesAsync();
            personelId = p.Id;
        }

        var c = Istemci();
        var giris = await GirisYapAsync(c, UygulamaFabrikasi.Sifre);
        Assert.Equal(HttpStatusCode.Redirect, giris.StatusCode);
        Assert.Equal("/personeller", giris.Headers.Location!.ToString());

        var sayfalar = new[]
        {
            "/", $"/?personel={personelId}", "/ozet", "/personeller", "/personeller/yeni", $"/personeller/{personelId}",
            "/tanimlar/sube", "/tanimlar/bolum", "/tanimlar/gorev", "/tanimlar/grup", "/tanimlar/izin",
            "/vardiyalar", "/calisma-plani", "/resmi-tatiller", "/izinler", "/hareketler",
            "/puantaj", $"/puantaj?personel={personelId}", "/hesap/sifre",
        };

        foreach (var s in sayfalar)
        {
            var r = await c.GetAsync(s);
            var govde = await r.Content.ReadAsStringAsync();
            Assert.True(r.StatusCode == HttpStatusCode.OK, $"{s} -> {(int)r.StatusCode}\n{govde[..Math.Min(govde.Length, 2000)]}");
        }

        var ana = WebUtility.HtmlDecode(await c.GetStringAsync($"/?personel={personelId}"));
        Assert.Contains("Giriş/Çıkış", ana);
        Assert.Contains("Deneme", ana);
        Assert.Contains("Göster [F8]", ana);

        var puantaj = WebUtility.HtmlDecode(await c.GetStringAsync($"/puantaj?personel={personelId}"));
        Assert.Contains("Aylık puantaj", puantaj);
        Assert.Contains("Çalışılan gün", puantaj);
        Assert.Contains("Eksik hareket", puantaj); // dünkü tek okutma
        Assert.Contains("Deneme Personel", puantaj);
    }
}
