using System.Security.Claims;
using BiltekPdks.Infrastructure.Security;
using BiltekPdks.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace BiltekPdks.Web.Endpoints;

public static class HesapEndpoints
{
    public static void MapHesapEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/hesap/giris", async (
            HttpContext http,
            GirisServisi giris,
            [FromForm] string? firmaKodu,
            [FromForm] string? kullaniciAdi,
            [FromForm] string? sifre,
            [FromForm] string? donus) =>
        {
            var sonuc = await giris.DogrulaAsync(firmaKodu, kullaniciAdi ?? "", sifre ?? "", http.RequestAborted);
            if (!sonuc.Basarili)
            {
                var url = $"/hesap/giris?hata={Uri.EscapeDataString(sonuc.Hata!)}&firma={Uri.EscapeDataString(firmaKodu ?? "")}";
                if (YerelMi(donus)) url += $"&donus={Uri.EscapeDataString(donus!)}";
                return Results.LocalRedirect(url);
            }

            var k = sonuc.Kullanici!;
            var talepler = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, k.Id.ToString()),
                new Claim(ClaimTypes.Name, k.AdSoyad),
                new Claim(ClaimTypes.Role, k.Rol.ToString()),
                new Claim(Talepler.KullaniciAdi, k.KullaniciAdi),
                new Claim(Talepler.FirmaId, k.FirmaId.ToString()),
                new Claim(Talepler.FirmaKodu, sonuc.FirmaKodu ?? ""),
            };
            // Ekran yetkileri oturuma yazılır; değişiklik bir sonraki girişte geçerli olur.
            talepler.AddRange((sonuc.Yetkiler ?? Array.Empty<string>()).Select(y => new Claim(Talepler.Yetki, y)));
            var kimlik = new ClaimsIdentity(talepler, CookieAuthenticationDefaults.AuthenticationScheme);

            await http.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(kimlik));
            return Results.LocalRedirect(YerelMi(donus) ? donus! : "/");
        });

        app.MapPost("/hesap/cikis", async (HttpContext http) =>
        {
            await http.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Results.LocalRedirect("/hesap/giris");
        });
    }

    /// <summary>Açık yönlendirme açığını önlemek için yalnızca site içi adreslere izin verir.</summary>
    internal static bool YerelMi(string? url) =>
        !string.IsNullOrEmpty(url) && url.StartsWith('/') && !url.StartsWith("//") && !url.StartsWith("/\\");
}
