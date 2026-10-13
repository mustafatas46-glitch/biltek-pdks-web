namespace BiltekPdks.Domain.Entities;

/// <summary>Kiracı (müşteri firma). Eski sistemdeki `firma` tablosunun karşılığı.</summary>
public class Firma : BaseEntity
{
    public string Kod { get; set; } = string.Empty;
    public string Ad { get; set; } = string.Empty;
    public string? VergiDairesi { get; set; }
    public string? VergiNo { get; set; }
    public string? SgkSicilNo { get; set; }
    public string? Adres { get; set; }
    public string? Il { get; set; }
    public string? Ilce { get; set; }
    public string? Telefon { get; set; }
    public string? Yetkili { get; set; }
    public bool Aktif { get; set; } = true;
    public int? EskiId { get; set; }
}
