using System.Security.Claims;
using BiltekPdks.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;

namespace BiltekPdks.Web.Components;

/// <summary>Bileşen kodunda yetki sorgulamak için (ör. ücret alanlarını gizlemek).</summary>
public sealed class YetkiKontrol
{
    private readonly AuthenticationStateProvider _auth;
    private readonly IAuthorizationService _yetki;

    public YetkiKontrol(AuthenticationStateProvider auth, IAuthorizationService yetki)
    {
        _auth = auth;
        _yetki = yetki;
    }

    public async Task<bool> VarMiAsync(string yetki)
    {
        var durum = await _auth.GetAuthenticationStateAsync();
        return (await _yetki.AuthorizeAsync(durum.User, yetki)).Succeeded;
    }
}
