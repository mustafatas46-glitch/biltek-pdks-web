using System.Security.Cryptography;
using BiltekPdks.Domain.Entities;
using BiltekPdks.Infrastructure.Data;
using BiltekPdks.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;

namespace BiltekPdks.Infrastructure.Seed;

public sealed record KurulumSonucu(bool IlkKurulum, string? FirmaKodu, string? YoneticiKullaniciAdi, string? UretilenSifre);

/// <summary>Veritabanı boşsa ilk firmayı ve yönetici hesabını oluşturur. Tekrar çalıştırmak güvenlidir.</summary>
public class KurulumServisi
{
    private readonly IDbContextFactory<PdksDbContext> _dbFactory;
    private readonly SifreServisi _sifre;

    public KurulumServisi(IDbContextFactory<PdksDbContext> dbFactory, SifreServisi sifre)
    {
        _dbFactory = dbFactory;
        _sifre = sifre;
    }

    public async Task<KurulumSonucu> CalistirAsync(string firmaKodu, string firmaAdi, string? yoneticiSifresi, CancellationToken ct = default)
    {
        await using var db = await _dbFactory.CreateDbContextAsync(ct);
        db.FirmaFiltresiKapali = true;

        if (await db.Firmalar.AnyAsync(ct))
        {
            // Sonradan eklenen tanım türleri eski kurulumlarda da bulunsun.
            foreach (var id in await db.Firmalar.Select(f => f.Id).ToListAsync(ct))
                await KazancKesintiTurleriniEkleAsync(db, id, ct);
            return new KurulumSonucu(false, null, null, null);
        }

        var firma = new Firma { Kod = firmaKodu.Trim().ToUpperInvariant(), Ad = firmaAdi.Trim() };
        db.Firmalar.Add(firma);
        await db.SaveChangesAsync(ct);

        string? uretilen = null;
        if (SifreServisi.KuralHatasi(yoneticiSifresi) is not null)
            yoneticiSifresi = uretilen = RastgeleSifre();

        var yonetici = new Kullanici
        {
            FirmaId = firma.Id,
            KullaniciAdi = "admin",
            AdSoyad = "Sistem Yöneticisi",
            Rol = KullaniciRolu.Yonetici,
        };
        yonetici.SifreHash = _sifre.Hashle(yonetici, yoneticiSifresi!);
        db.Kullanicilar.Add(yonetici);
        await db.SaveChangesAsync(ct);

        await VarsayilanTanimlariEkleAsync(db, firma.Id, ct);
        await KazancKesintiTurleriniEkleAsync(db, firma.Id, ct);

        return new KurulumSonucu(true, firma.Kod, yonetici.KullaniciAdi, uretilen);
    }

    /// <summary>Yeni firmaya başlangıç vardiyası, haftalık plan ve izin türleri ekler.</summary>
    private static async Task VarsayilanTanimlariEkleAsync(PdksDbContext db, int firmaId, CancellationToken ct)
    {
        var normal = new Vardiya
        {
            FirmaId = firmaId, Kod = "N", Ad = "Normal mesai 08:00-17:00",
            Baslangic = new TimeOnly(8, 0), Bitis = new TimeOnly(17, 0), MolaDakika = 60,
            GecKalmaToleransDakika = 5, ErkenCikmaToleransDakika = 0, FazlaMesaiEsikDakika = 30,
        };
        var grup = new CalismaGrubu { FirmaId = firmaId, Ad = "Genel", GunlukCalismaDakika = 480, DonemlikCalismaDakika = 2700 };
        db.AddRange(normal, grup);
        await db.SaveChangesAsync(ct);

        foreach (var gun in Enum.GetValues<DayOfWeek>())
        {
            var haftaici = gun is not (DayOfWeek.Saturday or DayOfWeek.Sunday);
            db.CalismaGrubuGunleri.Add(new CalismaGrubuGunu
            {
                FirmaId = firmaId, CalismaGrubuId = grup.Id, Gun = gun, VardiyaId = haftaici ? normal.Id : null,
            });
        }

        db.IzinTurleri.AddRange(
            new IzinTuru { FirmaId = firmaId, Kod = "YI", Ad = "Yıllık izin", Ucretli = true, YillikIzindenDuser = true },
            new IzinTuru { FirmaId = firmaId, Kod = "MI", Ad = "Mazeret izni", Ucretli = true },
            new IzinTuru { FirmaId = firmaId, Kod = "RP", Ad = "Rapor", Ucretli = false },
            new IzinTuru { FirmaId = firmaId, Kod = "UI", Ad = "Ücretsiz izin", Ucretli = false });
        await db.SaveChangesAsync(ct);
    }

    /// <summary>Firmada hiç kazanç/kesinti türü yoksa eski programdaki temel kalemleri ekler.</summary>
    private static async Task KazancKesintiTurleriniEkleAsync(PdksDbContext db, int firmaId, CancellationToken ct)
    {
        if (await db.KazancKesintiTurleri.AnyAsync(t => t.FirmaId == firmaId, ct)) return;
        (string Kod, string Ad, KazancKesintiTipi Tip)[] turler =
        {
            ("PRIM", "Prim", KazancKesintiTipi.Kazanc), ("YOL", "Yol ücreti", KazancKesintiTipi.Kazanc),
            ("YEMEK", "Yemek ücreti", KazancKesintiTipi.Kazanc), ("IKR", "İkramiye", KazancKesintiTipi.Kazanc),
            ("OZEL", "Özel ücret", KazancKesintiTipi.Kazanc), ("AVANS", "Avans", KazancKesintiTipi.Avans),
            ("KES", "Kesinti", KazancKesintiTipi.Kesinti), ("ICRA", "İcra kesintisi", KazancKesintiTipi.Kesinti),
        };
        foreach (var (kod, ad, tip) in turler)
            db.KazancKesintiTurleri.Add(new KazancKesintiTuru { FirmaId = firmaId, Kod = kod, Ad = ad, Tip = tip });
        await db.SaveChangesAsync(ct);
    }

    private static string RastgeleSifre()
    {
        const string harfler = "abcdefghjkmnpqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ";
        const string rakamlar = "23456789";
        var parcalar = new char[12];
        for (var i = 0; i < parcalar.Length; i++)
        {
            var kaynak = i % 3 == 2 ? rakamlar : harfler;
            parcalar[i] = kaynak[RandomNumberGenerator.GetInt32(kaynak.Length)];
        }
        return new string(parcalar);
    }
}
