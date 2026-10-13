using BiltekPdks.Infrastructure.Data;
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
        var baglanti = config.GetConnectionString("Pdks")
            ?? throw new InvalidOperationException("ConnectionStrings:Pdks tanımlı değil.");

        if (VeritabaniAyarlari.SqliteMi(saglayici))
        {
            services.AddDbContextFactory<SqlitePdksDbContext>(o => VeritabaniAyarlari.SqliteAyarla(o, baglanti));
            services.AddSingleton<IDbContextFactory<PdksDbContext>>(sp =>
                new FabrikaAdaptoru<SqlitePdksDbContext>(sp.GetRequiredService<IDbContextFactory<SqlitePdksDbContext>>()));
        }
        else
        {
            var surum = ServerVersion.Parse(config["Veritabani:SunucuSurumu"] ?? "8.0.36-mysql");
            services.AddDbContextFactory<MySqlPdksDbContext>(o => VeritabaniAyarlari.MySqlAyarla(o, baglanti, surum));
            services.AddSingleton<IDbContextFactory<PdksDbContext>>(sp =>
                new FabrikaAdaptoru<MySqlPdksDbContext>(sp.GetRequiredService<IDbContextFactory<MySqlPdksDbContext>>()));
        }

        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<SifreServisi>();
        services.AddScoped<GirisServisi>();
        services.AddScoped<KurulumServisi>();
        services.AddScoped<DemoVeriServisi>();
        services.AddSingleton<Puantaj.PuantajServisi>();
        return services;
    }
}
