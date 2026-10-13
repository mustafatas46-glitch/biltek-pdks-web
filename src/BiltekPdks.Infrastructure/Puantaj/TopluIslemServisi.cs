using BiltekPdks.Domain.Entities;
using BiltekPdks.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BiltekPdks.Infrastructure.Puantaj;

public sealed record TopluSonuc(int Eklenen, int Atlanan, IReadOnlyList<string> Atlananlar);

/// <summary>Toplu izin ve toplu kazanç-kesinti ekleme.</summary>
public static class TopluIslemServisi
{
    /// <summary>Seçilen personellere aynı izni yazar. Aynı aralıkta izni olan ya da o tarihlerde işte olmayan personel atlanır.</summary>
    public static async Task<TopluSonuc> IzinEkleAsync(PdksDbContext db, IReadOnlyCollection<int> personelIdleri, int izinTuruId,
        DateOnly baslangic, DateOnly bitis, string? aciklama, CancellationToken ct = default)
    {
        if (bitis < baslangic) throw new ArgumentException("Bitiş tarihi başlangıçtan önce olamaz.");
        if (bitis.DayNumber - baslangic.DayNumber > 366) throw new ArgumentException("İzin en çok bir yıl olabilir.");
        if (!await db.IzinTurleri.AnyAsync(t => t.Id == izinTuruId, ct)) throw new ArgumentException("İzin türü seçin.");

        var personeller = await db.Personeller.AsNoTracking().Where(p => personelIdleri.Contains(p.Id))
            .Select(p => new { p.Id, Ad = p.Ad + " " + p.Soyad, p.IseGirisTarihi, p.IstenCikisTarihi }).ToListAsync(ct);
        var cakisanlar = (await db.Izinler.AsNoTracking()
            .Where(i => personelIdleri.Contains(i.PersonelId) && i.Baslangic <= bitis && i.Bitis >= baslangic)
            .Select(i => i.PersonelId).ToListAsync(ct)).ToHashSet();

        var atlanan = new List<string>();
        var eklenen = 0;
        foreach (var p in personeller)
        {
            if (cakisanlar.Contains(p.Id)) { atlanan.Add($"{p.Ad}: bu tarihlerde başka izni var"); continue; }
            if (p.IseGirisTarihi > bitis || p.IstenCikisTarihi is { } c && c < baslangic) { atlanan.Add($"{p.Ad}: bu tarihlerde işte değil"); continue; }
            db.Izinler.Add(new Izin { PersonelId = p.Id, IzinTuruId = izinTuruId, Baslangic = baslangic, Bitis = bitis, Aciklama = Bos(aciklama) });
            eklenen++;
        }
        await db.SaveChangesAsync(ct);
        return new TopluSonuc(eklenen, atlanan.Count, atlanan);
    }

    /// <summary>Seçilen personellere aynı ay için aynı kazanç / kesinti / avans kalemini yazar.</summary>
    public static async Task<TopluSonuc> KazancKesintiEkleAsync(PdksDbContext db, IReadOnlyCollection<int> personelIdleri, int turId,
        DateOnly tarih, decimal tutar, string? aciklama, int? kaydedenKullaniciId, CancellationToken ct = default)
    {
        if (tutar <= 0) throw new ArgumentException("Tutar sıfırdan büyük olmalı.");
        if (!await db.KazancKesintiTurleri.AnyAsync(t => t.Id == turId, ct)) throw new ArgumentException("Kazanç / kesinti türü seçin.");

        var mevcut = await db.Personeller.AsNoTracking().Where(p => personelIdleri.Contains(p.Id)).Select(p => p.Id).ToListAsync(ct);
        foreach (var id in mevcut)
            db.KazancKesintiler.Add(new KazancKesinti
            {
                PersonelId = id, KazancKesintiTuruId = turId, Tarih = tarih, Tutar = tutar,
                Aciklama = Bos(aciklama), KaydedenKullaniciId = kaydedenKullaniciId,
            });
        await db.SaveChangesAsync(ct);
        return new TopluSonuc(mevcut.Count, 0, Array.Empty<string>());
    }

    private static string? Bos(string? s) => string.IsNullOrWhiteSpace(s) ? null : s.Trim();
}
