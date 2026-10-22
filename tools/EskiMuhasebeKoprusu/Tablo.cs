using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace BiltekMuhasebeKoprusu
{
    // Basit tablo: ilk satır başlık. Excel (.xlsx), CSV ve IKYS düz metin listesi okunur.
    internal sealed class Tablo
    {
        public List<string> Basliklar = new List<string>();
        public List<string[]> Satirlar = new List<string[]>();

        public static Tablo Oku(string yol)
        {
            string uz = Path.GetExtension(yol).ToLowerInvariant();
            if (uz == ".xlsx") return XlsxOku(yol);
            if (uz == ".xls") throw new Exception("Eski .xls biçimi okunamıyor. Dosyayı Excel'de açıp 'Farklı Kaydet > Excel Çalışma Kitabı (.xlsx)' ile kaydedin.");
            string metin = MetinOku(yol);
            var ikys = DuzListeOku(metin);
            if (ikys != null) return ikys;
            return CsvOku(metin);
        }

        static string MetinOku(string yol)
        {
            byte[] b = File.ReadAllBytes(yol);
            try { return new UTF8Encoding(false, true).GetString(b).TrimStart('﻿'); }
            catch (DecoderFallbackException) { return Encoding.GetEncoding(1254).GetString(b); }
        }

        // "1     AHMET YILMAZ        12345678901" biçimindeki liste.
        static Tablo DuzListeOku(string metin)
        {
            var rx = new Regex(@"^\s*(\d+)\s+(.+?)\s+(\d{11})\s*$");
            var t = new Tablo();
            t.Basliklar.AddRange(new[] { "SIRA", "AD SOYAD", "TC KİMLİK NO" });
            foreach (var satir in metin.Split('\n'))
            {
                var m = rx.Match(satir.TrimEnd('\r'));
                if (m.Success) t.Satirlar.Add(new[] { m.Groups[1].Value, m.Groups[2].Value.Trim(), m.Groups[3].Value });
            }
            return t.Satirlar.Count >= 1 && !metin.Contains(";") ? t : null;
        }

        static Tablo CsvOku(string metin)
        {
            var satirlar = metin.Replace("\r\n", "\n").Split('\n').Where(s => s.Trim().Length > 0).ToList();
            if (satirlar.Count == 0) throw new Exception("Dosya boş.");
            char ayrac = satirlar[0].Count(c => c == ';') >= satirlar[0].Count(c => c == ',') ? ';' : ',';
            if (satirlar[0].IndexOf('\t') >= 0) ayrac = '\t';
            var t = new Tablo();
            t.Basliklar.AddRange(CsvBol(satirlar[0], ayrac).Select(s => s.Trim()));
            for (int i = 1; i < satirlar.Count; i++) t.Satirlar.Add(CsvBol(satirlar[i], ayrac));
            return t;
        }

        static string[] CsvBol(string s, char ayrac)
        {
            var l = new List<string>();
            var sb = new StringBuilder();
            bool tirnak = false;
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                if (tirnak)
                {
                    if (c == '"' && i + 1 < s.Length && s[i + 1] == '"') { sb.Append('"'); i++; }
                    else if (c == '"') tirnak = false;
                    else sb.Append(c);
                }
                else if (c == '"') tirnak = true;
                else if (c == ayrac) { l.Add(sb.ToString()); sb.Clear(); }
                else sb.Append(c);
            }
            l.Add(sb.ToString());
            return l.ToArray();
        }

        static Tablo XlsxOku(string yol)
        {
            using (var fs = new FileStream(yol, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var zip = new ZipArchive(fs, ZipArchiveMode.Read))
            {
                var paylasilan = new List<string>();
                var ss = zip.GetEntry("xl/sharedStrings.xml");
                if (ss != null)
                {
                    var d = new XmlDocument();
                    using (var st = ss.Open()) d.Load(st);
                    foreach (XmlNode si in d.GetElementsByTagName("si"))
                    {
                        var sb = new StringBuilder();
                        foreach (XmlNode tn in ((XmlElement)si).GetElementsByTagName("t")) sb.Append(tn.InnerText);
                        paylasilan.Add(sb.ToString());
                    }
                }
                var sayfa = zip.GetEntry("xl/worksheets/sheet1.xml") ?? zip.Entries.First(e => e.FullName.StartsWith("xl/worksheets/sheet"));
                var doc = new XmlDocument();
                using (var st = sayfa.Open()) doc.Load(st);
                var ham = new List<Dictionary<int, string>>();
                foreach (XmlNode row in doc.GetElementsByTagName("row"))
                {
                    var hucreler = new Dictionary<int, string>();
                    int sira = 0;
                    foreach (XmlNode c in row.ChildNodes)
                    {
                        if (c.LocalName != "c") continue;
                        var e = (XmlElement)c;
                        int kol = e.HasAttribute("r") ? KolonNo(e.GetAttribute("r")) : sira;
                        sira = kol + 1;
                        string tip = e.GetAttribute("t");
                        string deger = null;
                        if (tip == "inlineStr")
                        {
                            var sb = new StringBuilder();
                            foreach (XmlNode tn in e.GetElementsByTagName("t")) sb.Append(tn.InnerText);
                            deger = sb.ToString();
                        }
                        else
                        {
                            var v = e.GetElementsByTagName("v");
                            if (v.Count > 0)
                            {
                                deger = v[0].InnerText;
                                if (tip == "s") { int idx; if (int.TryParse(deger, out idx) && idx < paylasilan.Count) deger = paylasilan[idx]; }
                            }
                        }
                        if (deger != null) hucreler[kol] = deger;
                    }
                    ham.Add(hucreler);
                }
                var t = new Tablo();
                int basSatir = ham.FindIndex(h => h.Count >= 2);
                if (basSatir < 0) throw new Exception("Excel sayfası boş.");
                int maxKol = ham.Where(h => h.Count > 0).Max(h => h.Keys.Max()) + 1;
                for (int k = 0; k < maxKol; k++) { string b; t.Basliklar.Add(ham[basSatir].TryGetValue(k, out b) ? b.Trim() : ""); }
                for (int i = basSatir + 1; i < ham.Count; i++)
                {
                    if (ham[i].Count == 0) continue;
                    var a = new string[maxKol];
                    foreach (var kv in ham[i]) if (kv.Key < maxKol) a[kv.Key] = kv.Value;
                    t.Satirlar.Add(a);
                }
                return t;
            }
        }

        static int KolonNo(string referans)
        {
            int n = 0;
            foreach (char c in referans)
            {
                if (c >= 'A' && c <= 'Z') n = n * 26 + (c - 'A' + 1);
                else break;
            }
            return n - 1;
        }

        public int KolonBul(params string[] adaylar)
        {
            for (int i = 0; i < Basliklar.Count; i++)
            {
                string b = Metin.Anahtar(Basliklar[i]);
                foreach (var a in adaylar) if (b == Metin.Anahtar(a)) return i;
            }
            for (int i = 0; i < Basliklar.Count; i++)
            {
                string b = Metin.Anahtar(Basliklar[i]);
                foreach (var a in adaylar) if (b.Contains(Metin.Anahtar(a))) return i;
            }
            return -1;
        }

        public static string Hucre(string[] satir, int kol)
        {
            if (kol < 0 || satir == null || kol >= satir.Length) return null;
            var s = satir[kol];
            return s == null ? null : s.Trim();
        }

        // Excel tarihleri seri sayı (45291) ya da metin (31.12.2025) olabilir.
        public static DateTime? Tarih(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;
            double seri;
            if (double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out seri) && seri > 20000 && seri < 80000)
                return DateTime.FromOADate(seri).Date;
            DateTime t;
            string[] bicimler = { "dd.MM.yyyy", "d.M.yyyy", "dd/MM/yyyy", "d/M/yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "dd.MM.yyyy HH:mm:ss", "yyyy-MM-dd HH:mm:ss" };
            if (DateTime.TryParseExact(s.Trim(), bicimler, CultureInfo.InvariantCulture, DateTimeStyles.None, out t)) return t.Date;
            return null;
        }

        public static decimal? Sayi(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;
            s = s.Trim().Replace(" ", "").Replace("TL", "").Replace("₺", "");
            decimal d;
            if (s.Contains(",") && s.Contains("."))
            {
                if (s.LastIndexOf(',') > s.LastIndexOf('.')) s = s.Replace(".", "").Replace(",", ".");
                else s = s.Replace(",", "");
            }
            else if (s.Contains(",")) s = s.Replace(",", ".");
            if (decimal.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out d)) return Math.Round(d, 2);
            return null;
        }
    }

    // En basit .xlsx yazıcı: tek sayfa, ilk satır kalın başlık, sayılar sayı olarak.
    internal static class XlsxYazici
    {
        public static void Yaz(string yol, string sayfaAdi, IList<string> basliklar, IEnumerable<object[]> satirlar, string ustSatir = null)
        {
            if (File.Exists(yol)) File.Delete(yol);
            using (var zip = ZipFile.Open(yol, ZipArchiveMode.Create))
            {
                Ekle(zip, "[Content_Types].xml", "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?><Types xmlns=\"http://schemas.openxmlformats.org/package/2006/content-types\"><Default Extension=\"rels\" ContentType=\"application/vnd.openxmlformats-package.relationships+xml\"/><Default Extension=\"xml\" ContentType=\"application/xml\"/><Override PartName=\"/xl/workbook.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml\"/><Override PartName=\"/xl/worksheets/sheet1.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml\"/><Override PartName=\"/xl/styles.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.styles+xml\"/></Types>");
                Ekle(zip, "_rels/.rels", "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?><Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\"><Relationship Id=\"rId1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument\" Target=\"xl/workbook.xml\"/></Relationships>");
                Ekle(zip, "xl/workbook.xml", "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?><workbook xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\" xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\"><sheets><sheet name=\"" + Xml(sayfaAdi.Length > 31 ? sayfaAdi.Substring(0, 31) : sayfaAdi) + "\" sheetId=\"1\" r:id=\"rId1\"/></sheets></workbook>");
                Ekle(zip, "xl/_rels/workbook.xml.rels", "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?><Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\"><Relationship Id=\"rId1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet\" Target=\"worksheets/sheet1.xml\"/><Relationship Id=\"rId2\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles\" Target=\"styles.xml\"/></Relationships>");
                Ekle(zip, "xl/styles.xml", "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?><styleSheet xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\"><fonts count=\"2\"><font><sz val=\"11\"/><name val=\"Calibri\"/></font><font><b/><sz val=\"11\"/><name val=\"Calibri\"/></font></fonts><fills count=\"3\"><fill><patternFill patternType=\"none\"/></fill><fill><patternFill patternType=\"gray125\"/></fill><fill><patternFill patternType=\"solid\"><fgColor rgb=\"FFDDEBF7\"/></patternFill></fill></fills><borders count=\"1\"><border/></borders><cellStyleXfs count=\"1\"><xf/></cellStyleXfs><cellXfs count=\"3\"><xf/><xf fontId=\"1\" fillId=\"2\" applyFont=\"1\" applyFill=\"1\"/><xf numFmtId=\"49\" applyNumberFormat=\"1\"/></cellXfs></styleSheet>");
                var sb = new StringBuilder();
                sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?><worksheet xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\"><sheetViews><sheetView workbookViewId=\"0\"><pane ySplit=\"" + (string.IsNullOrEmpty(ustSatir) ? 1 : 2) + "\" topLeftCell=\"A" + (string.IsNullOrEmpty(ustSatir) ? 2 : 3) + "\" activePane=\"bottomLeft\" state=\"frozen\"/></sheetView></sheetViews><cols>");
                for (int i = 0; i < basliklar.Count; i++) sb.Append("<col min=\"" + (i + 1) + "\" max=\"" + (i + 1) + "\" width=\"" + Math.Max(10, Math.Min(40, basliklar[i].Length + 4)) + "\" customWidth=\"1\"/>");
                sb.Append("</cols><sheetData>");
                int r = 1;
                if (!string.IsNullOrEmpty(ustSatir))
                {
                    sb.Append("<row r=\"1\"><c r=\"A1\" t=\"inlineStr\"><is><t>" + Xml(ustSatir) + "</t></is></c></row>");
                    r = 2;
                }
                sb.Append("<row r=\"" + r + "\">");
                for (int i = 0; i < basliklar.Count; i++) sb.Append("<c r=\"" + Ref(i, r) + "\" t=\"inlineStr\" s=\"1\"><is><t>" + Xml(basliklar[i]) + "</t></is></c>");
                sb.Append("</row>");
                r++;
                foreach (var satir in satirlar)
                {
                    sb.Append("<row r=\"" + r + "\">");
                    for (int i = 0; i < satir.Length; i++)
                    {
                        object v = satir[i];
                        if (v == null) continue;
                        if (v is decimal || v is int || v is long || v is double)
                            sb.Append("<c r=\"" + Ref(i, r) + "\"><v>" + Convert.ToDecimal(v).ToString(CultureInfo.InvariantCulture) + "</v></c>");
                        else
                            // TC ve sicil gibi baştaki sıfırı korunması gereken alanlar metin olarak yazılır.
                            sb.Append("<c r=\"" + Ref(i, r) + "\" t=\"inlineStr\" s=\"2\"><is><t xml:space=\"preserve\">" + Xml(v.ToString()) + "</t></is></c>");
                    }
                    sb.Append("</row>");
                    r++;
                }
                sb.Append("</sheetData></worksheet>");
                Ekle(zip, "xl/worksheets/sheet1.xml", sb.ToString());
            }
        }

        static void Ekle(ZipArchive zip, string ad, string icerik)
        {
            var e = zip.CreateEntry(ad, CompressionLevel.Optimal);
            using (var s = e.Open())
            {
                var b = new UTF8Encoding(false).GetBytes(icerik);
                s.Write(b, 0, b.Length);
            }
        }

        static string Ref(int kol, int satir)
        {
            string s = "";
            int k = kol + 1;
            while (k > 0) { int m = (k - 1) % 26; s = (char)('A' + m) + s; k = (k - 1) / 26; }
            return s + satir;
        }

        static string Xml(string s)
        {
            var sb = new StringBuilder();
            foreach (char c in s)
            {
                if (c == '<') sb.Append("&lt;");
                else if (c == '>') sb.Append("&gt;");
                else if (c == '&') sb.Append("&amp;");
                else if (c == '"') sb.Append("&quot;");
                else if (c < 0x20 && c != '\t' && c != '\n' && c != '\r') continue;
                else sb.Append(c);
            }
            return sb.ToString();
        }
    }

    internal static class Metin
    {
        static readonly CultureInfo Tr = new CultureInfo("tr-TR");

        public static string Buyuk(string s) { return s == null ? "" : s.Trim().ToUpper(Tr); }

        // Karşılaştırma anahtarı: Türkçe harfler sadeleşir, boşluk/noktalama atılır.
        public static string Anahtar(string s)
        {
            if (s == null) return "";
            s = Buyuk(s);
            var sb = new StringBuilder();
            foreach (char c in s)
            {
                switch (c)
                {
                    case 'İ': case 'I': sb.Append('I'); break;
                    case 'Ş': sb.Append('S'); break;
                    case 'Ğ': sb.Append('G'); break;
                    case 'Ü': sb.Append('U'); break;
                    case 'Ö': sb.Append('O'); break;
                    case 'Ç': sb.Append('C'); break;
                    case 'Â': sb.Append('A'); break;
                    case 'Î': sb.Append('I'); break;
                    case 'Û': sb.Append('U'); break;
                    default: if (char.IsLetterOrDigit(c)) sb.Append(c); break;
                }
            }
            return sb.ToString();
        }

        // UTF-8 olarak yazılıp Türkçe (1254) okunan bozuk metni onarır: "YÄ±llÄ±k" -> "Yıllık"
        public static string Onar(string s)
        {
            if (string.IsNullOrEmpty(s) || (s.IndexOf('Ã') < 0 && s.IndexOf('Ä') < 0 && s.IndexOf('Å') < 0)) return s;
            try
            {
                var b = Encoding.GetEncoding(1254).GetBytes(s);
                return new UTF8Encoding(false, true).GetString(b);
            }
            catch { return s; }
        }

        public static string SadeceRakam(string s)
        {
            if (s == null) return "";
            var sb = new StringBuilder();
            foreach (char c in s) if (c >= '0' && c <= '9') sb.Append(c);
            return sb.ToString();
        }

        public static bool TcGecerli(string tc)
        {
            if (tc == null || tc.Length != 11 || tc[0] == '0') return false;
            var d = new int[11];
            for (int i = 0; i < 11; i++) { if (tc[i] < '0' || tc[i] > '9') return false; d[i] = tc[i] - '0'; }
            int tek = d[0] + d[2] + d[4] + d[6] + d[8], cift = d[1] + d[3] + d[5] + d[7];
            if (((tek * 7 - cift) % 10 + 10) % 10 != d[9]) return false;
            int top = 0;
            for (int i = 0; i < 10; i++) top += d[i];
            return top % 10 == d[10];
        }

        // "AHMET CAN ÖZTÜRK" -> ad "AHMET CAN", soyad "ÖZTÜRK"
        public static void AdSoyadBol(string adSoyad, out string ad, out string soyad)
        {
            var p = Buyuk(adSoyad).Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (p.Length <= 1) { ad = p.Length == 1 ? p[0] : ""; soyad = ""; return; }
            soyad = p[p.Length - 1];
            ad = string.Join(" ", p, 0, p.Length - 1);
        }
    }
}
