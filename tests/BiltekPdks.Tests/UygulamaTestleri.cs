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
            "/vardiyalar", "/vardiyalar?sekme=mola", "/grup-tanimlama", "/calisma-plani", "/resmi-tatiller", "/izinler", "/hareketler",
            "/puantaj", $"/puantaj?personel={personelId}", "/puantaj/toplu", "/hesap/sifre", "/toplu/giris-cikis", "/hakkinda", "/toplu/izin", "/toplu/kazanc-kesinti", "/tanimlar/kazanc-kesinti",
            "/tanimlar/servis", "/tanimlar/bolge", "/tanimlar/birim", "/tanimlar/takim", "/tanimlar/meslek",
            "/araclar/excel-aktarim", "/tanimlar/kullanicilar", "/raporlar/maas-pusulasi", $"/raporlar/maas-pusulasi?personel={personelId}", "/raporlar/maas-listesi.xlsx?yil=2026&ay=10", "/raporlar/bordro-uyum", "/raporlar/bordro-uyum.xlsx?yil=2026&ay=9", "/araclar/aktarim-ornegi.xlsx?tur=Hareket",
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

        var kart = WebUtility.HtmlDecode(await c.GetStringAsync($"/personeller/{personelId}"));
        Assert.Contains("Personel Sicil Kartı", kart);
        Assert.Contains("Kazanç-Kesinti Bilgileri", kart);

        var toplu = WebUtility.HtmlDecode(await c.GetStringAsync("/puantaj/toplu"));
        Assert.Contains("Toplu aylık puantaj", toplu);
        Assert.Contains("Deneme Personel", toplu);

        var dun = DateTime.Today.AddDays(-1);
        var kisisel = await c.GetAsync($"/raporlar/kisisel-puantaj.xlsx?personel={personelId}&yil={dun.Year}&ay={dun.Month}");
        Assert.Equal(HttpStatusCode.OK, kisisel.StatusCode);
        Assert.Contains("Kişisel puantaj [F9]", ana);
        Assert.Contains("Rapor [F10]", ana);

        var excel = await c.GetAsync($"/raporlar/toplu-puantaj.xlsx?yil={dun.Year}&ay={dun.Month}");
        Assert.Equal(HttpStatusCode.OK, excel.StatusCode);
        Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excel.Content.Headers.ContentType?.MediaType);
        using var kitap = new ClosedXML.Excel.XLWorkbook(await excel.Content.ReadAsStreamAsync());
        Assert.Contains(kitap.Worksheet(1).CellsUsed(), x => x.GetString() == "Deneme Personel");
    }

    [Fact]
    public async Task Yonetici_veritabani_yedegini_indirebilir()
    {
        var c = Istemci();
        await GirisYapAsync(c, UygulamaFabrikasi.Sifre);
        var r = await c.GetAsync("/araclar/yedek");
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        var bayt = await r.Content.ReadAsByteArrayAsync();
        Assert.StartsWith("SQLite format 3", System.Text.Encoding.ASCII.GetString(bayt, 0, 15));
    }

    [Fact]
    public async Task Excel_raporu_oturumsuz_indirilemez()
    {
        var r = await Istemci().GetAsync("/raporlar/toplu-puantaj.xlsx?yil=2026&ay=10");
        Assert.NotEqual(HttpStatusCode.OK, r.StatusCode);
    }

    [Fact]
    public async Task Yetkisiz_kullanici_ucret_ekranlarina_giremez_yetkili_ekrana_girer()
    {
        using (var scope = _fabrika.Services.CreateScope())
        {
            var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<PdksDbContext>>();
            await using var sistem = await factory.CreateDbContextAsync();
            sistem.FirmaFiltresiKapali = true;
            var firmaId = await sistem.Firmalar.Select(f => f.Id).SingleAsync();
            if (!await sistem.Kullanicilar.AnyAsync(k => k.KullaniciAdi == "puantajci"))
            {
                await using var db = await factory.CreateDbContextAsync();
                db.AktifFirmaId = firmaId;
                var b = new BiltekPdks.Infrastructure.Security.KullaniciBilgisi
                {
                    KullaniciAdi = "puantajci", AdSoyad = "Puantaj Memuru", Rol = KullaniciRolu.InsanKaynaklari, OzelYetki = true,
                    Yetkiler = new() { Yetkiler.PuantajGor }, YeniSifre = UygulamaFabrikasi.Sifre,
                };
                Assert.Null(await BiltekPdks.Infrastructure.Security.KullaniciServisi.KaydetAsync(db, b, new BiltekPdks.Infrastructure.Security.SifreServisi(), null));
            }
        }

        var c = Istemci();
        var token = await TokenAsync(c);
        var giris = await c.PostAsync("/hesap/giris", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token, ["firmaKodu"] = "", ["kullaniciAdi"] = "puantajci",
            ["sifre"] = UygulamaFabrikasi.Sifre, ["donus"] = "/puantaj/toplu",
        }));
        Assert.Equal("/puantaj/toplu", giris.Headers.Location!.ToString());

        Assert.Equal(HttpStatusCode.OK, (await c.GetAsync("/raporlar/toplu-puantaj.xlsx?yil=2026&ay=10")).StatusCode);
        Assert.NotEqual(HttpStatusCode.OK, (await c.GetAsync("/raporlar/maas-listesi.xlsx?yil=2026&ay=10")).StatusCode);
        Assert.NotEqual(HttpStatusCode.OK, (await c.GetAsync("/araclar/yedek")).StatusCode);
        var maas = await c.GetAsync("/raporlar/maas-pusulasi");
        Assert.Equal(HttpStatusCode.Redirect, maas.StatusCode);
        Assert.Contains("/hesap/yetkisiz", maas.Headers.Location!.ToString());
        Assert.Contains("yetkiniz yok", WebUtility.HtmlDecode(await c.GetStringAsync("/hesap/yetkisiz")));
        var toplu = WebUtility.HtmlDecode(await c.GetStringAsync("/puantaj/toplu"));
        Assert.DoesNotContain("yetkiniz yok", toplu);
        Assert.DoesNotContain("Maaş Pusulası</a>", toplu); // menüde görünmez
    }
}
