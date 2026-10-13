namespace BiltekPdks.Domain.Entities;

public enum Cinsiyet { Belirtilmemis = 0, Erkek = 1, Kadin = 2 }

/// <summary>
/// Personel kartı. Eski `personel` tablosundaki 194 alanın çekirdek kısmı;
/// diğer alanlar ilgili modüllerle birlikte eklenecek.
/// </summary>
public class Personel : FirmaEntity
{
    public string KartNo { get; set; } = string.Empty;
    public string? SicilNo { get; set; }
    public string Ad { get; set; } = string.Empty;
    public string Soyad { get; set; } = string.Empty;
    public string? TcKimlikNo { get; set; }
    public Cinsiyet Cinsiyet { get; set; }
    public DateOnly? DogumTarihi { get; set; }

    public DateOnly IseGirisTarihi { get; set; }
    public DateOnly? IstenCikisTarihi { get; set; }
    public string? IstenAyrilmaNedeni { get; set; }

    public string? CepTelefonu { get; set; }
    public string? Eposta { get; set; }
    public string? Adres { get; set; }

    public int? SubeId { get; set; }
    public Sube? Sube { get; set; }
    public int? BolumId { get; set; }
    public Bolum? Bolum { get; set; }
    public int? GorevId { get; set; }
    public Gorev? Gorev { get; set; }
    public int? CalismaGrubuId { get; set; }
    public CalismaGrubu? CalismaGrubu { get; set; }

    // --- Genel / SGK ---
    public string? KartId { get; set; }
    public string? IlkSoyad { get; set; }
    public string? VergiNo { get; set; }
    public string? SgkSicilNo { get; set; }
    public DateOnly? SgkGirisTarihi { get; set; }
    /// <summary>Yıllık izin, kıdem ve ihbar hesabında esas alınan ilk işe başlama tarihi.</summary>
    public DateOnly? IlkIseBaslamaTarihi { get; set; }

    // --- Ücret ---
    public UcretTipi UcretTipi { get; set; } = UcretTipi.Maas;
    /// <summary>Aylık maaş ya da saat ücreti (<see cref="UcretTipi"/>'ne göre).</summary>
    public decimal? Ucret { get; set; }
    public decimal? EskiUcret { get; set; }
    /// <summary>Boşsa fazla mesai ücreti grup kuralından hesaplanır.</summary>
    public decimal? MesaiSaatUcreti { get; set; }
    public bool FazlaMesaiAlir { get; set; } = true;
    /// <summary>Devamsız gün için kesinti katsayısı (örn. 1,5 ya da 2). Boşsa 1.</summary>
    public decimal? DevamsizlikCezaKatsayisi { get; set; }

    // --- Ek gruplamalar ---
    public int? ServisId { get; set; }
    public Servis? Servis { get; set; }
    public int? BolgeId { get; set; }
    public Bolge? Bolge { get; set; }
    public int? BirimId { get; set; }
    public Birim? Birim { get; set; }
    public int? TakimId { get; set; }
    public Takim? Takim { get; set; }
    public int? MeslekId { get; set; }
    public Meslek? Meslek { get; set; }

    // --- Kişisel ---
    public string? MedeniHal { get; set; }
    public int? CocukSayisi { get; set; }
    public string? KanGrubu { get; set; }
    public string? EgitimDurumu { get; set; }
    public string? AskerlikDurumu { get; set; }
    public string? EvTelefonu { get; set; }
    public string? Il { get; set; }
    public string? Ilce { get; set; }

    // --- Kimlik ---
    public string? BabaAdi { get; set; }
    public string? AnneAdi { get; set; }
    public string? DogumYeri { get; set; }
    public string? Uyruk { get; set; }
    public string? NufusIl { get; set; }
    public string? NufusIlce { get; set; }

    // --- Banka / ehliyet ---
    public string? BankaAdi { get; set; }
    public string? Iban { get; set; }
    public string? EhliyetSinifi { get; set; }
    public string? EhliyetNo { get; set; }

    // --- Kişisel (sicil kartı 2. sekme) ---
    public string? IsTelefonu { get; set; }
    public string? WebAdresi { get; set; }
    public string? Gozluk { get; set; }
    public string? Sabika { get; set; }
    public string? MezunOkul { get; set; }
    public string? MezunBrans { get; set; }
    public DateOnly? MezuniyetTarihi { get; set; }
    public string? OkulNo { get; set; }
    public string? YabanciDil { get; set; }
    public int? Kilo { get; set; }
    public int? Boy { get; set; }
    public string? AyakkabiNo { get; set; }
    public string? ElbiseBeden { get; set; }
    public string? Aciklama { get; set; }

    // --- Kimlik (3. sekme: nüfus cüzdanı) ---
    public string? NufusMahalleKoy { get; set; }
    public string? CiltNo { get; set; }
    public string? AileSiraNo { get; set; }
    public string? SiraNo { get; set; }
    public string? CuzdanVerildigiYer { get; set; }
    public string? CuzdanVerilisNedeni { get; set; }
    public string? CuzdanKayitNo { get; set; }
    public DateOnly? CuzdanVerilisTarihi { get; set; }

    // --- Ehliyet (4. sekme) ---
    public string? EhliyetVerildigiYer { get; set; }
    public DateOnly? EhliyetVerilisTarihi { get; set; }
    public string? CihazProtez { get; set; }

    // --- Kazanç-kesinti parametreleri (5. sekme). Günlük seçilirse tutar × geldiği gün; aylıksa sabit. Boşsa eklenmez. ---
    public decimal? PrimUcreti { get; set; }
    public bool PrimGunluk { get; set; }
    public decimal? YolUcreti { get; set; }
    public bool YolGunluk { get; set; }
    public decimal? YemekUcreti { get; set; }
    public bool YemekGunluk { get; set; }
    public decimal? OzelUcret { get; set; }
    public bool OzelGunluk { get; set; }
    public bool WcKesintisiUygula { get; set; }

    public int? EskiId { get; set; }

    public string AdSoyad => $"{Ad} {Soyad}".Trim();

    /// <summary>Verilen tarihte işte mi (işe girmiş, henüz çıkmamış).</summary>
    public bool CalisiyorMu(DateOnly tarih) =>
        IseGirisTarihi <= tarih && (IstenCikisTarihi is null || IstenCikisTarihi >= tarih);
}
