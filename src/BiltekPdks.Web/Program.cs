using BiltekPdks.Infrastructure;
using BiltekPdks.Infrastructure.Data;
using BiltekPdks.Infrastructure.Seed;
using BiltekPdks.Web.Components;
using BiltekPdks.Web.Endpoints;
using BiltekPdks.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.WebEncoders;
using System.Text.Encodings.Web;
using System.Text.Unicode;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents().AddInteractiveServerComponents();
// Türkçe karakterler HTML'de &#x131; gibi kodlanmadan, olduğu gibi yazılsın.
builder.Services.Configure<WebEncoderOptions>(o => o.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All));
builder.Services.AddPdksAltyapi(builder.Configuration);
builder.Services.AddScoped<FirmaDb>();

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
builder.Services.AddAuthorization();
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
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();

static async Task VeritabaniniHazirlaAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var sp = scope.ServiceProvider;
    var log = sp.GetRequiredService<ILoggerFactory>().CreateLogger("Kurulum");

    await using (var db = await sp.GetRequiredService<IDbContextFactory<PdksDbContext>>().CreateDbContextAsync())
    {
        // Veritabanını en son sürüme getirir; mevcut veriler korunur.
        await VeritabaniGuncelleyici.GuncelleAsync(db);
    }

    var cfg = app.Configuration;
    var sonuc = await sp.GetRequiredService<KurulumServisi>().CalistirAsync(
        cfg["Kurulum:FirmaKodu"] ?? "FIRMA",
        cfg["Kurulum:FirmaAdi"] ?? "Firmam",
        cfg["Kurulum:YoneticiSifresi"]);

    if (cfg.GetValue<bool>("Kurulum:DemoVeri") && await sp.GetRequiredService<DemoVeriServisi>().OlusturAsync())
        log.LogWarning("Tanıtım amaçlı hayali personel ve hareketler eklendi (Kurulum:DemoVeri).");

    if (sonuc.IlkKurulum)
    {
        log.LogWarning("İlk kurulum yapıldı. Firma kodu: {Firma}, kullanıcı: {Kullanici}", sonuc.FirmaKodu, sonuc.YoneticiKullaniciAdi);
        if (sonuc.UretilenSifre is not null)
            log.LogWarning("Yönetici için geçici şifre üretildi: {Sifre}  (ilk girişten sonra değiştirin)", sonuc.UretilenSifre);
    }
}

public partial class Program { }
