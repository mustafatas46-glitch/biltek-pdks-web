using BiltekPdks.Domain.Puantaj;

namespace BiltekPdks.Tests;

public class ResmiTatilTakvimiTestleri
{
    [Fact]
    public void Yil_2026_sabit_ve_dini_bayramlari_icerir()
    {
        var t = ResmiTatilTakvimi.Yil(2026);
        Assert.Equal(7 + 3 + 4, t.Count);
        Assert.Contains(t, x => x.Tarih == new DateOnly(2026, 10, 29) && x.Ad == "Cumhuriyet Bayramı");
        Assert.Contains(t, x => x.Tarih == new DateOnly(2026, 7, 15));
        Assert.Contains(t, x => x.Tarih == new DateOnly(2026, 3, 22) && x.Ad.StartsWith("Ramazan"));
        Assert.Contains(t, x => x.Tarih == new DateOnly(2026, 5, 30) && x.Ad.StartsWith("Kurban"));
        Assert.Equal(t.OrderBy(x => x.Tarih), t);
    }

    [Fact]
    public void Dini_bayrami_bilinmeyen_yilda_yalniz_sabit_tatiller_gelir()
    {
        Assert.False(ResmiTatilTakvimi.DiniBayramlarBiliniyor(2031));
        Assert.Equal(7, ResmiTatilTakvimi.Yil(2031).Count);
    }
}
