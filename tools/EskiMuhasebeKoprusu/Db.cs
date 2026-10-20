using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace BiltekMuhasebeKoprusu
{
    // Eski Biltek PDKS'nin kullandığı libmySQL.dll (32 bit) üzerinden MySQL erişimi.
    internal static class MySqlNative
    {
        const string Dll = "libmySQL.dll";
        [DllImport(Dll, CallingConvention = CallingConvention.StdCall)] public static extern IntPtr mysql_init(IntPtr m);
        [DllImport(Dll, CallingConvention = CallingConvention.StdCall)] public static extern IntPtr mysql_real_connect(IntPtr m, string host, string user, string pass, string db, uint port, IntPtr sock, uint flags);
        [DllImport(Dll, CallingConvention = CallingConvention.StdCall)] public static extern int mysql_set_character_set(IntPtr m, string cs);
        [DllImport(Dll, CallingConvention = CallingConvention.StdCall)] public static extern int mysql_real_query(IntPtr m, byte[] q, uint len);
        [DllImport(Dll, CallingConvention = CallingConvention.StdCall)] public static extern IntPtr mysql_store_result(IntPtr m);
        [DllImport(Dll, CallingConvention = CallingConvention.StdCall)] public static extern uint mysql_num_fields(IntPtr r);
        [DllImport(Dll, CallingConvention = CallingConvention.StdCall)] public static extern IntPtr mysql_fetch_row(IntPtr r);
        [DllImport(Dll, CallingConvention = CallingConvention.StdCall)] public static extern IntPtr mysql_fetch_lengths(IntPtr r);
        [DllImport(Dll, CallingConvention = CallingConvention.StdCall)] public static extern void mysql_free_result(IntPtr r);
        [DllImport(Dll, CallingConvention = CallingConvention.StdCall)] public static extern IntPtr mysql_error(IntPtr m);
        [DllImport(Dll, CallingConvention = CallingConvention.StdCall)] public static extern ulong mysql_affected_rows(IntPtr m);
        [DllImport(Dll, CallingConvention = CallingConvention.StdCall)] public static extern ulong mysql_insert_id(IntPtr m);
        [DllImport(Dll, CallingConvention = CallingConvention.StdCall)] public static extern void mysql_close(IntPtr m);
    }

    internal sealed class Db : IDisposable
    {
        // Eski tablolar latin5 (Türkçe) karakter setinde.
        public static readonly Encoding Kodlama = Encoding.GetEncoding(28599);
        IntPtr _m;

        public Db(string sunucu, int port, string veritabani, string kullanici, string sifre)
        {
            _m = MySqlNative.mysql_init(IntPtr.Zero);
            if (_m == IntPtr.Zero) throw new Exception("MySQL istemcisi başlatılamadı (libmySQL.dll).");
            if (MySqlNative.mysql_real_connect(_m, sunucu, kullanici, sifre, veritabani, (uint)port, IntPtr.Zero, 0) == IntPtr.Zero)
            {
                string h = Hata();
                MySqlNative.mysql_close(_m);
                _m = IntPtr.Zero;
                throw new Exception("Veritabanına bağlanılamadı: " + h);
            }
            MySqlNative.mysql_set_character_set(_m, "latin5");
        }

        string Hata()
        {
            IntPtr p = MySqlNative.mysql_error(_m);
            return p == IntPtr.Zero ? "" : Marshal.PtrToStringAnsi(p);
        }

        void Calistir(string sql)
        {
            byte[] b = Kodlama.GetBytes(sql);
            if (MySqlNative.mysql_real_query(_m, b, (uint)b.Length) != 0)
                throw new Exception("SQL hatası: " + Hata() + "\r\n" + (sql.Length > 300 ? sql.Substring(0, 300) : sql));
        }

        public List<string[]> Sorgu(string sql)
        {
            Calistir(sql);
            var liste = new List<string[]>();
            IntPtr r = MySqlNative.mysql_store_result(_m);
            if (r == IntPtr.Zero) return liste;
            try
            {
                int n = (int)MySqlNative.mysql_num_fields(r);
                IntPtr satir;
                while ((satir = MySqlNative.mysql_fetch_row(r)) != IntPtr.Zero)
                {
                    IntPtr uz = MySqlNative.mysql_fetch_lengths(r);
                    var a = new string[n];
                    for (int i = 0; i < n; i++)
                    {
                        IntPtr hucre = Marshal.ReadIntPtr(satir, i * IntPtr.Size);
                        if (hucre == IntPtr.Zero) { a[i] = null; continue; }
                        int len = Marshal.ReadInt32(uz, i * 4);
                        var buf = new byte[len];
                        Marshal.Copy(hucre, buf, 0, len);
                        a[i] = Kodlama.GetString(buf);
                    }
                    liste.Add(a);
                }
            }
            finally { MySqlNative.mysql_free_result(r); }
            return liste;
        }

        public long Yaz(string sql)
        {
            Calistir(sql);
            IntPtr r = MySqlNative.mysql_store_result(_m);
            if (r != IntPtr.Zero) MySqlNative.mysql_free_result(r);
            return (long)MySqlNative.mysql_affected_rows(_m);
        }

        public long SonId() { return (long)MySqlNative.mysql_insert_id(_m); }

        public bool KolonVar(string tablo, string kolon)
        {
            var s = Sorgu("SHOW COLUMNS FROM `" + tablo + "` LIKE " + Q(kolon));
            return s.Count > 0;
        }

        public bool TabloVar(string tablo)
        {
            return Sorgu("SHOW TABLES LIKE " + Q(tablo)).Count > 0;
        }

        public static string Q(string s)
        {
            if (s == null) return "NULL";
            var sb = new StringBuilder("'");
            foreach (char c in s)
            {
                switch (c)
                {
                    case '\\': sb.Append("\\\\"); break;
                    case '\'': sb.Append("\\'"); break;
                    case '"': sb.Append("\\\""); break;
                    case '\0': sb.Append("\\0"); break;
                    case '\n': sb.Append("\\n"); break;
                    case '\r': sb.Append("\\r"); break;
                    case '\x1a': sb.Append("\\Z"); break;
                    default: sb.Append(c); break;
                }
            }
            return sb.Append('\'').ToString();
        }

        public static string QTarih(DateTime? t)
        {
            return t.HasValue ? "'" + t.Value.ToString("yyyy-MM-dd") + "'" : "NULL";
        }

        public static string QSayi(decimal? d)
        {
            return d.HasValue ? d.Value.ToString(CultureInfo.InvariantCulture) : "NULL";
        }

        public void Dispose()
        {
            if (_m != IntPtr.Zero) { MySqlNative.mysql_close(_m); _m = IntPtr.Zero; }
        }
    }
}
