using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace BiltekMuhasebeKoprusu
{
    // BilTek 3xx-5xx programlarının kullandığı Firebird/InterBase veritabanına fbclient.dll (gds32.dll) ile erişim.
    internal sealed class Fb : IDisposable
    {
        public static readonly Encoding Kodlama = Encoding.GetEncoding(1254);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)] delegate IntPtr d_attach(IntPtr[] st, short len, byte[] db, ref uint h, short dpbLen, byte[] dpb);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] delegate IntPtr d_detach(IntPtr[] st, ref uint h);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] delegate IntPtr d_start_multiple(IntPtr[] st, ref uint tr, short count, IntPtr teb);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] delegate IntPtr d_tr(IntPtr[] st, ref uint tr);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] delegate IntPtr d_alloc(IntPtr[] st, ref uint db, ref uint stmt);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] delegate IntPtr d_prepare(IntPtr[] st, ref uint tr, ref uint stmt, ushort len, byte[] sql, ushort dialect, IntPtr sqlda);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] delegate IntPtr d_describe(IntPtr[] st, ref uint stmt, ushort ver, IntPtr sqlda);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] delegate IntPtr d_execute(IntPtr[] st, ref uint tr, ref uint stmt, ushort ver, IntPtr sqlda);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] delegate IntPtr d_fetch(IntPtr[] st, ref uint stmt, ushort ver, IntPtr sqlda);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] delegate IntPtr d_free(IntPtr[] st, ref uint stmt, ushort opt);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] delegate int d_interpret(StringBuilder buf, uint len, ref IntPtr vec);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] delegate int d_interprete(StringBuilder buf, ref IntPtr vec);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)] static extern IntPtr LoadLibrary(string yol);
        [DllImport("kernel32.dll", CharSet = CharSet.Ansi)] static extern IntPtr GetProcAddress(IntPtr h, string ad);

        static IntPtr _lib;
        static d_attach attach; static d_detach detach; static d_start_multiple startMultiple; static d_tr commit, rollback;
        static d_alloc alloc; static d_prepare prepare; static d_describe describe; static d_execute execute; static d_fetch fetch; static d_free free;
        static d_interpret interpret; static d_interprete interprete;

        public static string DllYolu;

        static void Yukle()
        {
            if (_lib != IntPtr.Zero) return;
            var adaylar = new List<string>
            {
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "fbclient.dll"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "gds32.dll"),
                "fbclient.dll", "gds32.dll"
            };
            foreach (var pf in new[] { Environment.GetEnvironmentVariable("ProgramFiles(x86)"), Environment.GetEnvironmentVariable("ProgramFiles") })
            {
                if (string.IsNullOrEmpty(pf)) continue;
                foreach (var v in new[] { "Firebird_2_5", "Firebird_3_0", "Firebird_2_1", "Firebird_2_0", "Firebird_1_5" })
                    adaylar.Add(Path.Combine(pf, "Firebird", v, "bin", "fbclient.dll"));
                adaylar.Add(Path.Combine(pf, "Firebird", "bin", "fbclient.dll"));
            }
            foreach (var a in adaylar)
            {
                if (a.Contains("\\") && !File.Exists(a)) continue;
                _lib = LoadLibrary(a);
                if (_lib != IntPtr.Zero) { DllYolu = a; break; }
            }
            if (_lib == IntPtr.Zero) throw new Exception("Firebird istemcisi (fbclient.dll / gds32.dll) bulunamadı. Firebird kurulu olmalı ya da fbclient.dll bu programın yanına kopyalanmalı.");
            attach = F<d_attach>("isc_attach_database"); detach = F<d_detach>("isc_detach_database");
            startMultiple = F<d_start_multiple>("isc_start_multiple"); commit = F<d_tr>("isc_commit_transaction"); rollback = F<d_tr>("isc_rollback_transaction");
            alloc = F<d_alloc>("isc_dsql_allocate_statement"); prepare = F<d_prepare>("isc_dsql_prepare"); describe = F<d_describe>("isc_dsql_describe");
            execute = F<d_execute>("isc_dsql_execute"); fetch = F<d_fetch>("isc_dsql_fetch"); free = F<d_free>("isc_dsql_free_statement");
            if (GetProcAddress(_lib, "fb_interpret") != IntPtr.Zero) interpret = F<d_interpret>("fb_interpret");
            else interprete = F<d_interprete>("isc_interprete");
        }

        static T F<T>(string ad) where T : class
        {
            IntPtr p = GetProcAddress(_lib, ad);
            if (p == IntPtr.Zero) throw new Exception("Firebird istemcisinde " + ad + " yok (" + DllYolu + ").");
            return Marshal.GetDelegateForFunctionPointer(p, typeof(T)) as T;
        }

        uint _db, _tr;
        readonly IntPtr[] _st = new IntPtr[20];
        public ushort Dialect = 3;

        public Fb(string yol, string kullanici, string sifre)
        {
            Yukle();
            var dpb = new List<byte> { 1 };
            Ekle(dpb, 28, kullanici);
            Ekle(dpb, 29, sifre);
            Ekle(dpb, 48, "WIN1254");
            byte[] ad = Kodlama.GetBytes(yol);
            attach(_st, (short)ad.Length, ad, ref _db, (short)dpb.Count, dpb.ToArray());
            Kontrol("Veritabanına bağlanılamadı");
        }

        static void Ekle(List<byte> dpb, byte etiket, string deger)
        {
            var b = Kodlama.GetBytes(deger ?? "");
            dpb.Add(etiket); dpb.Add((byte)b.Length); dpb.AddRange(b);
        }

        void Kontrol(string ne)
        {
            if (_st[0] == (IntPtr)1 && _st[1] != IntPtr.Zero)
            {
                var sb = new StringBuilder();
                var h = GCHandle.Alloc(_st, GCHandleType.Pinned);
                try
                {
                    IntPtr vec = h.AddrOfPinnedObject();
                    for (int k = 0; k < 10; k++)
                    {
                        var tampon = new StringBuilder(1024);
                        int r = interpret != null ? interpret(tampon, 1024, ref vec) : interprete(tampon, ref vec);
                        if (r <= 0) break;
                        sb.Append(tampon.ToString()).Append(' ');
                    }
                }
                finally { h.Free(); }
                Array.Clear(_st, 0, _st.Length);
                throw new Exception(ne + ": " + sb.ToString().Trim());
            }
        }

        void Basla()
        {
            if (_tr != 0) return;
            // isc_tpb_version3, write, read_committed, rec_version, wait
            byte[] tpb = { 3, 9, 15, 17, 6 };
            IntPtr pTpb = Marshal.AllocHGlobal(tpb.Length);
            IntPtr pDb = Marshal.AllocHGlobal(4);
            IntPtr teb = Marshal.AllocHGlobal(12);
            try
            {
                Marshal.Copy(tpb, 0, pTpb, tpb.Length);
                Marshal.WriteInt32(pDb, (int)_db);
                Marshal.WriteIntPtr(teb, 0, pDb);
                Marshal.WriteInt32(teb, 4, tpb.Length);
                Marshal.WriteIntPtr(teb, 8, pTpb);
                startMultiple(_st, ref _tr, 1, teb);
                Kontrol("İşlem başlatılamadı");
            }
            finally { Marshal.FreeHGlobal(pTpb); Marshal.FreeHGlobal(pDb); Marshal.FreeHGlobal(teb); }
        }

        public void Onayla() { if (_tr != 0) { commit(_st, ref _tr); Kontrol("Kaydedilemedi"); _tr = 0; } }
        public void GeriAl() { if (_tr != 0) { rollback(_st, ref _tr); _tr = 0; Array.Clear(_st, 0, _st.Length); } }

        const int VarBoy = 152, BasBoy = 20;

        static IntPtr SqldaAyir(int n)
        {
            int boy = BasBoy + n * VarBoy;
            IntPtr p = Marshal.AllocHGlobal(boy);
            for (int i = 0; i < boy; i++) Marshal.WriteByte(p, i, 0);
            Marshal.WriteInt16(p, 0, 1);              // SQLDA_VERSION1
            Marshal.WriteInt16(p, 16, (short)n);      // sqln
            return p;
        }

        // Sorgu: her satır string[] (tarihler yyyy-MM-dd, saatler HH:mm:ss, sayılar InvariantCulture).
        public List<string[]> Sorgu(string sql) { return Calistir(sql, true); }

        public void Yaz(string sql) { Calistir(sql, false); }

        List<string[]> Calistir(string sql, bool oku)
        {
            Basla();
            var sonuc = new List<string[]>();
            uint stmt = 0;
            alloc(_st, ref _db, ref stmt);
            Kontrol("Sorgu hazırlanamadı");
            IntPtr da = SqldaAyir(64);
            var tamponlar = new List<IntPtr>();
            try
            {
                byte[] b = Kodlama.GetBytes(sql);
                prepare(_st, ref _tr, ref stmt, (ushort)b.Length, b, Dialect, da);
                Kontrol("SQL hatası (" + Kisalt(sql) + ")");
                int n = Marshal.ReadInt16(da, 18);
                if (n > 64)
                {
                    Marshal.FreeHGlobal(da);
                    da = SqldaAyir(n);
                    describe(_st, ref stmt, 1, da);
                    Kontrol("SQL tanımlanamadı");
                }
                var tip = new int[n]; var olcek = new int[n]; var boy = new int[n]; var veri = new IntPtr[n]; var ind = new IntPtr[n];
                for (int i = 0; i < n; i++)
                {
                    int o = BasBoy + i * VarBoy;
                    tip[i] = Marshal.ReadInt16(da, o) & ~1;
                    olcek[i] = Marshal.ReadInt16(da, o + 2);
                    boy[i] = Marshal.ReadInt16(da, o + 6) & 0xFFFF;
                    veri[i] = Marshal.AllocHGlobal(boy[i] + 2); tamponlar.Add(veri[i]);
                    ind[i] = Marshal.AllocHGlobal(2); tamponlar.Add(ind[i]);
                    Marshal.WriteIntPtr(da, o + 8, veri[i]);
                    Marshal.WriteIntPtr(da, o + 12, ind[i]);
                    Marshal.WriteInt16(da, o, (short)(tip[i] | 1));
                }
                execute(_st, ref _tr, ref stmt, 1, IntPtr.Zero);
                Kontrol("SQL çalıştırılamadı (" + Kisalt(sql) + ")");
                if (oku && n > 0)
                {
                    while (true)
                    {
                        IntPtr r = fetch(_st, ref stmt, 1, da);
                        if (r.ToInt64() == 100) break;
                        Kontrol("Kayıt okunamadı");
                        var satir = new string[n];
                        for (int i = 0; i < n; i++)
                            satir[i] = Marshal.ReadInt16(ind[i]) < 0 ? null : Coz(tip[i], olcek[i], boy[i], veri[i]);
                        sonuc.Add(satir);
                    }
                }
            }
            finally
            {
                free(_st, ref stmt, 2);
                Marshal.FreeHGlobal(da);
                foreach (var t in tamponlar) Marshal.FreeHGlobal(t);
            }
            return sonuc;
        }

        static string Kisalt(string s) { return s.Length > 200 ? s.Substring(0, 200) + "..." : s; }

        static readonly DateTime Taban = new DateTime(1858, 11, 17);

        static string Coz(int tip, int olcek, int boy, IntPtr p)
        {
            var ci = CultureInfo.InvariantCulture;
            switch (tip)
            {
                case 448: // VARYING
                {
                    int n = Marshal.ReadInt16(p) & 0xFFFF;
                    var b = new byte[n]; Marshal.Copy(p + 2, b, 0, n);
                    return Kodlama.GetString(b);
                }
                case 452: // TEXT
                {
                    var b = new byte[boy]; Marshal.Copy(p, b, 0, boy);
                    return Kodlama.GetString(b).TrimEnd(' ', '\0');
                }
                case 500: return Olcekle(Marshal.ReadInt16(p), olcek);
                case 496: return Olcekle(Marshal.ReadInt32(p), olcek);
                case 580: return Olcekle(Marshal.ReadInt64(p), olcek);
                case 480: return BitConverter.Int64BitsToDouble(Marshal.ReadInt64(p)).ToString("R", ci);
                case 482: { var b = new byte[4]; Marshal.Copy(p, b, 0, 4); return BitConverter.ToSingle(b, 0).ToString("R", ci); }
                case 570: return Taban.AddDays(Marshal.ReadInt32(p)).ToString("yyyy-MM-dd");
                case 560: return new DateTime(1, 1, 1).AddTicks((long)(uint)Marshal.ReadInt32(p) * 1000).ToString("HH:mm:ss");
                case 510:
                {
                    var t = Taban.AddDays(Marshal.ReadInt32(p)).AddTicks((long)(uint)Marshal.ReadInt32(p + 4) * 1000);
                    return t.TimeOfDay.Ticks == 0 ? t.ToString("yyyy-MM-dd") : t.ToString("yyyy-MM-dd HH:mm:ss");
                }
                case 520: return "(blob)";
                case 32764: return Marshal.ReadByte(p) != 0 ? "1" : "0";
                default: return "?" + tip;
            }
        }

        static string Olcekle(long v, int olcek)
        {
            if (olcek >= 0) return v.ToString(CultureInfo.InvariantCulture);
            return (v / (decimal)Math.Pow(10, -olcek)).ToString(CultureInfo.InvariantCulture);
        }

        public static string Q(string s) { return s == null ? "NULL" : "'" + s.Replace("'", "''") + "'"; }

        public void Dispose()
        {
            try { GeriAl(); } catch { }
            if (_db != 0) { detach(_st, ref _db); _db = 0; }
        }
    }
}
