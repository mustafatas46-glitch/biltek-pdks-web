using BiltekPdks.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BiltekPdks.Infrastructure.Data.Configurations;

internal sealed class FirmaYapilandirma : IEntityTypeConfiguration<Firma>
{
    public void Configure(EntityTypeBuilder<Firma> b)
    {
        b.ToTable("firma");
        b.Property(x => x.Kod).HasMaxLength(20).IsRequired();
        b.Property(x => x.Ad).HasMaxLength(150).IsRequired();
        b.Property(x => x.VergiDairesi).HasMaxLength(100);
        b.Property(x => x.VergiNo).HasMaxLength(20);
        b.Property(x => x.SgkSicilNo).HasMaxLength(40);
        b.Property(x => x.Adres).HasMaxLength(500);
        b.Property(x => x.Il).HasMaxLength(60);
        b.Property(x => x.Ilce).HasMaxLength(60);
        b.Property(x => x.Telefon).HasMaxLength(30);
        b.Property(x => x.Yetkili).HasMaxLength(100);
        b.HasIndex(x => x.Kod).IsUnique();
    }
}

internal sealed class KullaniciYapilandirma : IEntityTypeConfiguration<Kullanici>
{
    public void Configure(EntityTypeBuilder<Kullanici> b)
    {
        b.ToTable("kullanici");
        b.Property(x => x.KullaniciAdi).HasMaxLength(50).IsRequired();
        b.Property(x => x.SifreHash).HasMaxLength(200).IsRequired();
        b.Property(x => x.AdSoyad).HasMaxLength(120).IsRequired();
        b.Property(x => x.Rol).HasConversion<int>();
        b.HasIndex(x => new { x.FirmaId, x.KullaniciAdi }).IsUnique();
        b.HasOne(x => x.Personel).WithMany().HasForeignKey(x => x.PersonelId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

internal sealed class PersonelYapilandirma : IEntityTypeConfiguration<Personel>
{
    public void Configure(EntityTypeBuilder<Personel> b)
    {
        b.ToTable("personel");
        b.Ignore(x => x.AdSoyad);
        b.Property(x => x.KartNo).HasMaxLength(20).IsRequired();
        b.Property(x => x.SicilNo).HasMaxLength(30);
        b.Property(x => x.Ad).HasMaxLength(60).IsRequired();
        b.Property(x => x.Soyad).HasMaxLength(60).IsRequired();
        b.Property(x => x.TcKimlikNo).HasMaxLength(11);
        b.Property(x => x.Cinsiyet).HasConversion<int>();
        b.Property(x => x.IstenAyrilmaNedeni).HasMaxLength(500);
        b.Property(x => x.CepTelefonu).HasMaxLength(30);
        b.Property(x => x.Eposta).HasMaxLength(150);
        b.Property(x => x.Adres).HasMaxLength(500);

        b.HasIndex(x => new { x.FirmaId, x.KartNo }).IsUnique();
        b.HasIndex(x => new { x.FirmaId, x.Soyad, x.Ad });

        b.HasOne(x => x.Sube).WithMany().HasForeignKey(x => x.SubeId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Bolum).WithMany().HasForeignKey(x => x.BolumId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Gorev).WithMany().HasForeignKey(x => x.GorevId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.CalismaGrubu).WithMany().HasForeignKey(x => x.CalismaGrubuId).OnDelete(DeleteBehavior.Restrict);
    }
}

internal abstract class TanimYapilandirma<T> : IEntityTypeConfiguration<T> where T : TanimEntity
{
    protected abstract string Tablo { get; }

    public virtual void Configure(EntityTypeBuilder<T> b)
    {
        b.ToTable(Tablo);
        b.Property(x => x.Kod).HasMaxLength(20);
        b.Property(x => x.Ad).HasMaxLength(120).IsRequired();
        b.HasIndex(x => new { x.FirmaId, x.Ad });
    }
}

internal sealed class SubeYapilandirma : TanimYapilandirma<Sube> { protected override string Tablo => "sube"; }
internal sealed class BolumYapilandirma : TanimYapilandirma<Bolum> { protected override string Tablo => "bolum"; }
internal sealed class GorevYapilandirma : TanimYapilandirma<Gorev> { protected override string Tablo => "gorev"; }
internal sealed class CalismaGrubuYapilandirma : TanimYapilandirma<CalismaGrubu> { protected override string Tablo => "calisma_grubu"; }
