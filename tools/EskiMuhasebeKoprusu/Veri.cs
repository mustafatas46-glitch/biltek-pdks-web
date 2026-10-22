using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace BiltekMuhasebeKoprusu
{
    internal sealed class Personel
    {
        public long Id;
        public string KartNo, Ad, Soyad, SicilNo, KimlikNo, SgkNo, Durum;
        public int? FirmaId, GrupId;
        public DateTime? IseGiris, IstenCikis;
        public decimal? Ucret;
        public string AdSoyad { get { return ((Ad ?? "") + " " + (Soyad ?? "")).Trim(); } }
        public bool Aktif { get { return !IstenCikis.HasValue || IstenCikis.Value.Date >= DateTime.Today; } }
    }

    // Muhasebe / IKYS dosyasındaki bir kişi.
    internal sealed class MuhasebeKisi
    {
        public int Satir;
        public string AdSoyad, Tc, SicilNo, SgkNo;
        public DateTime? IseGiris, IstenCikis;
        public decimal? Ucret;
    }

    internal sealed class Firma
    {
        public int Id; public string Ad;
        public override string ToString() { return Ad; }
    }

    internal static class Ayarlar
    {
        public static string Klasor { get { return AppDomain.CurrentDomain.BaseDirectory; } }
        static string Dosya { get { return Path.Combine(Klasor, "muhasebe-koprusu.ini"); } }

        public static Dictionary<string, string> Oku()
        {
            var d = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
#if FIREBIRD
            // BilTek: Data\Settings.ini [Database] Path=127.0.0.1:C:\...\DATABASE.GDB  (en yeni BilTek5xx klasörü)
            var fbAdaylar = new List<string> { Path.Combine(Klasor, "Data", "Settings.ini"), Path.Combine(Klasor, "..", "Data", "Settings.ini") };
            try
            {
                var ust = Path.GetFullPath(Path.Combine(Klasor, ".."));
                fbAdaylar.AddRange(Directory.GetDirectories(ust, "BilTek*").OrderByDescending(x => x, StringComparer.OrdinalIgnoreCase).Select(x => Path.Combine(x, "Data", "Settings.ini")));
            }
            catch { }
            foreach (var aday in fbAdaylar)
            {
                if (!File.Exists(aday)) continue;
                var ini = IniOku(aday);
                string yol;
                if (!ini.TryGetValue("Database.Path", out yol) || yol.Trim().Length == 0) continue;
                yol = yol.Trim();
                int i = yol.IndexOf(':');
                if (i > 1 && !(i == 1)) { d["Sunucu"] = yol.Substring(0, i); d["Veritabani"] = yol.Substring(i + 1); }
                else { d["Sunucu"] = ""; d["Veritabani"] = yol; }
                if (d["Sunucu"].Contains("/")) { var p = d["Sunucu"].Split('/'); d["Sunucu"] = p[0]; d["Port"] = p[1]; }
                d["Kaynak"] = Path.GetFullPath(aday);
                break;
            }
            if (false)
#endif
            // Önce eski programın Settings.ini dosyası, sonra köprünün kendi ayarları.
            foreach (var aday in new[] { Path.Combine(Klasor, "Settings.ini"), Path.Combine(Klasor, "..", "Settings.ini"), @"D:\Biltek-Pdks\Settings.ini", @"C:\Biltek-Pdks\Settings.ini" })
            {
                if (!File.Exists(aday)) continue;
                var ini = IniOku(aday);
                Kopyala(ini, "Login.Server", d, "Sunucu");
                Kopyala(ini, "Login.Database", d, "Veritabani");
                Kopyala(ini, "MySql.User", d, "Kullanici");
                Kopyala(ini, "MySql.Password", d, "Sifre");
                Kopyala(ini, "MySql.Port", d, "Port");
                d["Kaynak"] = Path.GetFullPath(aday);
                break;
            }
            if (File.Exists(Dosya)) foreach (var kv in IniOku(Dosya)) if (kv.Key.StartsWith("Kopru.")) d[kv.Key.Substring(6)] = kv.Value;
            return d;
        }

        public static void Kaydet(Dictionary<string, string> d)
        {
            var sb = new StringBuilder("[Kopru]\r\n");
            foreach (var kv in d) if (kv.Key != "Kaynak" && kv.Key != "Sifre") sb.Append(kv.Key + "=" + kv.Value + "\r\n");
            try { File.WriteAllText(Dosya, sb.ToString(), Encoding.UTF8); } catch { }
        }

        static void Kopyala(Dictionary<string, string> kaynak, string k, Dictionary<string, string> hedef, string h)
        {
            string v;
            if (kaynak.TryGetValue(k, out v) && v.Trim().Length > 0) hedef[h] = v.Trim();
        }

        static Dictionary<string, string> IniOku(string yol)
        {
            var d = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            string bolum = "";
            foreach (var ham in File.ReadAllLines(yol, Encoding.GetEncoding(1254)))
            {
                var s = ham.Trim();
                if (s.StartsWith("[") && s.EndsWith("]")) { bolum = s.Substring(1, s.Length - 2); continue; }
                int i = s.IndexOf('=');
                if (i <= 0 || s.StartsWith("#") || s.StartsWith(";")) continue;
                d[bolum + "." + s.Substring(0, i).Trim()] = s.Substring(i + 1).Trim();
            }
            return d;
        }
    }

    internal static class PdksVeri
    {
        public static List<Firma> Firmalar(Db db)
        {
            return db.Sorgu("SELECT id, ad FROM firma ORDER BY ad").Select(r => new Firma { Id = int.Parse(r[0]), Ad = (r[1] ?? "") + " (" + r[0] + ")" }).ToList();
        }

        public static List<Personel> Personeller(Db db, int? firmaId)
        {
            string sql = "SELECT id, kartno, ad, soyad, sicilno, kimlikno, firmaid, grupid, isegiristarihi, istencikistarihi, ucret, " +
                         (db.KolonVar("personel", "sgk_no") ? "sgk_no" : "NULL") + ", durum FROM personel";
            if (firmaId.HasValue) sql += " WHERE firmaid = " + firmaId.Value;
            return db.Sorgu(sql).Select(r => new Personel
            {
                Id = long.Parse(r[0]), KartNo = r[1], Ad = r[2], Soyad = r[3], SicilNo = r[4], KimlikNo = r[5],
                FirmaId = Int(r[6]), GrupId = Int(r[7]), IseGiris = Trh(r[8]), IstenCikis = Trh(r[9]),
                Ucret = Tablo.Sayi(r[10]), SgkNo = r[11], Durum = r[12]
            }).ToList();
        }

        static int? Int(string s) { int i; return int.TryParse(s, out i) ? i : (int?)null; }

        public static DateTime? Trh(string s)
        {
            DateTime t;
            if (s == null || s.StartsWith("0000")) return null;
            if (s.Length > 10) s = s.Substring(0, 10);
            return DateTime.TryParseExact(s, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out t) ? t : (DateTime?)null;
        }

        // Yeni personel eklenirken, firmada en çok kullanılan değeri öneri olarak al (grup, durum).
        public static string EnCok(Db db, string kolon, int? firmaId)
        {
            string sql = "SELECT " + kolon + ", COUNT(*) n FROM personel WHERE " + kolon + " IS NOT NULL AND (istencikistarihi IS NULL OR istencikistarihi >= CURDATE())" +
                         (firmaId.HasValue ? " AND firmaid = " + firmaId.Value : "") + " GROUP BY " + kolon + " ORDER BY n DESC LIMIT 1";
            var r = db.Sorgu(sql);
            return r.Count > 0 ? r[0][0] : null;
        }

        public static string GrupAdi(Db db, string grupId)
        {
            if (grupId == null) return "(yok)";
            var r = db.Sorgu("SELECT ad FROM grup WHERE id = " + int.Parse(grupId));
            return r.Count > 0 ? r[0][0] + " (" + grupId + ")" : grupId;
        }
    }

    // Değişiklikleri geri almak için SQL dosyası. Her işlemden önce yazılır.
    internal sealed class GeriAlma : IDisposable
    {
        readonly StreamWriter _w;
        public readonly string Yol;
        public int Sayi;

        public GeriAlma(string islem, string[] baslik)
        {
            string klasor = Path.Combine(Ayarlar.Klasor, "GeriAlma");
            Directory.CreateDirectory(klasor);
            Yol = Path.Combine(klasor, DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_" + islem + ".sql");
            _w = new StreamWriter(Yol, false, new UTF8Encoding(false));
            _w.WriteLine("-- Biltek Muhasebe Köprüsü geri alma dosyası");
            _w.WriteLine("-- İşlem: " + islem + "   Tarih: " + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"));
            foreach (var b in baslik) _w.WriteLine(b);
            _w.Flush();
        }

        public void Ekle(string sql) { _w.WriteLine(sql); _w.Flush(); Sayi++; }
        public void Dispose() { _w.Dispose(); }
    }
}
