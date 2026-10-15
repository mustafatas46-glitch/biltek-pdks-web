using BiltekPdks.Domain.Entities;
using BiltekPdks.Infrastructure.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace BiltekPdks.Tests;

/// <summary>Her test için ayrı, bellekte çalışan SQLite veritabanı.</summary>
public sealed class TestVeritabani : IDbContextFactory<PdksDbContext>, IDisposable
{
    private readonly SqliteConnection _baglanti;
    private readonly DbContextOptions<SqlitePdksDbContext> _secenekler;

    public TestVeritabani() : this(":memory:") { }

    /// <summary>Dosyaya yazan test veritabanı (ör. masaüstü pdks.db yerine).</summary>
    public TestVeritabani(string dosya)
    {
        _baglanti = new SqliteConnection($"Data Source={dosya}");
        _baglanti.Open();
        _secenekler = new DbContextOptionsBuilder<SqlitePdksDbContext>().UseSqlite(_baglanti).Options;
        using var db = new SqlitePdksDbContext(_secenekler);
        VeritabaniGuncelleyici.GuncelleAsync(db).GetAwaiter().GetResult();
    }

    public PdksDbContext CreateDbContext() => new SqlitePdksDbContext(_secenekler);

    public PdksDbContext Db(int firmaId) => new SqlitePdksDbContext(_secenekler) { AktifFirmaId = firmaId };

    public PdksDbContext Sistem() => new SqlitePdksDbContext(_secenekler) { FirmaFiltresiKapali = true };

    public int FirmaEkle(string kod)
    {
        using var db = Sistem();
        var f = new Firma { Kod = kod, Ad = kod + " A.Ş." };
        db.Firmalar.Add(f);
        db.SaveChanges();
        return f.Id;
    }

    public void Dispose() => _baglanti.Dispose();
}

public sealed class SabitSaat : TimeProvider
{
    public DateTimeOffset Simdi { get; set; } = new(2026, 10, 13, 9, 0, 0, TimeSpan.Zero);
    public override DateTimeOffset GetUtcNow() => Simdi;
}
