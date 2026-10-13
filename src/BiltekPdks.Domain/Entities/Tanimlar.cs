namespace BiltekPdks.Domain.Entities;

public class Sube : TanimEntity { }

public class Bolum : TanimEntity { }

public class Gorev : TanimEntity { }

/// <summary>
/// Çalışma grubu (eski `grup` tablosu). Süreler dakika cinsinden tutulur;
/// eski sistemdeki "08:30" gibi metin süreler aktarımda dakikaya çevrilir.
/// </summary>
public class CalismaGrubu : TanimEntity
{
    public int GunlukCalismaDakika { get; set; } = 450;
    public int DonemlikCalismaDakika { get; set; } = 2700;
    public bool CumartesiCalisilir { get; set; }
    public bool PazarCalisilir { get; set; }

    // --- Ücret hesabı kuralları ---
    /// <summary>Aylık maaştan saat ücreti bulunurken bölünen saat (eski programda 225).</summary>
    public int MaasBoleniSaat { get; set; } = 225;
    /// <summary>Normal fazla mesai (%50) çarpanı.</summary>
    public decimal FazlaMesaiCarpani { get; set; } = 1.5m;
    /// <summary>Tatil mesaisi (%100) çarpanı.</summary>
    public decimal TatilMesaisiCarpani { get; set; } = 2.0m;
}
