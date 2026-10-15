using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using BiltekPdks.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BiltekPdks.Infrastructure.Firmalar;

/// <summary>
/// Firma verisinin hangi veritabanında durduğunu bilen katman.
/// <list type="bullet">
/// <item><b>Tek</b>: tüm firmalar tek veritabanında (masaüstü kurulumu, eski davranış).</item>
/// <item><b>FirmaBasina</b>: her firmanın ayrı veritabanı vardır; hangi firmanın nerede olduğu katalogda tutulur.</item>
/// </list>
/// Her iki modda da firma etiketi (FirmaId) filtresi ikinci güvenlik katmanı olarak çalışmaya devam eder.
/// </summary>
public interface IFirmaVeritabanlari
{
    bool AyriVeritabani { get; }

    /// <summary>Firmanın veritabanı için bağlam fabrikası. Firma yoksa <see cref="FirmaBulunamadi"/> fırlatır.</summary>
    IDbContextFactory<PdksDbContext> Fabrika(string? firmaKodu);

    /// <summary>Katalogdaki aktif firmalar (Tek modda boş liste).</summary>
    Task<IReadOnlyList<KatalogFirma>> FirmalarAsync(CancellationToken ct = default);

    /// <summary>Yeni firma veritabanını oluşturur ve kataloğa ekler (yalnız FirmaBasina modunda).</summary>
    Task<KatalogFirma> FirmaEkleAsync(string kod, string ad, CancellationToken ct = default);

    /// <summary>
    /// Var olan bir SQLite veritabanı dosyasıyla (ör. firmanın masaüstü sürümündeki pdks.db) firma açar.
    /// Dosya firmanın klasörüne kopyalanır ve son sürüme güncellenir.
    /// </summary>
    Task<KatalogFirma> FirmaEkleAsync(string kod, string ad, Stream mevcutVeritabani, CancellationToken ct = default);

    /// <summary>Firmayı aktif / pasif yapar. Pasif firmada kimse giriş yapamaz; veri silinmez.</summary>
    Task AktiflikAsync(string kod, bool aktif, CancellationToken ct = default);
}

public sealed class FirmaBulunamadi : Exception
{
    public FirmaBulunamadi(string? kod) : base($"Firma bulunamadı: {kod}") { }
}

public static class FirmaKodu
{
    private static readonly Regex Gecerli = new("^[A-Z0-9_]{2,20}$", RegexOptions.Compiled);

    /// <summary>Firma kodunu büyük harfe çevirir ve yalnızca A-Z, 0-9, _ içerdiğini doğrular (veritabanı adına girer).</summary>
    public static string Normallestir(string? kod)
    {
        var k = (kod ?? "").Trim()
            .Replace('ı', 'I').Replace('i', 'I').Replace('ş', 'S').Replace('ğ', 'G').Replace('ü', 'U').Replace('ö', 'O').Replace('ç', 'C')
            .ToUpperInvariant()
            .Replace('İ', 'I').Replace('Ş', 'S').Replace('Ğ', 'G').Replace('Ü', 'U').Replace('Ö', 'O').Replace('Ç', 'C');
        if (!Gecerli.IsMatch(k)) throw new ArgumentException("Firma kodu 2-20 karakter olmalı; yalnızca harf, rakam ve _ içerebilir.");
        return k;
    }
}

/// <summary>Tek veritabanı modu: her firma aynı fabrikayı kullanır.</summary>
public sealed class TekVeritabani : IFirmaVeritabanlari
{
    private readonly IDbContextFactory<PdksDbContext> _fabrika;
    public TekVeritabani(IDbContextFactory<PdksDbContext> fabrika) => _fabrika = fabrika;
    public bool AyriVeritabani => false;
    public IDbContextFactory<PdksDbContext> Fabrika(string? firmaKodu) => _fabrika;
    public Task<IReadOnlyList<KatalogFirma>> FirmalarAsync(CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<KatalogFirma>>(Array.Empty<KatalogFirma>());
    public Task<KatalogFirma> FirmaEkleAsync(string kod, string ad, CancellationToken ct = default) =>
        throw new InvalidOperationException("Tek veritabanı kurulumunda ayrı firma veritabanı oluşturulmaz.");
    public Task<KatalogFirma> FirmaEkleAsync(string kod, string ad, Stream mevcutVeritabani, CancellationToken ct = default) =>
        throw new InvalidOperationException("Tek veritabanı kurulumunda ayrı firma veritabanı oluşturulmaz.");
    public Task AktiflikAsync(string kod, bool aktif, CancellationToken ct = default) =>
        throw new InvalidOperationException("Tek veritabanı kurulumunda firma kataloğu yoktur.");
}

/// <summary>Firma başına ayrı veritabanı modu.</summary>
public sealed class FirmaBasinaVeritabani : IFirmaVeritabanlari
{
    private readonly bool _sqlite;
    private readonly string _sablon;
    private readonly Func<KatalogDbContext> _katalog;
    private readonly ServerVersion? _surum;
    private readonly ConcurrentDictionary<string, IDbContextFactory<PdksDbContext>> _fabrikalar = new();
    private readonly ConcurrentDictionary<string, bool> _bilinen = new();

    /// <param name="sablon">Firma bağlantı şablonu; <c>{kod}</c> küçük harfli firma koduyla değişir.
    /// Örn. SQLite: <c>Data Source=veri/{kod}/pdks.db</c> · MySQL: <c>Server=..;Database=pdks_{kod};User=..;Password=..</c></param>
    public FirmaBasinaVeritabani(bool sqlite, string sablon, Func<KatalogDbContext> katalog, ServerVersion? surum = null)
    {
        if (!sablon.Contains("{kod}")) throw new InvalidOperationException("ConnectionStrings:FirmaSablonu içinde {kod} olmalı.");
        _sqlite = sqlite;
        _sablon = sablon;
        _katalog = katalog;
        _surum = surum;
        using var k = _katalog();
        k.Database.EnsureCreated();
    }

    public bool AyriVeritabani => true;

    public string BaglantiMetni(string kod) => _sablon.Replace("{kod}", kod.ToLowerInvariant());

    public IDbContextFactory<PdksDbContext> Fabrika(string? firmaKodu)
    {
        var kod = FirmaKodu.Normallestir(firmaKodu);
        if (!_bilinen.ContainsKey(kod))
        {
            using var k = _katalog();
            if (!k.Firmalar.AsNoTracking().Any(f => f.Kod == kod && f.Aktif)) throw new FirmaBulunamadi(kod);
            _bilinen[kod] = true;
        }
        return _fabrikalar.GetOrAdd(kod, Olustur);
    }

    private IDbContextFactory<PdksDbContext> Olustur(string kod)
    {
        var baglanti = BaglantiMetni(kod);
        if (_sqlite)
        {
            var dosya = new Microsoft.Data.Sqlite.SqliteConnectionStringBuilder(baglanti).DataSource;
            var klasor = Path.GetDirectoryName(Path.GetFullPath(dosya));
            if (!string.IsNullOrEmpty(klasor)) Directory.CreateDirectory(klasor);
            var o = new DbContextOptionsBuilder<SqlitePdksDbContext>();
            VeritabaniAyarlari.SqliteAyarla(o, baglanti);
            return new SecenekFabrikasi(() => new SqlitePdksDbContext(o.Options));
        }
        var m = new DbContextOptionsBuilder<MySqlPdksDbContext>();
        VeritabaniAyarlari.MySqlAyarla(m, baglanti, _surum);
        return new SecenekFabrikasi(() => new MySqlPdksDbContext(m.Options));
    }

    public async Task<IReadOnlyList<KatalogFirma>> FirmalarAsync(CancellationToken ct = default)
    {
        await using var k = _katalog();
        return await k.Firmalar.AsNoTracking().OrderBy(f => f.Kod).ToListAsync(ct);
    }

    public Task<KatalogFirma> FirmaEkleAsync(string kod, string ad, CancellationToken ct = default) => EkleAsync(kod, ad, null, ct);

    public Task<KatalogFirma> FirmaEkleAsync(string kod, string ad, Stream mevcutVeritabani, CancellationToken ct = default) =>
        EkleAsync(kod, ad, mevcutVeritabani, ct);

    public async Task AktiflikAsync(string kod, bool aktif, CancellationToken ct = default)
    {
        kod = FirmaKodu.Normallestir(kod);
        await using var k = _katalog();
        var f = await k.Firmalar.FirstOrDefaultAsync(x => x.Kod == kod, ct) ?? throw new FirmaBulunamadi(kod);
        f.Aktif = aktif;
        await k.SaveChangesAsync(ct);
        if (aktif) _bilinen[kod] = true;
        else { _bilinen.TryRemove(kod, out _); _fabrikalar.TryRemove(kod, out _); }
    }

    private async Task<KatalogFirma> EkleAsync(string kod, string ad, Stream? mevcut, CancellationToken ct)
    {
        kod = FirmaKodu.Normallestir(kod);
        if (string.IsNullOrWhiteSpace(ad)) throw new ArgumentException("Firma adı boş olamaz.");
        await using var k = _katalog();
        if (await k.Firmalar.AnyAsync(f => f.Kod == kod, ct)) throw new ArgumentException($"{kod} kodlu firma zaten var.");

        if (mevcut is not null)
        {
            if (!_sqlite) throw new InvalidOperationException("Var olan veritabanı dosyası yalnızca SQLite kurulumunda yüklenebilir.");
            var dosya = Path.GetFullPath(new Microsoft.Data.Sqlite.SqliteConnectionStringBuilder(BaglantiMetni(kod)).DataSource);
            if (File.Exists(dosya)) throw new ArgumentException($"{kod} için veritabanı dosyası zaten var: {dosya}");
            Directory.CreateDirectory(Path.GetDirectoryName(dosya)!);
            var baslik = new byte[16];
            await using (var hedef = File.Create(dosya))
            {
                await mevcut.CopyToAsync(hedef, ct);
            }
            await using (var oku = File.OpenRead(dosya))
                _ = await oku.ReadAsync(baslik, ct);
            if (System.Text.Encoding.ASCII.GetString(baslik, 0, 15) != "SQLite format 3")
            {
                File.Delete(dosya);
                throw new ArgumentException("Yüklenen dosya bir SQLite veritabanı (pdks.db) değil.");
            }
        }

        // Önce veritabanı kurulur; hata olursa katalogda yarım kayıt kalmaz.
        await using (var db = Olustur(kod).CreateDbContext())
            await VeritabaniGuncelleyici.GuncelleAsync(db, ct);

        var f = new KatalogFirma { Kod = kod, Ad = ad.Trim(), Aktif = true, OlusturmaZamani = DateTime.UtcNow };
        k.Firmalar.Add(f);
        await k.SaveChangesAsync(ct);
        _bilinen[kod] = true;
        return f;
    }

    private sealed class SecenekFabrikasi : IDbContextFactory<PdksDbContext>
    {
        private readonly Func<PdksDbContext> _yeni;
        public SecenekFabrikasi(Func<PdksDbContext> yeni) => _yeni = yeni;
        public PdksDbContext CreateDbContext() => _yeni();
    }
}
