using BiltekPdks.Domain.Entities;
using BiltekPdks.Infrastructure.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace BiltekPdks.Web.Endpoints;

public static class AracEndpoints
{
    public static void MapAracEndpoints(this IEndpointRouteBuilder app)
    {
        // Araçlar → Data Yedekle: veritabanının tutarlı bir kopyasını indirir (yalnızca yönetici).
        app.MapGet("/araclar/yedek", async (HttpContext http, IDbContextFactory<PdksDbContext> factory) =>
        {
            await using var db = await factory.CreateDbContextAsync(http.RequestAborted);
            if (!db.Database.IsSqlite())
                return Results.Text("Sunucu (MySQL) kurulumunda yedek, veritabanı sunucusunun kendi yedekleme aracıyla alınır.", statusCode: 400);

            var gecici = Path.Combine(Path.GetTempPath(), $"pdks-yedek-{Guid.NewGuid():N}.db");
            try
            {
                // VACUUM INTO, program çalışırken bile tutarlı bir kopya üretir.
                var hedef = new SqliteConnectionStringBuilder { DataSource = gecici }.DataSource.Replace("'", "''");
                await db.Database.ExecuteSqlRawAsync($"VACUUM INTO '{hedef}'", http.RequestAborted);
                var icerik = await File.ReadAllBytesAsync(gecici, http.RequestAborted);
                return Results.File(icerik, "application/octet-stream", $"pdks-yedek-{DateTime.Now:yyyy-MM-dd-HHmm}.db");
            }
            finally
            {
                try { File.Delete(gecici); } catch (IOException) { }
            }
        }).RequireAuthorization(p => p.RequireRole(nameof(KullaniciRolu.Yonetici)));
    }
}
