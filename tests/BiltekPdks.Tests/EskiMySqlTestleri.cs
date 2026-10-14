using System.Text.RegularExpressions;
using BiltekPdks.Infrastructure.Aktarim;
using MySqlConnector;

namespace BiltekPdks.Tests;

/// <summary>
/// Gerçek eski MySQL şemasıyla (latin5) okuma testi. CI'da MySQL 5.5 servisiyle çalışır;
/// ESKI_MYSQL_TEST ortam değişkeni yoksa (yerel geliştirme) atlanır.
/// </summary>
public class EskiMySqlTestleri
{
    private static readonly string? Baglanti = Environment.GetEnvironmentVariable("ESKI_MYSQL_TEST");
    private static readonly string[] Tablolar =
        { "sube", "bolum", "gorev", "grup", "personel", "pts_izinturleri", "pts_izin", "pts_resmitatiller", "pts_giriscikis",
          "pts_puantajparametre", "pts_calismaparametre", "pts_molaparametre", "pts_altgrup", "pts_gunparametre", "pts_donemparametre" };

    private static string SemaDosyasi()
    {
        var d = new DirectoryInfo(AppContext.BaseDirectory);
        while (d is not null && !File.Exists(Path.Combine(d.FullName, "BiltekPdks.sln"))) d = d.Parent;
        return Path.Combine(d!.FullName, "docs", "eski-sema", "data2015_schema.sql");
    }

    [Fact]
    public async Task Latin5_turkce_karakterler_ve_sifir_tarihler_dogru_okunur()
    {
        if (string.IsNullOrEmpty(Baglanti))
        {
            // Yerelde MySQL yoksa atlanır; ama CI'da bu testin atlanmasına izin verilmez.
            Assert.Null(Environment.GetEnvironmentVariable("GITHUB_ACTIONS"));
            return;
        }

        var sema = await File.ReadAllTextAsync(SemaDosyasi());
        await using (var c = new MySqlConnection(Baglanti))
        {
            await c.OpenAsync();
            async Task Calistir(string sql) { await using var cmd = new MySqlCommand(sql, c); await cmd.ExecuteNonQueryAsync(); }

            await Calistir("SET SESSION sql_mode = ''");
            foreach (var t in Tablolar)
            {
                var m = Regex.Match(sema, $@"CREATE TABLE `{t}` \(.*?\n\)[^;]*;", RegexOptions.Singleline);
                Assert.True(m.Success, $"{t} şemada bulunamadı");
                await Calistir($"DROP TABLE IF EXISTS `{t}`");
                await Calistir(m.Value);
            }

            await Calistir("INSERT INTO bolum (id, kod, ad) VALUES (1, 'U', 'Üretim Şubesi İğne')");
            await Calistir("INSERT INTO personel (id, kartno, ad, soyad, bolumid, isegiristarihi, istencikistarihi, cinsiyet) " +
                           "VALUES (7, '000123', 'Çağrı', 'Öztürk', 1, '2019-02-01', '0000-00-00', 'Bay')");
            await Calistir("INSERT INTO pts_izinturleri (id, kod, ad, aciklama, tip) VALUES (1, 1, 'YI', 'Yıllık İzin', 'ucretli')");
            await Calistir("INSERT INTO pts_resmitatiller (tarih, tatil) VALUES ('2026-10-29', 'Cumhuriyet Bayramı'), ('2026-10-29', 'Cumhuriyet Bayramı')");
            await Calistir("INSERT INTO pts_giriscikis (personelid, giristarihi, girissaati, cikistarihi, cikissaati, ellegiris) " +
                           "VALUES (7, '2026-10-01', '07:58:00', '2026-10-01', '17:03:00', 'E'), (7, '2025-01-01', '08:00:00', NULL, NULL, NULL)");

            await Calistir("INSERT INTO pts_puantajparametre (id, ad, saat) VALUES (1, 'Sabah Çalışması', '07:30')");
            await Calistir("INSERT INTO pts_calismaparametre (puantajid, bordroid, bordroad, igs, dcs) VALUES (1, 1, 'Normal Çalışma', '08:00', '16:00')");
            await Calistir("INSERT INTO pts_molaparametre (puantajid, ad, baslamasaati, bitissaati, tip) VALUES (1, 'Öğle', '12:00', '12:30', 'ucretsiz')");
            await Calistir("INSERT INTO pts_altgrup (id, grupid, ad, gunluksaat) VALUES (3, 5, '08-16 Şiş', '07:30')");
            await Calistir("INSERT INTO pts_gunparametre (grupid, altgrupid, g1, g2, g3, g4, g5) VALUES (5, 3, 1, 1, 1, 1, 1)");
            await Calistir("INSERT INTO pts_donemparametre (grupid, altgrupid, bastarih, bittarih, girissaati, eksiksaat) VALUES (5, 3, '2026-02-01', '2026-02-28', '08:00:00', '15:00')");

            // Tablolar gerçekten latin5 mi?
            await using var kontrol = new MySqlCommand("SELECT TABLE_COLLATION FROM information_schema.TABLES WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'personel'", c);
            Assert.StartsWith("latin5", (string)(await kontrol.ExecuteScalarAsync())!);
        }

        var veri = await EskiVeritabaniOkuyucu.OkuAsync(Baglanti, hareketBaslangic: new DateTime(2026, 1, 1), eskiFirmaId: null);

        var p = Assert.Single(veri.Personeller);
        Assert.Equal("Çağrı", p.Ad);
        Assert.Equal("Öztürk", p.Soyad);
        Assert.Equal(new DateTime(2019, 2, 1), p.IseGirisTarihi);
        Assert.Null(p.IstenCikisTarihi); // 0000-00-00 boş sayılır
        Assert.Equal("Üretim Şubesi İğne", Assert.Single(veri.Bolumler).Ad);
        Assert.Equal("Yıllık İzin", Assert.Single(veri.IzinTurleri).Aciklama);
        Assert.Single(veri.Tatiller); // DISTINCT
        var gc = Assert.Single(veri.GirisCikislar); // 2025 kaydı tarih filtresiyle elendi
        Assert.Equal(new TimeSpan(7, 58, 0), gc.GirisSaati);
        Assert.Equal("E", gc.ElleGiris);

        Assert.Equal("Sabah Çalışması", Assert.Single(veri.PuantajTanimlari).Ad);
        Assert.Equal("16:00", Assert.Single(veri.CalismaParametreleri).Dcs);
        Assert.Equal("Öğle", Assert.Single(veri.Molalar).Ad);
        Assert.Equal("08-16 Şiş", Assert.Single(veri.AltGruplar).Ad);
        var gp = Assert.Single(veri.GunParametreleri);
        Assert.Equal((3, (int?)1, (int?)null), (gp.AltGrupId, gp.Gunler[0], gp.Gunler[6]));
        var donem = Assert.Single(veri.Donemler);
        Assert.Equal(((DateTime?)new DateTime(2026, 2, 28), (TimeSpan?)new TimeSpan(8, 0, 0), "15:00"), (donem.Bitis, donem.GirisSaati, donem.EksikSaat));
    }
}
