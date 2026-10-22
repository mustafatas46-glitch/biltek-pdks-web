using System;
using System.Collections.Generic;
using System.Linq;
using BiltekMuhasebeKoprusu;

static class KaynakTest
{
    static int hata;
    static void Bekle(bool kosul, string ne) { Console.WriteLine((kosul ? "OK   " : "HATA ") + ne); if (!kosul) hata++; }

    static int Main(string[] args)
    {
        try
        {
            string db = args[0];
            using (var k = new FbKaynak("127.0.0.1", 3050, db, "SYSDBA", "masterkey"))
            {
                var p = k.Personeller(null);
                Bekle(p.Count == 3, "3 personel okundu: " + p.Count);
                var emre = p.First(x => x.Id == 1);
                Bekle(emre.KimlikNo == null && emre.SgkNo == null, "bos maskeli alanlar null");
                Bekle(emre.KartNo == "00001" && emre.IseGiris == new DateTime(2025, 1, 1) && emre.Ucret == 45000m, "PKNO/IGTARIH/MAAS");
                Bekle(p.Any(x => x.Ad == "ALİ" && x.Soyad == "GÜLER"), "Turkce isim okundu");

                var tablo = new Tablo();
                tablo.Basliklar.AddRange(new[] { "SIRA", "AD SOYAD", "TC KİMLİK NO", "İŞE GİRİŞ TARİHİ", "SİCİL NO" });
                tablo.Satirlar.Add(new[] { "1", "EMRE KAYA", "27176288124", "02.01.2025", "S1" });
                tablo.Satirlar.Add(new[] { "2", "ALİ GÜLER", "14332441530", null, null });
                tablo.Satirlar.Add(new[] { "3", "AHMET CAN ÖZTÜRK", "48340914182", "01.03.2025", "S9" });
                var kisiler = Esitleme.Kisiler(tablo);
                var es = Esitleme.Karsilastir(kisiler, p);
                foreach (var s in es) Console.WriteLine("  " + s.Durum + " | " + s.MuhasebeAdSoyad + " | " + s.PdksAdSoyad);
                Bekle(es.Any(s => s.Durum == "TC eksik - isimle bulundu" && s.PdksId == 1), "EMRE: TC eksik");
                Bekle(es.Any(s => s.Durum == "Eşleşti" && s.PdksId == 2), "ALI: eslesti");
                Bekle(es.Any(s => s.Durum == "PDKS'de yok" && s.MuhasebeAdSoyad == "AHMET CAN ÖZTÜRK"), "AHMET: yok");
                Bekle(es.Any(s => s.Durum == "Muhasebede yok" && s.PdksId == 3), "ESKI: muhasebede yok");

                Console.WriteLine("  yeni: " + k.YeniPersonelBilgi(null));
                var sec = es.Where(s => s.Islem != Islem.Yok && s.Islem != Islem.IsimDuzelt).ToList();
                foreach (var s in sec) s.Uygula = true;
                string gy;
                using (var g = new GeriAlma("test", k.GeriAlmaBaslik)) { k.EsitlemeUygula(sec, new DateTime(2025, 3, 31), null, g); gy = g.Yol; }
                Console.WriteLine("  geri alma: " + System.IO.File.ReadAllText(gy).Replace("\r", "").Replace("\n", " / "));
                var p2 = k.Personeller(null);
                Bekle(p2.First(x => x.Id == 1).KimlikNo == "27176288124", "TC yazildi");
                var yeni = p2.FirstOrDefault(x => x.KimlikNo == "48340914182");
                Bekle(yeni != null && yeni.Id == 4 && yeni.Ad == "AHMET CAN" && yeni.Soyad == "ÖZTÜRK" && yeni.IseGiris == new DateTime(2025, 3, 1) && yeni.SicilNo == "S9" && yeni.GrupId == 1, "yeni personel eklendi");
                var eski = p2.First(x => x.Id == 3);
                Bekle(eski.IstenCikis == new DateTime(2025, 3, 31) && eski.Durum == "2", "cikis verildi + DURUM=Cikti");

                var alan = Esitleme.AlanFarklari(kisiler, p2);
                foreach (var s in alan) Console.WriteLine("  alan: " + s.PdksAdSoyad + " " + s.Alan + " " + s.EskiDeger + " -> " + s.YeniDeger);
                using (var g = new GeriAlma("test2", k.GeriAlmaBaslik)) k.AlanUygula(alan.Where(s => k.AlanYazilabilir(s.Alan)).ToList(), g);
                var p3 = k.Personeller(null);
                Bekle(p3.First(x => x.Id == 1).IseGiris == new DateTime(2025, 1, 2) && p3.First(x => x.Id == 1).SicilNo == "S1", "alanlar guncellendi");

                var d = k.Donemler(null);
                Bekle(d.Count == 1 && d[0].ToString().Contains("Şubat 2025"), "donem: " + (d.Count > 0 ? d[0].ToString() : "-"));
                var h = k.Puantaj(d[0], null);
                var gor = Puantaj.Gorunum(h, true);
                Console.WriteLine("  " + string.Join(" | ", gor.Basliklar));
                foreach (var r in gor.Satirlar) Console.WriteLine("  " + string.Join(" | ", r.Select(x => x == null ? "" : x.ToString())));
                int iNc = gor.Basliklar.IndexOf("Normal Çalışma (Saat)"), iFm = gor.Basliklar.IndexOf("%50 Fazla Mesai (Saat)"), iIz = gor.Basliklar.IndexOf("İzin: Hasta - Ücretsiz (Gün)"), iSgk = gor.Basliklar.IndexOf("SGK Gün");
                var sat = gor.Satirlar[0];
                Bekle(iNc >= 0 && (decimal)sat[iNc] == 149.22m, "NC saat 149:13 -> 149,22");
                Bekle(iFm >= 0 && (decimal)sat[iFm] == 24.5m, "FM %50 24:30 -> 24,5");
                Bekle(iIz >= 0 && (decimal)sat[iIz] == 2m, "Hasta izni 2 gun");
                Bekle(iSgk >= 0 && (decimal)sat[iSgk] == 28m, "SGK gun 28");
                Bekle((string)sat[0] == "27176288124", "puantajda TC");

                var nebim = new Sablon { Ad = "Nebim" };
                foreach (var b in new[] { "Personel Kodu|TC", "Personel Adı|ADSOYAD", "Normal Çalışma (Gün)|OTO", "Normal Çalışma (Saat)|OTO", "Fazla Mesai 1 (Saat)|OTO", "SGK Prim Gün|OTO" })
                { var x = b.Split('|'); nebim.Sutunlar.Add(new SablonSutun { Baslik = x[0], Kaynak = x[1] }); }
                var ns = SablonUygula.Uygula(nebim, h);
                Console.WriteLine("  nebim: " + string.Join(" | ", ns.Satirlar[0].Select(x => x == null ? "" : x.ToString())));
                Bekle(ns.Satirlar[0][2] is decimal && (decimal)ns.Satirlar[0][2] == 15m && (decimal)ns.Satirlar[0][3] == 149.22m && (decimal)ns.Satirlar[0][4] == 24.5m, "Nebim sablonu otomatik eslesti");
            }
        }
        catch (Exception ex) { Console.WriteLine("ISTISNA " + ex); hata++; }
        Console.WriteLine(hata == 0 ? "TUM TESTLER TAMAM" : hata + " HATA");
        return hata == 0 ? 0 : 1;
    }
}
