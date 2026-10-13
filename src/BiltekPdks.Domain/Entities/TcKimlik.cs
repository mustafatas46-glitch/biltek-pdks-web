namespace BiltekPdks.Domain.Entities;

public static class TcKimlik
{
    /// <summary>T.C. kimlik numarasının algoritmik doğruluğunu kontrol eder.</summary>
    public static bool GecerliMi(string? no)
    {
        if (no is null || no.Length != 11 || no[0] == '0' || !no.All(char.IsAsciiDigit))
            return false;

        var d = no.Select(c => c - '0').ToArray();
        var tek = d[0] + d[2] + d[4] + d[6] + d[8];
        var cift = d[1] + d[3] + d[5] + d[7];
        var onuncu = ((tek * 7 - cift) % 10 + 10) % 10;
        if (onuncu != d[9]) return false;

        var onbirinci = d.Take(10).Sum() % 10;
        return onbirinci == d[10];
    }
}
