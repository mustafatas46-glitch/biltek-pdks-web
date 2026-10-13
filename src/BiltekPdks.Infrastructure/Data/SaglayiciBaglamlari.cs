using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BiltekPdks.Infrastructure.Data;

// SQLite (yerel kurulum) ve MySQL (sunucu/bulut) aynı modeli kullanır ama migration'ları ayrıdır.
// Migration'lar CI'daki "migration-olustur" işiyle üretilir: Migrations/Sqlite ve Migrations/MySql.

public sealed class SqlitePdksDbContext : PdksDbContext
{
    public SqlitePdksDbContext(DbContextOptions<SqlitePdksDbContext> options) : base(options) { }
}

public sealed class MySqlPdksDbContext : PdksDbContext
{
    public MySqlPdksDbContext(DbContextOptions<MySqlPdksDbContext> options) : base(options) { }
}

/// <summary>Yalnızca "dotnet ef migrations add" sırasında kullanılır; veritabanına bağlanmaz.</summary>
public sealed class SqliteTasarimFabrikasi : IDesignTimeDbContextFactory<SqlitePdksDbContext>
{
    public SqlitePdksDbContext CreateDbContext(string[] args) =>
        new(new DbContextOptionsBuilder<SqlitePdksDbContext>().UseSqlite("Data Source=tasarim.db").Options);
}

public sealed class MySqlTasarimFabrikasi : IDesignTimeDbContextFactory<MySqlPdksDbContext>
{
    public MySqlPdksDbContext CreateDbContext(string[] args) =>
        new(new DbContextOptionsBuilder<MySqlPdksDbContext>()
            .UseMySql("Server=localhost;Database=tasarim;User=tasarim;Password=tasarim", VeritabaniAyarlari.VarsayilanMySqlSurumu)
            .Options);
}

/// <summary>Alt sınıf fabrikasını uygulamanın kullandığı ortak <see cref="IDbContextFactory{PdksDbContext}"/> olarak sunar.</summary>
internal sealed class FabrikaAdaptoru<T> : IDbContextFactory<PdksDbContext> where T : PdksDbContext
{
    private readonly IDbContextFactory<T> _ic;
    public FabrikaAdaptoru(IDbContextFactory<T> ic) => _ic = ic;
    public PdksDbContext CreateDbContext() => _ic.CreateDbContext();
}

public static class VeritabaniAyarlari
{
    public static readonly ServerVersion VarsayilanMySqlSurumu = ServerVersion.Parse("8.0.36-mysql");

    public static bool SqliteMi(string? saglayici) =>
        string.Equals(saglayici, "Sqlite", StringComparison.OrdinalIgnoreCase);

    public static void SqliteAyarla(DbContextOptionsBuilder o, string baglanti) => o.UseSqlite(baglanti);

    public static void MySqlAyarla(DbContextOptionsBuilder o, string baglanti, ServerVersion? surum = null) =>
        o.UseMySql(baglanti, surum ?? VarsayilanMySqlSurumu, m => m.EnableRetryOnFailure());
}
