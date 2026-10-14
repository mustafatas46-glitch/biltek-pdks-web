using BiltekPdks.Infrastructure.Puantaj;
using ClosedXML.Excel;

namespace BiltekPdks.Infrastructure.Raporlar;

/// <summary>Aylık maaş listesi (kişisel bordro): 1. sayfa özet liste, 2. sayfa kalem kalem dökümü.</summary>
public static class MaasListesiExcel
{
    public static byte[] Olustur(IReadOnlyList<MaasPusulasi> pusulalar, int yil, int ay, string firmaAdi)
    {
        using var kitap = new XLWorkbook();
        var s = kitap.Worksheets.Add("Maaş listesi");
        s.Cell(1, 1).Value = $"{firmaAdi} — {TopluPuantajExcel.AyAdlari[ay - 1]} {yil} maaş listesi";
        s.Cell(1, 1).Style.Font.Bold = true;
        s.Cell(1, 1).Style.Font.FontSize = 14;
        s.Cell(2, 1).Value = "Vergi ve SGK hesaplanmamıştır; ücret sicil kartındaki tutar esas alınmıştır.";
        s.Cell(2, 1).Style.Font.Italic = true;

        string[] b = { "Sicil", "Kart", "Ad Soyad", "Bölüm", "Ücret", "Tip", "Gün (30)", "Normal çalışma", "Fazla mesai", "Tatil mesaisi",
            "Prim/yol/yemek/özel", "Diğer kazanç", "Toplam kazanç", "Ceza", "Kesinti + avans", "Toplam kesinti", "Net ödenecek", "Uyarı" };
        for (var i = 0; i < b.Length; i++) s.Cell(4, i + 1).Value = b[i];
        s.Row(4).Style.Font.Bold = true;

        var r = 5;
        var ek = MaasHesabi.EkUcretAdlari;
        foreach (var p in pusulalar)
        {
            decimal K(string ad) => p.Kazanclar.Where(x => x.Ad == ad).Sum(x => x.Tutar);
            var ekToplam = p.Kazanclar.Where(x => ek.Contains(x.Ad)).Sum(x => x.Tutar);
            var standart = K("Normal çalışma") + K("Ücretli izin") + K("Fazla mesai %50") + K("Tatil mesaisi %100") + ekToplam;
            var ceza = p.Kesintiler.Where(x => x.Ad.Contains("cezası")).Sum(x => x.Tutar);
            object[] d =
            {
                p.Personel.SicilNo ?? "", p.Personel.KartNo, $"{p.Personel.Ad} {p.Personel.Soyad}", p.Personel.Bolum?.Ad ?? "",
                p.Ucret, p.UcretTipi == Domain.Entities.UcretTipi.SaatUcreti ? "Saat" : "Aylık", p.Puantaj.OdenecekGun,
                K("Normal çalışma") + K("Ücretli izin"), K("Fazla mesai %50"), K("Tatil mesaisi %100"), ekToplam,
                p.ToplamKazanc - standart, p.ToplamKazanc, ceza, p.ToplamKesinti - ceza, p.ToplamKesinti, p.NetOdenecek,
                string.Join(" ", p.Uyarilar),
            };
            for (var i = 0; i < d.Length; i++)
                s.Cell(r, i + 1).Value = d[i] switch { decimal m => (double)m, int n => n, _ => d[i]?.ToString() ?? "" };
            r++;
        }
        if (pusulalar.Count > 0)
        {
            s.Cell(r, 3).Value = "TOPLAM";
            for (var c = 8; c <= 17; c++) s.Cell(r, c).FormulaA1 = $"SUM({s.Cell(5, c).Address}:{s.Cell(r - 1, c).Address})";
            s.Row(r).Style.Font.Bold = true;
        }
        s.Range(5, 5, Math.Max(r, 5), 5).Style.NumberFormat.Format = "#,##0.00";
        s.Range(5, 8, Math.Max(r, 5), 17).Style.NumberFormat.Format = "#,##0.00";
        s.Columns().AdjustToContents();
        s.SheetView.FreezeRows(4);

        var d2 = kitap.Worksheets.Add("Döküm");
        string[] b2 = { "Sicil", "Ad Soyad", "Tür", "Kalem", "Açıklama", "Tutar" };
        for (var i = 0; i < b2.Length; i++) d2.Cell(1, i + 1).Value = b2[i];
        d2.Row(1).Style.Font.Bold = true;
        var r2 = 2;
        foreach (var p in pusulalar)
            foreach (var (tur, k) in p.Kazanclar.Select(x => ("Kazanç", x)).Concat(p.Kesintiler.Select(x => ("Kesinti", x))))
            {
                d2.Cell(r2, 1).Value = p.Personel.SicilNo ?? p.Personel.KartNo;
                d2.Cell(r2, 2).Value = $"{p.Personel.Ad} {p.Personel.Soyad}";
                d2.Cell(r2, 3).Value = tur;
                d2.Cell(r2, 4).Value = k.Ad;
                d2.Cell(r2, 5).Value = k.Aciklama;
                d2.Cell(r2, 6).Value = (double)k.Tutar;
                d2.Cell(r2, 6).Style.NumberFormat.Format = "#,##0.00";
                r2++;
            }
        d2.Columns().AdjustToContents();

        using var ms = new MemoryStream();
        kitap.SaveAs(ms);
        return ms.ToArray();
    }
}
