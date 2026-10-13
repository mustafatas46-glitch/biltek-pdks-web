using BiltekPdks.Domain.Entities;
using BiltekPdks.Web.Components.Pages.Personeller;

namespace BiltekPdks.Tests;

public class SicilKartiTestleri
{
    [Fact]
    public void Form_tum_sekmelerdeki_alanlari_personele_aktarir_ama_kimlik_alanlarina_dokunmaz()
    {
        var mevcut = new Personel { Id = 7, FirmaId = 3, EskiId = 99, KartNo = "1", Ad = "Ali", Soyad = "Kaya", IseGirisTarihi = new DateOnly(2024, 1, 1) };
        var form = PersonelFormu.Kaynak(mevcut);
        form.Ek = new Personel
        {
            Id = 1234, FirmaId = 555, EskiId = 1,
            MezunOkul = "  Anadolu Lisesi ", Kilo = 80, CiltNo = "12", EhliyetSinifi = "B", CihazProtez = "  ",
            PrimUcreti = 1500m, PrimGunluk = true, WcKesintisiUygula = true, UcretTipi = UcretTipi.SaatUcreti, Ucret = 250m, ServisId = 4,
        };
        form.Ad = "Veli";

        var hedef = new Personel { Id = 7, FirmaId = 3, EskiId = 99 };
        form.Aktar(hedef);

        Assert.Equal((7, 3, 99), (hedef.Id, hedef.FirmaId, hedef.EskiId));
        Assert.Equal("Veli", hedef.Ad);
        Assert.Equal("Anadolu Lisesi", hedef.MezunOkul);
        Assert.Null(hedef.CihazProtez);
        Assert.Equal(80, hedef.Kilo);
        Assert.Equal("12", hedef.CiltNo);
        Assert.Equal(1500m, hedef.PrimUcreti);
        Assert.True(hedef.PrimGunluk);
        Assert.True(hedef.WcKesintisiUygula);
        Assert.Equal(UcretTipi.SaatUcreti, hedef.UcretTipi);
        Assert.Equal(4, hedef.ServisId);
    }
}
