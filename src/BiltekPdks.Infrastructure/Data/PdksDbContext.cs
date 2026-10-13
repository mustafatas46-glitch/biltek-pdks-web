using BiltekPdks.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BiltekPdks.Infrastructure.Data;

/// <summary>
/// Ana veritabanı bağlamı. Firma izolasyonu burada sağlanır:
/// <list type="bullet">
/// <item>Sorgular yalnızca <see cref="AktifFirmaId"/> firmasının kayıtlarını döndürür.</item>
/// <item>Yeni kayıtlara FirmaId otomatik yazılır; başka firmaya kayıt yazmak engellenir.</item>
/// </list>
/// </summary>
public class PdksDbContext : DbContext
{
    public PdksDbContext(DbContextOptions<PdksDbContext> options) : base(options) { }

    /// <summary>Oturumdaki kullanıcının firması. Null ise firma kayıtları görünmez.</summary>
    public int? AktifFirmaId { get; set; }

    /// <summary>
    /// Yalnızca sistem işleri için (giriş doğrulama, kurulum, aktarım).
    /// Açıkken firma filtresi uygulanmaz.
    /// </summary>
    public bool FirmaFiltresiKapali { get; set; }

    public DbSet<Firma> Firmalar => Set<Firma>();
    public DbSet<Kullanici> Kullanicilar => Set<Kullanici>();
    public DbSet<Sube> Subeler => Set<Sube>();
    public DbSet<Bolum> Bolumler => Set<Bolum>();
    public DbSet<Gorev> Gorevler => Set<Gorev>();
    public DbSet<CalismaGrubu> CalismaGruplari => Set<CalismaGrubu>();
    public DbSet<Personel> Personeller => Set<Personel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PdksDbContext).Assembly);

        FirmaKurallari<Kullanici>(modelBuilder);
        FirmaKurallari<Sube>(modelBuilder);
        FirmaKurallari<Bolum>(modelBuilder);
        FirmaKurallari<Gorev>(modelBuilder);
        FirmaKurallari<CalismaGrubu>(modelBuilder);
        FirmaKurallari<Personel>(modelBuilder);
    }

    private void FirmaKurallari<T>(ModelBuilder modelBuilder) where T : FirmaEntity
    {
        modelBuilder.Entity<T>()
            .HasQueryFilter(e => FirmaFiltresiKapali || e.FirmaId == AktifFirmaId);

        modelBuilder.Entity<T>()
            .HasOne<Firma>()
            .WithMany()
            .HasForeignKey(e => e.FirmaId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<T>().HasIndex(e => e.FirmaId);
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        KurallariUygula();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        KurallariUygula();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void KurallariUygula()
    {
        var simdi = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
                entry.Entity.OlusturmaTarihi = simdi;
            else if (entry.State == EntityState.Modified)
                entry.Entity.GuncellemeTarihi = simdi;
        }

        foreach (var entry in ChangeTracker.Entries<FirmaEntity>())
        {
            if (entry.State is not (EntityState.Added or EntityState.Modified or EntityState.Deleted))
                continue;

            if (entry.State == EntityState.Added && entry.Entity.FirmaId == 0)
            {
                entry.Entity.FirmaId = AktifFirmaId
                    ?? throw new FirmaIzolasyonHatasi("Firma seçilmeden kayıt eklenemez.");
                continue;
            }

            if (!FirmaFiltresiKapali && entry.Entity.FirmaId != AktifFirmaId)
                throw new FirmaIzolasyonHatasi(
                    $"{entry.Entity.GetType().Name} kaydı başka bir firmaya ait; işlem engellendi.");
        }
    }
}

public class FirmaIzolasyonHatasi : InvalidOperationException
{
    public FirmaIzolasyonHatasi(string message) : base(message) { }
}
