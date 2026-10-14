namespace BiltekPdks.Domain.Entities;

public sealed record YetkiTanimi(string Kod, string Grup, string Aciklama);

/// <summary>
/// Ekran ve işlem yetkileri. Yönetici her şeye erişir; diğer kullanıcılar kendilerine verilen yetkilerle çalışır.
/// Kullanıcıya özel yetki işaretlenmemişse rolün varsayılan yetkileri geçerlidir.
/// </summary>
public static class Yetkiler
{
    public const string PersonelGor = "personel.gor";
    public const string PersonelDuzenle = "personel.duzenle";
    public const string UcretGor = "ucret.gor";
    public const string HareketDuzenle = "hareket.duzenle";
    public const string IzinDuzenle = "izin.duzenle";
    public const string KazancKesintiDuzenle = "kazanckesinti.duzenle";
    public const string PuantajGor = "puantaj.gor";
    public const string TanimDuzenle = "tanim.duzenle";
    public const string HesapParametreleri = "hesapparametre.duzenle";
    public const string VeriAktarim = "veri.aktarim";
    public const string Yedek = "yedek";
    public const string KullaniciYonetimi = "kullanici.yonetim";

    public static readonly IReadOnlyList<YetkiTanimi> Hepsi = new YetkiTanimi[]
    {
        new(PersonelGor, "Personel", "Personel listesi ve sicil kartını görme"),
        new(PersonelDuzenle, "Personel", "Personel ekleme / değiştirme"),
        new(UcretGor, "Personel", "Ücret bilgileri, maaş pusulası, maaş listesi, bordro uyum"),
        new(HareketDuzenle, "İşlemler", "Giriş-çıkış ekleme / düzeltme (toplu dahil)"),
        new(IzinDuzenle, "İşlemler", "İzin ekleme / düzeltme (toplu dahil)"),
        new(KazancKesintiDuzenle, "İşlemler", "Kazanç-kesinti / avans ekleme"),
        new(PuantajGor, "Raporlar", "Puantaj ekranları ve puantaj raporları"),
        new(TanimDuzenle, "Tanımlar", "Bölüm, görev, izin türü, kazanç-kesinti türü tanımları"),
        new(HesapParametreleri, "Tanımlar", "Grup, alt grup, çalışma saatleri, mola, dönem, resmi tatil"),
        new(VeriAktarim, "Araçlar", "Excel'den veri alma"),
        new(Yedek, "Araçlar", "Data yedekleme"),
        new(KullaniciYonetimi, "Araçlar", "Kullanıcı tanımlama ve yetki verme"),
    };

    /// <summary>Kullanıcıya özel yetki verilmemişse rolün yetkileri.</summary>
    public static IReadOnlyList<string> RolVarsayilani(KullaniciRolu rol) => rol switch
    {
        KullaniciRolu.Yonetici => Hepsi.Select(y => y.Kod).ToArray(),
        KullaniciRolu.InsanKaynaklari => Hepsi.Select(y => y.Kod).Where(k => k is not (Yedek or KullaniciYonetimi)).ToArray(),
        KullaniciRolu.BolumSorumlusu => new[] { PersonelGor, PuantajGor, HareketDuzenle, IzinDuzenle },
        _ => new[] { PuantajGor },
    };

    public static string RolAdi(KullaniciRolu rol) => rol switch
    {
        KullaniciRolu.Yonetici => "Yönetici",
        KullaniciRolu.InsanKaynaklari => "İnsan kaynakları",
        KullaniciRolu.BolumSorumlusu => "Bölüm sorumlusu",
        _ => "Personel",
    };
}

/// <summary>Kullanıcıya özel verilen yetki (Kullanici.OzelYetki işaretliyse geçerlidir).</summary>
public class KullaniciYetkisi : FirmaEntity
{
    public int KullaniciId { get; set; }
    public string Yetki { get; set; } = string.Empty;
}
