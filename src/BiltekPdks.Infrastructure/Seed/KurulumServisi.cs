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
            return new KurulumSonucu(false, null, null, null);

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

        return new KurulumSonucu(true, firma.Kod, yonetici.KullaniciAdi, uretilen);
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
