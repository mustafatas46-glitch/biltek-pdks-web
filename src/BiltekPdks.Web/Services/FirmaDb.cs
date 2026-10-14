using System.Security.Claims;
using BiltekPdks.Infrastructure.Data;
using BiltekPdks.Infrastructure.Firmalar;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;

namespace BiltekPdks.Web.Services;

/// <summary>Oturumdaki kullanıcının firmasına kilitli veritabanı bağlamı açar.</summary>
public class FirmaDb
{
    private readonly IFirmaVeritabanlari _veritabanlari;
    private readonly AuthenticationStateProvider _auth;

    public FirmaDb(IFirmaVeritabanlari veritabanlari, AuthenticationStateProvider auth)
    {
        _veritabanlari = veritabanlari;
        _auth = auth;
    }

    public async Task<PdksDbContext> AcAsync()
    {
        var durum = await _auth.GetAuthenticationStateAsync();
        var db = await _veritabanlari.Fabrika(durum.User).CreateDbContextAsync();
        db.AktifFirmaId = int.TryParse(durum.User.FindFirst(Talepler.FirmaId)?.Value, out var id) ? id : null;
        db.KisitlariUygula(durum.User);
        db.IslemYapanKullaniciId = int.TryParse(durum.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out var k) ? k : null;
        return db;
    }

    /// <summary>Oturumdaki kullanıcının id'si (kayıtlara "kim yaptı" bilgisi yazmak için).</summary>
    /// <summary>Oturumdaki kullanıcının firma kodu (ayrı veritabanı modunda veritabanını seçer).</summary>
    public async Task<string?> FirmaKoduAsync() => (await _auth.GetAuthenticationStateAsync()).User.FindFirst(Talepler.FirmaKodu)?.Value;

    public async Task<int?> KullaniciIdAsync()
    {
        var durum = await _auth.GetAuthenticationStateAsync();
        return int.TryParse(durum.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out var id) ? id : null;
    }
}

public static class Talepler
{
    public const string FirmaId = "firma_id";
    public const string KullaniciAdi = "kullanici_adi";
    public const string FirmaKodu = "firma_kodu";
    public const string Yetki = "yetki";
    public const string Bolum = "bolum";
}

public static class FirmaVeritabaniUzantilari
{
    /// <summary>Oturumdaki kullanıcının bölüm kısıtını bağlama uygular (talep yoksa kısıt yok).</summary>
    public static void KisitlariUygula(this PdksDbContext db, ClaimsPrincipal kullanici)
    {
        var bolumler = kullanici.FindAll(Talepler.Bolum).Select(c => int.TryParse(c.Value, out var b) ? b : 0).Where(b => b > 0).ToList();
        db.IzinliBolumler = bolumler.Count > 0 ? bolumler : null;
    }

    /// <summary>Oturumdaki kullanıcının firmasının veritabanı fabrikası.</summary>
    public static IDbContextFactory<PdksDbContext> Fabrika(this IFirmaVeritabanlari v, ClaimsPrincipal kullanici) =>
        v.AyriVeritabani ? v.Fabrika(kullanici.FindFirst(Talepler.FirmaKodu)?.Value ?? throw new FirmaBulunamadi(null)) : v.Fabrika((string?)null);
}
