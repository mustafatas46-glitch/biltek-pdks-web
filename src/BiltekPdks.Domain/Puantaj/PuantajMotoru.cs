namespace BiltekPdks.Domain.Puantaj;

/// <summary>Bir günün planlanan çalışma düzeni (vardiya) ve kuralları. Süreler dakika cinsindendir.</summary>
public sealed record VardiyaKurali(
    TimeOnly Baslangic,
    TimeOnly Bitis,
    int MolaDakika = 60,
    int GecKalmaToleransi = 0,
    int ErkenCikmaToleransi = 0,
    int FazlaMesaiEsigi = 30)
{
    /// <summary>Bitiş saati başlangıçtan önce ya da eşitse vardiya ertesi güne taşar (gece vardiyası).</summary>
    public bool GeceyiAsar => Bitis <= Baslangic;

    public DateTime BaslangicZamani(DateOnly tarih) => tarih.ToDateTime(Baslangic);

    public DateTime BitisZamani(DateOnly tarih) =>
        (GeceyiAsar ? tarih.AddDays(1) : tarih).ToDateTime(Bitis);

    /// <summary>Molası düşülmüş planlanan net çalışma süresi.</summary>
    public int PlanlananDakika(DateOnly tarih) =>
        Math.Max(0, (int)(BitisZamani(tarih) - BaslangicZamani(tarih)).TotalMinutes - MolaDakika);
}

public enum PuantajDurumu
{
    Calisti = 1,
    Devamsiz = 2,
    Izinli = 3,
    HaftaTatili = 4,
    ResmiTatil = 5,
    /// <summary>Tek hareket var (giriş ya da çıkış eksik); elle düzeltme gerekir.</summary>
    EksikHareket = 6,
    /// <summary>İşe girişten önce ya da işten çıkıştan sonra.</summary>
    KapsamDisi = 7,
}

/// <summary>Bir personelin bir günü için hesaplama girdisi.</summary>
public sealed record GunGirdisi(
    DateOnly Tarih,
    VardiyaKurali? Vardiya,
    IReadOnlyList<DateTime> Hareketler,
    bool ResmiTatil = false,
    bool Izinli = false,
    bool IzinUcretli = true,
    bool KapsamDisi = false);

/// <summary>Bir günün hesaplanmış puantajı. Tüm süreler dakika.</summary>
public sealed record GunlukPuantaj(
    DateOnly Tarih,
    PuantajDurumu Durum,
    DateTime? Giris,
    DateTime? Cikis,
    int PlanlananDakika,
    int CalismaDakika,
    int NormalCalismaDakika,
    int GecKalmaDakika,
    int ErkenCikmaDakika,
    int EksikDakika,
    int FazlaMesaiDakika,
    int TatilMesaisiDakika,
    bool IzinUcretli = true);

/// <summary>
/// Günlük puantaj hesaplama kuralları. Veritabanından bağımsızdır; aynı girdiye her zaman aynı sonucu verir.
/// <para>
/// Kurallar (1. sürüm):
/// <list type="bullet">
/// <item>Penceredeki ilk hareket giriş, son hareket çıkış kabul edilir (ara çıkışlar sonraki sürümde).</item>
/// <item>Çalışma 4 saati aşarsa vardiya molası düşülür.</item>
/// <item>Geç kalma / erken çıkma tolerans içindeyse sıfırlanır ve eksik süreye yansımaz.</item>
/// <item>Fazla mesai, net çalışmanın planı aştığı kısımdır; eşiğin altındaysa yazılmaz.</item>
/// <item>Tatil günü çalışmada yasal ara dinlenmesi düşülür (≤4 sa: 15 dk, ≤7,5 sa: 30 dk, üstü: 60 dk).</item>
/// </list>
/// </para>
/// </summary>
public static class PuantajMotoru
{
    /// <summary>Vardiyadan önce ve sonra hareket aranacak pay.</summary>
    public static readonly TimeSpan OncekiPay = TimeSpan.FromHours(4);
    public static readonly TimeSpan SonrakiPay = TimeSpan.FromHours(8);

    /// <summary>Vardiyası olmayan günlerde gün başlangıcı (gece yarısından sonraki çıkışlar önceki güne yazılır).</summary>
    public static readonly TimeOnly TatilGunuBaslangici = new(4, 0);

    /// <summary>Bu süreden yakın ardışık okutmalar tek hareket sayılır.</summary>
    public static readonly TimeSpan MukerrerAraligi = TimeSpan.FromMinutes(1);

    public static (DateTime Baslangic, DateTime Bitis) HareketPenceresi(DateOnly tarih, VardiyaKurali? vardiya)
    {
        if (vardiya is null)
        {
            var bas = tarih.ToDateTime(TatilGunuBaslangici);
            return (bas, bas.AddDays(1));
        }
        return (vardiya.BaslangicZamani(tarih) - OncekiPay, vardiya.BitisZamani(tarih) + SonrakiPay);
    }

    /// <summary>Sıralar ve bir dakikadan yakın tekrar okutmaları ayıklar.</summary>
    public static List<DateTime> Temizle(IEnumerable<DateTime> hareketler)
    {
        var sonuc = new List<DateTime>();
        foreach (var h in hareketler.OrderBy(x => x))
        {
            if (sonuc.Count > 0 && h - sonuc[^1] < MukerrerAraligi) continue;
            sonuc.Add(h);
        }
        return sonuc;
    }

    public static int YasalAraDinlenme(int brutDakika) => brutDakika switch
    {
        <= 0 => 0,
        <= 240 => 15,
        <= 450 => 30,
        _ => 60,
    };

    public static GunlukPuantaj Hesapla(GunGirdisi g)
    {
        var v = g.Vardiya;
        var planlanan = v?.PlanlananDakika(g.Tarih) ?? 0;

        if (g.KapsamDisi)
            return Bos(g.Tarih, PuantajDurumu.KapsamDisi, 0);

        var hareketler = Temizle(g.Hareketler);
        DateTime? giris = hareketler.Count > 0 ? hareketler[0] : null;
        DateTime? cikis = hareketler.Count > 1 ? hareketler[^1] : null;
        var brut = giris is not null && cikis is not null ? (int)(cikis.Value - giris.Value).TotalMinutes : 0;

        // İzinli gün: eksik/ceza yazılmaz.
        if (g.Izinli)
            return Bos(g.Tarih, PuantajDurumu.Izinli, planlanan, giris, cikis) with { IzinUcretli = g.IzinUcretli };

        // Planlı çalışma olmayan gün (hafta tatili) ya da resmi tatil: çalışılan süre tatil mesaisidir.
        if (v is null || g.ResmiTatil)
        {
            var durum = g.ResmiTatil ? PuantajDurumu.ResmiTatil : PuantajDurumu.HaftaTatili;
            if (hareketler.Count == 1)
                return Bos(g.Tarih, PuantajDurumu.EksikHareket, 0, giris, null);
            var net = Math.Max(0, brut - YasalAraDinlenme(brut));
            return Bos(g.Tarih, durum, 0, giris, cikis) with { CalismaDakika = net, TatilMesaisiDakika = net };
        }

        if (hareketler.Count == 0)
            return Bos(g.Tarih, PuantajDurumu.Devamsiz, planlanan) with { EksikDakika = planlanan };

        if (hareketler.Count == 1)
            return Bos(g.Tarih, PuantajDurumu.EksikHareket, planlanan, giris, null);

        var vardiyaBas = v.BaslangicZamani(g.Tarih);
        var vardiyaBit = v.BitisZamani(g.Tarih);

        var netCalisma = Math.Max(0, brut - (brut > 240 ? v.MolaDakika : 0));

        var gec = Math.Max(0, (int)(giris!.Value - vardiyaBas).TotalMinutes);
        var erken = Math.Max(0, (int)(vardiyaBit - cikis!.Value).TotalMinutes);

        var tolereEdilen = 0;
        if (gec <= v.GecKalmaToleransi) { tolereEdilen += gec; gec = 0; }
        if (erken <= v.ErkenCikmaToleransi) { tolereEdilen += erken; erken = 0; }

        var normal = Math.Min(planlanan, netCalisma + tolereEdilen);
        var eksik = Math.Max(0, planlanan - normal);
        var fazla = Math.Max(0, netCalisma - planlanan);
        if (fazla < v.FazlaMesaiEsigi) fazla = 0;

        return new GunlukPuantaj(
            g.Tarih, PuantajDurumu.Calisti, giris, cikis, planlanan,
            CalismaDakika: netCalisma,
            NormalCalismaDakika: normal,
            GecKalmaDakika: gec,
            ErkenCikmaDakika: erken,
            EksikDakika: eksik,
            FazlaMesaiDakika: fazla,
            TatilMesaisiDakika: 0);
    }

    private static GunlukPuantaj Bos(DateOnly tarih, PuantajDurumu durum, int planlanan, DateTime? giris = null, DateTime? cikis = null) =>
        new(tarih, durum, giris, cikis, planlanan, 0, 0, 0, 0, 0, 0, 0);

    /// <summary>Dakikayı "8:30" biçiminde yazar.</summary>
    public static string SaatDakika(int dakika) =>
        dakika == 0 ? "" : $"{dakika / 60}:{dakika % 60:00}";
}
