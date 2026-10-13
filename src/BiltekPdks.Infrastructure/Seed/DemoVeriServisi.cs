using BiltekPdks.Domain.Entities;
using BiltekPdks.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BiltekPdks.Infrastructure.Seed;

/// <summary>
/// Yalnızca tanıtım/deneme için hayali personel ve hareket üretir ("Kurulum:DemoVeri" = true).
/// Firmada personel varsa hiçbir şey yapmaz.
/// </summary>
public class DemoVeriServisi
{
    private readonly IDbContextFactory<PdksDbContext> _factory;
    private readonly TimeProvider _saat;

    public DemoVeriServisi(IDbContextFactory<PdksDbContext> factory, TimeProvider saat)
    {
        _factory = factory;
        _saat = saat;
    }

    public async Task<bool> OlusturAsync(CancellationToken ct = default)
    {
        await using var sistem = await _factory.CreateDbContextAsync(ct);
        sistem.FirmaFiltresiKapali = true;
        var firma = await sistem.Firmalar.OrderBy(f => f.Id).FirstOrDefaultAsync(ct);
        if (firma is null) return false;

        await using var db = await _factory.CreateDbContextAsync(ct);
        db.AktifFirmaId = firma.Id;
        if (await db.Personeller.AnyAsync(ct)) return false;

        var grup = await db.CalismaGruplari.OrderBy(g => g.Id).FirstAsync(ct);
        var sube = new Sube { Kod = "M", Ad = "Merkez" };
        var uretim = new Bolum { Kod = "U", Ad = "Üretim" };
        var depo = new Bolum { Kod = "D", Ad = "Depo" };
        var idari = new Bolum { Kod = "I", Ad = "İdari İşler" };
        var operator_ = new Gorev { Ad = "Operatör" };
        var sef = new Gorev { Ad = "Vardiya Şefi" };
        var uzman = new Gorev { Ad = "Uzman" };
        db.AddRange(sube, uretim, depo, idari, operator_, sef, uzman);
        await db.SaveChangesAsync(ct);

        var kisiler = new (string Ad, string Soyad, Bolum B, Gorev G)[]
        {
            ("Ahmet", "Yıldırım", uretim, sef), ("Elif", "Şahin", idari, uzman), ("Mehmet", "Kaya", uretim, operator_),
            ("Zeynep", "Çelik", depo, operator_), ("Mustafa", "Aydın", uretim, operator_), ("Ayşe", "Öztürk", idari, uzman),
            ("Emre", "Doğan", depo, sef), ("Selin", "Arslan", uretim, operator_),
        };
        var personeller = new List<Personel>();
        for (var i = 0; i < kisiler.Length; i++)
        {
            var k = kisiler[i];
            var p = new Personel
            {
                KartNo = $"{1001 + i:D6}", SicilNo = $"S{101 + i}", Ad = k.Ad, Soyad = k.Soyad,
                IseGirisTarihi = new DateOnly(2022, 1, 1).AddDays(i * 97),
                SubeId = sube.Id, BolumId = k.B.Id, GorevId = k.G.Id, CalismaGrubuId = grup.Id,
                Cinsiyet = i % 2 == 0 ? Cinsiyet.Erkek : Cinsiyet.Kadin,
            };
            personeller.Add(p);
        }
        db.Personeller.AddRange(personeller);
        await db.SaveChangesAsync(ct);

        var bugun = DateOnly.FromDateTime(_saat.GetLocalNow().DateTime);
        var ayBasi = new DateOnly(bugun.Year, bugun.Month, 1);
        var rnd = new Random(42);

        var yillik = await db.IzinTurleri.FirstAsync(t => t.Kod == "YI", ct);
        db.Izinler.Add(new Izin { PersonelId = personeller[5].Id, IzinTuruId = yillik.Id, Baslangic = ayBasi.AddDays(5), Bitis = ayBasi.AddDays(7), Aciklama = "Yıllık izin" });

        for (var t = ayBasi; t < bugun; t = t.AddDays(1))
        {
            if (t.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday) continue;
            for (var i = 0; i < personeller.Count; i++)
            {
                var p = personeller[i];
                if (i == 5 && t >= ayBasi.AddDays(5) && t <= ayBasi.AddDays(7)) continue; // izinli
                var zar = rnd.Next(100);
                if (zar < 3) continue; // devamsız
                var giris = t.ToDateTime(new TimeOnly(7, 45)).AddMinutes(rnd.Next(0, 20));
                if (zar < 12) giris = giris.AddMinutes(25 + rnd.Next(30)); // geç kalan
                var cikis = t.ToDateTime(new TimeOnly(17, 0)).AddMinutes(rnd.Next(0, 15));
                if (zar > 85) cikis = cikis.AddMinutes(60 + rnd.Next(90)); // fazla mesai
                db.Hareketler.Add(new Hareket { PersonelId = p.Id, Zaman = giris, Kaynak = HareketKaynagi.Terminal, TerminalKodu = "01" });
                if (zar != 50) // nadiren çıkış okutmayı unutur
                    db.Hareketler.Add(new Hareket { PersonelId = p.Id, Zaman = cikis, Kaynak = HareketKaynagi.Terminal, TerminalKodu = "02" });
            }
        }
        await db.SaveChangesAsync(ct);
        return true;
    }
}
