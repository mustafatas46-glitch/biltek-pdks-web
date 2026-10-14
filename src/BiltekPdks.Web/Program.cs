using BiltekPdks.Domain.Entities;
using BiltekPdks.Infrastructure;
using BiltekPdks.Infrastructure.Data;
using BiltekPdks.Infrastructure.Firmalar;
using BiltekPdks.Infrastructure.Security;
using BiltekPdks.Infrastructure.Seed;
using BiltekPdks.Web.Components;
using BiltekPdks.Web.Endpoints;
using BiltekPdks.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.WebEncoders;
using System.Text.Encodings.Web;
using System.Text.Unicode;

// Paketlenmiş kurulumda program hangi klasörden başlatılırsa başlatılsın stil dosyaları (wwwroot) bulunsun.
var paketKlasoru = AppContext.BaseDirectory;
var builder = Directory.Exists(Path.Combine(paketKlasoru, "wwwroot"))
    ? WebApplication.CreateBuilder(new WebApplicationOptions { Args = args, ContentRootPath = paketKlasoru })
    : WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents().AddInteractiveServerComponents();
// Türkçe karakterler HTML'de &#x131; gibi kodlanmadan, olduğu gibi yazılsın.
builder.Services.Configure<WebEncoderOptions>(o => o.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All));
builder.Services.AddPdksAltyapi(builder.Configuration);
builder.Services.AddScoped<FirmaDb>();
builder.Services.AddScoped<BiltekPdks.Web.Components.YetkiKontrol>();

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.LoginPath = "/hesap/giris";
        o.AccessDeniedPath = "/hesap/giris";
        o.ReturnUrlParameter = "donus";
        o.Cookie.Name = "biltek.pdks";
        o.Cookie.HttpOnly = true;
        o.Cookie.SameSite = SameSiteMode.Lax;
        o.ExpireTimeSpan = TimeSpan.FromHours(10);
        o.SlidingExpiration = true;
    });
builder.Services.AddAuthorization(o =>
{
    // Her yetki bir politika: yönetici her şeye, diğerleri oturumdaki "yetki" taleplerine göre erişir.
    foreach (var y in Yetkiler.Hepsi)
        o.AddPolicy(y.Kod, p => p.RequireAuthenticatedUser().RequireAssertion(c =>
            c.User.IsInRole(nameof(KullaniciRolu.Yonetici)) || c.User.HasClaim(Talepler.Yetki, y.Kod)));
});
builder.Services.AddCascadingAuthenticationState();

var app = builder.Build();

await VeritabaniniHazirlaAsync(app);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/hata", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapHesapEndpoints();
app.MapRaporEndpoints();
app.MapAracEndpoints();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();

static async Task VeritabaniniHazirlaAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var sp = scope.ServiceProvider;
    var log = sp.GetRequiredService<ILoggerFactory>().CreateLogger("Kurulum");
    var cfg = app.Configuration;
    var kurulumKodu = cfg["Kurulum:FirmaKodu"] ?? "FIRMA";
    var kurulumAdi = cfg["Kurulum:FirmaAdi"] ?? "Firmam";
    var veritabanlari = sp.GetRequiredService<IFirmaVeritabanlari>();
    var sifre = sp.GetRequiredService<SifreServisi>();

    if (veritabanlari.AyriVeritabani)
    {
        // Her firmanın ayrı veritabanı: katalog boşsa ilk firma açılır, sonra tüm firmalar güncellenir.
        if ((await veritabanlari.FirmalarAsync()).Count == 0)
            await veritabanlari.FirmaEkleAsync(kurulumKodu, kurulumAdi);

        foreach (var f in await veritabanlari.FirmalarAsync())
        {
            if (!f.Aktif) continue;
            var fabrika = veritabanlari.Fabrika(f.Kod);
            await using (var db = await fabrika.CreateDbContextAsync())
                await VeritabaniGuncelleyici.GuncelleAsync(db);
            var s = await new KurulumServisi(fabrika, sifre).CalistirAsync(f.Kod, f.Ad, cfg["Kurulum:YoneticiSifresi"]);
            KurulumuBildir(log, s);
        }
        return;
    }

    await using (var db = await sp.GetRequiredService<IDbContextFactory<PdksDbContext>>().CreateDbContextAsync())
    {
        // Veritabanını en son sürüme getirir; mevcut veriler korunur.
        await VeritabaniGuncelleyici.GuncelleAsync(db);
    }

    var sonuc = await sp.GetRequiredService<KurulumServisi>().CalistirAsync(kurulumKodu, kurulumAdi, cfg["Kurulum:YoneticiSifresi"]);

    if (cfg.GetValue<bool>("Kurulum:DemoVeri") && await sp.GetRequiredService<DemoVeriServisi>().OlusturAsync())
        log.LogWarning("Tanıtım amaçlı hayali personel ve hareketler eklendi (Kurulum:DemoVeri).");

    KurulumuBildir(log, sonuc);
}

static void KurulumuBildir(ILogger log, KurulumSonucu sonuc)
{
    if (!sonuc.IlkKurulum) return;
    log.LogWarning("İlk kurulum yapıldı. Firma kodu: {Firma}, kullanıcı: {Kullanici}", sonuc.FirmaKodu, sonuc.YoneticiKullaniciAdi);
    if (sonuc.UretilenSifre is not null)
        log.LogWarning("Yönetici için geçici şifre üretildi: {Sifre}  (ilk girişten sonra değiştirin)", sonuc.UretilenSifre);
}

public partial class Program { }
