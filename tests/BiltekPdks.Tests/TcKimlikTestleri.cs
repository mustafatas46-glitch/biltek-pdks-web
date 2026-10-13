using BiltekPdks.Domain.Entities;

namespace BiltekPdks.Tests;

public class TcKimlikTestleri
{
    [Theory]
    [InlineData("10000000146")]
    [InlineData("11111111110")]
    public void Gecerli_numaralar_kabul_edilir(string no) => Assert.True(TcKimlik.GecerliMi(no));

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("1234567890")]
    [InlineData("01234567890")]
    [InlineData("10000000147")]
    [InlineData("1000000014a")]
    public void Gecersiz_numaralar_reddedilir(string? no) => Assert.False(TcKimlik.GecerliMi(no));
}
