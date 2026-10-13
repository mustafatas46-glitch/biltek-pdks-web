using BiltekPdks.Domain.Entities;
using BiltekPdks.Infrastructure.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace BiltekPdks.Tests;

/// <summary>Her test için ayrı, bellekte çalışan SQLite veritabanı.</summary>
public sealed class TestVeritabani : IDbContextFactory<PdksDbContext>, IDisposable
{
    private readonly SqliteConnection _baglanti;
    private readonly DbContextOptions<PdksDbContext> _secenekler;

    public TestVeritabani()
    {
        _baglanti = new SqliteConnection("Data Source=:memory:");
        _baglanti.Open();
        _secenekler = new DbContextOptionsBuilder<PdksDbContext>().UseSqlite(_baglanti).Options;
        using var db = new PdksDbContext(_secenekler);
        db.Database.EnsureCreated();
    }

    public PdksDbContext CreateDbContext() => new(_secenekler);

    public PdksDbContext Db(int firmaId) => new(_secenekler) { AktifFirmaId = firmaId };

    public PdksDbContext Sistem() => new(_secenekler) { FirmaFiltresiKapali = true };

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
