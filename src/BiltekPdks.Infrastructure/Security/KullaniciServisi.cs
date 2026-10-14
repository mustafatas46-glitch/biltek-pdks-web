using BiltekPdks.Domain.Entities;
using BiltekPdks.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BiltekPdks.Infrastructure.Security;

public sealed class KullaniciBilgisi
{
    public int Id { get; set; }
    public string KullaniciAdi { get; set; } = "";
    public string AdSoyad { get; set; } = "";
    public KullaniciRolu Rol { get; set; } = KullaniciRolu.InsanKaynaklari;
    public bool Aktif { get; set; } = true;
    public bool OzelYetki { get; set; }
    public int? PersonelId { get; set; }
    public HashSet<string> Yetkiler { get; set; } = new();
    /// <summary>Doluysa şifre bu değere ayarlanır (yeni kullanıcıda zorunlu).</summary>
    public string? YeniSifre { get; set; }
    public DateTime? SonGirisTarihi { get; set; }
    public bool Kilitli { get; set; }
}

/// <summary>Kullanıcı tanımlama: ekleme, değiştirme, şifre belirleme, yetki verme. Son yönetici korunur.</summary>
public static class KullaniciServisi
{
    public static async Task<List<KullaniciBilgisi>> ListeleAsync(PdksDbContext db, CancellationToken ct = default)
    {
        var liste = await db.Kullanicilar.AsNoTracking().OrderBy(k => k.KullaniciAdi).ToListAsync(ct);
        var yetkiler = (await db.KullaniciYetkileri.AsNoTracking().ToListAsync(ct)).ToLookup(y => y.KullaniciId, y => y.Yetki);
        var simdi = DateTime.UtcNow;
        return liste.Select(k => new KullaniciBilgisi
        {
            Id = k.Id, KullaniciAdi = k.KullaniciAdi, AdSoyad = k.AdSoyad, Rol = k.Rol, Aktif = k.Aktif, OzelYetki = k.OzelYetki,
            PersonelId = k.PersonelId, SonGirisTarihi = k.SonGirisTarihi, Kilitli = k.KilitBitis is { } kb && kb > simdi,
            Yetkiler = (k.OzelYetki ? yetkiler[k.Id] : Domain.Entities.Yetkiler.RolVarsayilani(k.Rol)).ToHashSet(),
        }).ToList();
    }

    /// <summary>Kaydeder; hata varsa açıklamasını döner (null = başarılı).</summary>
    public static async Task<string?> KaydetAsync(PdksDbContext db, KullaniciBilgisi b, SifreServisi sifre, int? islemYapanId, CancellationToken ct = default)
    {
        var ad = (b.KullaniciAdi ?? "").Trim().ToLowerInvariant();
        if (ad.Length is < 3 or > 50 || !ad.All(c => char.IsAsciiLetterOrDigit(c) || c is '.' or '_' or '-'))
            return "Kullanıcı adı 3-50 karakter olmalı; harf, rakam, nokta, alt çizgi ve tire içerebilir (Türkçe karakter olmadan).";
        if (string.IsNullOrWhiteSpace(b.AdSoyad)) return "Ad soyad boş olamaz.";
        if (await db.Kullanicilar.AnyAsync(k => k.KullaniciAdi == ad && k.Id != b.Id, ct)) return "Bu kullanıcı adı kullanılıyor.";
        if (b.Id == 0 && string.IsNullOrEmpty(b.YeniSifre)) return "Yeni kullanıcı için şifre girin.";
        if (!string.IsNullOrEmpty(b.YeniSifre) && SifreServisi.KuralHatasi(b.YeniSifre) is { } kural) return kural;
        var gecersiz = b.Yetkiler.Where(y => Yetkiler.Hepsi.All(t => t.Kod != y)).ToList();
        if (gecersiz.Count > 0) return "Bilinmeyen yetki: " + string.Join(", ", gecersiz);

        Kullanici k;
        if (b.Id == 0)
        {
            k = new Kullanici();
            db.Kullanicilar.Add(k);
        }
        else
        {
            k = await db.Kullanicilar.FirstOrDefaultAsync(x => x.Id == b.Id, ct) ?? throw new ArgumentException("Kullanıcı bulunamadı.");
            // Firmada en az bir aktif yönetici kalmalı; kişi kendi yöneticiliğini de kaldıramaz.
            var yoneticilikKalkiyor = k.Rol == KullaniciRolu.Yonetici && k.Aktif && (b.Rol != KullaniciRolu.Yonetici || !b.Aktif);
            if (yoneticilikKalkiyor)
            {
                if (k.Id == islemYapanId) return "Kendi yöneticiliğinizi kaldıramaz, kendinizi pasif yapamazsınız.";
                if (!await db.Kullanicilar.AnyAsync(x => x.Id != k.Id && x.Rol == KullaniciRolu.Yonetici && x.Aktif, ct))
                    return "Firmada en az bir aktif yönetici kalmalı.";
            }
        }

        k.KullaniciAdi = ad;
        k.AdSoyad = b.AdSoyad.Trim();
        k.Rol = b.Rol;
        k.Aktif = b.Aktif;
        k.PersonelId = b.PersonelId;
        k.OzelYetki = b.Rol != KullaniciRolu.Yonetici && b.OzelYetki;
        if (!string.IsNullOrEmpty(b.YeniSifre))
        {
            k.SifreHash = sifre.Hashle(k, b.YeniSifre);
            k.BasarisizGirisSayisi = 0;
            k.KilitBitis = null;
        }
        await db.SaveChangesAsync(ct);

        var mevcut = await db.KullaniciYetkileri.Where(y => y.KullaniciId == k.Id).ToListAsync(ct);
        db.KullaniciYetkileri.RemoveRange(mevcut);
        if (k.OzelYetki)
            db.KullaniciYetkileri.AddRange(b.Yetkiler.Distinct().Select(y => new KullaniciYetkisi { KullaniciId = k.Id, Yetki = y }));
        await db.SaveChangesAsync(ct);
        b.Id = k.Id;
        b.YeniSifre = null;
        return null;
    }

    public static async Task KilidiAcAsync(PdksDbContext db, int id, CancellationToken ct = default)
    {
        var k = await db.Kullanicilar.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (k is null) return;
        k.KilitBitis = null;
        k.BasarisizGirisSayisi = 0;
        await db.SaveChangesAsync(ct);
    }
}
