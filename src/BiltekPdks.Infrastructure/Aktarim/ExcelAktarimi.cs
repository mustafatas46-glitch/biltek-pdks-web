using System.Text.Json;
using BiltekPdks.Domain.Entities;
using BiltekPdks.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BiltekPdks.Infrastructure.Aktarim;

/// <summary>İçeri alınacak bir alan (ör. "Kart No"). <see cref="EsAdlar"/> Excel başlığıyla otomatik eşleştirmede kullanılır.</summary>
public sealed record AlanTanimi(string Anahtar, string Etiket, bool Zorunlu = false, string? Ipucu = null, params string[] EsAdlar);

public enum SatirDurumu { Eklenecek, Guncellenecek, Atlanacak, Hatali }

public sealed record SatirSonucu(int SatirNo, SatirDurumu Durum, string Ozet, string? Mesaj = null);

/// <summary>Satırdan bağımsız seçenekler (dosyada dönem sütunu yoksa ekrandan seçilen ay gibi).</summary>
public sealed record AktarimSecenekleri(DateOnly? Donem = null);

public sealed class AktarimSonucu
{
    public List<SatirSonucu> Satirlar { get; } = new();
    public int Eklenen { get; set; }
    public int Guncellenen { get; set; }
    public int Atlanan { get; set; }
    public int Hatali { get; set; }
    public int? AktarimId { get; set; }

    internal void Ekle(int satirNo, SatirDurumu d, string ozet, string? mesaj = null)
    {
        Satirlar.Add(new SatirSonucu(satirNo, d, ozet, mesaj));
        switch (d)
        {
            case SatirDurumu.Eklenecek: Eklenen++; break;
            case SatirDurumu.Guncellenecek: Guncellenen++; break;
            case SatirDurumu.Atlanacak: Atlanan++; break;
            default: Hatali++; break;
        }
    }
}

/// <summary>
/// Müşteriden gelen Excel dosyalarını (personel, ücret, izin, kazanç-kesinti, hareket, bordro) içeri alır.
/// <list type="bullet">
/// <item>Önce önizleme yapılır (<c>uygula: false</c>): hiçbir şey yazılmaz, her satırın ne olacağı gösterilir.</item>
/// <item>Aktarılan her kayıt <see cref="AktarimKaydi"/>'na bağlanır (kaynak izi); hareketlerin kaynağı "Aktarım" olarak işaretlenir.</item>
/// <item>Hareket, izin, kazanç-kesinti ve bordro aktarımı bütünüyle geri alınabilir.</item>
/// </list>
/// </summary>
public static class ExcelAktarimi
{
    private const int AraKayitSiniri = 5000;

    // ---- Alan tanımları ----

    private static readonly AlanTanimi[] PersonelAnahtarlari =
    {
        new("KartNo", "Kart No", false, "Personel bu alanlardan biriyle bulunur", "KART", "KARTNUMARASI", "KARTNO", "PDKSNO"),
        new("SicilNo", "Sicil No", false, null, "SICIL", "SICILNUMARASI", "PERSONELNO", "PERSONELKODU"),
        new("TcKimlikNo", "T.C. Kimlik No", false, null, "TC", "TCNO", "TCKIMLIK", "TCKIMLIKNO", "KIMLIKNO", "TCKN"),
        new("AdSoyad", "Ad Soyad (tek sütun)", false, "Yalnızca tek eşleşme varsa kullanılır", "ADISOYADI", "ADSOYAD", "PERSONEL", "PERSONELADI", "ISIM"),
    };

    public static IReadOnlyList<AlanTanimi> Alanlar(AktarimTuru tur) => tur switch
    {
        AktarimTuru.Personel => new AlanTanimi[]
        {
            new("KartNo", "Kart No", true, "Personel bu numarayla bulunur; yoksa yeni kayıt açılır", "KART", "KARTNUMARASI", "KARTNO", "PDKSNO"),
            new("SicilNo", "Sicil No", false, null, "SICIL", "SICILNUMARASI", "PERSONELNO", "PERSONELKODU"),
            new("Ad", "Ad", false, "Yeni kayıt için gerekli", "ADI", "AD", "ISIM"),
            new("Soyad", "Soyad", false, "Yeni kayıt için gerekli", "SOYADI", "SOYAD", "SOYISIM"),
            new("AdSoyad", "Ad Soyad (tek sütun)", false, "Ad ve soyad ayrı sütunda değilse", "ADISOYADI", "ADSOYAD", "PERSONELADI"),
            new("TcKimlikNo", "T.C. Kimlik No", false, null, "TC", "TCNO", "TCKIMLIK", "TCKIMLIKNO", "KIMLIKNO", "TCKN"),
            new("Cinsiyet", "Cinsiyet", false, "E / K", "CINSIYET", "CINSIYETI"),
            new("DogumTarihi", "Doğum Tarihi", false, null, "DOGUMTARIHI", "DOGUMTAR"),
            new("IseGirisTarihi", "İşe Giriş Tarihi", false, "Yeni kayıt için gerekli", "ISEGIRIS", "ISEGIRISTARIHI", "GIRISTARIHI", "ISEBASLAMA", "ISEBASLAMATARIHI"),
            new("IstenCikisTarihi", "İşten Çıkış Tarihi", false, null, "ISTENCIKIS", "ISTENCIKISTARIHI", "CIKISTARIHI", "AYRILISTARIHI"),
            new("Sube", "Şube", false, "Yoksa tanımı açılır", "SUBE", "SUBESI", "ISYERI"),
            new("Bolum", "Bölüm", false, "Yoksa tanımı açılır", "BOLUM", "BOLUMU", "DEPARTMAN"),
            new("Gorev", "Görev", false, "Yoksa tanımı açılır", "GOREV", "GOREVI", "UNVAN", "UNVANI", "POZISYON"),
            new("CalismaGrubu", "Çalışma Grubu", false, "Tanımlı olmalı; boşsa ilk grup", "GRUP", "CALISMAGRUBU", "GRUBU"),
            new("CepTelefonu", "Cep Telefonu", false, null, "CEP", "CEPTELEFONU", "TELEFON", "GSM"),
            new("Eposta", "E-posta", false, null, "EPOSTA", "EMAIL", "MAIL"),
            new("Adres", "Adres", false, null, "ADRES", "ADRESI"),
            new("Ucret", "Ücret", false, null, "UCRET", "MAAS", "UCRETI", "BRUTUCRET", "NETUCRET"),
            new("Iban", "IBAN", false, null, "IBAN", "IBANNO"),
            new("SgkSicilNo", "SGK Sicil No", false, null, "SGKNO", "SGKSICIL", "SGKSICILNO", "SSKNO"),
            new("BabaAdi", "Baba Adı", false, null, "BABAADI"),
            new("AnneAdi", "Anne Adı", false, null, "ANNEADI"),
            new("DogumYeri", "Doğum Yeri", false, null, "DOGUMYERI"),
            new("KanGrubu", "Kan Grubu", false, null, "KANGRUBU"),
        },
        AktarimTuru.Ucret => PersonelAnahtarlari.Concat(new AlanTanimi[]
        {
            new("Ucret", "Ücret", true, null, "UCRET", "MAAS", "UCRETI", "YENIUCRET", "BRUTUCRET", "NETUCRET"),
            new("UcretTipi", "Ücret Tipi", false, "Maaş / Saat", "UCRETTIPI", "UCRETTURU"),
            new("MesaiSaatUcreti", "Mesai Saat Ücreti", false, null, "MESAISAATUCRETI", "SAATUCRETI"),
        }).ToArray(),
        AktarimTuru.Izin => PersonelAnahtarlari.Concat(new AlanTanimi[]
        {
            new("IzinTuru", "İzin Türü", true, "Tanımlı izin türü adı ya da kodu", "IZINTURU", "IZINTIPI", "IZINCINSI", "TUR"),
            new("Baslangic", "Başlangıç", true, null, "BASLANGIC", "BASLANGICTARIHI", "IZINBASLANGIC", "BASTARIH"),
            new("Bitis", "Bitiş", false, "Ya bitiş ya gün sayısı", "BITIS", "BITISTARIHI", "IZINBITIS", "BITTARIH"),
            new("GunSayisi", "Gün Sayısı", false, null, "GUN", "GUNSAYISI", "IZINGUN", "SURE"),
            new("Aciklama", "Açıklama", false, null, "ACIKLAMA", "NOT"),
        }).ToArray(),
        AktarimTuru.KazancKesinti => PersonelAnahtarlari.Concat(new AlanTanimi[]
        {
            new("Tur", "Kalem (Kazanç/Kesinti Türü)", true, "Tanımlı kalem adı", "KALEM", "TUR", "KAZANCKESINTI", "KAZANCKESINTITURU", "ODEMETURU"),
            new("Tarih", "Tarih / Ay", false, "Yoksa ekranda seçilen ay", "TARIH", "AY", "DONEM"),
            new("Tutar", "Tutar", true, null, "TUTAR", "MIKTAR", "TUTARTL"),
            new("Aciklama", "Açıklama", false, null, "ACIKLAMA", "NOT"),
        }).ToArray(),
        AktarimTuru.Hareket => PersonelAnahtarlari.Concat(new AlanTanimi[]
        {
            new("Zaman", "Tarih + Saat (tek sütun)", false, null, "ZAMAN", "TARIHSAAT", "OKUTMAZAMANI"),
            new("Tarih", "Tarih", false, null, "TARIH", "GUN"),
            new("Saat", "Saat", false, "Her satır bir okutma ise", "SAAT", "OKUTMASAATI"),
            new("Giris", "Giriş Saati", false, "Her satır bir gün ise", "GIRIS", "GIRISSAATI", "GELIS"),
            new("Cikis", "Çıkış Saati", false, "Çıkış girişten küçükse ertesi gün sayılır", "CIKIS", "CIKISSAATI", "GIDIS"),
            new("TerminalKodu", "Terminal / Cihaz", false, null, "TERMINAL", "CIHAZ", "OKUYUCU"),
        }).ToArray(),
        AktarimTuru.Bordro => PersonelAnahtarlari.Concat(new AlanTanimi[]
        {
            new("Donem", "Dönem (ay)", false, "Yoksa Yıl + Ay ya da ekranda seçilen ay", "DONEM", "BORDRODONEMI"),
            new("Yil", "Yıl", false, null, "YIL"),
            new("Ay", "Ay", false, null, "AY"),
            new("CalisilanGun", "Çalışılan Gün", false, null, "CALISILANGUN", "GUN", "PRIMGUNU", "SGKGUN", "SGKGUNU", "ODENENGUN"),
            new("IzinGun", "İzin Günü", false, null, "IZINGUN", "IZINGUNU", "IZIN"),
            new("DevamsizGun", "Devamsız Gün", false, null, "DEVAMSIZ", "DEVAMSIZGUN", "DEVAMSIZLIK", "EKSIKGUN"),
            new("FazlaMesaiSaat", "Fazla Mesai (saat)", false, null, "FAZLAMESAI", "FM", "FMSAAT", "MESAI", "MESAISAAT"),
            new("BrutUcret", "Brüt Ücret", false, null, "BRUT", "BRUTUCRET", "TOPLAMBRUT"),
            new("NetUcret", "Net Ücret", false, null, "NET", "NETUCRET", "NETODENEN", "ODENECEK"),
            new("Aciklama", "Açıklama", false, null, "ACIKLAMA", "NOT"),
        }).ToArray(),
        _ => Array.Empty<AlanTanimi>(),
    };

    public static string TurAdi(AktarimTuru t) => t switch
    {
        AktarimTuru.Personel => "Personel",
        AktarimTuru.Ucret => "Ücret",
        AktarimTuru.Izin => "İzin",
        AktarimTuru.KazancKesinti => "Kazanç-Kesinti",
        AktarimTuru.Hareket => "Giriş-Çıkış Hareketi",
        AktarimTuru.Bordro => "Bordro",
        _ => t.ToString(),
    };

    /// <summary>Hareket, izin, kazanç-kesinti ve bordro aktarımı geri alınabilir; personel ve ücret değişiklikleri işlem kaydında durur.</summary>
    public static bool GeriAlinabilir(AktarimTuru t) => t is AktarimTuru.Hareket or AktarimTuru.Izin or AktarimTuru.KazancKesinti or AktarimTuru.Bordro;

    /// <summary>Excel başlıklarını alanlarla adından eşleştirir (alan → başlık).</summary>
    public static Dictionary<string, string> OtomatikEslestir(AktarimTuru tur, IReadOnlyList<string> basliklar)
    {
        var sonuc = new Dictionary<string, string>();
        var kullanilan = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var a in Alanlar(tur))
        {
            var adaylar = a.EsAdlar.Append(DegerCozucu.Anahtar(a.Etiket)).Append(DegerCozucu.Anahtar(a.Anahtar)).ToHashSet();
            var b = basliklar.FirstOrDefault(x => !kullanilan.Contains(x) && adaylar.Contains(DegerCozucu.Anahtar(x)));
            if (b is null) continue;
            sonuc[a.Anahtar] = b;
            kullanilan.Add(b);
        }
        return sonuc;
    }

    /// <summary>Eşleştirmede eksik olanlar (boşsa aktarım yapılabilir).</summary>
    public static List<string> EslesmeHatalari(AktarimTuru tur, ExcelTablo t, IReadOnlyDictionary<string, string> eslesme, AktarimSecenekleri? s = null)
    {
        bool Var(string alan) => eslesme.TryGetValue(alan, out var b) && t.SutunBul(b) >= 0;
        var h = Alanlar(tur).Where(a => a.Zorunlu && !Var(a.Anahtar)).Select(a => $"\"{a.Etiket}\" sütununu seçin.").ToList();
        if (tur != AktarimTuru.Personel && !PersonelAnahtarlari.Any(a => Var(a.Anahtar)))
            h.Add("Personeli bulmak için Kart No, Sicil No, T.C. Kimlik No ya da Ad Soyad sütunlarından birini seçin.");
        switch (tur)
        {
            case AktarimTuru.Izin when !Var("Bitis") && !Var("GunSayisi"):
                h.Add("\"Bitiş\" ya da \"Gün Sayısı\" sütununu seçin.");
                break;
            case AktarimTuru.Hareket when !Var("Zaman") && !(Var("Tarih") && (Var("Saat") || Var("Giris") || Var("Cikis"))):
                h.Add("Hareket için ya \"Tarih + Saat (tek sütun)\" ya da \"Tarih\" ile birlikte \"Saat\" veya \"Giriş/Çıkış Saati\" seçin.");
                break;
            case AktarimTuru.KazancKesinti when !Var("Tarih") && s?.Donem is null:
                h.Add("\"Tarih / Ay\" sütununu seçin ya da ayı ekrandan belirtin.");
                break;
            case AktarimTuru.Bordro when !Var("Donem") && !(Var("Yil") && Var("Ay")) && s?.Donem is null:
                h.Add("Bordro dönemi için \"Dönem\" ya da \"Yıl\" + \"Ay\" sütununu seçin veya ayı ekrandan belirtin.");
                break;
        }
        return h;
    }

    public static string EslesmeJson(IReadOnlyDictionary<string, string> e) => JsonSerializer.Serialize(e);

    public static Dictionary<string, string> EslesmeOku(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return new();
        try { return JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new(); }
        catch (JsonException) { return new(); }
    }

    // ---- Çalıştırma ----

    /// <summary>
    /// Dosyayı işler. <paramref name="uygula"/> false ise yalnızca önizleme yapar, veritabanına bir şey yazmaz.
    /// </summary>
    public static async Task<AktarimSonucu> IsleAsync(PdksDbContext db, AktarimTuru tur, ExcelTablo tablo,
        IReadOnlyDictionary<string, string> eslesme, bool uygula, string dosyaAdi, int? kullaniciId,
        AktarimSecenekleri? secenekler = null, CancellationToken ct = default)
    {
        secenekler ??= new AktarimSecenekleri();
        var hatalar = EslesmeHatalari(tur, tablo, eslesme, secenekler);
        if (hatalar.Count > 0) throw new ArgumentException(string.Join(" ", hatalar));

        var sutunlar = new Dictionary<string, int>();
        foreach (var (alan, baslik) in eslesme)
        {
            var i = tablo.SutunBul(baslik);
            if (i >= 0) sutunlar[alan] = i;
        }

        var b = new Baglam(db, tablo, sutunlar, Path.GetFileName(dosyaAdi), uygula, secenekler);
        var sonuc = new AktarimSonucu();

        AktarimKaydi? kayit = null;
        if (uygula)
        {
            kayit = new AktarimKaydi
            {
                Tur = tur, Zaman = DateTime.UtcNow, KullaniciId = kullaniciId, DosyaAdi = Kisalt(b.DosyaAdi, 250)!,
                SatirSayisi = tablo.Satirlar.Count, Eslesme = Kisalt(EslesmeJson(eslesme), 4000),
            };
            db.AktarimKayitlari.Add(kayit);
            await db.SaveChangesAsync(ct);
            b.AktarimId = kayit.Id;
        }

        // Toplu eklemelerde kayıt bazında işlem günlüğü yerine aktarım kaydı tutulur; personel/ücret değişiklikleri günlüğe yazılır.
        var gunlukKullanici = db.IslemYapanKullaniciId;
        if (tur is not (AktarimTuru.Personel or AktarimTuru.Ucret)) db.IslemYapanKullaniciId = null;
        try
        {
            switch (tur)
            {
                case AktarimTuru.Personel: await PersonelAsync(b, sonuc, ct); break;
                case AktarimTuru.Ucret: await UcretAsync(b, sonuc, ct); break;
                case AktarimTuru.Izin: await IzinAsync(b, sonuc, ct); break;
                case AktarimTuru.KazancKesinti: await KazancKesintiAsync(b, sonuc, ct); break;
                case AktarimTuru.Hareket: await HareketAsync(b, sonuc, ct); break;
                case AktarimTuru.Bordro: await BordroAsync(b, sonuc, ct); break;
                default: throw new ArgumentException("Aktarım türü seçin.");
            }
            if (uygula) await db.SaveChangesAsync(ct);
        }
        catch when (kayit is not null)
        {
            // Yarım kalan aktarım bırakılmaz: bu aktarımla yazılmış ara kayıtlar silinir.
            db.ChangeTracker.Clear();
            var silinecek = kayit!.Id;
            await AktarimKayitlariniSilAsync(db, tur, silinecek, CancellationToken.None);
            await db.AktarimKayitlari.Where(k => k.Id == silinecek).ExecuteDeleteAsync(CancellationToken.None);
            throw;
        }
        finally
        {
            db.IslemYapanKullaniciId = gunlukKullanici;
        }

        if (kayit is not null)
        {
            db.ChangeTracker.Clear();
            db.AktarimKayitlari.Attach(kayit);
            kayit.Eklenen = sonuc.Eklenen;
            kayit.Guncellenen = sonuc.Guncellenen;
            kayit.Atlanan = sonuc.Atlanan;
            kayit.Hatali = sonuc.Hatali;
            await db.SaveChangesAsync(ct);
            sonuc.AktarimId = kayit.Id;
        }
        return sonuc;
    }

    /// <summary>Aktarımı geri alır: hareketler iptal edilir, izin / kazanç-kesinti / bordro satırları silinir.</summary>
    public static async Task<int> GeriAlAsync(PdksDbContext db, int aktarimId, CancellationToken ct = default)
    {
        var k = await db.AktarimKayitlari.FirstOrDefaultAsync(x => x.Id == aktarimId, ct)
                ?? throw new ArgumentException("Aktarım bulunamadı.");
        if (k.GeriAlindi) throw new ArgumentException("Bu aktarım zaten geri alınmış.");
        if (!GeriAlinabilir(k.Tur)) throw new ArgumentException($"{TurAdi(k.Tur)} aktarımı geri alınamaz; yapılan değişiklikler işlem kaydında görülebilir.");

        int adet;
        if (k.Tur == AktarimTuru.Hareket)
            adet = await db.Hareketler.Where(h => h.AktarimId == aktarimId && !h.Iptal)
                .ExecuteUpdateAsync(s => s.SetProperty(h => h.Iptal, true), ct);
        else
            adet = await AktarimKayitlariniSilAsync(db, k.Tur, aktarimId, ct);

        k.GeriAlindi = true;
        k.GeriAlmaZamani = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        return adet;
    }

    private static Task<int> AktarimKayitlariniSilAsync(PdksDbContext db, AktarimTuru tur, int id, CancellationToken ct) => tur switch
    {
        AktarimTuru.Hareket => db.Hareketler.Where(h => h.AktarimId == id).ExecuteDeleteAsync(ct),
        AktarimTuru.Izin => db.Izinler.Where(h => h.AktarimId == id).ExecuteDeleteAsync(ct),
        AktarimTuru.KazancKesinti => db.KazancKesintiler.Where(h => h.AktarimId == id).ExecuteDeleteAsync(ct),
        AktarimTuru.Bordro => db.BordroSatirlari.Where(h => h.AktarimId == id).ExecuteDeleteAsync(ct),
        _ => Task.FromResult(0),
    };

    // ---- Ortak yardımcılar ----

    private sealed class Baglam
    {
        public Baglam(PdksDbContext db, ExcelTablo t, Dictionary<string, int> s, string dosya, bool uygula, AktarimSecenekleri sec)
        {
            Db = db; Tablo = t; Sutunlar = s; DosyaAdi = dosya; Uygula = uygula; Secenekler = sec;
        }

        public PdksDbContext Db { get; }
        public ExcelTablo Tablo { get; }
        public Dictionary<string, int> Sutunlar { get; }
        public string DosyaAdi { get; }
        public bool Uygula { get; }
        public AktarimSecenekleri Secenekler { get; }
        public int? AktarimId { get; set; }
        private int _bekleyen;

        public bool Var(string alan) => Sutunlar.ContainsKey(alan);

        /// <summary>Alan eşleştirilmemişse boş hücre döner.</summary>
        public Hucre H(Hucre[] satir, string alan) => Sutunlar.TryGetValue(alan, out var i) && i < satir.Length ? satir[i] : new Hucre("");

        public string? M(Hucre[] satir, string alan)
        {
            var m = DegerCozucu.Metin(H(satir, alan));
            return m.Length == 0 ? null : m;
        }

        public string Kaynak => $"Excel: {DosyaAdi}";

        /// <summary>Çok satırlı aktarımlarda belleği şişirmemek için ara kayıt.</summary>
        public async Task EklendiAsync(CancellationToken ct)
        {
            if (!Uygula || ++_bekleyen < AraKayitSiniri) return;
            await Db.SaveChangesAsync(ct);
            Db.ChangeTracker.Clear();
            _bekleyen = 0;
        }
    }

    private sealed record PersonelOzet(int Id, string KartNo, string? SicilNo, string? TcKimlikNo, string Ad, string Soyad,
        DateOnly IseGirisTarihi, DateOnly? IstenCikisTarihi)
    {
        public string AdSoyad => $"{Ad} {Soyad}";
        public bool CalisiyorMu(DateOnly t) => IseGirisTarihi <= t && (IstenCikisTarihi is null || IstenCikisTarihi >= t);
    }

    /// <summary>Kart, sicil, T.C. ya da ad-soyad ile personel bulur. Aynı kart birden çok kişide ise o tarihte çalışan seçilir.</summary>
    private sealed class PersonelBulucu
    {
        private readonly Dictionary<string, List<PersonelOzet>> _kart = new(), _sicil = new(), _tc = new(), _ad = new();

        public static async Task<PersonelBulucu> OlusturAsync(PdksDbContext db, CancellationToken ct)
        {
            var liste = await db.Personeller.AsNoTracking()
                .Select(p => new PersonelOzet(p.Id, p.KartNo, p.SicilNo, p.TcKimlikNo, p.Ad, p.Soyad, p.IseGirisTarihi, p.IstenCikisTarihi))
                .ToListAsync(ct);
            var b = new PersonelBulucu();
            foreach (var p in liste)
            {
                Ekle(b._kart, KodAnahtari(p.KartNo), p);
                Ekle(b._sicil, KodAnahtari(p.SicilNo), p);
                Ekle(b._tc, KodAnahtari(p.TcKimlikNo), p);
                Ekle(b._ad, DegerCozucu.Anahtar(p.AdSoyad), p);
            }
            return b;
        }

        private static void Ekle(Dictionary<string, List<PersonelOzet>> d, string k, PersonelOzet p)
        {
            if (k.Length == 0) return;
            if (!d.TryGetValue(k, out var l)) d[k] = l = new List<PersonelOzet>();
            l.Add(p);
        }

        public (PersonelOzet? P, string? Hata) Bul(Baglam b, Hucre[] satir, DateOnly? tarih)
        {
            (string Alan, Dictionary<string, List<PersonelOzet>> Sozluk, Func<Hucre, string> Anahtar)[] sira =
            {
                ("KartNo", _kart, h => KodAnahtari(DegerCozucu.Kod(h))),
                ("SicilNo", _sicil, h => KodAnahtari(DegerCozucu.Kod(h))),
                ("TcKimlikNo", _tc, h => KodAnahtari(DegerCozucu.Kod(h))),
                ("AdSoyad", _ad, h => DegerCozucu.Anahtar(h.Metin)),
            };
            string? aranan = null;
            foreach (var (alan, sozluk, anahtar) in sira)
            {
                if (!b.Var(alan)) continue;
                var h = b.H(satir, alan);
                var k = anahtar(h);
                if (k.Length == 0) continue;
                var tanim = alan switch { "KartNo" => "kart", "SicilNo" => "sicil", "TcKimlikNo" => "T.C.", _ => "ad" };
                aranan ??= $"{tanim} {DegerCozucu.Kod(h)}";
                if (!sozluk.TryGetValue(k, out var adaylar)) continue;
                var secilen = Sec(adaylar, tarih);
                if (secilen is not null) return (secilen, null);
                if (alan == "AdSoyad") return (null, $"\"{h.Metin}\" adında birden çok personel var; kart ya da sicil no kullanın.");
            }
            return (null, aranan is null ? "Personel bilgisi boş." : $"Personel bulunamadı ({aranan}).");
        }

        private static PersonelOzet? Sec(List<PersonelOzet> l, DateOnly? tarih)
        {
            if (l.Count == 1) return l[0];
            if (tarih is { } t && l.Where(p => p.CalisiyorMu(t)).ToList() is { Count: 1 } calisan) return calisan[0];
            var aktif = l.Where(p => p.IstenCikisTarihi is null).ToList();
            return aktif.Count == 1 ? aktif[0] : null;
        }
    }

    /// <summary>Kart / sicil numaralarında baştaki sıfırlar Excel'de kaybolabildiğinden karşılaştırmada atılır.</summary>
    private static string KodAnahtari(string? s)
    {
        var k = (s ?? "").Trim().ToUpperInvariant();
        if (k.Length > 0 && k.All(char.IsAsciiDigit)) k = k.TrimStart('0');
        return k.Length == 0 && !string.IsNullOrWhiteSpace(s) ? "0" : k;
    }

    private static string? Kisalt(string? s, int en)
    {
        if (string.IsNullOrWhiteSpace(s)) return null;
        s = s.Trim();
        return s.Length <= en ? s : s[..en];
    }

    private static Dictionary<string, T> TanimSozlugu<T>(IEnumerable<T> liste) where T : TanimEntity
    {
        var d = new Dictionary<string, T>();
        foreach (var t in liste)
        {
            d.TryAdd(DegerCozucu.Anahtar(t.Ad), t);
            if (!string.IsNullOrWhiteSpace(t.Kod)) d.TryAdd(DegerCozucu.Anahtar(t.Kod), t);
        }
        return d;
    }

    // ---- Hareket ----

    private static async Task HareketAsync(Baglam b, AktarimSonucu s, CancellationToken ct)
    {
        var bulucu = await PersonelBulucu.OlusturAsync(b.Db, ct);
        var adaylar = new List<(int SatirNo, PersonelOzet? P, List<DateTime> Zamanlar, string? Hata, string? Terminal)>();

        foreach (var (satir, no) in b.Tablo.Satirlar.Zip(b.Tablo.SatirNolari))
        {
            var zamanlar = new List<DateTime>();
            string? hata = null;
            if (b.Var("Zaman") && DegerCozucu.Zaman(b.H(satir, "Zaman")) is { } z) zamanlar.Add(z);
            else if (b.Var("Tarih"))
            {
                var tarihH = b.H(satir, "Tarih");
                var d = DegerCozucu.Tarih(tarihH);
                if (d is null) hata = tarihH.Bos ? "Tarih boş." : $"Tarih okunamadı: {tarihH.Metin}";
                else
                {
                    var g = b.Var("Saat") ? DegerCozucu.Saat(b.H(satir, "Saat")) : null;
                    if (g is { } saat) zamanlar.Add(d.Value.ToDateTime(saat));
                    var gir = b.Var("Giris") ? DegerCozucu.Saat(b.H(satir, "Giris")) : null;
                    var cik = b.Var("Cikis") ? DegerCozucu.Saat(b.H(satir, "Cikis")) : null;
                    if (gir is { } gs) zamanlar.Add(d.Value.ToDateTime(gs));
                    if (cik is { } cs) zamanlar.Add((gir is { } g2 && cs <= g2 ? d.Value.AddDays(1) : d.Value).ToDateTime(cs));
                    if (zamanlar.Count == 0) hata = "Saat yok ya da okunamadı.";
                }
            }
            else hata = "Tarih/saat okunamadı.";

            zamanlar = zamanlar.Select(x => new DateTime(x.Year, x.Month, x.Day, x.Hour, x.Minute, 0)).Distinct().ToList();
            if (hata is null && zamanlar.Any(x => x.Year is < 2000 or > 2100)) hata = "Tarih geçersiz.";

            PersonelOzet? p = null;
            if (hata is null)
            {
                (p, hata) = bulucu.Bul(b, satir, DateOnly.FromDateTime(zamanlar[0]));
            }
            adaylar.Add((no, p, zamanlar, hata, Kisalt(b.M(satir, "TerminalKodu"), 20)));
        }

        var tumZamanlar = adaylar.Where(a => a.Hata is null).SelectMany(a => a.Zamanlar).ToList();
        var mevcut = new HashSet<(int, DateTime)>();
        if (tumZamanlar.Count > 0)
        {
            var ilk = tumZamanlar.Min().Date;
            var son = tumZamanlar.Max().Date.AddDays(1);
            var liste = await b.Db.Hareketler.AsNoTracking()
                .Where(h => !h.Iptal && h.Zaman >= ilk && h.Zaman < son)
                .Select(h => new { h.PersonelId, h.Zaman }).ToListAsync(ct);
            foreach (var h in liste)
                mevcut.Add((h.PersonelId, new DateTime(h.Zaman.Year, h.Zaman.Month, h.Zaman.Day, h.Zaman.Hour, h.Zaman.Minute, 0)));
        }

        foreach (var a in adaylar)
        {
            if (a.Hata is not null || a.P is null) { s.Ekle(a.SatirNo, SatirDurumu.Hatali, "", a.Hata ?? "Personel bulunamadı."); continue; }
            foreach (var z in a.Zamanlar)
            {
                var ozet = $"{a.P.AdSoyad} — {z:dd.MM.yyyy HH:mm}";
                if (!a.P.CalisiyorMu(DateOnly.FromDateTime(z))) { s.Ekle(a.SatirNo, SatirDurumu.Atlanacak, ozet, "Personel bu tarihte işte değil."); continue; }
                if (!mevcut.Add((a.P.Id, z))) { s.Ekle(a.SatirNo, SatirDurumu.Atlanacak, ozet, "Bu hareket zaten var."); continue; }
                s.Ekle(a.SatirNo, SatirDurumu.Eklenecek, ozet);
                if (!b.Uygula) continue;
                b.Db.Hareketler.Add(new Hareket
                {
                    PersonelId = a.P.Id, Zaman = z, Kaynak = HareketKaynagi.Aktarim, TerminalKodu = a.Terminal,
                    Aciklama = Kisalt(b.Kaynak, 250), AktarimId = b.AktarimId,
                });
                await b.EklendiAsync(ct);
            }
        }
    }

    // ---- İzin ----

    private static async Task IzinAsync(Baglam b, AktarimSonucu s, CancellationToken ct)
    {
        var bulucu = await PersonelBulucu.OlusturAsync(b.Db, ct);
        var turler = TanimSozlugu(await b.Db.IzinTurleri.AsNoTracking().ToListAsync(ct));
        var mevcut = await b.Db.Izinler.AsNoTracking().Select(i => new { i.PersonelId, i.Baslangic, i.Bitis }).ToListAsync(ct);
        var araliklar = mevcut.GroupBy(i => i.PersonelId).ToDictionary(g => g.Key, g => g.Select(i => (i.Baslangic, i.Bitis)).ToList());

        foreach (var (satir, no) in b.Tablo.Satirlar.Zip(b.Tablo.SatirNolari))
        {
            var basH = b.H(satir, "Baslangic");
            var bas = DegerCozucu.Tarih(basH);
            if (bas is null) { s.Ekle(no, SatirDurumu.Hatali, "", basH.Bos ? "Başlangıç tarihi boş." : $"Başlangıç tarihi okunamadı: {basH.Metin}"); continue; }
            DateOnly? bit = b.Var("Bitis") ? DegerCozucu.Tarih(b.H(satir, "Bitis")) : null;
            if (bit is null && b.Var("GunSayisi") && DegerCozucu.Sayi(b.H(satir, "GunSayisi")) is { } gun && gun >= 1)
                bit = bas.Value.AddDays((int)Math.Ceiling(gun) - 1);
            if (bit is null) { s.Ekle(no, SatirDurumu.Hatali, "", "Bitiş tarihi ya da gün sayısı okunamadı."); continue; }
            if (bit < bas) { s.Ekle(no, SatirDurumu.Hatali, "", "Bitiş başlangıçtan önce."); continue; }
            if (bit.Value.DayNumber - bas.Value.DayNumber > 366) { s.Ekle(no, SatirDurumu.Hatali, "", "İzin bir yıldan uzun olamaz."); continue; }

            var turAdi = b.M(satir, "IzinTuru");
            if (turAdi is null || !turler.TryGetValue(DegerCozucu.Anahtar(turAdi), out var tur))
            {
                s.Ekle(no, SatirDurumu.Hatali, "", turAdi is null ? "İzin türü boş." : $"\"{turAdi}\" izin türü tanımlı değil (Tanımlar → İzin Türleri).");
                continue;
            }

            var (p, hata) = bulucu.Bul(b, satir, bas);
            if (p is null) { s.Ekle(no, SatirDurumu.Hatali, "", hata); continue; }
            var ozet = $"{p.AdSoyad} — {tur.Ad} {bas:dd.MM.yyyy}–{bit:dd.MM.yyyy}";
            if (p.IseGirisTarihi > bit || p.IstenCikisTarihi is { } c && c < bas) { s.Ekle(no, SatirDurumu.Atlanacak, ozet, "Personel bu tarihlerde işte değil."); continue; }

            if (!araliklar.TryGetValue(p.Id, out var l)) araliklar[p.Id] = l = new();
            if (l.Any(x => x.Baslangic <= bit && x.Bitis >= bas)) { s.Ekle(no, SatirDurumu.Atlanacak, ozet, "Bu tarihlerde başka izni var."); continue; }
            l.Add((bas.Value, bit.Value));

            s.Ekle(no, SatirDurumu.Eklenecek, ozet);
            if (!b.Uygula) continue;
            b.Db.Izinler.Add(new Izin
            {
                PersonelId = p.Id, IzinTuruId = tur.Id, Baslangic = bas.Value, Bitis = bit.Value,
                Aciklama = Kisalt(b.M(satir, "Aciklama") ?? b.Kaynak, 250), AktarimId = b.AktarimId,
            });
            await b.EklendiAsync(ct);
        }
    }

    // ---- Kazanç-kesinti ----

    private static async Task KazancKesintiAsync(Baglam b, AktarimSonucu s, CancellationToken ct)
    {
        var bulucu = await PersonelBulucu.OlusturAsync(b.Db, ct);
        var turler = TanimSozlugu(await b.Db.KazancKesintiTurleri.AsNoTracking().ToListAsync(ct));
        var mevcut = (await b.Db.KazancKesintiler.AsNoTracking()
                .Select(k => new { k.PersonelId, k.KazancKesintiTuruId, k.Tarih, k.Tutar }).ToListAsync(ct))
            .Select(k => (k.PersonelId, k.KazancKesintiTuruId, k.Tarih.Year, k.Tarih.Month, k.Tutar)).ToHashSet();

        foreach (var (satir, no) in b.Tablo.Satirlar.Zip(b.Tablo.SatirNolari))
        {
            DateOnly? tarih = null;
            if (b.Var("Tarih")) tarih = DegerCozucu.Tarih(b.H(satir, "Tarih")) ?? DegerCozucu.AyBasi(b.H(satir, "Tarih"));
            tarih ??= b.Secenekler.Donem;
            if (tarih is null) { s.Ekle(no, SatirDurumu.Hatali, "", "Tarih / ay okunamadı."); continue; }

            var tutar = DegerCozucu.Sayi(b.H(satir, "Tutar"));
            if (tutar is null) { s.Ekle(no, SatirDurumu.Hatali, "", $"Tutar okunamadı: {b.H(satir, "Tutar").Metin}"); continue; }
            if (tutar <= 0) { s.Ekle(no, SatirDurumu.Atlanacak, "", "Tutar sıfır ya da eksi."); continue; }
            tutar = Math.Round(tutar.Value, 2);

            var turAdi = b.M(satir, "Tur");
            if (turAdi is null || !turler.TryGetValue(DegerCozucu.Anahtar(turAdi), out var tur))
            {
                s.Ekle(no, SatirDurumu.Hatali, "", turAdi is null ? "Kalem boş." : $"\"{turAdi}\" kalemi tanımlı değil (Tanımlar → Kazanç/Kesinti Türleri).");
                continue;
            }

            var (p, hata) = bulucu.Bul(b, satir, tarih);
            if (p is null) { s.Ekle(no, SatirDurumu.Hatali, "", hata); continue; }
            var ozet = $"{p.AdSoyad} — {tur.Ad} {tarih:MM.yyyy} {tutar:N2} ₺";
            if (!mevcut.Add((p.Id, tur.Id, tarih.Value.Year, tarih.Value.Month, tutar.Value)))
            {
                s.Ekle(no, SatirDurumu.Atlanacak, ozet, "Aynı ay, kalem ve tutarla kayıt zaten var.");
                continue;
            }

            s.Ekle(no, SatirDurumu.Eklenecek, ozet);
            if (!b.Uygula) continue;
            b.Db.KazancKesintiler.Add(new KazancKesinti
            {
                PersonelId = p.Id, KazancKesintiTuruId = tur.Id, Tarih = tarih.Value, Tutar = tutar.Value,
                Aciklama = Kisalt(b.M(satir, "Aciklama") ?? b.Kaynak, 250), AktarimId = b.AktarimId,
            });
            await b.EklendiAsync(ct);
        }
    }

    // ---- Bordro ----

    private static readonly string[] AyAdlari =
        { "OCAK", "SUBAT", "MART", "NISAN", "MAYIS", "HAZIRAN", "TEMMUZ", "AGUSTOS", "EYLUL", "EKIM", "KASIM", "ARALIK" };

    private static DateOnly? BordroDonemi(Baglam b, Hucre[] satir)
    {
        if (b.Var("Donem"))
        {
            var h = b.H(satir, "Donem");
            if (DegerCozucu.AyBasi(h) is { } d) return d;
            // "Eylül 2026" gibi
            var k = DegerCozucu.Anahtar(h.Metin);
            for (var i = 0; i < 12; i++)
                if (k.StartsWith(AyAdlari[i]) && int.TryParse(k[AyAdlari[i].Length..], out var y) && y is > 1999 and < 2101)
                    return new DateOnly(y, i + 1, 1);
        }
        if (b.Var("Yil") && b.Var("Ay") && DegerCozucu.Sayi(b.H(satir, "Yil")) is { } yil && yil is > 1999 and < 2101)
        {
            var ayH = b.H(satir, "Ay");
            var ay = DegerCozucu.Sayi(ayH) is { } a && a is >= 1 and <= 12 ? (int)a
                : Array.FindIndex(AyAdlari, x => DegerCozucu.Anahtar(ayH.Metin).StartsWith(x)) + 1;
            if (ay is >= 1 and <= 12) return new DateOnly((int)yil, ay, 1);
        }
        return b.Secenekler.Donem is { } sd ? new DateOnly(sd.Year, sd.Month, 1) : null;
    }

    private static async Task BordroAsync(Baglam b, AktarimSonucu s, CancellationToken ct)
    {
        var bulucu = await PersonelBulucu.OlusturAsync(b.Db, ct);
        var mevcut = b.Uygula
            ? await b.Db.BordroSatirlari.ToDictionaryAsync(x => (x.PersonelId, x.Yil, x.Ay), ct)
            : await b.Db.BordroSatirlari.AsNoTracking().ToDictionaryAsync(x => (x.PersonelId, x.Yil, x.Ay), ct);
        var dosyada = new HashSet<(int, int, int)>();

        foreach (var (satir, no) in b.Tablo.Satirlar.Zip(b.Tablo.SatirNolari))
        {
            var donem = BordroDonemi(b, satir);
            if (donem is null) { s.Ekle(no, SatirDurumu.Hatali, "", "Bordro dönemi okunamadı."); continue; }
            var (p, hata) = bulucu.Bul(b, satir, donem.Value.AddMonths(1).AddDays(-1));
            if (p is null) { s.Ekle(no, SatirDurumu.Hatali, "", hata); continue; }

            decimal? Oku(string alan) => b.Var(alan) ? DegerCozucu.Sayi(b.H(satir, alan)) : null;
            var cg = Oku("CalisilanGun");
            var ozet = $"{p.AdSoyad} — {donem:MM.yyyy}" + (cg is { } x ? $" · {x:0.##} gün" : "");
            if (!dosyada.Add((p.Id, donem.Value.Year, donem.Value.Month)))
            {
                s.Ekle(no, SatirDurumu.Atlanacak, ozet, "Bu personelin aynı ay bordrosu dosyada daha önce geçti.");
                continue;
            }

            var oncedenVar = mevcut.TryGetValue((p.Id, donem.Value.Year, donem.Value.Month), out var kayit);
            s.Ekle(no, oncedenVar ? SatirDurumu.Guncellenecek : SatirDurumu.Eklenecek, ozet, oncedenVar ? "Bu ayın bordrosu yenisiyle değiştirilecek." : null);
            if (!b.Uygula) continue;

            if (kayit is null)
            {
                kayit = new BordroSatiri { PersonelId = p.Id, Yil = donem.Value.Year, Ay = donem.Value.Month };
                b.Db.BordroSatirlari.Add(kayit);
            }
            kayit.CalisilanGun = cg;
            kayit.IzinGun = Oku("IzinGun");
            kayit.DevamsizGun = Oku("DevamsizGun");
            kayit.FazlaMesaiSaat = Oku("FazlaMesaiSaat");
            kayit.BrutUcret = Oku("BrutUcret");
            kayit.NetUcret = Oku("NetUcret");
            kayit.Aciklama = Kisalt(b.M(satir, "Aciklama") ?? b.Kaynak, 250);
            kayit.AktarimId = b.AktarimId;
        }
    }

    // ---- Ücret ----

    private static async Task UcretAsync(Baglam b, AktarimSonucu s, CancellationToken ct)
    {
        var bulucu = await PersonelBulucu.OlusturAsync(b.Db, ct);
        var personeller = b.Uygula
            ? await b.Db.Personeller.ToDictionaryAsync(p => p.Id, ct)
            : await b.Db.Personeller.AsNoTracking().ToDictionaryAsync(p => p.Id, ct);
        var islenen = new HashSet<int>();

        foreach (var (satir, no) in b.Tablo.Satirlar.Zip(b.Tablo.SatirNolari))
        {
            var ucret = DegerCozucu.Sayi(b.H(satir, "Ucret"));
            if (ucret is null) { s.Ekle(no, SatirDurumu.Hatali, "", $"Ücret okunamadı: {b.H(satir, "Ucret").Metin}"); continue; }
            if (ucret < 0) { s.Ekle(no, SatirDurumu.Hatali, "", "Ücret eksi olamaz."); continue; }
            ucret = Math.Round(ucret.Value, 2);

            var (bulunan, hata) = bulucu.Bul(b, satir, null);
            if (bulunan is null) { s.Ekle(no, SatirDurumu.Hatali, "", hata); continue; }
            if (!islenen.Add(bulunan.Id)) { s.Ekle(no, SatirDurumu.Atlanacak, bulunan.AdSoyad, "Bu personel dosyada daha önce geçti."); continue; }
            var p = personeller[bulunan.Id];

            UcretTipi? tip = null;
            var tipMetni = DegerCozucu.Anahtar(b.M(satir, "UcretTipi"));
            if (tipMetni.StartsWith("SAAT")) tip = UcretTipi.SaatUcreti;
            else if (tipMetni.StartsWith("MAAS") || tipMetni.StartsWith("AYLIK")) tip = UcretTipi.Maas;
            decimal? mesai = b.Var("MesaiSaatUcreti") ? DegerCozucu.Sayi(b.H(satir, "MesaiSaatUcreti")) : null;

            var degisen = new List<string>();
            if (p.Ucret != ucret) degisen.Add($"ücret {p.Ucret:N2} → {ucret:N2}");
            if (tip is { } t && p.UcretTipi != t) degisen.Add($"tip → {(t == UcretTipi.Maas ? "maaş" : "saat")}");
            if (mesai is { } m && p.MesaiSaatUcreti != Math.Round(m, 2)) degisen.Add($"mesai saat ücreti → {m:N2}");
            if (degisen.Count == 0) { s.Ekle(no, SatirDurumu.Atlanacak, p.AdSoyad, "Değişiklik yok."); continue; }

            s.Ekle(no, SatirDurumu.Guncellenecek, p.AdSoyad, string.Join(", ", degisen));
            if (!b.Uygula) continue;
            if (p.Ucret != ucret) { p.EskiUcret = p.Ucret; p.Ucret = ucret; }
            if (tip is { } t2) p.UcretTipi = t2;
            if (mesai is { } m2) p.MesaiSaatUcreti = Math.Round(m2, 2);
        }
    }

    // ---- Personel ----

    private sealed record MetinAlani(string Anahtar, int EnFazla, Func<Personel, string?> Al, Action<Personel, string?> Yaz);

    private static readonly MetinAlani[] MetinAlanlari =
    {
        new("SicilNo", 30, p => p.SicilNo, (p, v) => p.SicilNo = v),
        new("CepTelefonu", 30, p => p.CepTelefonu, (p, v) => p.CepTelefonu = v),
        new("Eposta", 150, p => p.Eposta, (p, v) => p.Eposta = v),
        new("Adres", 500, p => p.Adres, (p, v) => p.Adres = v),
        new("Iban", 34, p => p.Iban, (p, v) => p.Iban = v?.Replace(" ", "")),
        new("SgkSicilNo", 40, p => p.SgkSicilNo, (p, v) => p.SgkSicilNo = v),
        new("BabaAdi", 60, p => p.BabaAdi, (p, v) => p.BabaAdi = v),
        new("AnneAdi", 60, p => p.AnneAdi, (p, v) => p.AnneAdi = v),
        new("DogumYeri", 60, p => p.DogumYeri, (p, v) => p.DogumYeri = v),
        new("KanGrubu", 30, p => p.KanGrubu, (p, v) => p.KanGrubu = v),
    };

    private static async Task PersonelAsync(Baglam b, AktarimSonucu s, CancellationToken ct)
    {
        var db = b.Db;
        var personeller = b.Uygula ? await db.Personeller.ToListAsync(ct) : await db.Personeller.AsNoTracking().ToListAsync(ct);
        var kartlar = personeller.GroupBy(p => KodAnahtari(p.KartNo)).ToDictionary(g => g.Key, g => g.ToList());
        var tcler = personeller.Where(p => !string.IsNullOrEmpty(p.TcKimlikNo)).GroupBy(p => p.TcKimlikNo!).ToDictionary(g => g.Key, g => g.ToList());

        var subeler = TanimSozlugu(b.Uygula ? await db.Subeler.ToListAsync(ct) : await db.Subeler.AsNoTracking().ToListAsync(ct));
        var bolumler = TanimSozlugu(b.Uygula ? await db.Bolumler.ToListAsync(ct) : await db.Bolumler.AsNoTracking().ToListAsync(ct));
        var gorevler = TanimSozlugu(b.Uygula ? await db.Gorevler.ToListAsync(ct) : await db.Gorevler.AsNoTracking().ToListAsync(ct));
        var gruplar = TanimSozlugu(await db.CalismaGruplari.AsNoTracking().ToListAsync(ct));
        var varsayilanGrup = await db.CalismaGruplari.AsNoTracking().Where(g => g.Aktif).OrderBy(g => g.Id).Select(g => (int?)g.Id).FirstOrDefaultAsync(ct);
        var dosyadakiKartlar = new HashSet<string>();

        foreach (var (satir, no) in b.Tablo.Satirlar.Zip(b.Tablo.SatirNolari))
        {
            var kart = DegerCozucu.Kod(b.H(satir, "KartNo"));
            if (kart.Length == 0) { s.Ekle(no, SatirDurumu.Hatali, "", "Kart no boş."); continue; }
            if (kart.Length > 20) { s.Ekle(no, SatirDurumu.Hatali, kart, "Kart no en çok 20 karakter olabilir."); continue; }
            if (!dosyadakiKartlar.Add(KodAnahtari(kart))) { s.Ekle(no, SatirDurumu.Atlanacak, kart, "Bu kart no dosyada daha önce geçti."); continue; }

            string? ad = Kisalt(b.M(satir, "Ad"), 60), soyad = Kisalt(b.M(satir, "Soyad"), 60);
            if ((ad is null || soyad is null) && b.M(satir, "AdSoyad") is { } adSoyad)
            {
                var parca = adSoyad.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parca.Length >= 2) { ad ??= Kisalt(string.Join(' ', parca[..^1]), 60); soyad ??= Kisalt(parca[^1], 60); }
            }

            var tc = b.Var("TcKimlikNo") ? DegerCozucu.Kod(b.H(satir, "TcKimlikNo")) : "";
            if (tc.Length > 0 && !TcKimlik.GecerliMi(tc)) { s.Ekle(no, SatirDurumu.Hatali, kart, $"T.C. kimlik no geçersiz: {tc}"); continue; }

            DateOnly? Tarih(string alan) => b.Var(alan) ? DegerCozucu.Tarih(b.H(satir, alan)) : null;
            string? TarihHatasi(string alan, string etiket) =>
                b.Var(alan) && !b.H(satir, alan).Bos && DegerCozucu.Tarih(b.H(satir, alan)) is null ? $"{etiket} okunamadı: {b.H(satir, alan).Metin}" : null;
            var tarihHatasi = TarihHatasi("IseGirisTarihi", "İşe giriş tarihi") ?? TarihHatasi("IstenCikisTarihi", "Çıkış tarihi") ?? TarihHatasi("DogumTarihi", "Doğum tarihi");
            if (tarihHatasi is not null) { s.Ekle(no, SatirDurumu.Hatali, kart, tarihHatasi); continue; }
            var giris = Tarih("IseGirisTarihi");
            var cikis = Tarih("IstenCikisTarihi");

            // Var olan personel: önce kart (o kartı taşıyan aktif kişi), sonra T.C.
            Personel? p = null;
            if (kartlar.TryGetValue(KodAnahtari(kart), out var ayniKart))
                p = ayniKart.Count == 1 ? ayniKart[0] : ayniKart.Where(x => x.IstenCikisTarihi is null).OrderByDescending(x => x.IseGirisTarihi).FirstOrDefault()
                    ?? ayniKart.OrderByDescending(x => x.IseGirisTarihi).First();
            if (p is null && tc.Length > 0 && tcler.TryGetValue(tc, out var ayniTc))
                p = ayniTc.OrderByDescending(x => x.IseGirisTarihi).First();

            var yeni = p is null;
            if (yeni)
            {
                if (ad is null || soyad is null) { s.Ekle(no, SatirDurumu.Hatali, kart, "Yeni personel için ad ve soyad gerekli."); continue; }
                if (giris is null) { s.Ekle(no, SatirDurumu.Hatali, kart, "Yeni personel için işe giriş tarihi gerekli."); continue; }
                p = new Personel { KartNo = kart, Ad = ad, Soyad = soyad, IseGirisTarihi = giris.Value, CalismaGrubuId = varsayilanGrup, AktarimId = b.AktarimId };
            }
            var hedef = b.Uygula || yeni ? p! : Kopya(p!);
            var degisen = new List<string>();

            void Metin(string etiket, string? deger, Func<string?> al, Action<string?> yaz)
            {
                if (deger is null || string.Equals(al(), deger, StringComparison.Ordinal)) return;
                yaz(deger);
                if (!yeni) degisen.Add(etiket);
            }

            Metin("ad", ad, () => hedef.Ad, v => hedef.Ad = v!);
            Metin("soyad", soyad, () => hedef.Soyad, v => hedef.Soyad = v!);
            Metin("T.C.", tc.Length > 0 ? tc : null, () => hedef.TcKimlikNo, v => hedef.TcKimlikNo = v);
            foreach (var a in MetinAlanlari)
                if (b.Var(a.Anahtar))
                    Metin(a.Anahtar, Kisalt(b.M(satir, a.Anahtar), a.EnFazla), () => a.Al(hedef), v => a.Yaz(hedef, v));

            if (giris is { } g && hedef.IseGirisTarihi != g) { hedef.IseGirisTarihi = g; if (!yeni) degisen.Add("işe giriş"); }
            if (cikis is { } c && hedef.IstenCikisTarihi != c) { hedef.IstenCikisTarihi = c; if (!yeni) degisen.Add("çıkış"); }
            if (Tarih("DogumTarihi") is { } dt && hedef.DogumTarihi != dt) { hedef.DogumTarihi = dt; if (!yeni) degisen.Add("doğum tarihi"); }
            if (hedef.IstenCikisTarihi is { } c2 && c2 < hedef.IseGirisTarihi) { s.Ekle(no, SatirDurumu.Hatali, kart, "Çıkış tarihi işe giriş tarihinden önce."); continue; }

            var cins = DegerCozucu.Anahtar(b.M(satir, "Cinsiyet"));
            Cinsiyet? cinsiyet = cins switch
            {
                "E" or "ERKEK" or "BAY" or "M" or "1" => Cinsiyet.Erkek,
                "K" or "KADIN" or "BAYAN" or "F" or "2" => Cinsiyet.Kadin,
                _ => null,
            };
            if (cinsiyet is { } cv && hedef.Cinsiyet != cv) { hedef.Cinsiyet = cv; if (!yeni) degisen.Add("cinsiyet"); }

            if (b.Var("Ucret") && DegerCozucu.Sayi(b.H(satir, "Ucret")) is { } u && u >= 0 && hedef.Ucret != Math.Round(u, 2))
            {
                if (!yeni) { hedef.EskiUcret = hedef.Ucret; degisen.Add("ücret"); }
                hedef.Ucret = Math.Round(u, 2);
            }

            var notlar = new List<string>();
            T? Tanim<T>(string alan, Dictionary<string, T> sozluk, Func<string, T>? olustur, string etiket) where T : TanimEntity
            {
                var adi = Kisalt(b.M(satir, alan), 120);
                if (adi is null) return null;
                if (sozluk.TryGetValue(DegerCozucu.Anahtar(adi), out var t)) return t;
                if (olustur is null) return null;
                var yeniTanim = olustur(adi);
                sozluk[DegerCozucu.Anahtar(adi)] = yeniTanim;
                if (b.Uygula) db.Add(yeniTanim);
                notlar.Add($"yeni {etiket}: {adi}");
                return yeniTanim;
            }

            if (Tanim("Sube", subeler, a => new Sube { Ad = a }, "şube") is { } sube && (hedef.SubeId != sube.Id || sube.Id == 0))
            { if (sube.Id == 0) hedef.Sube = sube; else hedef.SubeId = sube.Id; if (!yeni) degisen.Add("şube"); }
            if (Tanim("Bolum", bolumler, a => new Bolum { Ad = a }, "bölüm") is { } bolum && (hedef.BolumId != bolum.Id || bolum.Id == 0))
            { if (bolum.Id == 0) hedef.Bolum = bolum; else hedef.BolumId = bolum.Id; if (!yeni) degisen.Add("bölüm"); }
            if (Tanim("Gorev", gorevler, a => new Gorev { Ad = a }, "görev") is { } gorev && (hedef.GorevId != gorev.Id || gorev.Id == 0))
            { if (gorev.Id == 0) hedef.Gorev = gorev; else hedef.GorevId = gorev.Id; if (!yeni) degisen.Add("görev"); }
            if (b.M(satir, "CalismaGrubu") is { } grupAdi)
            {
                var grup = Tanim<CalismaGrubu>("CalismaGrubu", gruplar, null, "grup");
                if (grup is null) { s.Ekle(no, SatirDurumu.Hatali, kart, $"\"{grupAdi}\" çalışma grubu tanımlı değil."); continue; }
                if (hedef.CalismaGrubuId != grup.Id) { hedef.CalismaGrubuId = grup.Id; if (!yeni) degisen.Add("grup"); }
            }
            if (yeni && hedef.CalismaGrubuId is null) { s.Ekle(no, SatirDurumu.Hatali, kart, "Çalışma grubu tanımlı değil; önce bir grup tanımlayın."); continue; }

            var ozet = $"{kart} {hedef.Ad} {hedef.Soyad}";
            var not = notlar.Count > 0 ? " (" + string.Join(", ", notlar) + ")" : "";
            if (yeni)
            {
                s.Ekle(no, SatirDurumu.Eklenecek, ozet, not.Length > 0 ? not.Trim() : null);
                kartlar[KodAnahtari(kart)] = new List<Personel> { p! };
                if (b.Uygula) db.Personeller.Add(p!);
            }
            else if (degisen.Count == 0 && not.Length == 0) s.Ekle(no, SatirDurumu.Atlanacak, ozet, "Değişiklik yok.");
            else s.Ekle(no, SatirDurumu.Guncellenecek, ozet, "Değişen: " + string.Join(", ", degisen) + not);
        }
    }

    /// <summary>Önizlemede var olan personelin üzerinde değişiklik denemek için sığ kopya.</summary>
    private static Personel Kopya(Personel p) => (Personel)typeof(object)
        .GetMethod("MemberwiseClone", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!
        .Invoke(p, null)!;
}
