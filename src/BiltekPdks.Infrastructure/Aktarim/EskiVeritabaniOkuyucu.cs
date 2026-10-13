using MySqlConnector;

namespace BiltekPdks.Infrastructure.Aktarim;

/// <summary>
/// Eski MySQL (5.1, latin5) veritabanını SALT OKUNUR şekilde okur. Hiçbir kayıt değiştirilmez.
/// </summary>
public static class EskiVeritabaniOkuyucu
{
    /// <summary>Eski sistemin sıfır tarihlerini ("0000-00-00") boş kabul etmek için gereken ayarları ekler.</summary>
    public static string BaglantiyiHazirla(string baglanti)
    {
        var b = new MySqlConnectionStringBuilder(baglanti)
        {
            ConvertZeroDateTime = true,
            AllowZeroDateTime = false,
            TreatTinyAsBoolean = false,
            DefaultCommandTimeout = 600,
        };
        return b.ConnectionString;
    }

    public static async Task<EskiVeri> OkuAsync(string baglanti, DateTime? hareketBaslangic, int? eskiFirmaId, CancellationToken ct = default)
    {
        var veri = new EskiVeri();
        await using var c = new MySqlConnection(BaglantiyiHazirla(baglanti));
        await c.OpenAsync(ct);

        await OkuAsync(c, "SELECT id, kod, ad FROM sube", r => veri.Subeler.Add(new(I(r, 0), S(r, 1), S(r, 2))), ct);
        await OkuAsync(c, "SELECT id, kod, ad FROM bolum", r => veri.Bolumler.Add(new(I(r, 0), S(r, 1), S(r, 2))), ct);
        await OkuAsync(c, "SELECT id, kod, ad FROM gorev", r => veri.Gorevler.Add(new(I(r, 0), S(r, 1), S(r, 2))), ct);
        await OkuAsync(c, "SELECT id, ad, gunluksaat, donemliksaat FROM grup",
            r => veri.Gruplar.Add(new(I(r, 0), S(r, 1), S(r, 2), S(r, 3))), ct);

        var personelSql = "SELECT id, kartno, ad, soyad, sicilno, firmaid, grupid, bolumid, gorevid, subeid, " +
                          "isegiristarihi, istencikistarihi, istenayrilmanedeni, kimlikno, dogumtarihi, cinsiyet, ceptel, email, adres FROM personel";
        if (eskiFirmaId is not null) personelSql += " WHERE firmaid = @firma";
        await OkuAsync(c, personelSql, r => veri.Personeller.Add(new(
            L(r, 0), S(r, 1), S(r, 2), S(r, 3), S(r, 4), NI(r, 5), NI(r, 6), NI(r, 7), NI(r, 8), NI(r, 9),
            D(r, 10), D(r, 11), S(r, 12), S(r, 13), D(r, 14), S(r, 15), S(r, 16), S(r, 17), S(r, 18))), ct,
            ("@firma", eskiFirmaId));

        await OkuAsync(c, "SELECT id, CAST(kod AS CHAR), ad, aciklama, tip FROM pts_izinturleri",
            r => veri.IzinTurleri.Add(new(I(r, 0), S(r, 1), S(r, 2), S(r, 3), S(r, 4))), ct);

        await OkuAsync(c, "SELECT id, personelid, giristarihi, girissaati, cikistarihi, cikissaati, izinturid, aciklama FROM pts_izin",
            r => veri.Izinler.Add(new(L(r, 0), L(r, 1), D(r, 2), T(r, 3), D(r, 4), T(r, 5), NI(r, 6), S(r, 7))), ct);

        await OkuAsync(c, "SELECT DISTINCT tarih, tatil FROM pts_resmitatiller WHERE tarih IS NOT NULL",
            r => { if (D(r, 0) is { } t) veri.Tatiller.Add(new(t, S(r, 1))); }, ct);

        var gcSql = "SELECT id, personelid, giristarihi, girissaati, ellegiris, giristerminalno, " +
                    "cikistarihi, cikissaati, ellecikis, cikisterminalno FROM pts_giriscikis";
        if (hareketBaslangic is not null) gcSql += " WHERE giristarihi >= @bas OR cikistarihi >= @bas";
        await OkuAsync(c, gcSql, r => veri.GirisCikislar.Add(new(
            L(r, 0), L(r, 1), D(r, 2), T(r, 3), S(r, 4), S(r, 5), D(r, 6), T(r, 7), S(r, 8), S(r, 9))), ct,
            ("@bas", hareketBaslangic?.Date));

        return veri;
    }

    private static async Task OkuAsync(MySqlConnection c, string sql, Action<MySqlDataReader> satir, CancellationToken ct,
        params (string Ad, object? Deger)[] parametreler)
    {
        await using var cmd = new MySqlCommand(sql, c);
        foreach (var (ad, deger) in parametreler)
            if (deger is not null) cmd.Parameters.AddWithValue(ad, deger);
        await using var r = await cmd.ExecuteReaderAsync(ct);
        while (await r.ReadAsync(ct)) satir(r);
    }

    private static string? S(MySqlDataReader r, int i) => r.IsDBNull(i) ? null : Convert.ToString(r.GetValue(i))?.Trim();
    private static int I(MySqlDataReader r, int i) => Convert.ToInt32(r.GetValue(i));
    private static long L(MySqlDataReader r, int i) => r.IsDBNull(i) ? 0 : Convert.ToInt64(r.GetValue(i));
    private static int? NI(MySqlDataReader r, int i) => r.IsDBNull(i) ? null : Convert.ToInt32(r.GetValue(i));

    private static DateTime? D(MySqlDataReader r, int i)
    {
        if (r.IsDBNull(i)) return null;
        var d = r.GetDateTime(i);
        return d == DateTime.MinValue || d.Year < 1900 ? null : d;
    }

    private static TimeSpan? T(MySqlDataReader r, int i) => r.IsDBNull(i) ? null : r.GetTimeSpan(i);
}
