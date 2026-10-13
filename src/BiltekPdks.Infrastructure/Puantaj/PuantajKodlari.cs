using BiltekPdks.Domain.Puantaj;

namespace BiltekPdks.Infrastructure.Puantaj;

/// <summary>Toplu puantaj tablosunda ve Excel'de günün kısa kodu ve açıklaması.</summary>
public static class PuantajKodlari
{
    public static string Kod(GunlukPuantaj g) => g.Durum switch
    {
        PuantajDurumu.Calisti => "Ç",
        PuantajDurumu.Devamsiz => "D",
        PuantajDurumu.Izinli => g.IzinUcretli ? "İ" : "Üİ",
        PuantajDurumu.HaftaTatili => g.TatilMesaisiDakika > 0 ? "HÇ" : "HT",
        PuantajDurumu.ResmiTatil => g.TatilMesaisiDakika > 0 ? "RÇ" : "RT",
        PuantajDurumu.EksikHareket => "E",
        _ => "",
    };

    public static string Aciklama(GunlukPuantaj g)
    {
        var parcalar = new List<string> { g.Tarih.ToString("dd.MM.yyyy"), DurumAdi(g) };
        if (g.Giris is { } gi) parcalar.Add($"Giriş {gi:HH:mm}");
        if (g.Cikis is { } ci) parcalar.Add($"Çıkış {ci:HH:mm}");
        if (g.NormalCalismaDakika > 0) parcalar.Add($"N.Ç {PuantajMotoru.SaatDakika(g.NormalCalismaDakika)}");
        if (g.FazlaMesaiDakika > 0) parcalar.Add($"%50 {PuantajMotoru.SaatDakika(g.FazlaMesaiDakika)}");
        if (g.TatilMesaisiDakika > 0) parcalar.Add($"%100 {PuantajMotoru.SaatDakika(g.TatilMesaisiDakika)}");
        if (g.GecKalmaDakika > 0) parcalar.Add($"Geç {PuantajMotoru.SaatDakika(g.GecKalmaDakika)}");
        if (g.ErkenCikmaDakika > 0) parcalar.Add($"Erken {PuantajMotoru.SaatDakika(g.ErkenCikmaDakika)}");
        if (g.EksikDakika > 0) parcalar.Add($"Eksik {PuantajMotoru.SaatDakika(g.EksikDakika)}");
        return string.Join(" · ", parcalar);
    }

    public static string DurumAdi(GunlukPuantaj g) => g.Durum switch
    {
        PuantajDurumu.Calisti => "Çalıştı",
        PuantajDurumu.Devamsiz => "Devamsız",
        PuantajDurumu.Izinli => g.IzinUcretli ? "İzinli" : "Ücretsiz izin",
        PuantajDurumu.HaftaTatili => "Hafta tatili",
        PuantajDurumu.ResmiTatil => "Resmi tatil",
        PuantajDurumu.EksikHareket => "Eksik hareket",
        _ => "Kapsam dışı",
    };

    /// <summary>Kodların anlamı (tablo altındaki açıklama ve Excel için).</summary>
    public static readonly IReadOnlyList<(string Kod, string Anlam)> Lejant = new[]
    {
        ("Ç", "Çalıştı"), ("D", "Devamsız"), ("İ", "Ücretli izin"), ("Üİ", "Ücretsiz izin"),
        ("HT", "Hafta tatili"), ("HÇ", "Hafta tatilinde çalıştı"), ("RT", "Resmi tatil"),
        ("RÇ", "Resmi tatilde çalıştı"), ("E", "Eksik hareket (giriş ya da çıkış yok)"),
    };

    /// <summary>Saat:dakika yerine Excel'de hesaplanabilir ondalık saat.</summary>
    public static double Saat(int dakika) => Math.Round(dakika / 60.0, 2);
}
