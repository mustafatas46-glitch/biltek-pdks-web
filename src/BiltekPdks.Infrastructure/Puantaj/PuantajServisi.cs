using BiltekPdks.Domain.Entities;
using BiltekPdks.Domain.Puantaj;
using BiltekPdks.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BiltekPdks.Infrastructure.Puantaj;

public sealed record PuantajToplamlari(
    int CalisilanGun,
    int DevamsizGun,
    int IzinliGun,
    int UcretsizIzinGun,
    int EksikHareketGun,
    int NormalCalismaDakika,
    int GecKalmaDakika,
    int ErkenCikmaDakika,
    int EksikDakika,
    int FazlaMesaiDakika,
    int TatilMesaisiDakika,
    int CezaDakika = 0)
{
    public static PuantajToplamlari Hesapla(IReadOnlyCollection<GunlukPuantaj> g) => new(
        g.Count(x => x.Durum == PuantajDurumu.Calisti),
        g.Count(x => x.Durum == PuantajDurumu.Devamsiz),
        g.Count(x => x.Durum == PuantajDurumu.Izinli && x.IzinUcretli),
        g.Count(x => x.Durum == PuantajDurumu.Izinli && !x.IzinUcretli),
        g.Count(x => x.Durum == PuantajDurumu.EksikHareket),
        g.Sum(x => x.NormalCalismaDakika),
        g.Sum(x => x.GecKalmaDakika),
        g.Sum(x => x.ErkenCikmaDakika),
        g.Sum(x => x.EksikDakika),
        g.Sum(x => x.FazlaMesaiDakika),
        g.Sum(x => x.TatilMesaisiDakika),
        g.Sum(x => x.CezaDakika));
}

public sealed record AylikPuantaj(Personel Personel, int Yil, int Ay, IReadOnlyList<GunlukPuantaj> Gunler, PuantajToplamlari Toplam)
{
    /// <summary>Ücret ve SGK için ayın gün sayısı (30 gün hesabı). Bkz. <see cref="OtuzGunHesabi"/>.</summary>
    public int OdenecekGun => OtuzGunHesabi(Personel.IseGirisTarihi, Personel.IstenCikisTarihi, Yil, Ay,
        Toplam.DevamsizGun + Toplam.UcretsizIzinGun);

    /// <summary>
    /// Her ay 30 gün sayılır (ay 28, 29 ya da 31 çekse de).
    /// <list type="bullet">
    /// <item>Ayın tamamında çalışan: 30 − eksik gün.</item>
    /// <item>Ay içinde işe giren ya da ayrılan: fiilen çalışılan takvim günü (en çok 30) − eksik gün.</item>
    /// </list>
    /// Eksik gün = devamsız + ücretsiz izinli gün. Sonuç 0'ın altına inmez.
    /// </summary>
    public static int OtuzGunHesabi(DateOnly iseGiris, DateOnly? istenCikis, int yil, int ay, int eksikGun)
    {
        var ilk = new DateOnly(yil, ay, 1);
        var son = ilk.AddMonths(1).AddDays(-1);
        if (iseGiris > son || istenCikis is { } c0 && c0 < ilk) return 0;

        var bas = iseGiris > ilk ? iseGiris : ilk;
        var bit = istenCikis is { } c && c < son ? c : son;

        var gun = bas == ilk && bit == son
            ? 30                                                  // ayın tamamı: şubat da 31 çeken ay da 30
            : Math.Min(30, bit.DayNumber - bas.DayNumber + 1);    // ay içinde giriş ya da çıkış
        return Math.Max(0, gun - eksikGun);
    }
}

/// <summary>Veritabanındaki plan, izin, tatil ve hareketleri toplayıp puantaj motoruna verir.</summary>
public class PuantajServisi
{
    private readonly TimeProvider _saat;

    public PuantajServisi(TimeProvider saat) => _saat = saat;

    public async Task<AylikPuantaj?> AylikAsync(PdksDbContext db, int personelId, int yil, int ay, CancellationToken ct = default)
    {
        var sonuc = await HesaplaAsync(db, db.Personeller.Where(p => p.Id == personelId), yil, ay, ct);
        return sonuc.Count == 0 ? null : sonuc[0];
    }

    /// <summary>
    /// Ay içinde çalışmış (işe giriş/çıkış tarihi aya değen) bütün personelin puantajı.
    /// İsteğe bağlı bölüm ve çalışma grubu filtresi uygulanır. Sıra: ad, soyad.
    /// </summary>
    public Task<List<AylikPuantaj>> TopluAsync(PdksDbContext db, int yil, int ay, int? bolumId = null, int? calismaGrubuId = null, CancellationToken ct = default)
    {
        var ilk = new DateOnly(yil, ay, 1);
        var son = ilk.AddMonths(1).AddDays(-1);
        var q = db.Personeller.Where(p => p.IseGirisTarihi <= son && (p.IstenCikisTarihi == null || p.IstenCikisTarihi >= ilk));
        if (bolumId is int b) q = q.Where(p => p.BolumId == b);
        if (calismaGrubuId is int g) q = q.Where(p => p.CalismaGrubuId == g);
        return HesaplaAsync(db, q, yil, ay, ct);
    }

    /// <summary>Verilen personeller için ayın verisini toplu okur (personel başına sorgu atmaz) ve hesaplar.</summary>
    private async Task<List<AylikPuantaj>> HesaplaAsync(PdksDbContext db, IQueryable<Personel> personelSorgusu, int yil, int ay, CancellationToken ct)
    {
        var ilk = new DateOnly(yil, ay, 1);
        var son = ilk.AddMonths(1).AddDays(-1);

        var personeller = await personelSorgusu.AsNoTracking()
            .Include(p => p.Bolum).Include(p => p.CalismaGrubu)
            .OrderBy(p => p.Ad).ThenBy(p => p.Soyad).ThenBy(p => p.Id)
            .ToListAsync(ct);
        if (personeller.Count == 0) return new();

        var idSorgusu = personelSorgusu.Select(p => p.Id);

        var vardiyalar = await db.Vardiyalar.AsNoTracking().ToDictionaryAsync(v => v.Id, ct);

        var grupPlanlari = (await db.CalismaGrubuGunleri.AsNoTracking().ToListAsync(ct))
            .GroupBy(x => x.CalismaGrubuId)
            .ToDictionary(x => x.Key, x => (IReadOnlyDictionary<DayOfWeek, int?>)x.ToDictionary(y => y.Gun, y => y.VardiyaId));

        var kisiselPlanlar = (await db.PersonelVardiyalari.AsNoTracking()
                .Where(x => idSorgusu.Contains(x.PersonelId) && x.Tarih >= ilk && x.Tarih <= son)
                .ToListAsync(ct))
            .GroupBy(x => x.PersonelId)
            .ToDictionary(x => x.Key, x => (IReadOnlyDictionary<DateOnly, int?>)x.ToDictionary(y => y.Tarih, y => y.VardiyaId));

        var tatiller = (await db.ResmiTatiller.AsNoTracking()
            .Where(x => x.Tarih >= ilk && x.Tarih <= son).Select(x => x.Tarih).ToListAsync(ct)).ToHashSet();

        var izinler = (await db.Izinler.AsNoTracking().Include(x => x.IzinTuru)
                .Where(x => idSorgusu.Contains(x.PersonelId) && x.Baslangic <= son && x.Bitis >= ilk)
                .ToListAsync(ct))
            .ToLookup(x => x.PersonelId);

        var aramaBas = ilk.AddDays(-1).ToDateTime(TimeOnly.MinValue);
        var aramaBit = son.AddDays(2).ToDateTime(TimeOnly.MinValue);
        var hareketler = (await db.Hareketler.AsNoTracking()
                .Where(x => idSorgusu.Contains(x.PersonelId) && !x.Iptal && x.Zaman >= aramaBas && x.Zaman < aramaBit)
                .Select(x => new { x.PersonelId, x.Zaman })
                .ToListAsync(ct))
            .ToLookup(x => x.PersonelId, x => x.Zaman);

        var bugun = DateOnly.FromDateTime(_saat.GetLocalNow().DateTime);
        var bosPlan = new Dictionary<DayOfWeek, int?>();
        var bosKisisel = new Dictionary<DateOnly, int?>();

        var sonuc = new List<AylikPuantaj>(personeller.Count);
        foreach (var p in personeller)
        {
            var grupPlani = p.CalismaGrubuId is int gid && grupPlanlari.TryGetValue(gid, out var gp) ? gp : bosPlan;
            var kisisel = kisiselPlanlar.TryGetValue(p.Id, out var kp) ? kp : bosKisisel;
            var gunler = Hesapla(p, ilk, son, vardiyalar, grupPlani, kisisel, tatiller, izinler[p.Id].ToList(), hareketler[p.Id], bugun);
            sonuc.Add(new AylikPuantaj(p, yil, ay, gunler, PuantajToplamlari.Hesapla(gunler)));
        }
        return sonuc;
    }

    /// <summary>
    /// Saf hesaplama: her güne vardiyasını bulur, hareketleri günlere dağıtır ve motoru çalıştırır.
    /// Bir hareket yalnızca bir güne yazılır (günler sırayla işlenir).
    /// </summary>
    public static List<GunlukPuantaj> Hesapla(
        Personel personel,
        DateOnly ilk,
        DateOnly son,
        IReadOnlyDictionary<int, Vardiya> vardiyalar,
        IReadOnlyDictionary<DayOfWeek, int?> grupPlani,
        IReadOnlyDictionary<DateOnly, int?> kisiselPlan,
        IReadOnlySet<DateOnly> resmiTatiller,
        IReadOnlyList<Izin> izinler,
        IEnumerable<DateTime> hareketler,
        DateOnly? sonHesapGunu = null)
    {
        var kalan = PuantajMotoru.Temizle(hareketler);
        var sonuc = new List<GunlukPuantaj>();

        VardiyaKurali? KuralBul(DateOnly t)
        {
            int? vardiyaId = kisiselPlan.TryGetValue(t, out var k) ? k
                : grupPlani.TryGetValue(t.DayOfWeek, out var g) ? g
                : null;
            return vardiyaId is int id && vardiyalar.TryGetValue(id, out var v) ? v.Kural() : null;
        }

        // Önceki günden başlanır ki ay başına sarkan gece vardiyası çıkışları 1'ine yazılmasın.
        for (var t = ilk.AddDays(-1); t <= son; t = t.AddDays(1))
        {
            var kural = KuralBul(t);
            var (pBas, pBit) = Pencere(t, kural, KuralBul(t.AddDays(-1)), KuralBul(t.AddDays(1)));
            var gunun = kalan.Where(h => h >= pBas && h < pBit).ToList();
            kalan.RemoveAll(h => h >= pBas && h < pBit);

            var izin = izinler.FirstOrDefault(i => i.Kapsar(t));
            var kapsamDisi = t < personel.IseGirisTarihi
                || (personel.IstenCikisTarihi is { } c && t > c)
                || (sonHesapGunu is { } sg && t > sg)
                // Bugün henüz okutma yoksa gün bitmeden "devamsız" denmez.
                || (sonHesapGunu is { } bg && t == bg && gunun.Count == 0);

            var gunSonucu = PuantajMotoru.Hesapla(new GunGirdisi(
                t, kural, gunun,
                ResmiTatil: resmiTatiller.Contains(t),
                Izinli: izin is not null,
                IzinUcretli: izin?.IzinTuru?.Ucretli ?? true,
                KapsamDisi: kapsamDisi));
            if (t >= ilk) sonuc.Add(gunSonucu);
        }

        return sonuc;
    }

    /// <summary>
    /// Günün hareket penceresi. Komşu günlerde vardiya varsa pencere, iki vardiya arasındaki
    /// boşluğun ortasında kesilir; böylece bir okutma yanlış güne yazılmaz.
    /// </summary>
    public static (DateTime Baslangic, DateTime Bitis) Pencere(DateOnly t, VardiyaKurali? kural, VardiyaKurali? onceki, VardiyaKurali? sonraki)
    {
        var (bas, bit) = PuantajMotoru.HareketPenceresi(t, kural);
        if (kural is null) return (bas, bit);

        var vBas = kural.BaslangicZamani(t);
        var vBit = kural.BitisZamani(t);

        if (onceki is not null)
        {
            var oncekiBit = onceki.BitisZamani(t.AddDays(-1));
            if (oncekiBit < vBas)
                bas = Max(bas, oncekiBit + (vBas - oncekiBit) / 2);
        }
        if (sonraki is not null)
        {
            var sonrakiBas = sonraki.BaslangicZamani(t.AddDays(1));
            if (sonrakiBas > vBit)
                bit = Min(bit, vBit + (sonrakiBas - vBit) / 2);
        }
        return (bas, bit);
    }

    private static DateTime Max(DateTime a, DateTime b) => a > b ? a : b;
    private static DateTime Min(DateTime a, DateTime b) => a < b ? a : b;
}
