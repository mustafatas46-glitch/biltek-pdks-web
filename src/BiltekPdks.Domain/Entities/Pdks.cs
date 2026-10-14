using BiltekPdks.Domain.Puantaj;

namespace BiltekPdks.Domain.Entities;

/// <summary>Vardiya / çalışma saati tanımı (eski sistemdeki puantaj parametreleri).</summary>
public class Vardiya : TanimEntity
{
    public TimeOnly Baslangic { get; set; } = new(8, 0);
    public TimeOnly Bitis { get; set; } = new(17, 0);
    public int MolaDakika { get; set; } = 60;
    public int GecKalmaToleransDakika { get; set; }
    public int ErkenCikmaToleransDakika { get; set; }
    public int FazlaMesaiEsikDakika { get; set; } = 30;

    // --- Diğer Prm ---
    public int? DevamsizlikCezaDakika { get; set; }
    public decimal GecCezaKatsayisi { get; set; } = 1;
    public decimal ErkenCezaKatsayisi { get; set; } = 1;
    public decimal EksikCezaKatsayisi { get; set; } = 1;
    public bool NormalTamamlaSonraFazlaMesai { get; set; }
    public bool CokluGiriseEksikYazma { get; set; }
    public int MaxToleransDakika { get; set; }

    /// <summary>Mola saatleri (bilgi ve ücretsiz mola toplamı için). Ücretsiz molalar varsa toplamları <see cref="MolaDakika"/> olur.</summary>
    public List<VardiyaMola> Molalar { get; set; } = new();

    public VardiyaKurali Kural() => new(Baslangic, Bitis, MolaDakika,
        GecKalmaToleransDakika, ErkenCikmaToleransDakika, FazlaMesaiEsikDakika)
    {
        DevamsizlikCezaDakika = DevamsizlikCezaDakika,
        GecCezaKatsayisi = GecCezaKatsayisi,
        ErkenCezaKatsayisi = ErkenCezaKatsayisi,
        EksikCezaKatsayisi = EksikCezaKatsayisi,
        NormalTamamlaSonraFazlaMesai = NormalTamamlaSonraFazlaMesai,
        CokluGiriseEksikYazma = CokluGiriseEksikYazma,
        MaxToleransDakika = MaxToleransDakika,
    };
}

/// <summary>Günlük çalışma planındaki mola (ör. öğle yemeği 12:00-13:00, ücretsiz).</summary>
public class VardiyaMola : FirmaEntity
{
    public int VardiyaId { get; set; }
    public string Ad { get; set; } = string.Empty;
    public TimeOnly Baslangic { get; set; }
    public TimeOnly Bitis { get; set; }
    public bool Ucretli { get; set; }

    public int Dakika => (int)((Bitis > Baslangic ? Bitis - Baslangic : Bitis - Baslangic + TimeSpan.FromDays(1)).TotalMinutes);
}

/// <summary>Çalışma grubunun haftalık planı: hangi gün hangi vardiya. VardiyaId boşsa o gün hafta tatilidir.</summary>
public class CalismaGrubuGunu : FirmaEntity
{
    public int CalismaGrubuId { get; set; }
    public CalismaGrubu? CalismaGrubu { get; set; }
    public DayOfWeek Gun { get; set; }
    public int? VardiyaId { get; set; }
    public Vardiya? Vardiya { get; set; }
}

/// <summary>Bir personelin belirli bir gündeki vardiyasını grup planının yerine geçirir. VardiyaId boşsa o gün tatildir.</summary>
public class PersonelVardiya : FirmaEntity
{
    public int PersonelId { get; set; }
    public Personel? Personel { get; set; }
    public DateOnly Tarih { get; set; }
    public int? VardiyaId { get; set; }
    public Vardiya? Vardiya { get; set; }
}

public class ResmiTatil : FirmaEntity
{
    public DateOnly Tarih { get; set; }
    public string Ad { get; set; } = string.Empty;
}

public class IzinTuru : TanimEntity
{
    public bool Ucretli { get; set; } = true;
    public bool YillikIzindenDuser { get; set; }
}

/// <summary>Gün bazlı izin kaydı (başlangıç ve bitiş dahil).</summary>
public class Izin : FirmaEntity
{
    public int PersonelId { get; set; }
    public Personel? Personel { get; set; }
    public int IzinTuruId { get; set; }
    public IzinTuru? IzinTuru { get; set; }
    public DateOnly Baslangic { get; set; }
    public DateOnly Bitis { get; set; }
    public string? Aciklama { get; set; }

    public bool Kapsar(DateOnly tarih) => Baslangic <= tarih && tarih <= Bitis;
}

public enum HareketKaynagi { Terminal = 1, Elle = 2, Aktarim = 3 }

/// <summary>Ham kart okutma / giriş-çıkış hareketi. Silinmez; düzeltme için iptal edilir.</summary>
public class Hareket : FirmaEntity
{
    public int PersonelId { get; set; }
    public Personel? Personel { get; set; }
    public DateTime Zaman { get; set; }
    public HareketKaynagi Kaynak { get; set; } = HareketKaynagi.Elle;
    public string? TerminalKodu { get; set; }
    public string? Aciklama { get; set; }
    public bool Iptal { get; set; }
    public int? KaydedenKullaniciId { get; set; }
}
