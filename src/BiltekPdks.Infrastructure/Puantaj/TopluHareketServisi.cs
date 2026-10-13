using BiltekPdks.Domain.Entities;
using BiltekPdks.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BiltekPdks.Infrastructure.Puantaj;

/// <summary>Bir personele tarih aralığı için aynı saatlerde giriş-çıkış ekleme isteği.</summary>
public sealed record TopluHareketIstegi(
    int PersonelId,
    DateOnly Baslangic,
    DateOnly Bitis,
    TimeOnly Giris,
    TimeOnly Cikis,
    string Aciklama,
    bool YalnizCalismaGunleri = true,
    bool ResmiTatilleriAtla = true,
    bool HareketiOlanGunleriAtla = true,
    bool IzinliGunleriAtla = true);

public sealed record TopluHareketSonucu(int EklenenGun, int HareketiOlan, int CalismaGunuDegil, int ResmiTatil, int Izinli, int KapsamDisi)
{
    public int Atlanan => HareketiOlan + CalismaGunuDegil + ResmiTatil + Izinli + KapsamDisi;
}

/// <summary>
/// Toplu giriş-çıkış: seçilen aralıktaki her gün için bir giriş ve bir çıkış hareketi (kaynak: elle) yazar.
/// Çıkış saati girişten önce ya da eşitse çıkış ertesi güne yazılır (gece vardiyası).
/// </summary>
public static class TopluHareketServisi
{
    public const int EnFazlaGun = 366;

    public static async Task<TopluHareketSonucu> EkleAsync(PdksDbContext db, TopluHareketIstegi istek, int? kaydedenKullaniciId, bool deneme = false, CancellationToken ct = default)
    {
        if (istek.Bitis < istek.Baslangic) throw new ArgumentException("Bitiş tarihi başlangıçtan önce olamaz.");
        if (istek.Bitis.DayNumber - istek.Baslangic.DayNumber + 1 > EnFazlaGun) throw new ArgumentException($"Bir seferde en çok {EnFazlaGun} gün eklenebilir.");
        if (string.IsNullOrWhiteSpace(istek.Aciklama)) throw new ArgumentException("Açıklama yazın.");
        if (istek.Giris == istek.Cikis) throw new ArgumentException("Giriş ve çıkış saati aynı olamaz.");

        var p = await db.Personeller.AsNoTracking().FirstOrDefaultAsync(x => x.Id == istek.PersonelId, ct)
            ?? throw new ArgumentException("Personel bulunamadı.");

        var grupPlani = p.CalismaGrubuId is int gid
            ? await db.CalismaGrubuGunleri.AsNoTracking().Where(x => x.CalismaGrubuId == gid).ToDictionaryAsync(x => x.Gun, x => x.VardiyaId, ct)
            : new Dictionary<DayOfWeek, int?>();
        var kisisel = await db.PersonelVardiyalari.AsNoTracking()
            .Where(x => x.PersonelId == p.Id && x.Tarih >= istek.Baslangic && x.Tarih <= istek.Bitis)
            .ToDictionaryAsync(x => x.Tarih, x => x.VardiyaId, ct);
        var tatiller = (await db.ResmiTatiller.AsNoTracking()
            .Where(x => x.Tarih >= istek.Baslangic && x.Tarih <= istek.Bitis).Select(x => x.Tarih).ToListAsync(ct)).ToHashSet();
        var izinler = await db.Izinler.AsNoTracking()
            .Where(x => x.PersonelId == p.Id && x.Baslangic <= istek.Bitis && x.Bitis >= istek.Baslangic).ToListAsync(ct);

        var aramaBas = istek.Baslangic.ToDateTime(TimeOnly.MinValue);
        var aramaBit = istek.Bitis.AddDays(2).ToDateTime(TimeOnly.MinValue);
        var mevcut = await db.Hareketler.AsNoTracking()
            .Where(x => x.PersonelId == p.Id && !x.Iptal && x.Zaman >= aramaBas && x.Zaman < aramaBit)
            .Select(x => x.Zaman).ToListAsync(ct);

        int eklenen = 0, dolu = 0, plansiz = 0, tatil = 0, izinli = 0, kapsam = 0;
        var aciklama = istek.Aciklama.Trim();
        for (var t = istek.Baslangic; t <= istek.Bitis; t = t.AddDays(1))
        {
            if (t < p.IseGirisTarihi || p.IstenCikisTarihi is { } c && t > c) { kapsam++; continue; }
            if (istek.ResmiTatilleriAtla && tatiller.Contains(t)) { tatil++; continue; }
            if (istek.IzinliGunleriAtla && izinler.Any(i => i.Kapsar(t))) { izinli++; continue; }
            if (istek.YalnizCalismaGunleri)
            {
                int? vardiya = kisisel.TryGetValue(t, out var k) ? k : grupPlani.TryGetValue(t.DayOfWeek, out var g) ? g : null;
                if (vardiya is null) { plansiz++; continue; }
            }

            var giris = t.ToDateTime(istek.Giris);
            var cikis = (istek.Cikis <= istek.Giris ? t.AddDays(1) : t).ToDateTime(istek.Cikis);
            if (istek.HareketiOlanGunleriAtla && mevcut.Any(z => DateOnly.FromDateTime(z) == t || (z >= giris && z <= cikis)))
            { dolu++; continue; }

            if (!deneme)
            {
                db.Hareketler.Add(new Hareket { PersonelId = p.Id, Zaman = giris, Kaynak = HareketKaynagi.Elle, Aciklama = aciklama, KaydedenKullaniciId = kaydedenKullaniciId });
                db.Hareketler.Add(new Hareket { PersonelId = p.Id, Zaman = cikis, Kaynak = HareketKaynagi.Elle, Aciklama = aciklama, KaydedenKullaniciId = kaydedenKullaniciId });
            }
            mevcut.Add(giris);
            mevcut.Add(cikis);
            eklenen++;
        }
        if (!deneme) await db.SaveChangesAsync(ct);
        return new TopluHareketSonucu(eklenen, dolu, plansiz, tatil, izinli, kapsam);
    }
}
