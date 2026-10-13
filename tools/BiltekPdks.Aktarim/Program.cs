using System.Diagnostics;
using BiltekPdks.Infrastructure.Aktarim;
using BiltekPdks.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

// Eski Biltek PDKS (MySQL 5.1) verisini yeni sisteme aktarır. Eski veritabanı yalnızca OKUNUR.
//
// Örnek:
//   pdks-aktarim --eski "Server=localhost;Port=3307;Database=data2015;User=root;Password=root"
//                --yeni "Data Source=C:\pdks\pdks.db" --saglayici Sqlite --firma FIRMA
//                [--hareket-baslangic 2025-01-01] [--eski-firma-id 1] [--deneme]

var a = Argumanlar.Oku(args);
if (a is null)
{
    Console.WriteLine("""
        Kullanım:
          pdks-aktarim --eski <eski MySQL bağlantısı> --yeni <yeni veritabanı bağlantısı> --firma <firma kodu>
                       [--saglayici MySql|Sqlite] [--hareket-baslangic yyyy-aa-gg] [--eski-firma-id N] [--deneme]

          --deneme   Hiçbir şey kaydetmeden ne aktarılacağını raporlar.
        """);
    return 2;
}

var sure = Stopwatch.StartNew();
Console.WriteLine("Eski veritabanı okunuyor (salt okunur)...");
var eski = await EskiVeritabaniOkuyucu.OkuAsync(a.Eski, a.HareketBaslangic, a.EskiFirmaId);
Console.WriteLine($"  personel {eski.Personeller.Count}, izin {eski.Izinler.Count}, giriş-çıkış satırı {eski.GirisCikislar.Count}, tatil {eski.Tatiller.Count}");

PdksDbContext YeniBaglam() =>
    VeritabaniAyarlari.SqliteMi(a.Saglayici)
        ? new SqlitePdksDbContext(new DbContextOptionsBuilder<SqlitePdksDbContext>().UseSqlite(a.Yeni).Options)
        : new MySqlPdksDbContext(new DbContextOptionsBuilder<MySqlPdksDbContext>().UseMySql(a.Yeni, VeritabaniAyarlari.VarsayilanMySqlSurumu).Options);

await using (var sistem = YeniBaglam())
{
    sistem.FirmaFiltresiKapali = true;
    await VeritabaniGuncelleyici.GuncelleAsync(sistem);
    var kod = a.Firma.Trim().ToUpperInvariant();
    var firma = await sistem.Firmalar.FirstOrDefaultAsync(f => f.Kod == kod);
    if (firma is null)
    {
        Console.Error.WriteLine($"HATA: '{kod}' kodlu firma yeni sistemde yok. Önce web uygulamasını bir kez çalıştırın.");
        return 1;
    }

    await using var db = YeniBaglam();
    db.AktifFirmaId = firma.Id;
    await using var islem = await db.Database.BeginTransactionAsync();
    Console.WriteLine($"'{firma.Ad}' firmasına aktarılıyor...");
    var rapor = await EskiSistemAktarimi.AktarAsync(db, firma.Id, eski);

    if (a.Deneme)
    {
        await islem.RollbackAsync();
        Console.WriteLine("DENEME MODU: hiçbir şey kaydedilmedi.");
    }
    else
    {
        await islem.CommitAsync();
    }

    Console.WriteLine(rapor.ToString());
    var dosya = $"aktarim-raporu-{DateTime.Now:yyyyMMdd-HHmmss}.txt";
    await File.WriteAllTextAsync(dosya, rapor.ToString());
    Console.WriteLine($"Rapor: {Path.GetFullPath(dosya)}  ({sure.Elapsed.TotalSeconds:0} sn)");
}
return 0;

internal sealed record Argumanlar(string Eski, string Yeni, string Firma, string Saglayici, DateTime? HareketBaslangic, int? EskiFirmaId, bool Deneme)
{
    public static Argumanlar? Oku(string[] args)
    {
        string? Al(string ad)
        {
            var i = Array.IndexOf(args, ad);
            return i >= 0 && i + 1 < args.Length ? args[i + 1] : null;
        }

        var eski = Al("--eski");
        var yeni = Al("--yeni");
        var firma = Al("--firma");
        if (eski is null || yeni is null || firma is null) return null;

        DateTime? bas = DateTime.TryParse(Al("--hareket-baslangic"), out var b) ? b : null;
        int? eskiFirma = int.TryParse(Al("--eski-firma-id"), out var f) ? f : null;
        return new Argumanlar(eski, yeni, firma, Al("--saglayici") ?? "MySql", bas, eskiFirma, args.Contains("--deneme"));
    }
}
