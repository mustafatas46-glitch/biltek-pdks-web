using BiltekPdks.Domain.Entities;
using BiltekPdks.Infrastructure.Data;
using BiltekPdks.Infrastructure.Puantaj;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;

namespace BiltekPdks.Infrastructure.Raporlar;

/// <summary>Bir personelin ay içindeki PDKS puantajı ile muhasebe bordrosunun karşılaştırması.</summary>
public sealed record UyumSatiri(Personel Personel, AylikPuantaj? Puantaj, BordroSatiri? Bordro)
{
    /// <summary>FM saatinde bu kadarlık fark (yuvarlama) uyumlu sayılır.</summary>
    public const decimal FmToleransSaat = 0.5m;

    public decimal? PdksGun => Puantaj?.OdenecekGun;
    public decimal? PdksIzin => Puantaj is null ? null : Puantaj.Toplam.IzinliGun + Puantaj.Toplam.UcretsizIzinGun;
    public decimal? PdksDevamsiz => Puantaj?.Toplam.DevamsizGun;
    public decimal? PdksFmSaat => Puantaj is null ? null : Math.Round((Puantaj.Toplam.FazlaMesaiDakika + Puantaj.Toplam.TatilMesaisiDakika) / 60m, 2);

    public decimal? GunFarki => Fark(Bordro?.CalisilanGun, PdksGun);
    public decimal? IzinFarki => Fark(Bordro?.IzinGun, PdksIzin);
    public decimal? DevamsizFarki => Fark(Bordro?.DevamsizGun, PdksDevamsiz);
    public decimal? FmFarki => Fark(Bordro?.FazlaMesaiSaat, PdksFmSaat);

    private static decimal? Fark(decimal? bordro, decimal? pdks) => bordro is { } b && pdks is { } p ? b - p : null;

    /// <summary>Uyumsuzluğun açıklaması; boşsa satır uyumludur.</summary>
    public IReadOnlyList<string> Farklar
    {
        get
        {
            var l = new List<string>();
            if (Bordro is null) { l.Add("Bordroda yok"); return l; }
            if (Puantaj is null) { l.Add("Bu ay PDKS'de çalışmıyor"); return l; }
            if (GunFarki is { } g && g != 0) l.Add($"Gün {Isaret(g)}");
            if (IzinFarki is { } i && i != 0) l.Add($"İzin {Isaret(i)}");
            if (DevamsizFarki is { } d && d != 0) l.Add($"Devamsız {Isaret(d)}");
            if (FmFarki is { } f && Math.Abs(f) > FmToleransSaat) l.Add($"FM {Isaret(f)} saat");
            if (Puantaj.Toplam.EksikHareketGun > 0) l.Add($"{Puantaj.Toplam.EksikHareketGun} gün eksik hareket");
            return l;
        }
    }

    public bool Uyumlu => Farklar.Count == 0;

    private static string Isaret(decimal x) => (x > 0 ? "+" : "") + x.ToString("0.##");
}

/// <summary>
/// Bordro–PDKS uyum raporu: muhasebeden aktarılan bordro ile kart hareketlerinden hesaplanan puantajı yan yana koyar.
/// Farklar burada görünür; düzeltme gerekiyorsa normal ekranlardan (izin, elle hareket) açık kayıtla yapılır.
/// </summary>
public static class BordroUyumu
{
    public static async Task<List<UyumSatiri>> HesaplaAsync(PdksDbContext db, PuantajServisi puantaj, int yil, int ay, int? bolumId = null, CancellationToken ct = default)
    {
        var pdks = await puantaj.TopluAsync(db, yil, ay, bolumId, null, ct);
        var bordrolar = await db.BordroSatirlari.AsNoTracking().Include(b => b.Personel)
            .Where(b => b.Yil == yil && b.Ay == ay && (bolumId == null || b.Personel!.BolumId == bolumId))
            .ToListAsync(ct);
        var bordroSozluk = bordrolar.ToDictionary(b => b.PersonelId);

        var sonuc = pdks.Select(p => new UyumSatiri(p.Personel, p, bordroSozluk.GetValueOrDefault(p.Personel.Id))).ToList();
        var pdksteOlan = pdks.Select(p => p.Personel.Id).ToHashSet();
        sonuc.AddRange(bordrolar.Where(b => !pdksteOlan.Contains(b.PersonelId) && b.Personel is not null)
            .Select(b => new UyumSatiri(b.Personel!, null, b)));
        return sonuc.OrderBy(x => x.Personel.Ad).ThenBy(x => x.Personel.Soyad).ToList();
    }

    public static byte[] Excel(IReadOnlyList<UyumSatiri> satirlar, int yil, int ay, string firmaAdi)
    {
        using var kitap = new XLWorkbook();
        var s = kitap.Worksheets.Add("Bordro-PDKS uyum");
        s.Cell(1, 1).Value = $"{firmaAdi} — {TopluPuantajExcel.AyAdlari[ay - 1]} {yil} Bordro–PDKS uyum raporu";
        s.Cell(1, 1).Style.Font.Bold = true;
        s.Cell(1, 1).Style.Font.FontSize = 14;

        var basliklar = new[]
        {
            "Sicil", "Kart", "Ad Soyad", "Gün PDKS", "Gün Bordro", "İzin PDKS", "İzin Bordro",
            "Devamsız PDKS", "Devamsız Bordro", "FM saat PDKS", "FM saat Bordro", "Eksik hareket gün", "Durum",
        };
        for (var i = 0; i < basliklar.Length; i++) s.Cell(3, i + 1).Value = basliklar[i];
        s.Row(3).Style.Font.Bold = true;

        var r = 4;
        foreach (var x in satirlar)
        {
            s.Cell(r, 1).Value = x.Personel.SicilNo ?? "";
            s.Cell(r, 2).Value = x.Personel.KartNo;
            s.Cell(r, 3).Value = $"{x.Personel.Ad} {x.Personel.Soyad}";
            Yaz(s.Cell(r, 4), x.PdksGun);
            Yaz(s.Cell(r, 5), x.Bordro?.CalisilanGun);
            Yaz(s.Cell(r, 6), x.PdksIzin);
            Yaz(s.Cell(r, 7), x.Bordro?.IzinGun);
            Yaz(s.Cell(r, 8), x.PdksDevamsiz);
            Yaz(s.Cell(r, 9), x.Bordro?.DevamsizGun);
            Yaz(s.Cell(r, 10), x.PdksFmSaat);
            Yaz(s.Cell(r, 11), x.Bordro?.FazlaMesaiSaat);
            Yaz(s.Cell(r, 12), x.Puantaj?.Toplam.EksikHareketGun);
            s.Cell(r, 13).Value = x.Uyumlu ? "Uyumlu" : string.Join("; ", x.Farklar);
            if (!x.Uyumlu) s.Range(r, 1, r, 13).Style.Fill.BackgroundColor = XLColor.FromHtml("#FDECEA");
            r++;
        }
        s.Columns().AdjustToContents();
        s.SheetView.FreezeRows(3);
        using var ms = new MemoryStream();
        kitap.SaveAs(ms);
        return ms.ToArray();
    }

    private static void Yaz(IXLCell c, decimal? v)
    {
        if (v is { } d) c.Value = (double)d;
    }
}
