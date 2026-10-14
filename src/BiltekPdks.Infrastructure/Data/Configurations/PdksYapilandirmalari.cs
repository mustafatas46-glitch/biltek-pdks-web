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
        b.HasIndex(x => x.AktarimId);
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
        b.HasIndex(x => x.AktarimId);
        b.HasOne(x => x.Personel).WithMany().HasForeignKey(x => x.PersonelId).OnDelete(DeleteBehavior.Cascade);
    }
}

internal sealed class AktarimKaydiYapilandirma : IEntityTypeConfiguration<AktarimKaydi>
{
    public void Configure(EntityTypeBuilder<AktarimKaydi> b)
    {
        b.ToTable("aktarim_kaydi");
        b.Property(x => x.Tur).HasConversion<int>();
        b.Property(x => x.DosyaAdi).HasMaxLength(250).IsRequired();
        b.Property(x => x.Eslesme).HasMaxLength(4000);
        b.HasIndex(x => new { x.FirmaId, x.Zaman });
    }
}

internal sealed class AktarimSablonuYapilandirma : IEntityTypeConfiguration<AktarimSablonu>
{
    public void Configure(EntityTypeBuilder<AktarimSablonu> b)
    {
        b.ToTable("aktarim_sablonu");
        b.Property(x => x.Tur).HasConversion<int>();
        b.Property(x => x.Ad).HasMaxLength(120).IsRequired();
        b.Property(x => x.Eslesme).HasMaxLength(4000).IsRequired();
        b.HasIndex(x => new { x.FirmaId, x.Tur, x.Ad }).IsUnique();
    }
}

internal sealed class BordroSatiriYapilandirma : IEntityTypeConfiguration<BordroSatiri>
{
    public void Configure(EntityTypeBuilder<BordroSatiri> b)
    {
        b.ToTable("bordro_satiri");
        b.Property(x => x.CalisilanGun).HasPrecision(6, 2);
        b.Property(x => x.IzinGun).HasPrecision(6, 2);
        b.Property(x => x.DevamsizGun).HasPrecision(6, 2);
        b.Property(x => x.FazlaMesaiSaat).HasPrecision(8, 2);
        b.Property(x => x.BrutUcret).HasPrecision(18, 2);
        b.Property(x => x.NetUcret).HasPrecision(18, 2);
        b.Property(x => x.Aciklama).HasMaxLength(250);
        b.HasIndex(x => new { x.PersonelId, x.Yil, x.Ay }).IsUnique();
        b.HasIndex(x => x.AktarimId);
        b.HasOne(x => x.Personel).WithMany().HasForeignKey(x => x.PersonelId).OnDelete(DeleteBehavior.Cascade);
    }
}

internal sealed class AltGrupYapilandirma : TanimYapilandirma<AltGrup>
{
    protected override string Tablo => "alt_grup";

    public override void Configure(EntityTypeBuilder<AltGrup> b)
    {
        base.Configure(b);
        b.HasOne(x => x.CalismaGrubu).WithMany().HasForeignKey(x => x.CalismaGrubuId).OnDelete(DeleteBehavior.Cascade);
    }
}

internal sealed class AltGrupGunuYapilandirma : IEntityTypeConfiguration<AltGrupGunu>
{
    public void Configure(EntityTypeBuilder<AltGrupGunu> b)
    {
        b.ToTable("alt_grup_gunu");
        b.Property(x => x.Gun).HasConversion<int>();
        b.HasIndex(x => new { x.AltGrupId, x.Gun }).IsUnique();
        b.HasOne(x => x.AltGrup).WithMany().HasForeignKey(x => x.AltGrupId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne<Vardiya>().WithMany().HasForeignKey(x => x.VardiyaId).OnDelete(DeleteBehavior.Restrict);
    }
}

internal sealed class DonemTanimiYapilandirma : IEntityTypeConfiguration<DonemTanimi>
{
    public void Configure(EntityTypeBuilder<DonemTanimi> b)
    {
        b.ToTable("donem_tanimi");
        b.HasIndex(x => new { x.CalismaGrubuId, x.Baslangic });
        b.HasOne<CalismaGrubu>().WithMany().HasForeignKey(x => x.CalismaGrubuId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne<AltGrup>().WithMany().HasForeignKey(x => x.AltGrupId).OnDelete(DeleteBehavior.Cascade);
    }
}

internal sealed class KullaniciYetkisiYapilandirma : IEntityTypeConfiguration<KullaniciYetkisi>
{
    public void Configure(EntityTypeBuilder<KullaniciYetkisi> b)
    {
        b.ToTable("kullanici_yetkisi");
        b.Property(x => x.Yetki).HasMaxLength(60).IsRequired();
        b.HasIndex(x => new { x.KullaniciId, x.Yetki }).IsUnique();
        b.HasOne<Kullanici>().WithMany().HasForeignKey(x => x.KullaniciId).OnDelete(DeleteBehavior.Cascade);
    }
}
