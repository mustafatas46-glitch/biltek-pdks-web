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

        // Kart numarası işten ayrılan personelden yeni gelene verilebilir; aktif personelde tekillik uygulamada kontrol edilir.
        b.HasIndex(x => new { x.FirmaId, x.KartNo });
        b.HasIndex(x => new { x.FirmaId, x.Soyad, x.Ad });

        b.HasOne(x => x.Sube).WithMany().HasForeignKey(x => x.SubeId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Bolum).WithMany().HasForeignKey(x => x.BolumId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Gorev).WithMany().HasForeignKey(x => x.GorevId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.CalismaGrubu).WithMany().HasForeignKey(x => x.CalismaGrubuId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Servis).WithMany().HasForeignKey(x => x.ServisId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Bolge).WithMany().HasForeignKey(x => x.BolgeId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Birim).WithMany().HasForeignKey(x => x.BirimId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Takim).WithMany().HasForeignKey(x => x.TakimId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Meslek).WithMany().HasForeignKey(x => x.MeslekId).OnDelete(DeleteBehavior.Restrict);

        b.Property(x => x.UcretTipi).HasConversion<int>();
        b.Property(x => x.Ucret).HasPrecision(18, 2);
        b.Property(x => x.EskiUcret).HasPrecision(18, 2);
        b.Property(x => x.MesaiSaatUcreti).HasPrecision(18, 2);
        b.Property(x => x.DevamsizlikCezaKatsayisi).HasPrecision(5, 2);
        b.Property(x => x.KartId).HasMaxLength(20);
        b.Property(x => x.IlkSoyad).HasMaxLength(60);
        b.Property(x => x.VergiNo).HasMaxLength(20);
        b.Property(x => x.SgkSicilNo).HasMaxLength(40);
        foreach (var alan in new[] { nameof(Personel.MedeniHal), nameof(Personel.KanGrubu), nameof(Personel.EgitimDurumu),
                     nameof(Personel.AskerlikDurumu), nameof(Personel.EhliyetSinifi) })
            b.Property(alan).HasMaxLength(30);
        foreach (var alan in new[] { nameof(Personel.EvTelefonu), nameof(Personel.EhliyetNo) })
            b.Property(alan).HasMaxLength(30);
        foreach (var alan in new[] { nameof(Personel.Il), nameof(Personel.Ilce), nameof(Personel.BabaAdi), nameof(Personel.AnneAdi),
                     nameof(Personel.DogumYeri), nameof(Personel.Uyruk), nameof(Personel.NufusIl), nameof(Personel.NufusIlce), nameof(Personel.BankaAdi) })
            b.Property(alan).HasMaxLength(60);
        b.Property(x => x.Iban).HasMaxLength(34);

        foreach (var alan in new[] { nameof(Personel.IsTelefonu), nameof(Personel.Gozluk), nameof(Personel.Sabika), nameof(Personel.OkulNo),
                     nameof(Personel.AyakkabiNo), nameof(Personel.ElbiseBeden), nameof(Personel.CiltNo), nameof(Personel.AileSiraNo),
                     nameof(Personel.SiraNo), nameof(Personel.CuzdanKayitNo) })
            b.Property(alan).HasMaxLength(30);
        foreach (var alan in new[] { nameof(Personel.MezunOkul), nameof(Personel.MezunBrans), nameof(Personel.YabanciDil),
                     nameof(Personel.NufusMahalleKoy), nameof(Personel.CuzdanVerildigiYer), nameof(Personel.CuzdanVerilisNedeni),
                     nameof(Personel.EhliyetVerildigiYer) })
            b.Property(alan).HasMaxLength(100);
        b.Property(x => x.WebAdresi).HasMaxLength(150);
        b.Property(x => x.CihazProtez).HasMaxLength(200);
        b.Property(x => x.Aciklama).HasMaxLength(2000);
        b.Property(x => x.PrimUcreti).HasPrecision(18, 2);
        b.Property(x => x.YolUcreti).HasPrecision(18, 2);
        b.Property(x => x.YemekUcreti).HasPrecision(18, 2);
        b.Property(x => x.OzelUcret).HasPrecision(18, 2);
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

internal sealed class ServisYapilandirma : TanimYapilandirma<Servis> { protected override string Tablo => "servis"; }
internal sealed class BolgeYapilandirma : TanimYapilandirma<Bolge> { protected override string Tablo => "bolge"; }
internal sealed class BirimYapilandirma : TanimYapilandirma<Birim> { protected override string Tablo => "birim"; }
internal sealed class TakimYapilandirma : TanimYapilandirma<Takim> { protected override string Tablo => "takim"; }
internal sealed class MeslekYapilandirma : TanimYapilandirma<Meslek> { protected override string Tablo => "meslek"; }

internal sealed class CalismaGrubuUcretYapilandirma : TanimYapilandirma<CalismaGrubu>
{
    protected override string Tablo => "calisma_grubu";

    public override void Configure(EntityTypeBuilder<CalismaGrubu> b)
    {
        base.Configure(b);
        b.Property(x => x.FazlaMesaiCarpani).HasPrecision(5, 2);
        b.Property(x => x.TatilMesaisiCarpani).HasPrecision(5, 2);
    }
}

internal sealed class KazancKesintiTuruYapilandirma : TanimYapilandirma<KazancKesintiTuru>
{
    protected override string Tablo => "kazanc_kesinti_turu";

    public override void Configure(EntityTypeBuilder<KazancKesintiTuru> b)
    {
        base.Configure(b);
        b.Property(x => x.Tip).HasConversion<int>();
    }
}

internal sealed class KazancKesintiYapilandirma : IEntityTypeConfiguration<KazancKesinti>
{
    public void Configure(EntityTypeBuilder<KazancKesinti> b)
    {
        b.ToTable("kazanc_kesinti");
        b.Property(x => x.Tutar).HasPrecision(18, 2);
        b.Property(x => x.Aciklama).HasMaxLength(250);
        b.HasIndex(x => new { x.PersonelId, x.Tarih });
        b.HasIndex(x => x.AktarimId);
        b.HasOne(x => x.Personel).WithMany().HasForeignKey(x => x.PersonelId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.KazancKesintiTuru).WithMany().HasForeignKey(x => x.KazancKesintiTuruId).OnDelete(DeleteBehavior.Restrict);
    }
}

internal sealed class IslemKaydiYapilandirma : IEntityTypeConfiguration<IslemKaydi>
{
    public void Configure(EntityTypeBuilder<IslemKaydi> b)
    {
        b.ToTable("islem_kaydi");
        b.Property(x => x.Tablo).HasMaxLength(60).IsRequired();
        b.Property(x => x.Islem).HasMaxLength(20).IsRequired();
        b.HasIndex(x => new { x.FirmaId, x.Zaman });
        b.HasIndex(x => new { x.Tablo, x.KayitId });
    }
}
