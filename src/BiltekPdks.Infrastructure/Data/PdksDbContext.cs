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

    /// <summary>Sağlayıcıya özel alt sınıflar (SQLite / MySQL) için; her birinin kendi migration'ları vardır.</summary>
    protected PdksDbContext(DbContextOptions options) : base(options) { }

    /// <summary>Oturumdaki kullanıcının firması. Null ise firma kayıtları görünmez.</summary>
    public int? AktifFirmaId { get; set; }

    /// <summary>
    /// Yalnızca sistem işleri için (giriş doğrulama, kurulum, aktarım).
    /// Açıkken firma filtresi uygulanmaz.
    /// </summary>
    public bool FirmaFiltresiKapali { get; set; }

    /// <summary>
    /// Doluysa yapılan her ekleme/değişiklik/silme <see cref="IslemKaydi"/> tablosuna yazılır.
    /// Ekranlar (FirmaDb) doldurur; toplu aktarım gibi sistem işleri doldurmaz.
    /// </summary>
    public int? IslemYapanKullaniciId { get; set; }

    private static readonly HashSet<string> GizliAlanlar = new() { nameof(Kullanici.SifreHash) };

    public DbSet<Firma> Firmalar => Set<Firma>();
    public DbSet<Kullanici> Kullanicilar => Set<Kullanici>();
    public DbSet<Sube> Subeler => Set<Sube>();
    public DbSet<Bolum> Bolumler => Set<Bolum>();
    public DbSet<Gorev> Gorevler => Set<Gorev>();
    public DbSet<CalismaGrubu> CalismaGruplari => Set<CalismaGrubu>();
    public DbSet<Personel> Personeller => Set<Personel>();
    public DbSet<Vardiya> Vardiyalar => Set<Vardiya>();
    public DbSet<VardiyaMola> VardiyaMolalari => Set<VardiyaMola>();
    public DbSet<CalismaGrubuGunu> CalismaGrubuGunleri => Set<CalismaGrubuGunu>();
    public DbSet<PersonelVardiya> PersonelVardiyalari => Set<PersonelVardiya>();
    public DbSet<ResmiTatil> ResmiTatiller => Set<ResmiTatil>();
    public DbSet<IzinTuru> IzinTurleri => Set<IzinTuru>();
    public DbSet<Izin> Izinler => Set<Izin>();
    public DbSet<Hareket> Hareketler => Set<Hareket>();
    public DbSet<Servis> Servisler => Set<Servis>();
    public DbSet<Bolge> Bolgeler => Set<Bolge>();
    public DbSet<Birim> Birimler => Set<Birim>();
    public DbSet<Takim> Takimlar => Set<Takim>();
    public DbSet<Meslek> Meslekler => Set<Meslek>();
    public DbSet<KazancKesintiTuru> KazancKesintiTurleri => Set<KazancKesintiTuru>();
    public DbSet<KazancKesinti> KazancKesintiler => Set<KazancKesinti>();
    public DbSet<IslemKaydi> IslemKayitlari => Set<IslemKaydi>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PdksDbContext).Assembly);

        FirmaKurallari<Kullanici>(modelBuilder);
        FirmaKurallari<Sube>(modelBuilder);
        FirmaKurallari<Bolum>(modelBuilder);
        FirmaKurallari<Gorev>(modelBuilder);
        FirmaKurallari<CalismaGrubu>(modelBuilder);
        FirmaKurallari<Personel>(modelBuilder);
        FirmaKurallari<Vardiya>(modelBuilder);
        FirmaKurallari<VardiyaMola>(modelBuilder);
        FirmaKurallari<CalismaGrubuGunu>(modelBuilder);
        FirmaKurallari<PersonelVardiya>(modelBuilder);
        FirmaKurallari<ResmiTatil>(modelBuilder);
        FirmaKurallari<IzinTuru>(modelBuilder);
        FirmaKurallari<Izin>(modelBuilder);
        FirmaKurallari<Hareket>(modelBuilder);
        FirmaKurallari<Servis>(modelBuilder);
        FirmaKurallari<Bolge>(modelBuilder);
        FirmaKurallari<Birim>(modelBuilder);
        FirmaKurallari<Takim>(modelBuilder);
        FirmaKurallari<Meslek>(modelBuilder);
        FirmaKurallari<KazancKesintiTuru>(modelBuilder);
        FirmaKurallari<KazancKesinti>(modelBuilder);
        FirmaKurallari<IslemKaydi>(modelBuilder);
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
        var kayitlar = IslemleriTopla();
        var sonuc = base.SaveChanges(acceptAllChangesOnSuccess);
        if (kayitlar.Count > 0)
        {
            IslemKayitlari.AddRange(kayitlar.Select(k => k.Olustur()));
            base.SaveChanges(acceptAllChangesOnSuccess);
        }
        return sonuc;
    }

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        KurallariUygula();
        var kayitlar = IslemleriTopla();
        var sonuc = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        if (kayitlar.Count > 0)
        {
            IslemKayitlari.AddRange(kayitlar.Select(k => k.Olustur()));
            await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
        return sonuc;
    }

    private sealed record BekleyenIslem(FirmaEntity Varlik, string Tablo, string Islem, string? Degisiklikler, int? KullaniciId)
    {
        // Id, kayıt veritabanına yazıldıktan sonra okunur (eklemelerde o ana kadar 0'dır).
        public IslemKaydi Olustur() => new()
        {
            FirmaId = Varlik.FirmaId,
            Zaman = DateTime.UtcNow,
            KullaniciId = KullaniciId,
            Tablo = Tablo,
            KayitId = Varlik.Id,
            Islem = Islem,
            Degisiklikler = Degisiklikler,
        };
    }

    private List<BekleyenIslem> IslemleriTopla()
    {
        var sonuc = new List<BekleyenIslem>();
        if (IslemYapanKullaniciId is null) return sonuc;

        foreach (var e in ChangeTracker.Entries<FirmaEntity>())
        {
            if (e.Entity is IslemKaydi) continue;
            string islem;
            string? degisiklikler = null;
            switch (e.State)
            {
                case EntityState.Added:
                    islem = "Ekle";
                    break;
                case EntityState.Deleted:
                    islem = "Sil";
                    break;
                case EntityState.Modified:
                    islem = "Değiştir";
                    var farklar = e.Properties
                        .Where(p => p.IsModified && !Equals(p.OriginalValue, p.CurrentValue)
                                    && p.Metadata.Name != nameof(BaseEntity.GuncellemeTarihi))
                        .ToDictionary(
                            p => p.Metadata.Name,
                            p => GizliAlanlar.Contains(p.Metadata.Name)
                                ? new[] { "***", "***" }
                                : new[] { p.OriginalValue?.ToString(), p.CurrentValue?.ToString() });
                    if (farklar.Count == 0) continue;
                    degisiklikler = System.Text.Json.JsonSerializer.Serialize(farklar,
                        new System.Text.Json.JsonSerializerOptions { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
                    break;
                default:
                    continue;
            }
            sonuc.Add(new BekleyenIslem(e.Entity, e.Metadata.GetTableName() ?? e.Entity.GetType().Name, islem, degisiklikler, IslemYapanKullaniciId));
        }
        return sonuc;
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
