using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace BiltekMuhasebeKoprusu
{
    internal sealed class Donem
    {
        public string Bas, Bit;
        public override string ToString()
        {
            var b = PdksVeri.Trh(Bas); var e = PdksVeri.Trh(Bit);
            return (b.HasValue ? b.Value.ToString("dd.MM.yyyy") : Bas) + " - " + (e.HasValue ? e.Value.ToString("dd.MM.yyyy") : Bit);
        }
    }

    internal sealed class PuantajKolon
    {
        public string Baslik, Sql;   // Sql: pts_puantajtoplam içindeki alan
        public bool Saat;            // "07:30" biçimli alan -> ondalık saate çevrilir
    }

    internal sealed class PuantajSonuc
    {
        public List<string> Basliklar = new List<string>();
        public List<object[]> Satirlar = new List<object[]>();
        public int TcEksik;
        public string UstSatir;
    }

    internal sealed class PuantajKisi
    {
        public string Tc, Sicil, Kart, Ad, Soyad, FirmaKod;
        public decimal[] Degerler;
    }

    internal sealed class PuantajHam
    {
        public Donem Donem;
        public List<PuantajKolon> Kolonlar;
        public List<PuantajKisi> Kisiler = new List<PuantajKisi>();
    }

    internal static class Puantaj
    {
        public static List<Donem> Donemler(Db db, int? firmaId)
        {
            if (!db.TabloVar("pts_puantajtoplam")) throw new Exception("Bu veritabanında pts_puantajtoplam tablosu yok.");
            string sql = "SELECT DISTINCT t.bastarih, t.bittarih FROM pts_puantajtoplam t" +
                         (firmaId.HasValue ? " JOIN personel p ON p.id = t.personelid WHERE p.firmaid = " + firmaId.Value : "") +
                         " ORDER BY t.bastarih DESC LIMIT 60";
            return db.Sorgu(sql).Where(r => r[0] != null).Select(r => new Donem { Bas = r[0], Bit = r[1] }).ToList();
        }

        public static List<PuantajKolon> Kolonlar(Db db)
        {
            var mevcut = new HashSet<string>(db.Sorgu("SHOW COLUMNS FROM pts_puantajtoplam").Select(r => r[0]), StringComparer.OrdinalIgnoreCase);
            var l = new List<PuantajKolon>();
            Action<string, string, bool> ekle = (baslik, alan, saat) => { if (mevcut.Contains(alan)) l.Add(new PuantajKolon { Baslik = baslik, Sql = alan, Saat = saat }); };

            ekle("Normal Çalışma (Gün)", "ncgun", false);
            // Çalışma türleri (normal, fazla mesai, hafta tatili, resmi tatil...): cgunN / csaatN, N = tür kodu
            foreach (var r in db.Sorgu("SELECT kod, ad, aciklama FROM pts_calismaturleri WHERE kod BETWEEN 1 AND 32 ORDER BY kod"))
            {
                string ad = Ad(r[1], r[2]);
                ekle(ad + " (Gün)", "cgun" + r[0], false);
                ekle(ad + " (Saat)", "csaat" + r[0], true);
            }
            foreach (var r in db.Sorgu("SELECT kod, ad, aciklama FROM pts_izinturleri WHERE kod BETWEEN 1 AND 32 ORDER BY kod"))
            {
                string ad = Ad(r[1], r[2]);
                ekle(ad + " (Gün)", "igun" + r[0], false);
                ekle(ad + " (Saat)", "isaat" + r[0], true);
            }
            ekle("Hafta Tatili (Gün)", "htkgun", false);
            ekle("Hafta İçi Tatil (Gün)", "hitgun", false);
            ekle("Devamsızlık (Gün)", "devamsizlikgun_net", false);
            ekle("Devamsızlık (Saat)", "devamsizliksaat_net", true);
            ekle("Geç Kalma (Saat)", "geckalmasaat_net", true);
            ekle("Erken Çıkma (Saat)", "erkencikmasaat_net", true);
            ekle("Eksik Süre (Saat)", "eksiksuresaat_net", true);
            ekle("Dönem Farkı (Gün)", "donemfarkigun", false);
            return l;
        }

        static string Ad(string kisa, string aciklama)
        {
            if (!string.IsNullOrWhiteSpace(aciklama) && aciklama.Trim().Length <= 30) return aciklama.Trim();
            return (kisa ?? "").Trim();
        }

        public static PuantajHam Ham(Db db, Donem d, int? firmaId)
        {
            var kolonlar = Kolonlar(db);
            string sql = "SELECT p.kimlikno, p.sicilno, p.kartno, p.ad, p.soyad, f.kod" +
                         string.Concat(kolonlar.Select(k => ", t." + k.Sql)) +
                         " FROM pts_puantajtoplam t JOIN personel p ON p.id = t.personelid LEFT JOIN firma f ON f.id = p.firmaid" +
                         " WHERE t.bastarih = " + Db.Q(d.Bas) + " AND t.bittarih = " + Db.Q(d.Bit) +
                         (firmaId.HasValue ? " AND p.firmaid = " + firmaId.Value : "") +
                         " ORDER BY p.ad, p.soyad";
            var h = new PuantajHam { Kolonlar = kolonlar, Donem = d };
            foreach (var r in db.Sorgu(sql))
            {
                h.Kisiler.Add(new PuantajKisi
                {
                    Tc = Metin.SadeceRakam(r[0]), Sicil = r[1] ?? "", Kart = r[2] ?? "", Ad = r[3] ?? "", Soyad = r[4] ?? "", FirmaKod = r[5] ?? "",
                    Degerler = kolonlar.Select((k, i) => Deger(r[6 + i], k.Saat)).ToArray()
                });
            }
            return h;
        }

        public static PuantajSonuc Getir(Db db, Donem d, int? firmaId, bool bosKolonlariGizle)
        {
            var h = Ham(db, d, firmaId);
            var kolonlar = h.Kolonlar;
            var gorunur = Enumerable.Range(0, kolonlar.Count)
                                    .Where(i => !bosKolonlariGizle || h.Kisiler.Any(k => k.Degerler[i] != 0m)).ToList();
            var s = new PuantajSonuc();
            s.Basliklar.AddRange(new[] { "TC Kimlik No", "Sicil No", "Kart No", "Adı", "Soyadı" });
            s.Basliklar.AddRange(gorunur.Select(i => kolonlar[i].Baslik));
            s.Basliklar.Add("Uyarı");
            foreach (var k in h.Kisiler)
            {
                string uyari = Metin.TcGecerli(k.Tc) ? "" : "TC eksik/hatalı - muhasebeye aktarılamaz";
                if (uyari.Length > 0) s.TcEksik++;
                var satir = new List<object> { k.Tc, k.Sicil, k.Kart, k.Ad, k.Soyad };
                satir.AddRange(gorunur.Select(i => (object)k.Degerler[i]));
                satir.Add(uyari);
                s.Satirlar.Add(satir.ToArray());
            }
            return s;
        }

        // "07:30" -> 7.5 ; "-01:15" -> -1.25 ; sayısal alanlar olduğu gibi.
        static decimal Deger(string s, bool saat)
        {
            if (string.IsNullOrWhiteSpace(s)) return 0m;
            s = s.Trim();
            if (saat && s.Contains(":"))
            {
                bool eksi = s.StartsWith("-");
                var p = s.TrimStart('-').Split(':');
                int h, m;
                if (p.Length >= 2 && int.TryParse(p[0], out h) && int.TryParse(p[1], out m))
                {
                    decimal v = Math.Round(h + m / 60m, 2);
                    return eksi ? -v : v;
                }
                return 0m;
            }
            decimal d;
            return decimal.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out d) ? d : 0m;
        }
    }
}
