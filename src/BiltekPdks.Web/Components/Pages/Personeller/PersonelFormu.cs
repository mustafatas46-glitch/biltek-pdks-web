using System.ComponentModel.DataAnnotations;
using BiltekPdks.Domain.Entities;

namespace BiltekPdks.Web.Components.Pages.Personeller;

/// <summary>Personel düzenleme ekranının form modeli.</summary>
public class PersonelFormu : IValidatableObject
{
    [Required(ErrorMessage = "Kart no zorunlu."), StringLength(20)]
    public string KartNo { get; set; } = "";

    [StringLength(30)] public string? SicilNo { get; set; }

    [Required(ErrorMessage = "Ad zorunlu."), StringLength(60)]
    public string Ad { get; set; } = "";

    [Required(ErrorMessage = "Soyad zorunlu."), StringLength(60)]
    public string Soyad { get; set; } = "";

    [StringLength(11)] public string? TcKimlikNo { get; set; }
    public Cinsiyet Cinsiyet { get; set; }
    public DateOnly? DogumTarihi { get; set; }

    [Required(ErrorMessage = "İşe giriş tarihi zorunlu.")]
    public DateOnly? IseGirisTarihi { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    public DateOnly? IstenCikisTarihi { get; set; }
    [StringLength(500)] public string? IstenAyrilmaNedeni { get; set; }

    [StringLength(30)] public string? CepTelefonu { get; set; }
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta girin."), StringLength(150)]
    public string? Eposta { get; set; }
    [StringLength(500)] public string? Adres { get; set; }

    public int? SubeId { get; set; }
    public int? BolumId { get; set; }
    public int? GorevId { get; set; }
    public int? CalismaGrubuId { get; set; }

    /// <summary>Sicil kartının diğer sekmelerindeki alanlar (kişisel, kimlik, banka, ücret…). Doğrudan bu nesneye bağlanır.</summary>
    public Personel Ek { get; set; } = new();

    // Formun kendisinin yönettiği ya da hiç değiştirilmemesi gereken alanlar.
    private static readonly HashSet<string> EktenKopyalanmaz = new()
    {
        nameof(Personel.Id), nameof(Personel.FirmaId), nameof(Personel.EskiId),
        nameof(KartNo), nameof(SicilNo), nameof(Ad), nameof(Soyad), nameof(TcKimlikNo), nameof(Cinsiyet), nameof(DogumTarihi),
        nameof(IseGirisTarihi), nameof(IstenCikisTarihi), nameof(IstenAyrilmaNedeni), nameof(CepTelefonu), nameof(Eposta), nameof(Adres),
        nameof(SubeId), nameof(BolumId), nameof(GorevId), nameof(CalismaGrubuId),
    };

    private static readonly System.Reflection.PropertyInfo[] EkAlanlari = typeof(Personel).GetProperties()
        .Where(p => p.CanRead && p.CanWrite && !EktenKopyalanmaz.Contains(p.Name) && BasitTurMu(p.PropertyType))
        .ToArray();

    private static bool BasitTurMu(Type t)
    {
        t = Nullable.GetUnderlyingType(t) ?? t;
        return t.IsPrimitive || t.IsEnum || t == typeof(string) || t == typeof(decimal) || t == typeof(DateOnly) || t == typeof(DateTime);
    }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!string.IsNullOrWhiteSpace(TcKimlikNo) && !TcKimlik.GecerliMi(TcKimlikNo.Trim()))
            yield return new ValidationResult("T.C. kimlik numarası geçersiz.", new[] { nameof(TcKimlikNo) });

        if (IstenCikisTarihi is not null && IseGirisTarihi is not null && IstenCikisTarihi < IseGirisTarihi)
            yield return new ValidationResult("Çıkış tarihi işe giriş tarihinden önce olamaz.", new[] { nameof(IstenCikisTarihi) });

        if (Ek.Ucret is < 0 || Ek.PrimUcreti is < 0 || Ek.YolUcreti is < 0 || Ek.YemekUcreti is < 0 || Ek.OzelUcret is < 0)
            yield return new ValidationResult("Ücret alanları eksi olamaz.");
    }

    public static PersonelFormu Kaynak(Personel p) => new()
    {
        KartNo = p.KartNo, SicilNo = p.SicilNo, Ad = p.Ad, Soyad = p.Soyad, TcKimlikNo = p.TcKimlikNo,
        Cinsiyet = p.Cinsiyet, DogumTarihi = p.DogumTarihi, IseGirisTarihi = p.IseGirisTarihi,
        IstenCikisTarihi = p.IstenCikisTarihi, IstenAyrilmaNedeni = p.IstenAyrilmaNedeni,
        CepTelefonu = p.CepTelefonu, Eposta = p.Eposta, Adres = p.Adres,
        SubeId = p.SubeId, BolumId = p.BolumId, GorevId = p.GorevId, CalismaGrubuId = p.CalismaGrubuId,
        Ek = p,
    };

    public void Aktar(Personel p)
    {
        foreach (var alan in EkAlanlari)
        {
            var deger = alan.GetValue(Ek);
            if (deger is string metin) deger = Bosalt(metin);
            alan.SetValue(p, deger);
        }
        p.KartNo = KartNo.Trim();
        p.SicilNo = Bosalt(SicilNo);
        p.Ad = Ad.Trim();
        p.Soyad = Soyad.Trim();
        p.TcKimlikNo = Bosalt(TcKimlikNo);
        p.Cinsiyet = Cinsiyet;
        p.DogumTarihi = DogumTarihi;
        p.IseGirisTarihi = IseGirisTarihi!.Value;
        p.IstenCikisTarihi = IstenCikisTarihi;
        p.IstenAyrilmaNedeni = Bosalt(IstenAyrilmaNedeni);
        p.CepTelefonu = Bosalt(CepTelefonu);
        p.Eposta = Bosalt(Eposta);
        p.Adres = Bosalt(Adres);
        p.SubeId = SubeId;
        p.BolumId = BolumId;
        p.GorevId = GorevId;
        p.CalismaGrubuId = CalismaGrubuId;
    }

    private static string? Bosalt(string? s) => string.IsNullOrWhiteSpace(s) ? null : s.Trim();
}
