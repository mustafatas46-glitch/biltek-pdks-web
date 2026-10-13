using Microsoft.EntityFrameworkCore;

namespace BiltekPdks.Infrastructure.Data;

/// <summary>
/// Veritabanını en son sürüme getirir (migration'ları uygular). Veri silinmez.
/// İlk sürümlerde EnsureCreated ile oluşturulmuş SQLite dosyaları "ilk şema" olarak işaretlenip
/// sonraki migration'lar üzerine uygulanır.
/// </summary>
public static class VeritabaniGuncelleyici
{
    public static async Task GuncelleAsync(PdksDbContext db, CancellationToken ct = default)
    {
        if (db.Database.IsSqlite())
            await EskiSqliteKurulumunuIsaretleAsync(db, ct);
        await db.Database.MigrateAsync(ct);
    }

    private static async Task EskiSqliteKurulumunuIsaretleAsync(PdksDbContext db, CancellationToken ct)
    {
        var baglanti = db.Database.GetDbConnection();
        await db.Database.OpenConnectionAsync(ct);
        try
        {
            async Task<bool> TabloVarMiAsync(string ad)
            {
                await using var k = baglanti.CreateCommand();
                k.CommandText = "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name=$ad";
                var p = k.CreateParameter();
                p.ParameterName = "$ad";
                p.Value = ad;
                k.Parameters.Add(p);
                return Convert.ToInt64(await k.ExecuteScalarAsync(ct)) > 0;
            }

            if (!await TabloVarMiAsync("personel") || await TabloVarMiAsync("__EFMigrationsHistory"))
                return;

            var ilkMigration = db.Database.GetMigrations().First();
            await using var komut = baglanti.CreateCommand();
            komut.CommandText =
                "CREATE TABLE \"__EFMigrationsHistory\" (\"MigrationId\" TEXT NOT NULL CONSTRAINT \"PK___EFMigrationsHistory\" PRIMARY KEY, \"ProductVersion\" TEXT NOT NULL);" +
                "INSERT INTO \"__EFMigrationsHistory\" (\"MigrationId\", \"ProductVersion\") VALUES ($id, '8.0.11');";
            var id = komut.CreateParameter();
            id.ParameterName = "$id";
            id.Value = ilkMigration;
            komut.Parameters.Add(id);
            await komut.ExecuteNonQueryAsync(ct);
        }
        finally
        {
            await db.Database.CloseConnectionAsync();
        }
    }
}
