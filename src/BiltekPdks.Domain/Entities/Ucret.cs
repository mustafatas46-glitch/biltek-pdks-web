namespace BiltekPdks.Domain.Entities;

public enum UcretTipi { Maas = 1, SaatUcreti = 2 }

public enum KazancKesintiTipi { Kazanc = 1, Kesinti = 2, Avans = 3 }

/// <summary>Prim, yol, yemek, ikramiye, avans, kesinti gibi kalemlerin türü.</summary>
public class KazancKesintiTuru : TanimEntity
{
    public KazancKesintiTipi Tip { get; set; } = KazancKesintiTipi.Kazanc;
}

/// <summary>Bir personele belirli bir ay için girilen kazanç / kesinti / avans. Ay, <see cref="Tarih"/>'ten alınır.</summary>
public class KazancKesinti : FirmaEntity
{
    public int PersonelId { get; set; }
    public Personel? Personel { get; set; }
    public int KazancKesintiTuruId { get; set; }
    public KazancKesintiTuru? KazancKesintiTuru { get; set; }
    public DateOnly Tarih { get; set; }
    public decimal Tutar { get; set; }
    public string? Aciklama { get; set; }
    public int? KaydedenKullaniciId { get; set; }
    /// <summary>Excel aktarımıyla geldiyse aktarımın kaydı (kaynak izi).</summary>
    public int? AktarimId { get; set; }
}

/// <summary>Kim, ne zaman, hangi kaydı değiştirdi (KVKK ve denetim için).</summary>
public class IslemKaydi : FirmaEntity
{
    public DateTime Zaman { get; set; }
    public int? KullaniciId { get; set; }
    public string Tablo { get; set; } = string.Empty;
    public int KayitId { get; set; }
    public string Islem { get; set; } = string.Empty;
    public string? Degisiklikler { get; set; }
}
