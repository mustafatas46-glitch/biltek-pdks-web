using Microsoft.EntityFrameworkCore;

namespace BiltekPdks.Infrastructure.Firmalar;

/// <summary>Katalogdaki firma kaydı: yalnızca firmanın kimliği ve durumu. Personel verisi içermez.</summary>
public class KatalogFirma
{
    public int Id { get; set; }
    public string Kod { get; set; } = string.Empty;
    public string Ad { get; set; } = string.Empty;
    public bool Aktif { get; set; } = true;
    public DateTime OlusturmaZamani { get; set; }
}

/// <summary>Firma kataloğu (ana veritabanı). Hangi firmanın hangi veritabanında olduğunu bilir.</summary>
public class KatalogDbContext : DbContext
{
    public KatalogDbContext(DbContextOptions<KatalogDbContext> options) : base(options) { }

    public DbSet<KatalogFirma> Firmalar => Set<KatalogFirma>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<KatalogFirma>(e =>
        {
            e.ToTable("katalog_firma");
            e.Property(x => x.Kod).HasMaxLength(20).IsRequired();
            e.Property(x => x.Ad).HasMaxLength(200).IsRequired();
            e.HasIndex(x => x.Kod).IsUnique();
        });
    }
}
