using BiltekPdks.Domain.Entities;
using BiltekPdks.Infrastructure.Data;
using BiltekPdks.Infrastructure.Firmalar;
using Microsoft.EntityFrameworkCore;

namespace BiltekPdks.Infrastructure.Security;

public sealed record GirisSonucu(bool Basarili, Kullanici? Kullanici, string? Hata, string? FirmaKodu = null, IReadOnlyList<string>? Yetkiler = null)
{
    public static GirisSonucu Hatali(string hata) => new(false, null, hata);
}

/// <summary>Kullanıcı adı/şifre doğrulama; 5 hatalı denemede hesabı 15 dakika kilitler.</summary>
public class GirisServisi
{
    public const int AzamiHataliDeneme = 5;
    public static readonly TimeSpan KilitSuresi = TimeSpan.FromMinutes(15);

    private readonly IFirmaVeritabanlari _veritabanlari;
    private readonly SifreServisi _sifre;
    private readonly TimeProvider _saat;

    public GirisServisi(IFirmaVeritabanlari veritabanlari, SifreServisi sifre, TimeProvider saat)
    {
        _veritabanlari = veritabanlari;
        _sifre = sifre;
        _saat = saat;
    }

    public async Task<GirisSonucu> DogrulaAsync(string? firmaKodu, string kullaniciAdi, string sifre, CancellationToken ct = default)
    {
        const string genelHata = "Kullanıcı adı veya şifre hatalı.";
        if (string.IsNullOrWhiteSpace(kullaniciAdi) || string.IsNullOrEmpty(sifre))
            return GirisSonucu.Hatali(genelHata);

        // Firma başına ayrı veritabanında önce firmanın veritabanı bulunur.
        if (_veritabanlari.AyriVeritabani)
        {
            if (string.IsNullOrWhiteSpace(firmaKodu))
            {
                var firmalar = (await _veritabanlari.FirmalarAsync(ct)).Where(f => f.Aktif).Take(2).ToList();
                if (firmalar.Count != 1) return GirisSonucu.Hatali("Firma kodunu girin.");
                firmaKodu = firmalar[0].Kod;
            }
            try { firmaKodu = FirmaKodu.Normallestir(firmaKodu); _veritabanlari.Fabrika(firmaKodu); }
            catch (Exception e) when (e is FirmaBulunamadi or ArgumentException) { return GirisSonucu.Hatali(genelHata); }
        }

        await using var db = await _veritabanlari.Fabrika(firmaKodu).CreateDbContextAsync(ct);
        db.FirmaFiltresiKapali = true;

        Firma? firma;
        if (string.IsNullOrWhiteSpace(firmaKodu) || _veritabanlari.AyriVeritabani)
        {
            // Tek firmalı kurulumda firma kodu sorulmaz; ayrı veritabanında zaten yalnızca o firma vardır.
            var aktifler = await db.Firmalar.Where(f => f.Aktif).Take(2).ToListAsync(ct);
            if (aktifler.Count != 1) return GirisSonucu.Hatali("Firma kodunu girin.");
            firma = aktifler[0];
        }
        else
        {
            var kod = firmaKodu.Trim().ToUpperInvariant();
            firma = await db.Firmalar.FirstOrDefaultAsync(f => f.Kod == kod && f.Aktif, ct);
            if (firma is null) return GirisSonucu.Hatali(genelHata);
        }

        var ad = kullaniciAdi.Trim().ToLowerInvariant();
        var kullanici = await db.Kullanicilar
            .FirstOrDefaultAsync(k => k.FirmaId == firma.Id && k.KullaniciAdi == ad, ct);
        if (kullanici is null || !kullanici.Aktif) return GirisSonucu.Hatali(genelHata);

        var simdi = _saat.GetUtcNow().UtcDateTime;
        if (kullanici.KilitBitis is { } bitis && bitis > simdi)
            return GirisSonucu.Hatali("Çok sayıda hatalı deneme. Hesap geçici olarak kilitlendi.");

        if (!_sifre.Dogrula(kullanici, sifre))
        {
            kullanici.BasarisizGirisSayisi++;
            if (kullanici.BasarisizGirisSayisi >= AzamiHataliDeneme)
            {
                kullanici.KilitBitis = simdi.Add(KilitSuresi);
                kullanici.BasarisizGirisSayisi = 0;
            }
            await db.SaveChangesAsync(ct);
            return GirisSonucu.Hatali(genelHata);
        }

        kullanici.BasarisizGirisSayisi = 0;
        kullanici.KilitBitis = null;
        kullanici.SonGirisTarihi = simdi;
        await db.SaveChangesAsync(ct);
        var yetkiler = await GecerliYetkilerAsync(db, kullanici, ct);
        return new GirisSonucu(true, kullanici, null, _veritabanlari.AyriVeritabani ? firmaKodu : firma.Kod, yetkiler);
    }

    /// <summary>Kullanıcının geçerli yetkileri: yönetici hepsi; özel yetkili kullanıcı kendi listesi; diğerleri rol varsayılanı.</summary>
    public static async Task<IReadOnlyList<string>> GecerliYetkilerAsync(PdksDbContext db, Kullanici k, CancellationToken ct = default)
    {
        if (k.Rol == KullaniciRolu.Yonetici || !k.OzelYetki) return Yetkiler.RolVarsayilani(k.Rol);
        return await db.KullaniciYetkileri.IgnoreQueryFilters()
            .Where(y => y.KullaniciId == k.Id && y.FirmaId == k.FirmaId).Select(y => y.Yetki).ToListAsync(ct);
    }

    /// <summary>Oturumdaki kullanıcının şifresini değiştirir. Başarılıysa null, değilse hata mesajı döner.</summary>
    public async Task<string?> SifreDegistirAsync(int kullaniciId, string mevcutSifre, string yeniSifre, string yeniSifreTekrar, string? firmaKodu = null, CancellationToken ct = default)
    {
        if (yeniSifre != yeniSifreTekrar) return "Yeni şifreler birbiriyle aynı değil.";
        if (SifreServisi.KuralHatasi(yeniSifre) is { } kural) return kural;
        if (yeniSifre == mevcutSifre) return "Yeni şifre eskisiyle aynı olamaz.";

        await using var db = await _veritabanlari.Fabrika(firmaKodu).CreateDbContextAsync(ct);
        db.FirmaFiltresiKapali = true;
        var k = await db.Kullanicilar.FirstOrDefaultAsync(x => x.Id == kullaniciId && x.Aktif, ct);
        if (k is null) return "Kullanıcı bulunamadı.";
        if (!_sifre.Dogrula(k, mevcutSifre)) return "Mevcut şifre hatalı.";

        k.SifreHash = _sifre.Hashle(k, yeniSifre);
        await db.SaveChangesAsync(ct);
        return null;
    }
}
