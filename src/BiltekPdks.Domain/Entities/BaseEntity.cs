namespace BiltekPdks.Domain.Entities;

/// <summary>Tüm tabloların ortak alanları.</summary>
public abstract class BaseEntity
{
    public int Id { get; set; }
    public DateTime OlusturmaTarihi { get; set; } = DateTime.UtcNow;
    public DateTime? GuncellemeTarihi { get; set; }
}

/// <summary>
/// Bir firmaya (kiracıya) ait kayıtlar. FirmaId otomatik doldurulur ve
/// sorgular yalnızca oturumdaki firmanın kayıtlarını döndürür.
/// </summary>
public abstract class FirmaEntity : BaseEntity
{
    public int FirmaId { get; set; }
}

/// <summary>Kod + ad + aktiflik taşıyan basit tanım tabloları (şube, bölüm, görev...).</summary>
public abstract class TanimEntity : FirmaEntity
{
    public string? Kod { get; set; }
    public string Ad { get; set; } = string.Empty;
    public bool Aktif { get; set; } = true;

    /// <summary>Eski Biltek veritabanındaki id (aktarım için).</summary>
    public int? EskiId { get; set; }
}
