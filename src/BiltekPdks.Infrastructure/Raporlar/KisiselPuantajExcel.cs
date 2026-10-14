using BiltekPdks.Domain.Puantaj;
using BiltekPdks.Infrastructure.Puantaj;
using ClosedXML.Excel;

namespace BiltekPdks.Infrastructure.Raporlar;

/// <summary>Bir personelin aylık puantaj raporu (günlük satırlar + toplamlar), yazdırmaya hazır.</summary>
public static class KisiselPuantajExcel
{
    private static readonly string[] GunAdi = { "Pazar", "Pazartesi", "Salı", "Çarşamba", "Perşembe", "Cuma", "Cumartesi" };

    public static byte[] Olustur(AylikPuantaj a, string firmaAdi)
    {
        using var kitap = new XLWorkbook();
        var s = kitap.Worksheets.Add("Puantaj");
        var p = a.Personel;

        s.Cell(1, 1).Value = $"{firmaAdi} — {TopluPuantajExcel.AyAdlari[a.Ay - 1]} {a.Yil} kişisel puantaj";
        s.Cell(1, 1).Style.Font.Bold = true;
        s.Cell(1, 1).Style.Font.FontSize = 14;
        s.Cell(2, 1).Value = $"{p.Ad} {p.Soyad}";
        s.Cell(2, 1).Style.Font.Bold = true;
        s.Cell(3, 1).Value = $"Kart: {p.KartNo}   Sicil: {p.SicilNo}   Bölüm: {p.Bolum?.Ad}   Grup: {p.CalismaGrubu?.Ad}   İşe giriş: {p.IseGirisTarihi:dd.MM.yyyy}";

        const int b = 5;
        string[] basliklar = { "Tarih", "Gün", "Durum", "Giriş", "Çıkış", "N.Ç", "%50", "%100", "Geç", "Erken", "Eksik", "Ceza" };
        for (var i = 0; i < basliklar.Length; i++) s.Cell(b, i + 1).Value = basliklar[i];
        var bas = s.Range(b, 1, b, basliklar.Length);
        bas.Style.Font.Bold = true;
        bas.Style.Fill.BackgroundColor = XLColor.FromHtml("#E9ECEF");
        bas.Style.Border.BottomBorder = XLBorderStyleValues.Thin;

        var r = b + 1;
        foreach (var g in a.Gunler)
        {
            s.Cell(r, 1).Value = g.Tarih.ToDateTime(TimeOnly.MinValue);
            s.Cell(r, 1).Style.DateFormat.Format = "dd.MM.yyyy";
            s.Cell(r, 2).Value = GunAdi[(int)g.Tarih.DayOfWeek];
            s.Cell(r, 3).Value = g.Durum == PuantajDurumu.KapsamDisi ? "" : PuantajKodlari.DurumAdi(g);
            s.Cell(r, 4).Value = g.Giris?.ToString("HH:mm") ?? "";
            s.Cell(r, 5).Value = g.Cikis is { } c ? (DateOnly.FromDateTime(c) != g.Tarih ? c.ToString("dd.MM HH:mm") : c.ToString("HH:mm")) : "";
            int[] dk = { g.NormalCalismaDakika, g.FazlaMesaiDakika, g.TatilMesaisiDakika, g.GecKalmaDakika, g.ErkenCikmaDakika, g.EksikDakika, g.CezaDakika };
            for (var i = 0; i < dk.Length; i++) s.Cell(r, 6 + i).Value = PuantajMotoru.SaatDakika(dk[i]);
            var renk = g.Durum switch
            {
                PuantajDurumu.Devamsiz => XLColor.FromHtml("#F8D7DA"),
                PuantajDurumu.EksikHareket => XLColor.FromHtml("#FFF3CD"),
                PuantajDurumu.Izinli => XLColor.FromHtml("#CFF4FC"),
                PuantajDurumu.HaftaTatili or PuantajDurumu.ResmiTatil => XLColor.FromHtml("#F1F3F5"),
                _ => null,
            };
            if (renk is not null) s.Range(r, 1, r, basliklar.Length).Style.Fill.BackgroundColor = renk;
            r++;
        }

        var t = a.Toplam;
        s.Cell(r, 1).Value = "TOPLAM";
        int[] top = { t.NormalCalismaDakika, t.FazlaMesaiDakika, t.TatilMesaisiDakika, t.GecKalmaDakika, t.ErkenCikmaDakika, t.EksikDakika, t.CezaDakika };
        for (var i = 0; i < top.Length; i++) s.Cell(r, 6 + i).Value = PuantajMotoru.SaatDakika(top[i]);
        s.Range(r, 1, r, basliklar.Length).Style.Font.Bold = true;
        s.Range(r, 1, r, basliklar.Length).Style.Border.TopBorder = XLBorderStyleValues.Thin;

        r += 2;
        (string, object)[] ozet =
        {
            ("Gün (30 gün hesabı)", a.OdenecekGun), ("Çalışılan gün", t.CalisilanGun), ("Devamsız gün", t.DevamsizGun),
            ("Ücretli izin", t.IzinliGun), ("Ücretsiz izin", t.UcretsizIzinGun), ("Eksik hareket", t.EksikHareketGun),
        };
        foreach (var (ad, deger) in ozet)
        {
            s.Cell(r, 1).Value = ad;
            s.Cell(r, 3).Value = (int)deger;
            s.Cell(r, 1).Style.Font.Bold = ad.StartsWith("Gün");
            r++;
        }

        s.Columns(1, basliklar.Length).AdjustToContents();
        s.Column(1).Width = 12;
        s.SheetView.Freeze(b, 0);
        s.PageSetup.PageOrientation = XLPageOrientation.Portrait;
        s.PageSetup.FitToPages(1, 1);

        using var akis = new MemoryStream();
        kitap.SaveAs(akis);
        return akis.ToArray();
    }
}
