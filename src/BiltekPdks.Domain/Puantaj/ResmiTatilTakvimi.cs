namespace BiltekPdks.Domain.Puantaj;

/// <summary>Türkiye'deki resmi tatiller. Dini bayramlar ay takvimine göre değiştiği için yalnızca bilinen yıllar listelidir.</summary>
public static class ResmiTatilTakvimi
{
    public sealed record Tatil(DateOnly Tarih, string Ad);

    private static readonly (int Ay, int Gun, string Ad)[] Sabit =
    {
        (1, 1, "Yılbaşı"),
        (4, 23, "Ulusal Egemenlik ve Çocuk Bayramı"),
        (5, 1, "Emek ve Dayanışma Günü"),
        (5, 19, "Atatürk'ü Anma, Gençlik ve Spor Bayramı"),
        (7, 15, "Demokrasi ve Millî Birlik Günü"),
        (8, 30, "Zafer Bayramı"),
        (10, 29, "Cumhuriyet Bayramı"),
    };

    // Diyanet takvimine göre bayram günleri (arife yarım günleri hariç).
    private static readonly Dictionary<int, (DateOnly Ramazan, DateOnly Kurban)> DiniBayramlar = new()
    {
        [2025] = (new DateOnly(2025, 3, 30), new DateOnly(2025, 6, 6)),
        [2026] = (new DateOnly(2026, 3, 20), new DateOnly(2026, 5, 27)),
    };

    public static bool DiniBayramlarBiliniyor(int yil) => DiniBayramlar.ContainsKey(yil);

    public static List<Tatil> Yil(int yil)
    {
        var liste = Sabit.Select(s => new Tatil(new DateOnly(yil, s.Ay, s.Gun), s.Ad)).ToList();
        if (DiniBayramlar.TryGetValue(yil, out var d))
        {
            for (var i = 0; i < 3; i++) liste.Add(new Tatil(d.Ramazan.AddDays(i), $"Ramazan Bayramı {i + 1}. gün"));
            for (var i = 0; i < 4; i++) liste.Add(new Tatil(d.Kurban.AddDays(i), $"Kurban Bayramı {i + 1}. gün"));
        }
        return liste.OrderBy(t => t.Tarih).ToList();
    }
}
