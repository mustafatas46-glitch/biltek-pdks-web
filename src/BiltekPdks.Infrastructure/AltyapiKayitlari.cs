using BiltekPdks.Infrastructure.Data;
using BiltekPdks.Infrastructure.Firmalar;
using BiltekPdks.Infrastructure.Security;
using BiltekPdks.Infrastructure.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BiltekPdks.Infrastructure;

public static class AltyapiKayitlari
{
    /// <summary>
    /// Veritabanı ve altyapı servislerini kaydeder.
    /// appsettings: "Veritabani:Saglayici" = "MySql" (varsayılan) veya "Sqlite";
    /// "ConnectionStrings:Pdks" bağlantı cümlesi.
    /// </summary>
    public static IServiceCollection AddPdksAltyapi(this IServiceCollection services, IConfiguration config)
    {
        var saglayici = config["Veritabani:Saglayici"] ?? "MySql";
        var sqlite = VeritabaniAyarlari.SqliteMi(saglayici);
        var surum = ServerVersion.Parse(config["Veritabani:SunucuSurumu"] ?? "8.0.36-mysql");

        if (string.Equals(config["Veritabani:Mod"], "FirmaBasina", StringComparison.OrdinalIgnoreCase))
        {
            // Her firmanın ayrı veritabanı: katalog + firma bağlantı şablonu.
            var katalog = config.GetConnectionString("Katalog") ?? throw new InvalidOperationException("ConnectionStrings:Katalog tanımlı değil.");
            var sablon = config.GetConnectionString("FirmaSablonu") ?? throw new InvalidOperationException("ConnectionStrings:FirmaSablonu tanımlı değil.");
            var ko = new DbContextOptionsBuilder<KatalogDbContext>();
            if (sqlite)
            {
                var dosya = new Microsoft.Data.Sqlite.SqliteConnectionStringBuilder(katalog).DataSource;
                var klasor = Path.GetDirectoryName(Path.GetFullPath(dosya));
                if (!string.IsNullOrEmpty(klasor)) Directory.CreateDirectory(klasor);
                ko.UseSqlite(katalog);
            }
            else ko.UseMySql(katalog, surum, m => m.EnableRetryOnFailure());
            var secenek = ko.Options;
            services.AddSingleton<IFirmaVeritabanlari>(_ => new FirmaBasinaVeritabani(sqlite, sablon, () => new KatalogDbContext(secenek), surum));
            OrtakServisler(services);
            return services;
        }

        var baglanti = config.GetConnectionString("Pdks")
            ?? throw new InvalidOperationException("ConnectionStrings:Pdks tanımlı değil.");

        if (sqlite)
        {
            services.AddDbContextFactory<SqlitePdksDbContext>(o => VeritabaniAyarlari.SqliteAyarla(o, baglanti));
            services.AddSingleton<IDbContextFactory<PdksDbContext>>(sp =>
                new FabrikaAdaptoru<SqlitePdksDbContext>(sp.GetRequiredService<IDbContextFactory<SqlitePdksDbContext>>()));
        }
        else
        {
            services.AddDbContextFactory<MySqlPdksDbContext>(o => VeritabaniAyarlari.MySqlAyarla(o, baglanti, surum));
            services.AddSingleton<IDbContextFactory<PdksDbContext>>(sp =>
                new FabrikaAdaptoru<MySqlPdksDbContext>(sp.GetRequiredService<IDbContextFactory<MySqlPdksDbContext>>()));
        }

        services.AddSingleton<IFirmaVeritabanlari>(sp => new TekVeritabani(sp.GetRequiredService<IDbContextFactory<PdksDbContext>>()));
        services.AddScoped<KurulumServisi>();
        services.AddScoped<DemoVeriServisi>();
        OrtakServisler(services);
        return services;
    }

    private static void OrtakServisler(IServiceCollection services)
    {
        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<SifreServisi>();
        services.AddScoped<GirisServisi>();
        services.AddSingleton<Puantaj.PuantajServisi>();
    }
}
