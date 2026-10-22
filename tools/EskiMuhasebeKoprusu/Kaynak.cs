using System;
using System.Collections.Generic;
using System.Linq;

namespace BiltekMuhasebeKoprusu
{
    // PDKS veritabanı: eski Biltek PDKS (MySQL) ya da BilTek 3xx-5xx (Firebird).
    internal interface IKaynak : IDisposable
    {
        string Ad { get; }
        List<Firma> Firmalar();
        List<Personel> Personeller(int? firmaId);
        // Yeni personel eklenirken kullanılacak varsayılanların açıklaması (onay mesajında gösterilir).
        string YeniPersonelBilgi(int? firmaId);
        void EsitlemeUygula(List<EsitlemeSatiri> secilen, DateTime cikisTarihi, int? firmaId, GeriAlma geri);
        bool AlanYazilabilir(string alan);
        void AlanUygula(List<EsitlemeSatiri> secilen, GeriAlma geri);
        List<Donem> Donemler(int? firmaId);
        List<PuantajKolon> PuantajKolonlari();
        PuantajHam Puantaj(Donem d, int? firmaId);
        string[] GeriAlmaBaslik { get; }
    }

    internal sealed class MySqlKaynak : IKaynak
    {
        readonly Db _db;
        readonly string _vt;
        string _grupId, _durum;

        public MySqlKaynak(string sunucu, int port, string vt, string kullanici, string sifre)
        {
            _vt = vt;
            _db = new Db(sunucu, port, vt, kullanici, sifre);
            if (!_db.TabloVar("personel")) { _db.Dispose(); throw new Exception(vt + " bir Biltek PDKS veritabanı değil (personel tablosu yok)."); }
        }

        public string Ad { get { return "Biltek PDKS (MySQL) - " + _vt; } }
        public string[] GeriAlmaBaslik { get { return new[] { "-- HeidiSQL ile " + _vt + " veritabanında çalıştırınca bu işlemdeki değişiklikler geri alınır.", "SET NAMES utf8;", "USE `" + _vt + "`;" }; } }

        public List<Firma> Firmalar() { return PdksVeri.Firmalar(_db); }
        public List<Personel> Personeller(int? firmaId) { return PdksVeri.Personeller(_db, firmaId); }

        public string YeniPersonelBilgi(int? firmaId)
        {
            if (!firmaId.HasValue) throw new Exception("Yeni personel eklemek için üstten bir firma seçin.");
            _grupId = PdksVeri.EnCok(_db, "grupid", firmaId);
            _durum = PdksVeri.EnCok(_db, "durum", firmaId);
            return "grup: " + PdksVeri.GrupAdi(_db, _grupId) + ", kart no boş";
        }

        void Islem(GeriAlma geri, Action a)
        {
            _db.Yaz("START TRANSACTION");
            try { a(); _db.Yaz("COMMIT"); }
            catch { _db.Yaz("ROLLBACK"); geri.Ekle("-- HATA: islem iptal edildi, veritabani DEGISMEDI. Bu dosyayi calistirmayin."); throw; }
        }

        public void EsitlemeUygula(List<EsitlemeSatiri> secilen, DateTime cikis, int? firmaId, GeriAlma geri)
        {
            Islem(geri, delegate
            {
                foreach (var s in secilen)
                {
                    var p = s.Personel;
                    switch (s.Islem)
                    {
                        case BiltekMuhasebeKoprusu.Islem.TcYaz:
                            geri.Ekle("UPDATE personel SET kimlikno = " + Db.Q(p.KimlikNo) + " WHERE id = " + p.Id + ";");
                            _db.Yaz("UPDATE personel SET kimlikno = " + Db.Q(s.MuhasebeTc) + " WHERE id = " + p.Id);
                            break;
                        case BiltekMuhasebeKoprusu.Islem.IsimDuzelt:
                        {
                            string ad, soyad;
                            Metin.AdSoyadBol(s.MuhasebeAdSoyad, out ad, out soyad);
                            geri.Ekle("UPDATE personel SET ad = " + Db.Q(p.Ad) + ", soyad = " + Db.Q(p.Soyad) + " WHERE id = " + p.Id + ";");
                            _db.Yaz("UPDATE personel SET ad = " + Db.Q(ad) + ", soyad = " + Db.Q(soyad) + " WHERE id = " + p.Id);
                            break;
                        }
                        case BiltekMuhasebeKoprusu.Islem.Pasife:
                            geri.Ekle("UPDATE personel SET istencikistarihi = " + Db.QTarih(p.IstenCikis) + " WHERE id = " + p.Id + ";");
                            _db.Yaz("UPDATE personel SET istencikistarihi = " + Db.QTarih(cikis.Date) + " WHERE id = " + p.Id);
                            break;
                        case BiltekMuhasebeKoprusu.Islem.Ekle:
                        {
                            string ad, soyad;
                            Metin.AdSoyadBol(s.MuhasebeAdSoyad, out ad, out soyad);
                            var k = s.Kisi;
                            _db.Yaz("INSERT INTO personel (kartfc, kartno, kartid, ad, soyad, kimlikno, sicilno, firmaid, grupid, durum, isegiristarihi, ucret) VALUES ('', '', '', " +
                                    Db.Q(ad) + ", " + Db.Q(soyad) + ", " + Db.Q(s.MuhasebeTc) + ", " + Db.Q(k.SicilNo) + ", " + firmaId.Value + ", " +
                                    (_grupId == null ? "NULL" : _grupId) + ", " + Db.Q(_durum) + ", " + Db.QTarih(k.IseGiris) + ", " + Db.QSayi(k.Ucret) + ")");
                            geri.Ekle("DELETE FROM personel WHERE id = " + _db.SonId() + ";  -- " + s.MuhasebeAdSoyad);
                            break;
                        }
                    }
                }
            });
        }

        public bool AlanYazilabilir(string alan) { return true; }

        public void AlanUygula(List<EsitlemeSatiri> secilen, GeriAlma geri)
        {
            Islem(geri, delegate
            {
                foreach (var s in secilen)
                {
                    var p = s.Personel; var k = s.Kisi;
                    string kolon, eski, yeni;
                    switch (s.Alan)
                    {
                        case "İşe giriş tarihi": kolon = "isegiristarihi"; eski = Db.QTarih(p.IseGiris); yeni = Db.QTarih(k.IseGiris); break;
                        case "İşten çıkış tarihi": kolon = "istencikistarihi"; eski = Db.QTarih(p.IstenCikis); yeni = Db.QTarih(k.IstenCikis); break;
                        case "Ücret": kolon = "ucret"; eski = Db.QSayi(p.Ucret); yeni = Db.QSayi(k.Ucret); break;
                        case "Sicil no": kolon = "sicilno"; eski = Db.Q(p.SicilNo); yeni = Db.Q(k.SicilNo); break;
                        case "SGK no": kolon = "sgk_no"; eski = Db.Q(p.SgkNo); yeni = Db.Q(k.SgkNo); break;
                        default: continue;
                    }
                    geri.Ekle("UPDATE personel SET " + kolon + " = " + eski + " WHERE id = " + p.Id + ";");
                    _db.Yaz("UPDATE personel SET " + kolon + " = " + yeni + " WHERE id = " + p.Id);
                }
            });
        }

        public List<Donem> Donemler(int? firmaId) { return BiltekMuhasebeKoprusu.Puantaj.Donemler(_db, firmaId); }
        public List<PuantajKolon> PuantajKolonlari() { return BiltekMuhasebeKoprusu.Puantaj.Kolonlar(_db); }
        public PuantajHam Puantaj(Donem d, int? firmaId) { return BiltekMuhasebeKoprusu.Puantaj.Ham(_db, d, firmaId); }
        public void Dispose() { _db.Dispose(); }
    }

    // BilTek 3xx-5xx (Firebird): personel KIMLIK, dönem puantajı UCRETLER, bordro kalemleri BORDRO, izinler OZELIZIN.
    internal sealed class FbKaynak : IKaynak
    {
        readonly Fb _db;
        readonly string _yol;
        readonly HashSet<string> _ucretAlan;
        string _grup, _durum, _cikisDurum;

        public FbKaynak(string sunucu, int port, string dosya, string kullanici, string sifre)
        {
            _yol = string.IsNullOrWhiteSpace(sunucu) ? dosya : sunucu.Trim() + (port > 0 && port != 3050 ? "/" + port : "") + ":" + dosya;
            _db = new Fb(_yol, kullanici, sifre);
            var tablolar = new HashSet<string>(_db.Sorgu("SELECT TRIM(RDB$RELATION_NAME) FROM RDB$RELATIONS WHERE COALESCE(RDB$SYSTEM_FLAG,0)=0").Select(r => r[0]));
            if (!tablolar.Contains("KIMLIK")) { _db.Dispose(); throw new Exception("Bu bir BilTek veritabanı değil (KIMLIK tablosu yok): " + _yol); }
            _ucretAlan = tablolar.Contains("UCRETLER") ? Alanlar("UCRETLER") : new HashSet<string>();
        }

        HashSet<string> Alanlar(string tablo)
        {
            return new HashSet<string>(_db.Sorgu("SELECT TRIM(RDB$FIELD_NAME) FROM RDB$RELATION_FIELDS WHERE RDB$RELATION_NAME = " + Fb.Q(tablo)).Select(r => r[0]));
        }

        public string Ad { get { return "BilTek (Firebird) - " + _yol; } }
        public string[] GeriAlmaBaslik { get { return new[] { "-- IBExpert / isql ile " + _yol + " veritabanında çalıştırınca bu işlemdeki değişiklikler geri alınır." }; } }

        static string T(DateTime? t) { return t.HasValue ? "CAST('" + t.Value.ToString("yyyy-MM-dd") + "' AS TIMESTAMP)" : "NULL"; }
        static string N(decimal? d) { return d.HasValue ? d.Value.ToString(System.Globalization.CultureInfo.InvariantCulture) : "NULL"; }
        static int? I(string s) { int i; return int.TryParse(s, out i) ? i : (int?)null; }

        public List<Firma> Firmalar()
        {
            return _db.Sorgu("SELECT KOD, AD FROM FIRMA ORDER BY AD").Select(r => new Firma { Id = int.Parse(r[0]), Ad = (r[1] ?? "").Trim() + " (" + r[0] + ")" }).ToList();
        }

        public List<Personel> Personeller(int? firmaId)
        {
            string sql = "SELECT PS, PKNO, AD, SOYAD, SICILNO, UKNO, SIRKET, GRUP, IGTARIH, ICTARIH, MAAS, SSKNO, DURUM FROM KIMLIK" +
                         (firmaId.HasValue ? " WHERE SIRKET = " + firmaId.Value : "");
            return _db.Sorgu(sql).Where(r => !string.IsNullOrWhiteSpace(r[2]) || !string.IsNullOrWhiteSpace(r[3])).Select(r => new Personel
            {
                Id = long.Parse(r[0]), KartNo = Bos(r[1]), Ad = Bos(r[2]), Soyad = Bos(r[3]), SicilNo = Bos(r[4]), KimlikNo = Bos(r[5]),
                FirmaId = I(r[6]), GrupId = I(r[7]), IseGiris = PdksVeri.Trh(r[8]), IstenCikis = PdksVeri.Trh(r[9]),
                Ucret = Tablo.Sayi(r[10]), SgkNo = Bos(r[11]), Durum = r[12]
            }).ToList();
        }

        // BilTek boş alanları boşlukla ya da "(   )    -" maskesiyle tutabiliyor.
        static string Bos(string s)
        {
            if (s == null) return null;
            s = s.Trim();
            return s.Trim('(', ')', '-', ' ', '.', '/').Length == 0 ? null : s;
        }

        string EnCok(string kolon)
        {
            var r = _db.Sorgu("SELECT " + kolon + ", COUNT(*) FROM KIMLIK WHERE " + kolon + " IS NOT NULL AND (ICTARIH IS NULL OR ICTARIH >= CURRENT_DATE) GROUP BY " + kolon + " ORDER BY 2 DESC");
            return r.Count > 0 ? r[0][0] : null;
        }

        string CikisDurumu()
        {
            foreach (var r in _db.Sorgu("SELECT KOD, AD FROM DURUM"))
            {
                string a = Metin.Anahtar(r[1]);
                if (a.StartsWith("CIK") || a.Contains("AYRIL") || a.Contains("PASIF")) return r[0];
            }
            return null;
        }

        public string YeniPersonelBilgi(int? firmaId)
        {
            _grup = EnCok("GRUP");
            _durum = EnCok("DURUM");
            string grupAd = _grup == null ? "(yok)" : (_db.Sorgu("SELECT AD FROM GRUP WHERE KOD = " + int.Parse(_grup)).Select(r => r[0]).FirstOrDefault() ?? _grup);
            return "grup: " + grupAd + ", personel/kart no (PKNO) boş - BilTek'ten verin";
        }

        void Islem(GeriAlma geri, Action a)
        {
            try { a(); _db.Onayla(); geri.Ekle("COMMIT;"); }
            catch { _db.GeriAl(); geri.Ekle("-- HATA: islem iptal edildi, veritabani DEGISMEDI. Bu dosyayi calistirmayin."); throw; }
        }

        public void EsitlemeUygula(List<EsitlemeSatiri> secilen, DateTime cikis, int? firmaId, GeriAlma geri)
        {
            if (secilen.Any(s => s.Islem == BiltekMuhasebeKoprusu.Islem.Pasife)) _cikisDurum = CikisDurumu();
            Islem(geri, delegate
            {
                int ps = 0;
                foreach (var s in secilen)
                {
                    var p = s.Personel;
                    switch (s.Islem)
                    {
                        case BiltekMuhasebeKoprusu.Islem.TcYaz:
                            geri.Ekle("UPDATE KIMLIK SET UKNO = " + Fb.Q(p.KimlikNo) + " WHERE PS = " + p.Id + ";");
                            _db.Yaz("UPDATE KIMLIK SET UKNO = " + Fb.Q(s.MuhasebeTc) + " WHERE PS = " + p.Id);
                            break;
                        case BiltekMuhasebeKoprusu.Islem.IsimDuzelt:
                        {
                            string ad, soyad;
                            Metin.AdSoyadBol(s.MuhasebeAdSoyad, out ad, out soyad);
                            geri.Ekle("UPDATE KIMLIK SET AD = " + Fb.Q(p.Ad) + ", SOYAD = " + Fb.Q(p.Soyad) + " WHERE PS = " + p.Id + ";");
                            _db.Yaz("UPDATE KIMLIK SET AD = " + Fb.Q(ad) + ", SOYAD = " + Fb.Q(soyad) + " WHERE PS = " + p.Id);
                            break;
                        }
                        case BiltekMuhasebeKoprusu.Islem.Pasife:
                            geri.Ekle("UPDATE KIMLIK SET ICTARIH = " + T(p.IstenCikis) + (_cikisDurum != null ? ", DURUM = " + (p.Durum ?? "NULL") : "") + " WHERE PS = " + p.Id + ";");
                            _db.Yaz("UPDATE KIMLIK SET ICTARIH = " + T(cikis.Date) + (_cikisDurum != null ? ", DURUM = " + _cikisDurum : "") + " WHERE PS = " + p.Id);
                            break;
                        case BiltekMuhasebeKoprusu.Islem.Ekle:
                        {
                            if (ps == 0) ps = int.Parse(_db.Sorgu("SELECT COALESCE(MAX(PS), 0) FROM KIMLIK")[0][0]);
                            ps++;
                            string ad, soyad;
                            Metin.AdSoyadBol(s.MuhasebeAdSoyad, out ad, out soyad);
                            var k = s.Kisi;
                            _db.Yaz("INSERT INTO KIMLIK (PS, AD, SOYAD, UKNO, SICILNO, SSKNO, SIRKET, GRUP, DURUM, IGTARIH, ESDRM) VALUES (" + ps + ", " +
                                    Fb.Q(ad) + ", " + Fb.Q(soyad) + ", " + Fb.Q(s.MuhasebeTc) + ", " + Fb.Q(Kes(k.SicilNo, 10)) + ", " + Fb.Q(Kes(k.SgkNo, 30)) + ", " +
                                    (firmaId.HasValue ? firmaId.Value.ToString() : "NULL") + ", " + (_grup ?? "NULL") + ", " + (_durum ?? "NULL") + ", " + T(k.IseGiris) + ", 0)");
                            geri.Ekle("DELETE FROM KIMLIK WHERE PS = " + ps + ";  -- " + s.MuhasebeAdSoyad);
                            break;
                        }
                    }
                }
            });
        }

        static string Kes(string s, int n) { return s == null ? null : (s.Length > n ? s.Substring(0, n) : s); }

        // Maaş BilTek'te tarihçeli (MAAS tablosu) tutulduğu için ücret buradan yazılmaz.
        public bool AlanYazilabilir(string alan) { return alan != "Ücret"; }

        public void AlanUygula(List<EsitlemeSatiri> secilen, GeriAlma geri)
        {
            Islem(geri, delegate
            {
                foreach (var s in secilen)
                {
                    var p = s.Personel; var k = s.Kisi;
                    string kolon, eski, yeni;
                    switch (s.Alan)
                    {
                        case "İşe giriş tarihi": kolon = "IGTARIH"; eski = T(p.IseGiris); yeni = T(k.IseGiris); break;
                        case "İşten çıkış tarihi": kolon = "ICTARIH"; eski = T(p.IstenCikis); yeni = T(k.IstenCikis); break;
                        case "Sicil no": kolon = "SICILNO"; eski = Fb.Q(p.SicilNo); yeni = Fb.Q(Kes(k.SicilNo, 10)); break;
                        case "SGK no": kolon = "SSKNO"; eski = Fb.Q(p.SgkNo); yeni = Fb.Q(Kes(k.SgkNo, 30)); break;
                        default: continue;
                    }
                    geri.Ekle("UPDATE KIMLIK SET " + kolon + " = " + eski + " WHERE PS = " + p.Id + ";");
                    _db.Yaz("UPDATE KIMLIK SET " + kolon + " = " + yeni + " WHERE PS = " + p.Id);
                }
            });
        }

        public List<Donem> Donemler(int? firmaId)
        {
            if (_ucretAlan.Count == 0) throw new Exception("Bu veritabanında UCRETLER (dönem puantajı) tablosu yok.");
            var adlar = new Dictionary<string, string>();
            try { foreach (var r in _db.Sorgu("SELECT BASTAR, BITTAR, AD FROM DONEM")) adlar[r[0] + "|" + r[1]] = (r[2] ?? "").Trim(); } catch { }
            string sql = "SELECT DISTINCT u.BASTAR, u.BITTAR FROM UCRETLER u" +
                         (firmaId.HasValue ? " JOIN KIMLIK k ON k.PKNO = u.PKNO WHERE k.SIRKET = " + firmaId.Value : "") +
                         " ORDER BY 1 DESC";
            return _db.Sorgu(sql).Where(r => r[0] != null).Take(60).Select(r =>
            {
                string ad; adlar.TryGetValue(r[0] + "|" + r[1], out ad);
                return new Donem { Bas = r[0], Bit = r[1], Ad = ad };
            }).ToList();
        }

        List<string> _izinTurleri;

        public List<PuantajKolon> PuantajKolonlari()
        {
            var l = new List<PuantajKolon>();
            Action<string, string, bool> ekle = (baslik, alan, saat) => { if (_ucretAlan.Contains(alan)) l.Add(new PuantajKolon { Baslik = baslik, Sql = alan, Saat = saat }); };
            ekle("SGK Gün", "SSKG", false);
            ekle("Normal Çalışma (Gün)", "NCGUN", false);
            ekle("Normal Çalışma (Saat)", "NCSAAT", true);
            try
            {
                foreach (var r in _db.Sorgu("SELECT KOD, AD, KAD FROM BORDRO ORDER BY KOD"))
                {
                    string ad = !string.IsNullOrWhiteSpace(r[1]) ? r[1].Trim() : (r[2] ?? "").Trim();
                    ekle(ad + " (Gün)", "GUN" + r[0], false);
                    ekle(ad + " (Saat)", "SAAT" + r[0], true);
                }
            }
            catch { }
            ekle("Fazla Mesai Toplam (Saat)", "FMSAAT", true);
            ekle("Devamsızlık (Gün)", "DEVG", false);
            ekle("Devamsızlık (Saat)", "DEVS", true);
            ekle("Geç Kalma (Saat)", "GECS", true);
            ekle("Erken Çıkma (Saat)", "ERS", true);
            ekle("Eksik Süre (Saat)", "EKS", true);
            ekle("Toplam Eksik (Saat)", "TOPEKS", true);
            // İzinler (OZELIZIN): mazeret + tip başına gün sayısı
            _izinTurleri = new List<string>();
            try
            {
                foreach (var r in _db.Sorgu("SELECT DISTINCT MAZERET, TIP FROM OZELIZIN WHERE MAZERET IS NOT NULL"))
                {
                    string anahtar = (r[0] ?? "").Trim() + "~" + (r[1] ?? "").Trim();
                    _izinTurleri.Add(anahtar);
                    l.Add(new PuantajKolon { Baslik = "İzin: " + Metin.Onar((r[0] ?? "").Trim()) + (string.IsNullOrWhiteSpace(r[1]) ? "" : " - " + Metin.Onar(r[1].Trim())) + " (Gün)", Sql = "IZIN:" + anahtar });
                }
            }
            catch { }
            return l;
        }

        public PuantajHam Puantaj(Donem d, int? firmaId)
        {
            var kolonlar = PuantajKolonlari();
            var alanlar = kolonlar.Where(k => !k.Sql.StartsWith("IZIN:")).ToList();
            string sql = "SELECT k.UKNO, k.SICILNO, u.PKNO, k.AD, k.SOYAD, k.SIRKET" + string.Concat(alanlar.Select(k => ", u." + k.Sql)) +
                         " FROM UCRETLER u LEFT JOIN KIMLIK k ON k.PKNO = u.PKNO" +
                         " WHERE u.BASTAR = CAST('" + d.Bas + "' AS TIMESTAMP) AND u.BITTAR = CAST('" + d.Bit + "' AS TIMESTAMP)" +
                         (firmaId.HasValue ? " AND k.SIRKET = " + firmaId.Value : "") +
                         " ORDER BY k.AD, k.SOYAD";
            var izin = new Dictionary<string, decimal>();
            if (kolonlar.Any(k => k.Sql.StartsWith("IZIN:")))
            {
                foreach (var r in _db.Sorgu("SELECT PKNO, MAZERET, TIP, COUNT(DISTINCT TARIH) FROM OZELIZIN WHERE TARIH BETWEEN CAST('" + d.Bas + "' AS TIMESTAMP) AND CAST('" + d.Bit + "' AS TIMESTAMP) GROUP BY PKNO, MAZERET, TIP"))
                    izin[(r[0] ?? "").Trim() + "|IZIN:" + (r[1] ?? "").Trim() + "~" + (r[2] ?? "").Trim()] = decimal.Parse(r[3]);
            }
            var h = new PuantajHam { Kolonlar = kolonlar, Donem = d };
            foreach (var r in _db.Sorgu(sql))
            {
                string pk = (r[2] ?? "").Trim();
                var deg = new decimal[kolonlar.Count];
                int j = 0;
                for (int i = 0; i < kolonlar.Count; i++)
                {
                    var k = kolonlar[i];
                    if (k.Sql.StartsWith("IZIN:")) { decimal v; deg[i] = izin.TryGetValue(pk + "|" + k.Sql, out v) ? v : 0m; }
                    else { deg[i] = BiltekMuhasebeKoprusu.Puantaj.Deger(r[6 + j], k.Saat); j++; }
                }
                h.Kisiler.Add(new PuantajKisi
                {
                    Tc = Metin.SadeceRakam(r[0]), Sicil = Bos(r[1]) ?? "", Kart = pk, Ad = Bos(r[3]) ?? "", Soyad = Bos(r[4]) ?? "", FirmaKod = r[5] ?? "",
                    Degerler = deg
                });
            }
            return h;
        }

        public void Dispose() { _db.Dispose(); }
    }
}
