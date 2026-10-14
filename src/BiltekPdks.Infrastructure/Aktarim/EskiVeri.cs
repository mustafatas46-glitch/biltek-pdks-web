namespace BiltekPdks.Infrastructure.Aktarim;

// Eski Biltek veritabanından (data2015) okunan ham kayıtlar. Alan adları eski tablolardaki karşılıklarıdır.

public sealed record EskiTanim(int Id, string? Kod, string? Ad);

public sealed record EskiGrup(int Id, string? Ad, string? GunlukSaat, string? DonemlikSaat);

public sealed record EskiPersonel(
    long Id, string? KartNo, string? Ad, string? Soyad, string? SicilNo, int? FirmaId,
    int? GrupId, int? BolumId, int? GorevId, int? SubeId,
    DateTime? IseGirisTarihi, DateTime? IstenCikisTarihi, string? IstenAyrilmaNedeni,
    string? KimlikNo, DateTime? DogumTarihi, string? Cinsiyet, string? CepTel, string? Email, string? Adres);

public sealed record EskiIzinTuru(int Id, string? Kod, string? Ad, string? Aciklama, string? Tip);

public sealed record EskiIzin(
    long Id, long PersonelId, DateTime? Baslangic, TimeSpan? BaslangicSaati,
    DateTime? Bitis, TimeSpan? BitisSaati, int? IzinTurId, string? Aciklama);

public sealed record EskiTatil(DateTime Tarih, string? Ad);

/// <summary>pts_giriscikis satırı: bir giriş ve bir çıkış (ikisi de boş olabilir).</summary>
public sealed record EskiGirisCikis(
    long Id, long PersonelId,
    DateTime? GirisTarihi, TimeSpan? GirisSaati, string? ElleGiris, string? GirisTerminal,
    DateTime? CikisTarihi, TimeSpan? CikisSaati, string? ElleCikis, string? CikisTerminal);

/// <summary>pts_puantajparametre: günlük çalışma saati tanımı (yeni sistemde Vardiya).</summary>
public sealed record EskiPuantajTanimi(int Id, string? Ad, string? GunlukSaat);

/// <summary>pts_calismaparametre satırı: çalışma türüne (normal, fazla mesai…) göre saat aralığı.</summary>
public sealed record EskiCalismaParametre(int Id, int PuantajId, string? BordroAd, string? Igs, string? Dcs);

/// <summary>pts_molaparametre satırı.</summary>
public sealed record EskiMola(int PuantajId, string? Ad, string? Baslama, string? Bitis, string? Tip);

/// <summary>pts_altgrup: vardiyalı grubun alt grubu.</summary>
public sealed record EskiAltGrup(int Id, int GrupId, string? Ad, string? GunlukSaat);

/// <summary>pts_gunparametre: alt grubun (alt grup yoksa grubun) haftalık planı; G1 = pazartesi … G7 = pazar.</summary>
public sealed record EskiGunParametre(int? GrupId, int AltGrupId, int?[] Gunler);

/// <summary>pts_donemparametre satırı.</summary>
public sealed record EskiDonem(int GrupId, int? AltGrupId, DateTime? Baslangic, DateTime? Bitis, TimeSpan? GirisSaati, TimeSpan? CikisSaati,
    int? EksikGun, string? EksikSaat, int? FazlaGun, string? FazlaSaat, int? GunFarki);

public sealed class EskiVeri
{
    public List<EskiPuantajTanimi> PuantajTanimlari { get; } = new();
    public List<EskiCalismaParametre> CalismaParametreleri { get; } = new();
    public List<EskiMola> Molalar { get; } = new();
    public List<EskiAltGrup> AltGruplar { get; } = new();
    public List<EskiGunParametre> GunParametreleri { get; } = new();
    public List<EskiDonem> Donemler { get; } = new();
    public List<EskiTanim> Subeler { get; } = new();
    public List<EskiTanim> Bolumler { get; } = new();
    public List<EskiTanim> Gorevler { get; } = new();
    public List<EskiGrup> Gruplar { get; } = new();
    public List<EskiPersonel> Personeller { get; } = new();
    public List<EskiIzinTuru> IzinTurleri { get; } = new();
    public List<EskiIzin> Izinler { get; } = new();
    public List<EskiTatil> Tatiller { get; } = new();
    public List<EskiGirisCikis> GirisCikislar { get; } = new();
}
