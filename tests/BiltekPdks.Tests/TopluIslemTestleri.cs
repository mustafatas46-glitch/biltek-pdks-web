using BiltekPdks.Domain.Entities;
using BiltekPdks.Infrastructure.Puantaj;
using BiltekPdks.Infrastructure.Security;
using BiltekPdks.Infrastructure.Seed;

namespace BiltekPdks.Tests;

public class TopluIslemTestleri
{
    [Fact]
    public async Task Toplu_izin_cakisani_ve_iste_olmayani_atlar_kazanc_kesinti_herkese_yazilir()
    {
        using var tv = new TestVeritabani();
        await new KurulumServisi(tv, new SifreServisi()).CalistirAsync("demo", "Demo", "Gizli1234");
        int firmaId;
        using (var db = tv.Sistem()) firmaId = db.Firmalar.Single().Id;

        int a, b, c, izinTuru;
        using (var d = tv.Db(firmaId))
        {
            var p1 = new Personel { KartNo = "1", Ad = "Ali", Soyad = "A", IseGirisTarihi = new DateOnly(2026, 1, 1) };
            var p2 = new Personel { KartNo = "2", Ad = "Ayşe", Soyad = "B", IseGirisTarihi = new DateOnly(2026, 1, 1) };
            var p3 = new Personel { KartNo = "3", Ad = "Can", Soyad = "C", IseGirisTarihi = new DateOnly(2026, 1, 1), IstenCikisTarihi = new DateOnly(2026, 6, 30) };
            d.Personeller.AddRange(p1, p2, p3);
            d.SaveChanges();
            (a, b, c) = (p1.Id, p2.Id, p3.Id);
            izinTuru = d.IzinTurleri.First().Id;
            d.Izinler.Add(new Izin { PersonelId = b, IzinTuruId = izinTuru, Baslangic = new DateOnly(2026, 10, 20), Bitis = new DateOnly(2026, 10, 22) });
            d.SaveChanges();
        }

        using var db2 = tv.Db(firmaId);
        var s = await TopluIslemServisi.IzinEkleAsync(db2, new[] { a, b, c }, izinTuru, new DateOnly(2026, 10, 21), new DateOnly(2026, 10, 23), "Genel izin");
        Assert.Equal(1, s.Eklenen);
        Assert.Equal(2, s.Atlanan);
        Assert.Equal(2, db2.Izinler.Count());

        await Assert.ThrowsAsync<ArgumentException>(() => TopluIslemServisi.IzinEkleAsync(db2, new[] { a }, izinTuru, new DateOnly(2026, 10, 5), new DateOnly(2026, 10, 1), null));

        var prim = db2.KazancKesintiTurleri.Single(t => t.Kod == "PRIM");
        Assert.Equal(8, db2.KazancKesintiTurleri.Count()); // kurulumda eklenen varsayılan kalemler
        var k = await TopluIslemServisi.KazancKesintiEkleAsync(db2, new[] { a, b }, prim.Id, new DateOnly(2026, 10, 1), 500m, "Ekim primi", 1);
        Assert.Equal(2, k.Eklenen);
        Assert.All(db2.KazancKesintiler.ToList(), x => Assert.Equal(500m, x.Tutar));
        await Assert.ThrowsAsync<ArgumentException>(() => TopluIslemServisi.KazancKesintiEkleAsync(db2, new[] { a }, prim.Id, new DateOnly(2026, 10, 1), 0m, null, 1));
    }
}
