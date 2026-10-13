namespace BiltekPdks.Domain.Entities;

public enum KullaniciRolu
{
    /// <summary>Firma içindeki her şeye erişir.</summary>
    Yonetici = 1,
    /// <summary>Personel, izin, puantaj işlemleri.</summary>
    InsanKaynaklari = 2,
    /// <summary>Kendi bölümünü görür.</summary>
    BolumSorumlusu = 3,
    /// <summary>Yalnızca kendi kayıtlarını görür.</summary>
    Personel = 4,
}

/// <summary>Program kullanıcısı. Şifre düz metin değil, hash olarak saklanır.</summary>
public class Kullanici : FirmaEntity
{
    public string KullaniciAdi { get; set; } = string.Empty;
    public string SifreHash { get; set; } = string.Empty;
    public string AdSoyad { get; set; } = string.Empty;
    public KullaniciRolu Rol { get; set; } = KullaniciRolu.Personel;
    public bool Aktif { get; set; } = true;
    public DateTime? SonGirisTarihi { get; set; }
    public int BasarisizGirisSayisi { get; set; }
    public DateTime? KilitBitis { get; set; }

    public int? PersonelId { get; set; }
    public Personel? Personel { get; set; }
}
