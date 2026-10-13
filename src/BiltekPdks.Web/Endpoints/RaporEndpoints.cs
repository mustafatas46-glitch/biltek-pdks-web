using System.Security.Claims;
using BiltekPdks.Infrastructure.Data;
using BiltekPdks.Infrastructure.Puantaj;
using BiltekPdks.Infrastructure.Raporlar;
using BiltekPdks.Web.Services;
using Microsoft.EntityFrameworkCore;

namespace BiltekPdks.Web.Endpoints;

public static class RaporEndpoints
{
    public static void MapRaporEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/raporlar/toplu-puantaj.xlsx", async (
            HttpContext http,
            IDbContextFactory<PdksDbContext> factory,
            PuantajServisi puantaj,
            int yil, int ay, int? bolum, int? grup) =>
        {
            if (ay is < 1 or > 12 || yil is < 2000 or > 2100) return Results.BadRequest("Geçersiz ay.");
            if (!int.TryParse(http.User.FindFirstValue(Talepler.FirmaId), out var firmaId)) return Results.Forbid();

            await using var db = await factory.CreateDbContextAsync(http.RequestAborted);
            db.AktifFirmaId = firmaId;
            var firmaAdi = await db.Firmalar.Where(f => f.Id == firmaId).Select(f => f.Ad).FirstOrDefaultAsync(http.RequestAborted) ?? "";
            var satirlar = await puantaj.TopluAsync(db, yil, ay, bolum, grup, http.RequestAborted);
            var dosya = TopluPuantajExcel.Olustur(satirlar, yil, ay, firmaAdi);
            return Results.File(dosya,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"toplu-puantaj-{yil}-{ay:00}.xlsx");
        }).RequireAuthorization();

        app.MapGet("/raporlar/kisisel-puantaj.xlsx", async (
            HttpContext http,
            IDbContextFactory<PdksDbContext> factory,
            PuantajServisi puantaj,
            int personel, int yil, int ay) =>
        {
            if (ay is < 1 or > 12 || yil is < 2000 or > 2100) return Results.BadRequest("Geçersiz ay.");
            if (!int.TryParse(http.User.FindFirstValue(Talepler.FirmaId), out var firmaId)) return Results.Forbid();

            await using var db = await factory.CreateDbContextAsync(http.RequestAborted);
            db.AktifFirmaId = firmaId;
            var sonuc = await puantaj.AylikAsync(db, personel, yil, ay, http.RequestAborted);
            if (sonuc is null) return Results.NotFound();
            var firmaAdi = await db.Firmalar.Where(f => f.Id == firmaId).Select(f => f.Ad).FirstOrDefaultAsync(http.RequestAborted) ?? "";
            var ad = $"{sonuc.Personel.Ad}-{sonuc.Personel.Soyad}".Replace(' ', '-');
            return Results.File(KisiselPuantajExcel.Olustur(sonuc, firmaAdi),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"puantaj-{ad}-{yil}-{ay:00}.xlsx");
        }).RequireAuthorization();
    }
}
