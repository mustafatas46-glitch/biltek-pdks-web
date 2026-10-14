using System.Globalization;
using System.Text.RegularExpressions;
using ClosedXML.Excel;

namespace BiltekPdks.Infrastructure.Aktarim;

/// <summary>Excel hücresinin ham değeri: yazı, sayı ya da tarih (hangisi geldiyse).</summary>
public readonly record struct Hucre(string Metin, double? Sayi = null, DateTime? Tarih = null, TimeSpan? Sure = null)
{
    public bool Bos => string.IsNullOrWhiteSpace(Metin) && Sayi is null && Tarih is null && Sure is null;
    public override string ToString() => Metin;
}

/// <summary>Excel dosyasının ilk sayfası: başlıklar ve veri satırları.</summary>
public sealed class ExcelTablo
{
    /// <summary>Bir dosyada en çok kaç veri satırı okunur (çok büyük dosyalar bölünerek aktarılmalı).</summary>
    public const int AzamiSatir = 300_000;

    public required string SayfaAdi { get; init; }
    public required IReadOnlyList<string> Basliklar { get; init; }
    /// <summary>Veri satırları; her biri <see cref="Basliklar"/> ile aynı uzunlukta.</summary>
    public required IReadOnlyList<Hucre[]> Satirlar { get; init; }
    /// <summary>Satırların Excel'deki satır numarası (hata mesajlarında gösterilir).</summary>
    public required IReadOnlyList<int> SatirNolari { get; init; }

    public int SutunBul(string? baslik)
    {
        if (string.IsNullOrWhiteSpace(baslik)) return -1;
        for (var i = 0; i < Basliklar.Count; i++)
            if (string.Equals(Basliklar[i], baslik, StringComparison.OrdinalIgnoreCase)) return i;
        return -1;
    }

    /// <param name="baslikSatiri">Başlıkların bulunduğu satır (1'den başlar). Altındaki satırlar veri sayılır.</param>
    public static ExcelTablo Oku(Stream dosya, int baslikSatiri = 1, string? sayfaAdi = null)
    {
        if (baslikSatiri < 1) baslikSatiri = 1;
        XLWorkbook kitap;
        try { kitap = new XLWorkbook(dosya); }
        catch (Exception e) when (e is not OutOfMemoryException)
        {
            throw new ArgumentException("Dosya okunamadı. Excel (.xlsx) dosyası seçin; eski .xls dosyasını Excel'de .xlsx olarak kaydedin.", e);
        }

        using (kitap)
        {
            var sayfa = sayfaAdi is null ? kitap.Worksheets.First() : kitap.Worksheet(sayfaAdi);
            var kullanilan = sayfa.RangeUsed();
            if (kullanilan is null) throw new ArgumentException("Excel sayfası boş.");
            var ilkSutun = kullanilan.FirstColumn().ColumnNumber();
            var sonSutun = kullanilan.LastColumn().ColumnNumber();
            var sonSatir = kullanilan.LastRow().RowNumber();

            var basliklar = new List<string>();
            var gorulen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            for (var c = ilkSutun; c <= sonSutun; c++)
            {
                var b = Duzelt(sayfa.Cell(baslikSatiri, c).GetFormattedString());
                if (b.Length == 0) b = $"Sütun {XLHelper.GetColumnLetterFromNumber(c)}";
                var ad = b;
                for (var n = 2; !gorulen.Add(ad); n++) ad = $"{b} ({n})";
                basliklar.Add(ad);
            }

            var satirlar = new List<Hucre[]>();
            var nolar = new List<int>();
            for (var r = baslikSatiri + 1; r <= sonSatir; r++)
            {
                var satir = new Hucre[basliklar.Count];
                var dolu = false;
                for (var c = ilkSutun; c <= sonSutun; c++)
                {
                    var h = HucreOku(sayfa.Cell(r, c));
                    satir[c - ilkSutun] = h;
                    dolu |= !h.Bos;
                }
                if (!dolu) continue;
                if (satirlar.Count >= AzamiSatir)
                    throw new ArgumentException($"Dosyada {AzamiSatir:N0} satırdan fazla veri var. Dosyayı bölerek aktarın.");
                satirlar.Add(satir);
                nolar.Add(r);
            }

            return new ExcelTablo { SayfaAdi = sayfa.Name, Basliklar = basliklar, Satirlar = satirlar, SatirNolari = nolar };
        }
    }

    private static Hucre HucreOku(IXLCell c)
    {
        var v = c.Value;
        return v.Type switch
        {
            XLDataType.Number => new Hucre(c.GetFormattedString(), Sayi: v.GetNumber()),
            XLDataType.DateTime => new Hucre(c.GetFormattedString(), Tarih: v.GetDateTime()),
            XLDataType.TimeSpan => new Hucre(c.GetFormattedString(), Sure: v.GetTimeSpan()),
            XLDataType.Text => new Hucre(Duzelt(v.GetText())),
            XLDataType.Boolean => new Hucre(v.GetBoolean() ? "1" : "0", Sayi: v.GetBoolean() ? 1 : 0),
            _ => new Hucre(""),
        };
    }

    private static string Duzelt(string s) => Regex.Replace(s ?? "", @"\s+", " ").Trim();
}

/// <summary>Türkçe Excel dosyalarındaki tarih, saat ve sayı biçimlerini çözer.</summary>
public static class DegerCozucu
{
    private static readonly CultureInfo Tr = CultureInfo.GetCultureInfo("tr-TR");

    private static readonly string[] TarihBicimleri =
    {
        "dd.MM.yyyy", "d.M.yyyy", "dd/MM/yyyy", "d/M/yyyy", "dd-MM-yyyy", "d-M-yyyy", "yyyy-MM-dd", "yyyy.MM.dd", "dd.MM.yy", "d.M.yy",
    };

    private static readonly string[] ZamanBicimleri =
    {
        "dd.MM.yyyy HH:mm", "dd.MM.yyyy HH:mm:ss", "d.M.yyyy H:mm", "d.M.yyyy H:mm:ss", "dd/MM/yyyy HH:mm", "dd/MM/yyyy HH:mm:ss",
        "d/M/yyyy H:mm", "d/M/yyyy H:mm:ss", "yyyy-MM-dd HH:mm", "yyyy-MM-dd HH:mm:ss", "yyyy-MM-ddTHH:mm:ss", "yyyy-MM-ddTHH:mm",
        "dd-MM-yyyy HH:mm", "dd-MM-yyyy HH:mm:ss", "dd.MM.yy HH:mm",
    };

    private static readonly string[] SaatBicimleri = { "HH:mm", "H:mm", "HH:mm:ss", "H:mm:ss", "HH.mm", "H.mm", "HHmm" };

    private static readonly string[] AyBicimleri = { "MM.yyyy", "M.yyyy", "MM/yyyy", "M/yyyy", "yyyy-MM", "yyyy.MM", "yyyy/MM" };

    public static DateOnly? Tarih(Hucre h)
    {
        if (h.Tarih is { } t) return DateOnly.FromDateTime(t);
        if (h.Sayi is { } s && s is > 1000 and < 2_958_465) return DateOnly.FromDateTime(DateTime.FromOADate(Math.Floor(s)));
        var m = h.Metin.Trim();
        if (m.Length == 0) return null;
        if (DateTime.TryParseExact(m, TarihBicimleri, Tr, DateTimeStyles.None, out var d)) return DateOnly.FromDateTime(d);
        if (DateTime.TryParseExact(m, ZamanBicimleri, Tr, DateTimeStyles.None, out d)) return DateOnly.FromDateTime(d);
        return null;
    }

    /// <summary>Ay başı: "09.2026", "2026-09" ya da herhangi bir tarih (ayın ilk günü alınır).</summary>
    public static DateOnly? AyBasi(Hucre h)
    {
        var m = h.Metin.Trim();
        if (h.Tarih is null && h.Sayi is null && DateTime.TryParseExact(m, AyBicimleri, Tr, DateTimeStyles.None, out var a))
            return new DateOnly(a.Year, a.Month, 1);
        var t = Tarih(h);
        return t is { } x ? new DateOnly(x.Year, x.Month, 1) : null;
    }

    public static TimeOnly? Saat(Hucre h)
    {
        if (h.Sure is { } sure && sure >= TimeSpan.Zero && sure < TimeSpan.FromDays(1)) return TimeOnly.FromTimeSpan(sure);
        if (h.Tarih is { } t) return TimeOnly.FromDateTime(t);
        if (h.Sayi is { } s && s is >= 0 and < 1) return TimeOnly.FromTimeSpan(TimeSpan.FromMinutes(Math.Round(s * 24 * 60)));
        var m = h.Metin.Trim();
        if (m.Length == 0) return null;
        if (TimeOnly.TryParseExact(m, SaatBicimleri, Tr, DateTimeStyles.None, out var z)) return z;
        return null;
    }

    public static DateTime? Zaman(Hucre h)
    {
        if (h.Tarih is { } t) return t;
        if (h.Sayi is { } s && s is > 1000 and < 2_958_465) return DateTime.FromOADate(s);
        var m = h.Metin.Trim();
        if (m.Length == 0) return null;
        if (DateTime.TryParseExact(m, ZamanBicimleri, Tr, DateTimeStyles.None, out var d)) return d;
        return null;
    }

    /// <summary>"1.234,56", "1234,56", "1234.56", "₺ 1.234" gibi yazımları çözer.</summary>
    public static decimal? Sayi(Hucre h)
    {
        if (h.Sayi is { } s) return Math.Round((decimal)s, 4);
        var m = Regex.Replace(h.Metin, @"[\s₺$€]|TL|tl", "");
        if (m.Length == 0) return null;
        var virgul = m.LastIndexOf(',');
        var nokta = m.LastIndexOf('.');
        if (virgul >= 0 && nokta >= 0)
            m = virgul > nokta ? m.Replace(".", "").Replace(',', '.') : m.Replace(",", "");
        else if (virgul >= 0)
            m = m.Replace(',', '.');
        else if (nokta >= 0 && Regex.IsMatch(m, @"^-?\d{1,3}(\.\d{3})+$"))
            m = m.Replace(".", "");
        return decimal.TryParse(m, NumberStyles.Number, CultureInfo.InvariantCulture, out var d) ? d : null;
    }

    /// <summary>Sayı hücresindeki kart / sicil numarasını "1234.0" yerine "1234" olarak verir.</summary>
    public static string Kod(Hucre h)
    {
        if (h.Sayi is { } s && Math.Abs(s - Math.Round(s)) < 1e-9 && Math.Abs(s) < 1e15) return ((long)Math.Round(s)).ToString(CultureInfo.InvariantCulture);
        return h.Metin.Trim();
    }

    public static string Metin(Hucre h) => h.Metin.Trim();

    /// <summary>Karşılaştırma için: büyük harf, Türkçe karakterler sadeleşir, boşluk ve noktalama atılır.</summary>
    public static string Anahtar(string? s)
    {
        if (string.IsNullOrWhiteSpace(s)) return "";
        var k = s.Trim().ToUpper(Tr)
            .Replace('İ', 'I').Replace('Ş', 'S').Replace('Ğ', 'G').Replace('Ü', 'U').Replace('Ö', 'O').Replace('Ç', 'C');
        return Regex.Replace(k, @"[^A-Z0-9]", "");
    }
}
