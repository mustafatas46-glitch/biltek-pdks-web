namespace BiltekPdks.Domain.Entities;

/// <summary>Excel'den içeri alınabilen veri türleri.</summary>
public enum AktarimTuru
{
    Personel = 1,
    Ucret = 2,
    Izin = 3,
    KazancKesinti = 4,
    Hareket = 5,
    Bordro = 6,
}

/// <summary>
/// Bir Excel dosyasının içeri alınması. Bu aktarımla eklenen her kayıt <c>AktarimId</c> ile buna bağlanır;
/// böylece hangi verinin hangi dosyadan, kim tarafından, ne zaman geldiği her zaman görülebilir ve
/// gerekirse aktarım bütünüyle geri alınabilir.
/// </summary>
public class AktarimKaydi : FirmaEntity
{
    public AktarimTuru Tur { get; set; }
    public DateTime Zaman { get; set; }
    public int? KullaniciId { get; set; }
    public string DosyaAdi { get; set; } = string.Empty;
    public int SatirSayisi { get; set; }
    public int Eklenen { get; set; }
    public int Guncellenen { get; set; }
    public int Atlanan { get; set; }
    public int Hatali { get; set; }
    /// <summary>Kullanılan sütun eşleştirmesi (JSON: alan → Excel sütun başlığı).</summary>
    public string? Eslesme { get; set; }
    public bool GeriAlindi { get; set; }
    public DateTime? GeriAlmaZamani { get; set; }
}

/// <summary>Müşteriye (firmaya) özel kaydedilmiş sütun eşleştirmesi; aynı biçimdeki dosyalar tekrar eşleştirilmez.</summary>
public class AktarimSablonu : FirmaEntity
{
    public AktarimTuru Tur { get; set; }
    public string Ad { get; set; } = string.Empty;
    /// <summary>JSON: alan → Excel sütun başlığı.</summary>
    public string Eslesme { get; set; } = "{}";
    public int BaslikSatiri { get; set; } = 1;
}

/// <summary>
/// Muhasebeden gelen aylık bordro satırı. PDKS puantajıyla karşılaştırılır (Bordro–PDKS uyum raporu);
/// puantajı değiştirmez.
/// </summary>
public class BordroSatiri : FirmaEntity
{
    public int PersonelId { get; set; }
    public Personel? Personel { get; set; }
    public int Yil { get; set; }
    public int Ay { get; set; }
    public decimal? CalisilanGun { get; set; }
    public decimal? IzinGun { get; set; }
    public decimal? DevamsizGun { get; set; }
    public decimal? FazlaMesaiSaat { get; set; }
    public decimal? BrutUcret { get; set; }
    public decimal? NetUcret { get; set; }
    public string? Aciklama { get; set; }
    public int? AktarimId { get; set; }
}
