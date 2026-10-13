using BiltekPdks.Infrastructure.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;

namespace BiltekPdks.Web.Services;

/// <summary>Oturumdaki kullanıcının firmasına kilitli veritabanı bağlamı açar.</summary>
public class FirmaDb
{
    private readonly IDbContextFactory<PdksDbContext> _factory;
    private readonly AuthenticationStateProvider _auth;

    public FirmaDb(IDbContextFactory<PdksDbContext> factory, AuthenticationStateProvider auth)
    {
        _factory = factory;
        _auth = auth;
    }

    public async Task<PdksDbContext> AcAsync()
    {
        var durum = await _auth.GetAuthenticationStateAsync();
        var db = await _factory.CreateDbContextAsync();
        db.AktifFirmaId = int.TryParse(durum.User.FindFirst(Talepler.FirmaId)?.Value, out var id) ? id : null;
        return db;
    }

    /// <summary>Oturumdaki kullanıcının id'si (kayıtlara "kim yaptı" bilgisi yazmak için).</summary>
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
}
