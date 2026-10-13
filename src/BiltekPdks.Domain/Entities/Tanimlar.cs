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
}
