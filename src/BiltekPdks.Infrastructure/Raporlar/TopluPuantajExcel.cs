using BiltekPdks.Infrastructure.Puantaj;
using ClosedXML.Excel;

namespace BiltekPdks.Infrastructure.Raporlar;

/// <summary>Toplu aylık puantajı Excel dosyasına yazar.</summary>
public static class TopluPuantajExcel
{
    public static readonly string[] AyAdlari =
        { "Ocak", "Şubat", "Mart", "Nisan", "Mayıs", "Haziran", "Temmuz", "Ağustos", "Eylül", "Ekim", "Kasım", "Aralık" };

    private static readonly string[] GunKisa = { "Pz", "Pt", "Sa", "Ça", "Pe", "Cu", "Ct" };

    public static byte[] Olustur(IReadOnlyList<AylikPuantaj> satirlar, int yil, int ay, string firmaAdi)
    {
        using var kitap = new XLWorkbook();
        var s = kitap.Worksheets.Add("Toplu puantaj");
        var gunSayisi = DateTime.DaysInMonth(yil, ay);

        s.Cell(1, 1).Value = $"{firmaAdi} — {AyAdlari[ay - 1]} {yil} toplu puantaj";
        s.Cell(1, 1).Style.Font.Bold = true;
        s.Cell(1, 1).Style.Font.FontSize = 14;

        const int baslik = 3;
        var sabit = new[] { "Sicil", "Kart", "Ad Soyad", "Bölüm" };
        for (var i = 0; i < sabit.Length; i++) s.Cell(baslik, i + 1).Value = sabit[i];
        var ilkGunSutunu = sabit.Length + 1;
        for (var d = 1; d <= gunSayisi; d++)
        {
            var t = new DateOnly(yil, ay, d);
            var c = s.Cell(baslik, ilkGunSutunu + d - 1);
            c.Value = $"{d}\n{GunKisa[(int)t.DayOfWeek]}";
            c.Style.Alignment.WrapText = true;
            if (t.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday) c.Style.Fill.BackgroundColor = XLColor.LightGray;
        }
        var toplamlar = new[] { "Gün (30)", "Çalışılan gün", "Devamsız", "Ücretli izin", "Ücretsiz izin", "Eksik hareket",
            "N.Ç (saat)", "%50 (saat)", "%100 (saat)", "Geç (saat)", "Erken (saat)", "Eksik (saat)", "Ceza (saat)" };
        var ilkToplamSutunu = ilkGunSutunu + gunSayisi;
        for (var i = 0; i < toplamlar.Length; i++) s.Cell(baslik, ilkToplamSutunu + i).Value = toplamlar[i];

        var basSatir = s.Range(baslik, 1, baslik, ilkToplamSutunu + toplamlar.Length - 1);
        basSatir.Style.Font.Bold = true;
        basSatir.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        basSatir.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        basSatir.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
        s.Row(baslik).Height = 30;

        var r = baslik + 1;
        foreach (var p in satirlar)
        {
            s.Cell(r, 1).Value = p.Personel.SicilNo ?? "";
            s.Cell(r, 2).Value = p.Personel.KartNo;
            s.Cell(r, 3).Value = $"{p.Personel.Ad} {p.Personel.Soyad}";
            s.Cell(r, 4).Value = p.Personel.Bolum?.Ad ?? "";
            foreach (var g in p.Gunler)
            {
                var c = s.Cell(r, ilkGunSutunu + g.Tarih.Day - 1);
                var kod = PuantajKodlari.Kod(g);
                c.Value = kod;
                c.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                var renk = Renk(kod);
                if (renk is not null) c.Style.Fill.BackgroundColor = renk;
            }
            var t = p.Toplam;
            object[] degerler =
            {
                p.OdenecekGun, t.CalisilanGun, t.DevamsizGun, t.IzinliGun, t.UcretsizIzinGun, t.EksikHareketGun,
                PuantajKodlari.Saat(t.NormalCalismaDakika), PuantajKodlari.Saat(t.FazlaMesaiDakika),
                PuantajKodlari.Saat(t.TatilMesaisiDakika), PuantajKodlari.Saat(t.GecKalmaDakika),
                PuantajKodlari.Saat(t.ErkenCikmaDakika), PuantajKodlari.Saat(t.EksikDakika),
                PuantajKodlari.Saat(t.CezaDakika),
            };
            for (var i = 0; i < degerler.Length; i++)
                s.Cell(r, ilkToplamSutunu + i).Value = degerler[i] is int n ? n : (double)degerler[i];
            r++;
        }

        if (satirlar.Count > 0)
        {
            s.Cell(r, 3).Value = "TOPLAM";
            s.Cell(r, 3).Style.Font.Bold = true;
            for (var i = 0; i < toplamlar.Length; i++)
            {
                var sutun = ilkToplamSutunu + i;
                var c = s.Cell(r, sutun);
                c.FormulaA1 = $"SUM({s.Cell(baslik + 1, sutun).Address}:{s.Cell(r - 1, sutun).Address})";
                c.Style.Font.Bold = true;
            }
        }

        s.Range(baslik + 1, ilkToplamSutunu + 6, Math.Max(r, baslik + 1), ilkToplamSutunu + toplamlar.Length - 1)
            .Style.NumberFormat.Format = "0.00";
        s.Columns(1, 4).AdjustToContents();
        s.Columns(ilkGunSutunu, ilkToplamSutunu - 1).Width = 4;
        s.Columns(ilkToplamSutunu, ilkToplamSutunu + toplamlar.Length - 1).Width = 11;
        s.Range(baslik, ilkToplamSutunu, baslik, ilkToplamSutunu + toplamlar.Length - 1).Style.Alignment.WrapText = true;
        s.SheetView.Freeze(baslik, sabit.Length);

        var l = r + 2;
        s.Cell(l, 1).Value = "Kodlar:";
        s.Cell(l, 1).Style.Font.Bold = true;
        foreach (var (kod, anlam) in PuantajKodlari.Lejant)
        {
            l++;
            s.Cell(l, 1).Value = kod;
            if (Renk(kod) is { } renk) s.Cell(l, 1).Style.Fill.BackgroundColor = renk;
            s.Cell(l, 2).Value = anlam;
        }

        s.PageSetup.PageOrientation = XLPageOrientation.Landscape;
        s.PageSetup.FitToPages(1, 0);

        using var akis = new MemoryStream();
        kitap.SaveAs(akis);
        return akis.ToArray();
    }

    private static XLColor? Renk(string kod) => kod switch
    {
        "D" => XLColor.FromHtml("#F8D7DA"),
        "E" => XLColor.FromHtml("#FFF3CD"),
        "İ" or "Üİ" => XLColor.FromHtml("#CFF4FC"),
        "HT" or "RT" => XLColor.FromHtml("#E9ECEF"),
        "HÇ" or "RÇ" => XLColor.FromHtml("#D1E7DD"),
        _ => null,
    };
}
