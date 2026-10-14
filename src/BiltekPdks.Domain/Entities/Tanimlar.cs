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

/// <summary>
/// Vardiyalı gruplarda alt grup (ör. 08-16, 16-24, 24-08). Her alt grubun kendi haftalık çalışma planı vardır.
/// Personele sabit alt grup verilebilir; verilmezse puantaj, günün ilk-son okutmasına en uygun alt grubu seçer.
/// </summary>
public class AltGrup : TanimEntity
{
    public int CalismaGrubuId { get; set; }
    public CalismaGrubu? CalismaGrubu { get; set; }
    public int GunlukCalismaDakika { get; set; } = 450;
}

/// <summary>Alt grubun haftalık planı: hangi gün hangi günlük çalışma saati (vardiya). Boş gün hafta tatilidir.</summary>
public class AltGrupGunu : FirmaEntity
{
    public int AltGrupId { get; set; }
    public AltGrup? AltGrup { get; set; }
    public DayOfWeek Gun { get; set; }
    public int? VardiyaId { get; set; }
}

/// <summary>
/// Grubun (isteğe bağlı alt grubun) dönem tanımı. Dönem Ekle/Değiştir penceresinde "Gün hesapla" ile
/// 30 gün hesabına göre eksik/fazla gün ve saat bulunur.
/// </summary>
public class DonemTanimi : FirmaEntity
{
    public int CalismaGrubuId { get; set; }
    public int? AltGrupId { get; set; }
    public DateOnly Baslangic { get; set; }
    public DateOnly Bitis { get; set; }
    public TimeOnly? GirisSaati { get; set; }
    public TimeOnly? CikisSaati { get; set; }
    public int GunFarki { get; set; }
    public int EksikGun { get; set; }
    public int EksikDakika { get; set; }
    public int FazlaGun { get; set; }
    public int FazlaDakika { get; set; }

    /// <summary>
    /// Her dönem 30 gün sayılır: dönem 31 gün çekiyorsa 1 gün fazla, 28 gün çekiyorsa 2 gün eksik.
    /// Saat karşılığı günlük çalışma süresiyle bulunur.
    /// </summary>
    public void GunHesapla(int gunlukCalismaDakika)
    {
        GunFarki = Bitis.DayNumber - Baslangic.DayNumber + 1;
        EksikGun = Math.Max(0, 30 - GunFarki);
        FazlaGun = Math.Max(0, GunFarki - 30);
        EksikDakika = EksikGun * gunlukCalismaDakika;
        FazlaDakika = FazlaGun * gunlukCalismaDakika;
    }
}
