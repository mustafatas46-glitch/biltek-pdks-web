using BiltekPdks.Domain.Entities;
using BiltekPdks.Domain.Puantaj;
using BiltekPdks.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BiltekPdks.Infrastructure.Puantaj;

public sealed record MaasKalemi(string Ad, string Aciklama, decimal Tutar);

/// <summary>Bir personelin aylık maaş pusulası: puantajdan gelen kazançlar ve kesintiler.</summary>
public sealed class MaasPusulasi
{
    public required AylikPuantaj Puantaj { get; init; }
    public Personel Personel => Puantaj.Personel;
    public decimal Ucret { get; init; }
    public UcretTipi UcretTipi { get; init; }
    public decimal SaatUcreti { get; init; }
    public decimal MesaiSaatUcreti { get; init; }
    public decimal GunlukUcret { get; init; }
    public List<MaasKalemi> Kazanclar { get; } = new();
    public List<MaasKalemi> Kesintiler { get; } = new();
    public List<string> Uyarilar { get; } = new();

    public decimal ToplamKazanc => Kazanclar.Sum(x => x.Tutar);
    public decimal ToplamKesinti => Kesintiler.Sum(x => x.Tutar);
    public decimal NetOdenecek => ToplamKazanc - ToplamKesinti;
}

/// <summary>
/// Puantaj sonuçlarından maaş pusulası hesaplar.
/// <list type="bullet">
/// <item>Aylık ücretli: ücret / 30 × gün (30 gün hesabı). Saat ücreti = ücret / maaş böleni (grup, varsayılan 225).</item>
/// <item>Saat ücretli: normal çalışma saati × saat ücreti; ücretli izin günleri günlük çalışma süresiyle ödenir.</item>
/// <item>Fazla mesai %50 ve tatil mesaisi %100: saat × (personelin mesai saat ücreti ya da saat ücreti) × grup çarpanı.</item>
/// <item>Prim / yol / yemek / özel ücret: "günlük" işaretliyse çalışılan gün ile çarpılır.</item>
/// <item>Ceza: geç / erken / eksik için katsayılı kesinti; devamsız günde yalnızca günlük ücreti aşan kısım (gün zaten 30'dan düşer).</item>
/// <item>Ay içindeki kazanç, kesinti ve avans kalemleri eklenir.</item>
/// </list>
/// Vergi ve SGK hesaplanmaz; ücret sicil kartına nasıl girildiyse (genelde net) o esas alınır.
/// </summary>
public static class MaasHesabi
{
    public static readonly string[] EkUcretAdlari = { "Prim (sicil)", "Yol ücreti (sicil)", "Yemek ücreti (sicil)", "Özel ücret (sicil)" };

    public static MaasPusulasi Hesapla(AylikPuantaj a, IReadOnlyList<KazancKesinti> kalemler)
    {
        var p = a.Personel;
        var grup = p.CalismaGrubu;
        var t = a.Toplam;
        var boleni = grup?.MaasBoleniSaat is int b and > 0 ? b : 225;
        var fmCarpan = grup?.FazlaMesaiCarpani is decimal f and > 0 ? f : 1.5m;
        var tatilCarpan = grup?.TatilMesaisiCarpani is decimal tc and > 0 ? tc : 2m;
        var gunlukDakika = grup?.GunlukCalismaDakika is int gd and > 0 ? gd : 450;

        var ucret = p.Ucret ?? 0;
        var saatUcretli = p.UcretTipi == UcretTipi.SaatUcreti;
        var saatUcreti = saatUcretli ? ucret : ucret / boleni;
        var gunlukUcret = saatUcretli ? saatUcreti * gunlukDakika / 60m : ucret / 30m;
        var mesaiSaatUcreti = p.MesaiSaatUcreti is decimal m and > 0 ? m : saatUcreti;

        var s = new MaasPusulasi
        {
            Puantaj = a, Ucret = ucret, UcretTipi = p.UcretTipi,
            SaatUcreti = Yuvarla(saatUcreti), MesaiSaatUcreti = Yuvarla(mesaiSaatUcreti), GunlukUcret = Yuvarla(gunlukUcret),
        };
        if (p.Ucret is null) s.Uyarilar.Add("Sicil kartında ücret girilmemiş.");

        // --- Kazançlar ---
        if (saatUcretli)
        {
            var saat = t.NormalCalismaDakika / 60m;
            s.Kazanclar.Add(new("Normal çalışma", $"{Saat(t.NormalCalismaDakika)} saat × {saatUcreti:N2}", Yuvarla(saat * saatUcreti)));
            if (t.IzinliGun > 0)
                s.Kazanclar.Add(new("Ücretli izin", $"{t.IzinliGun} gün × {gunlukUcret:N2}", Yuvarla(t.IzinliGun * gunlukUcret)));
        }
        else
        {
            s.Kazanclar.Add(new("Normal çalışma", $"{a.OdenecekGun} gün × {gunlukUcret:N2} (30 gün hesabı)", Yuvarla(a.OdenecekGun * gunlukUcret)));
        }

        if (p.FazlaMesaiAlir)
        {
            if (t.FazlaMesaiDakika > 0)
                s.Kazanclar.Add(new("Fazla mesai %50", $"{Saat(t.FazlaMesaiDakika)} saat × {mesaiSaatUcreti:N2} × {fmCarpan:0.##}",
                    Yuvarla(t.FazlaMesaiDakika / 60m * mesaiSaatUcreti * fmCarpan)));
            if (t.TatilMesaisiDakika > 0)
                s.Kazanclar.Add(new("Tatil mesaisi %100", $"{Saat(t.TatilMesaisiDakika)} saat × {mesaiSaatUcreti:N2} × {tatilCarpan:0.##}",
                    Yuvarla(t.TatilMesaisiDakika / 60m * mesaiSaatUcreti * tatilCarpan)));
        }
        else if (t.FazlaMesaiDakika + t.TatilMesaisiDakika > 0)
        {
            s.Uyarilar.Add($"Personel fazla mesai almıyor; {Saat(t.FazlaMesaiDakika + t.TatilMesaisiDakika)} saat mesai ödenmedi.");
        }

        void EkUcret(string ad, decimal? tutar, bool gunluk)
        {
            if (tutar is not decimal u || u <= 0) return;
            s.Kazanclar.Add(gunluk
                ? new(ad, $"{t.CalisilanGun} gün × {u:N2}", Yuvarla(t.CalisilanGun * u))
                : new(ad, "aylık", Yuvarla(u)));
        }
        // Sicil kartındaki sabit ek ücretler; aynı adlı kazanç-kesinti kalemlerinden ayırmak için "(sicil)" yazılır.
        EkUcret(EkUcretAdlari[0], p.PrimUcreti, p.PrimGunluk);
        EkUcret(EkUcretAdlari[1], p.YolUcreti, p.YolGunluk);
        EkUcret(EkUcretAdlari[2], p.YemekUcreti, p.YemekGunluk);
        EkUcret(EkUcretAdlari[3], p.OzelUcret, p.OzelGunluk);

        foreach (var k in kalemler.Where(x => x.KazancKesintiTuru?.Tip == KazancKesintiTipi.Kazanc))
            s.Kazanclar.Add(new(k.KazancKesintiTuru!.Ad, k.Aciklama ?? k.Tarih.ToString("dd.MM.yyyy"), Yuvarla(k.Tutar)));

        // --- Kesintiler ---
        var cezaDakika = 0;
        foreach (var g in a.Gunler)
        {
            if (g.Durum == PuantajDurumu.Devamsiz) cezaDakika += Math.Max(0, g.CezaDakika - g.PlanlananDakika);
            else if (g.Durum == PuantajDurumu.Calisti) cezaDakika += saatUcretli ? Math.Max(0, g.CezaDakika - g.EksikDakika) : g.CezaDakika;
        }
        if (cezaDakika > 0)
            s.Kesintiler.Add(new("Geç / erken / eksik / devamsızlık cezası", $"{Saat(cezaDakika)} saat × {saatUcreti:N2}",
                Yuvarla(cezaDakika / 60m * saatUcreti)));

        foreach (var k in kalemler.Where(x => x.KazancKesintiTuru?.Tip is KazancKesintiTipi.Kesinti or KazancKesintiTipi.Avans))
            s.Kesintiler.Add(new(k.KazancKesintiTuru!.Ad, k.Aciklama ?? k.Tarih.ToString("dd.MM.yyyy"), Yuvarla(k.Tutar)));

        if (t.EksikHareketGun > 0)
            s.Uyarilar.Add($"{t.EksikHareketGun} günde eksik hareket var; bu günler tamamlanmadan pusula kesinleşmemeli.");
        return s;
    }

    /// <summary>Ayın puantajını ve kazanç-kesinti kalemlerini toplu okuyup pusulaları hesaplar.</summary>
    public static async Task<List<MaasPusulasi>> HesaplaAsync(PdksDbContext db, PuantajServisi puantaj, int yil, int ay,
        int? personelId = null, int? bolumId = null, CancellationToken ct = default)
    {
        List<AylikPuantaj> puantajlar;
        if (personelId is int pid)
            puantajlar = await puantaj.AylikAsync(db, pid, yil, ay, ct) is { } tek ? new() { tek } : new();
        else
            puantajlar = await puantaj.TopluAsync(db, yil, ay, bolumId, null, ct);

        var ilk = new DateOnly(yil, ay, 1);
        var son = ilk.AddMonths(1).AddDays(-1);
        var idler = puantajlar.Select(x => x.Personel.Id).ToList();
        var kalemler = (await db.KazancKesintiler.AsNoTracking().Include(k => k.KazancKesintiTuru)
                .Where(k => k.Tarih >= ilk && k.Tarih <= son).ToListAsync(ct))
            .Where(k => idler.Contains(k.PersonelId))
            .ToLookup(k => k.PersonelId);

        return puantajlar.Select(x => Hesapla(x, kalemler[x.Personel.Id].OrderBy(k => k.Tarih).ToList())).ToList();
    }

    private static decimal Yuvarla(decimal d) => Math.Round(d, 2, MidpointRounding.AwayFromZero);
    private static string Saat(int dakika) => PuantajMotoru.SaatDakika(dakika) is { Length: > 0 } s ? s : "0:00";
}
