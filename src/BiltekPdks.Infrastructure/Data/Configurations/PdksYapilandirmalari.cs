using BiltekPdks.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BiltekPdks.Infrastructure.Data.Configurations;

internal sealed class VardiyaYapilandirma : TanimYapilandirma<Vardiya>
{
    protected override string Tablo => "vardiya";

    public override void Configure(EntityTypeBuilder<Vardiya> b)
    {
        base.Configure(b);
        b.Property(x => x.GecCezaKatsayisi).HasPrecision(5, 2);
        b.Property(x => x.ErkenCezaKatsayisi).HasPrecision(5, 2);
        b.Property(x => x.EksikCezaKatsayisi).HasPrecision(5, 2);
        b.HasMany(x => x.Molalar).WithOne().HasForeignKey(x => x.VardiyaId).OnDelete(DeleteBehavior.Cascade);
    }
}

internal sealed class VardiyaMolaYapilandirma : IEntityTypeConfiguration<VardiyaMola>
{
    public void Configure(EntityTypeBuilder<VardiyaMola> b)
    {
        b.ToTable("vardiya_mola");
        b.Ignore(x => x.Dakika);
        b.Property(x => x.Ad).HasMaxLength(60).IsRequired();
    }
}

internal sealed class IzinTuruYapilandirma : TanimYapilandirma<IzinTuru> { protected override string Tablo => "izin_turu"; }

internal sealed class CalismaGrubuGunuYapilandirma : IEntityTypeConfiguration<CalismaGrubuGunu>
{
    public void Configure(EntityTypeBuilder<CalismaGrubuGunu> b)
    {
        b.ToTable("calisma_grubu_gunu");
        b.Property(x => x.Gun).HasConversion<int>();
        b.HasIndex(x => new { x.CalismaGrubuId, x.Gun }).IsUnique();
        b.HasOne(x => x.CalismaGrubu).WithMany().HasForeignKey(x => x.CalismaGrubuId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.Vardiya).WithMany().HasForeignKey(x => x.VardiyaId).OnDelete(DeleteBehavior.Restrict);
    }
}

internal sealed class PersonelVardiyaYapilandirma : IEntityTypeConfiguration<PersonelVardiya>
{
    public void Configure(EntityTypeBuilder<PersonelVardiya> b)
    {
        b.ToTable("personel_vardiya");
        b.HasIndex(x => new { x.PersonelId, x.Tarih }).IsUnique();
        b.HasOne(x => x.Personel).WithMany().HasForeignKey(x => x.PersonelId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.Vardiya).WithMany().HasForeignKey(x => x.VardiyaId).OnDelete(DeleteBehavior.Restrict);
    }
}

internal sealed class ResmiTatilYapilandirma : IEntityTypeConfiguration<ResmiTatil>
{
    public void Configure(EntityTypeBuilder<ResmiTatil> b)
    {
        b.ToTable("resmi_tatil");
        b.Property(x => x.Ad).HasMaxLength(120).IsRequired();
        b.HasIndex(x => new { x.FirmaId, x.Tarih }).IsUnique();
    }
}

internal sealed class IzinYapilandirma : IEntityTypeConfiguration<Izin>
{
    public void Configure(EntityTypeBuilder<Izin> b)
    {
        b.ToTable("izin");
        b.Property(x => x.Aciklama).HasMaxLength(250);
        b.HasIndex(x => new { x.PersonelId, x.Baslangic });
        b.HasOne(x => x.Personel).WithMany().HasForeignKey(x => x.PersonelId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.IzinTuru).WithMany().HasForeignKey(x => x.IzinTuruId).OnDelete(DeleteBehavior.Restrict);
    }
}

internal sealed class HareketYapilandirma : IEntityTypeConfiguration<Hareket>
{
    public void Configure(EntityTypeBuilder<Hareket> b)
    {
        b.ToTable("hareket");
        b.Property(x => x.Kaynak).HasConversion<int>();
        b.Property(x => x.TerminalKodu).HasMaxLength(20);
        b.Property(x => x.Aciklama).HasMaxLength(250);
        b.HasIndex(x => new { x.PersonelId, x.Zaman });
        b.HasOne(x => x.Personel).WithMany().HasForeignKey(x => x.PersonelId).OnDelete(DeleteBehavior.Cascade);
    }
}
