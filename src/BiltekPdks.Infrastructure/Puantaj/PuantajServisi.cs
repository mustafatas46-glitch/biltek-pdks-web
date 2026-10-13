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
    int TatilMesaisiDakika)
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
        g.Sum(x => x.TatilMesaisiDakika));
}

public sealed record AylikPuantaj(Personel Personel, int Yil, int Ay, IReadOnlyList<GunlukPuantaj> Gunler, PuantajToplamlari Toplam);

/// <summary>Veritabanındaki plan, izin, tatil ve hareketleri toplayıp puantaj motoruna verir.</summary>
public class PuantajServisi
{
    private readonly TimeProvider _saat;

    public PuantajServisi(TimeProvider saat) => _saat = saat;

    public async Task<AylikPuantaj?> AylikAsync(PdksDbContext db, int personelId, int yil, int ay, CancellationToken ct = default)
    {
        var personel = await db.Personeller.AsNoTracking().FirstOrDefaultAsync(p => p.Id == personelId, ct);
        if (personel is null) return null;

        var ilk = new DateOnly(yil, ay, 1);
        var son = ilk.AddMonths(1).AddDays(-1);

        var vardiyalar = await db.Vardiyalar.AsNoTracking().ToDictionaryAsync(v => v.Id, ct);

        var grupPlani = personel.CalismaGrubuId is int grupId
            ? await db.CalismaGrubuGunleri.AsNoTracking().Where(x => x.CalismaGrubuId == grupId)
                .ToDictionaryAsync(x => x.Gun, x => x.VardiyaId, ct)
            : new Dictionary<DayOfWeek, int?>();

        var kisisel = await db.PersonelVardiyalari.AsNoTracking()
            .Where(x => x.PersonelId == personelId && x.Tarih >= ilk && x.Tarih <= son)
            .ToDictionaryAsync(x => x.Tarih, x => x.VardiyaId, ct);

        var tatiller = (await db.ResmiTatiller.AsNoTracking()
            .Where(x => x.Tarih >= ilk && x.Tarih <= son).Select(x => x.Tarih).ToListAsync(ct)).ToHashSet();

        var izinler = await db.Izinler.AsNoTracking().Include(x => x.IzinTuru)
            .Where(x => x.PersonelId == personelId && x.Baslangic <= son && x.Bitis >= ilk)
            .ToListAsync(ct);

        var aramaBas = ilk.AddDays(-1).ToDateTime(TimeOnly.MinValue);
        var aramaBit = son.AddDays(2).ToDateTime(TimeOnly.MinValue);
        var hareketler = await db.Hareketler.AsNoTracking()
            .Where(x => x.PersonelId == personelId && !x.Iptal && x.Zaman >= aramaBas && x.Zaman < aramaBit)
            .Select(x => x.Zaman)
            .ToListAsync(ct);

        var bugun = DateOnly.FromDateTime(_saat.GetLocalNow().DateTime);
        var gunler = Hesapla(personel, ilk, son, vardiyalar, grupPlani, kisisel, tatiller, izinler, hareketler, bugun);
        return new AylikPuantaj(personel, yil, ay, gunler, PuantajToplamlari.Hesapla(gunler));
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
                || (sonHesapGunu is { } sg && t > sg);

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
