namespace BiltekPdks.Domain.Entities;

public enum Cinsiyet { Belirtilmemis = 0, Erkek = 1, Kadin = 2 }

/// <summary>
/// Personel kartı. Eski `personel` tablosundaki 194 alanın çekirdek kısmı;
/// diğer alanlar ilgili modüllerle birlikte eklenecek.
/// </summary>
public class Personel : FirmaEntity
{
    public string KartNo { get; set; } = string.Empty;
    public string? SicilNo { get; set; }
    public string Ad { get; set; } = string.Empty;
    public string Soyad { get; set; } = string.Empty;
    public string? TcKimlikNo { get; set; }
    public Cinsiyet Cinsiyet { get; set; }
    public DateOnly? DogumTarihi { get; set; }

    public DateOnly IseGirisTarihi { get; set; }
    public DateOnly? IstenCikisTarihi { get; set; }
    public string? IstenAyrilmaNedeni { get; set; }

    public string? CepTelefonu { get; set; }
    public string? Eposta { get; set; }
    public string? Adres { get; set; }

    public int? SubeId { get; set; }
    public Sube? Sube { get; set; }
    public int? BolumId { get; set; }
    public Bolum? Bolum { get; set; }
    public int? GorevId { get; set; }
    public Gorev? Gorev { get; set; }
    public int? CalismaGrubuId { get; set; }
    public CalismaGrubu? CalismaGrubu { get; set; }

    public int? EskiId { get; set; }

    public string AdSoyad => $"{Ad} {Soyad}".Trim();

    /// <summary>Verilen tarihte işte mi (işe girmiş, henüz çıkmamış).</summary>
    public bool CalisiyorMu(DateOnly tarih) =>
        IseGirisTarihi <= tarih && (IstenCikisTarihi is null || IstenCikisTarihi >= tarih);
}
