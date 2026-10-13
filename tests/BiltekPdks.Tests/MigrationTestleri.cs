#pragma warning disable EF1001 // Model farkı karşılaştırması EF'nin iç API'lerini kullanır.
using BiltekPdks.Domain.Entities;
using BiltekPdks.Infrastructure.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BiltekPdks.Tests;

public class MigrationTestleri
{
    /// <summary>Model değişip migration üretilmemişse bu test kırılır ("migration-olustur" işini çalıştırın).</summary>
    private static void FarkYokOlmali(PdksDbContext db)
    {
        var migrations = db.GetService<IMigrationsAssembly>();
        var snapshot = migrations.ModelSnapshot?.Model;
        Assert.NotNull(snapshot);
        if (snapshot is IMutableModel m) snapshot = m.FinalizeModel();
        snapshot = db.GetService<IModelRuntimeInitializer>().Initialize(snapshot!);
        var fark = db.GetService<IMigrationsModelDiffer>().HasDifferences(
            snapshot.GetRelationalModel(),
            db.GetService<IDesignTimeModel>().Model.GetRelationalModel());
        Assert.False(fark, $"{db.GetType().Name}: model değişmiş ama migration üretilmemiş.");
    }

    [Fact]
    public void Sqlite_modeli_migrationlarla_ayni() =>
        FarkYokOlmali(new SqliteTasarimFabrikasi().CreateDbContext(Array.Empty<string>()));

    [Fact]
    public void MySql_modeli_migrationlarla_ayni() =>
        FarkYokOlmali(new MySqlTasarimFabrikasi().CreateDbContext(Array.Empty<string>()));

    [Fact]
    public async Task Eski_surumle_kurulmus_veritabani_veri_kaybetmeden_guncellenir()
    {
        await using var baglanti = new SqliteConnection("Data Source=:memory:");
        await baglanti.OpenAsync();
        var secenek = new DbContextOptionsBuilder<SqlitePdksDbContext>().UseSqlite(baglanti).Options;

        // İlk dağıtılan paketler veritabanını EnsureCreated ile kuruyordu.
        await using (var eski = new SqlitePdksDbContext(secenek) { FirmaFiltresiKapali = true })
        {
            await eski.Database.EnsureCreatedAsync();
            eski.Firmalar.Add(new Firma { Kod = "ESKI", Ad = "Eski Kurulum" });
            await eski.SaveChangesAsync();
        }

        await using (var db = new SqlitePdksDbContext(secenek) { FirmaFiltresiKapali = true })
        {
            await VeritabaniGuncelleyici.GuncelleAsync(db);
            Assert.Empty(await db.Database.GetPendingMigrationsAsync());
            Assert.Equal("ESKI", (await db.Firmalar.SingleAsync()).Kod);
        }

        // İkinci çalıştırma da sorunsuz olmalı.
        await using (var db = new SqlitePdksDbContext(secenek))
            await VeritabaniGuncelleyici.GuncelleAsync(db);
    }
}
