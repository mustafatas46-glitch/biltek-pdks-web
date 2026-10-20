using System;
using System.IO;
using System.Linq;
using System.Text;
using BiltekMuhasebeKoprusu;

// BilTek (Firebird) veritabanının tablo ve alanlarını sema_raporu.txt dosyasına yazar. Veritabanına yazmaz.
static class FbSema
{
    static int Main(string[] args)
    {
        string yol = args.Length > 0 ? args[0] : @"127.0.0.1:C:\BilTek3-4-5-6-7-8-510\BilTek510\Data\DATABASE.GDB";
        string cikti = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sema_raporu.txt");
        var w = new StreamWriter(cikti, false, new UTF8Encoding(false));
        try
        {
            w.WriteLine("Veritabani: " + yol);
            using (var db = new Fb(yol, "SYSDBA", args.Length > 1 ? args[1] : "masterkey"))
            {
                w.WriteLine("Istemci: " + Fb.DllYolu);
                var dial = db.Sorgu("SELECT MON$SQL_DIALECT, MON$ODS_MAJOR, MON$ODS_MINOR FROM MON$DATABASE");
                if (dial.Count > 0) w.WriteLine("Dialect: " + dial[0][0] + "  ODS: " + dial[0][1] + "." + dial[0][2]);
                var tablolar = db.Sorgu("SELECT TRIM(RDB$RELATION_NAME), RDB$VIEW_BLR IS NOT NULL FROM RDB$RELATIONS WHERE COALESCE(RDB$SYSTEM_FLAG,0)=0 ORDER BY 1");
                w.WriteLine("Tablo sayisi: " + tablolar.Count);
                foreach (var t in tablolar)
                {
                    string ad = t[0];
                    string sayi = "?";
                    try { sayi = db.Sorgu("SELECT COUNT(*) FROM \"" + ad + "\"")[0][0]; } catch (Exception ex) { sayi = "hata: " + ex.Message; }
                    w.WriteLine();
                    w.WriteLine("### " + ad + (t[1] == "1" ? " (VIEW)" : "") + "  kayit: " + sayi);
                    var alanlar = db.Sorgu("SELECT TRIM(rf.RDB$FIELD_NAME), f.RDB$FIELD_TYPE, f.RDB$FIELD_LENGTH, f.RDB$FIELD_SCALE, f.RDB$FIELD_SUB_TYPE, rf.RDB$NULL_FLAG " +
                        "FROM RDB$RELATION_FIELDS rf JOIN RDB$FIELDS f ON f.RDB$FIELD_NAME = rf.RDB$FIELD_SOURCE WHERE rf.RDB$RELATION_NAME = " + Fb.Q(ad) + " ORDER BY rf.RDB$FIELD_POSITION");
                    w.WriteLine(string.Join(", ", alanlar.Select(a => a[0] + ":" + Tip(a[1], a[2], a[3], a[4]) + (a[5] == "1" ? "!" : ""))));
                    try
                    {
                        var ornek = db.Sorgu("SELECT FIRST 2 * FROM \"" + ad + "\"");
                        foreach (var r in ornek)
                            w.WriteLine("  ornek: " + string.Join(" | ", r.Select((v, i) => Gizle(i < alanlar.Count ? alanlar[i][0] : "", v))));
                    }
                    catch (Exception ex) { w.WriteLine("  ornek okunamadi: " + ex.Message); }
                }
                var pk = db.Sorgu("SELECT TRIM(rc.RDB$RELATION_NAME), TRIM(s.RDB$FIELD_NAME), TRIM(rc.RDB$CONSTRAINT_TYPE) FROM RDB$RELATION_CONSTRAINTS rc JOIN RDB$INDEX_SEGMENTS s ON s.RDB$INDEX_NAME = rc.RDB$INDEX_NAME WHERE rc.RDB$CONSTRAINT_TYPE IN ('PRIMARY KEY','UNIQUE') ORDER BY 1");
                w.WriteLine(); w.WriteLine("### ANAHTARLAR");
                foreach (var r in pk) w.WriteLine(r[0] + "." + r[1] + " " + r[2]);
                var gen = db.Sorgu("SELECT TRIM(RDB$GENERATOR_NAME) FROM RDB$GENERATORS WHERE COALESCE(RDB$SYSTEM_FLAG,0)=0");
                w.WriteLine(); w.WriteLine("### GENERATORLER: " + string.Join(", ", gen.Select(g => g[0])));
                var trg = db.Sorgu("SELECT TRIM(RDB$TRIGGER_NAME), TRIM(RDB$RELATION_NAME), RDB$TRIGGER_TYPE FROM RDB$TRIGGERS WHERE COALESCE(RDB$SYSTEM_FLAG,0)=0");
                w.WriteLine(); w.WriteLine("### TRIGGERLAR"); foreach (var r in trg) w.WriteLine(string.Join(" ", r));
            }
            w.WriteLine(); w.WriteLine("TAMAM");
            return 0;
        }
        catch (Exception ex) { w.WriteLine("HATA: " + ex); return 1; }
        finally { w.Dispose(); }
    }

    static string Tip(string t, string len, string scale, string sub)
    {
        switch (t)
        {
            case "7": return scale != "0" ? "NUMERIC(4," + (-int.Parse(scale)) + ")" : "SMALLINT";
            case "8": return scale != "0" ? "NUMERIC(9," + (-int.Parse(scale)) + ")" : "INTEGER";
            case "16": return scale != "0" ? "NUMERIC(18," + (-int.Parse(scale)) + ")" : "BIGINT";
            case "10": return "FLOAT";
            case "27": return "DOUBLE";
            case "12": return "DATE";
            case "13": return "TIME";
            case "35": return "TIMESTAMP";
            case "14": return "CHAR(" + len + ")";
            case "37": return "VARCHAR(" + len + ")";
            case "261": return "BLOB" + (sub == "1" ? "TEXT" : "");
            default: return "T" + t;
        }
    }

    // Kişisel verileri rapora açık yazma: TC, telefon, adres, isim kısmen gizlenir.
    static string Gizle(string alan, string v)
    {
        if (v == null) return "null";
        if (v.Length > 40) v = v.Substring(0, 40) + "…";
        string a = alan.ToUpperInvariant();
        if (a.Contains("KIMLIK") || a.Contains("TC") || a.Contains("SSK") || a.Contains("SIGORTA") || a.Contains("VKN") || a.Contains("UKN") || a.Contains("TEL") || a.Contains("GSM") || a.Contains("ADRES") || a.Contains("HESAP") || a.Contains("IBAN") || a.Contains("BHN") || a.Contains("PASS") || a.Contains("SIFRE"))
            return v.Length <= 2 ? "**" : v.Substring(0, 2) + new string('*', Math.Max(0, v.Length - 4)) + (v.Length > 3 ? v.Substring(v.Length - 2) : "") + " (" + v.Length + ")";
        if (a == "AD" || a == "SOYAD" || a.EndsWith("ADI") || a.Contains("ANAAD") || a.Contains("BABAAD"))
            return v.Length <= 1 ? v : v.Substring(0, 1) + "***";
        return v;
    }
}
