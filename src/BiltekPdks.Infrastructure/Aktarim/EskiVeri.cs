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

public sealed class EskiVeri
{
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
