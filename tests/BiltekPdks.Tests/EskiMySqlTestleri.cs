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
        { "sube", "bolum", "gorev", "grup", "personel", "pts_izinturleri", "pts_izin", "pts_resmitatiller", "pts_giriscikis" };

    private static string SemaDosyasi()
    {
        var d = new DirectoryInfo(AppContext.BaseDirectory);
        while (d is not null && !File.Exists(Path.Combine(d.FullName, "BiltekPdks.sln"))) d = d.Parent;
        return Path.Combine(d!.FullName, "docs", "eski-sema", "data2015_schema.sql");
    }

    [Fact]
    public async Task Latin5_turkce_karakterler_ve_sifir_tarihler_dogru_okunur()
    {
        if (string.IsNullOrEmpty(Baglanti)) return; // yerelde MySQL yok

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
    }
}
