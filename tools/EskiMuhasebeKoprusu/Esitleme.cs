using System;
using System.Collections.Generic;
using System.Linq;

namespace BiltekMuhasebeKoprusu
{
    internal enum Islem { Yok, TcYaz, Ekle, IsimDuzelt, Pasife, AlanGuncelle }

    internal sealed class EsitlemeSatiri
    {
        public bool Uygula;
        public string Durum;
        public string MuhasebeAdSoyad, MuhasebeTc;
        public string PdksAdSoyad, PdksTc, PdksKartNo;
        public long? PdksId;
        public Islem Islem;
        public string Aciklama;
        public MuhasebeKisi Kisi;
        public Personel Personel;
        // Alan güncellemesi için
        public string Alan, EskiDeger, YeniDeger;
    }

    internal static class Esitleme
    {
        public static List<MuhasebeKisi> Kisiler(Tablo t)
        {
            int kTc = t.KolonBul("TC KİMLİK NO", "TC KIMLIK", "TCKN", "T.C.", "KIMLIK NO", "TC NO", "TC");
            if (kTc < 0) throw new Exception("Dosyada TC Kimlik No sütunu bulunamadı. Başlıklar: " + string.Join(", ", t.Basliklar));
            int kAdSoyad = t.KolonBul("AD SOYAD", "ADI SOYADI", "ADSOYAD", "PERSONEL ADI", "PERSONEL");
            int kAd = t.KolonBul("ADI", "AD");
            int kSoyad = t.KolonBul("SOYADI", "SOYAD");
            if (kAd == kAdSoyad) kAd = -1;
            if (kSoyad == kAdSoyad) kSoyad = -1;
            int kSicil = t.KolonBul("SİCİL NO", "SICIL", "PERSONEL KODU", "PERSONEL NO");
            int kSgk = t.KolonBul("SGK NO", "SGK SİCİL", "SİGORTA NO");
            int kGiris = t.KolonBul("İŞE GİRİŞ TARİHİ", "ISE GIRIS", "GİRİŞ TARİHİ");
            int kCikis = t.KolonBul("İŞTEN ÇIKIŞ TARİHİ", "ISTEN CIKIS", "ÇIKIŞ TARİHİ");
            int kUcret = t.KolonBul("ÜCRET", "UCRET", "MAAŞ", "MAAS");
            var l = new List<MuhasebeKisi>();
            for (int i = 0; i < t.Satirlar.Count; i++)
            {
                var r = t.Satirlar[i];
                string tc = Metin.SadeceRakam(Tablo.Hucre(r, kTc));
                string adSoyad = kAdSoyad >= 0 ? Tablo.Hucre(r, kAdSoyad) : ((Tablo.Hucre(r, kAd) ?? "") + " " + (Tablo.Hucre(r, kSoyad) ?? "")).Trim();
                if (tc.Length == 0 && string.IsNullOrEmpty(adSoyad)) continue;
                l.Add(new MuhasebeKisi
                {
                    Satir = i + 2, Tc = tc, AdSoyad = Metin.Buyuk(adSoyad),
                    SicilNo = Tablo.Hucre(r, kSicil), SgkNo = Tablo.Hucre(r, kSgk),
                    IseGiris = Tablo.Tarih(Tablo.Hucre(r, kGiris)), IstenCikis = Tablo.Tarih(Tablo.Hucre(r, kCikis)),
                    Ucret = Tablo.Sayi(Tablo.Hucre(r, kUcret))
                });
            }
            return l;
        }

        public static List<EsitlemeSatiri> Karsilastir(List<MuhasebeKisi> kisiler, List<Personel> pdks)
        {
            var sonuc = new List<EsitlemeSatiri>();
            var tcIle = pdks.Where(p => !string.IsNullOrEmpty(Metin.SadeceRakam(p.KimlikNo)))
                            .GroupBy(p => Metin.SadeceRakam(p.KimlikNo)).ToDictionary(g => g.Key, g => g.ToList());
            var isimIle = pdks.GroupBy(p => Metin.Anahtar(p.AdSoyad)).ToDictionary(g => g.Key, g => g.ToList());
            var kullanilan = new HashSet<long>();
            var listeTc = new HashSet<string>();

            foreach (var k in kisiler)
            {
                var s = new EsitlemeSatiri { Kisi = k, MuhasebeAdSoyad = k.AdSoyad, MuhasebeTc = k.Tc };
                sonuc.Add(s);
                if (!Metin.TcGecerli(k.Tc))
                {
                    s.Durum = "Geçersiz TC (muhasebe)";
                    s.Aciklama = "Muhasebe dosyasında " + k.Satir + ". satırdaki TC hatalı; önce muhasebede düzeltin.";
                    continue;
                }
                if (!listeTc.Add(k.Tc))
                {
                    s.Durum = "Muhasebede mükerrer TC";
                    s.Aciklama = "Bu TC muhasebe dosyasında birden fazla geçiyor.";
                    continue;
                }
                List<Personel> bulunan;
                if (tcIle.TryGetValue(k.Tc, out bulunan))
                {
                    var p = bulunan.OrderByDescending(x => x.Aktif).First();
                    Bagla(s, p);
                    foreach (var x in bulunan) kullanilan.Add(x.Id);
                    if (bulunan.Count > 1)
                    {
                        s.Durum = "PDKS'de mükerrer TC";
                        s.Aciklama = "Bu TC PDKS'de " + bulunan.Count + " kişide kayıtlı (id: " + string.Join(", ", bulunan.Select(x => x.Id)) + "). Eski programda birini düzeltin.";
                    }
                    else if (Metin.Anahtar(p.AdSoyad) != Metin.Anahtar(k.AdSoyad))
                    {
                        s.Durum = "Eşleşti - isim farklı";
                        s.Islem = Islem.IsimDuzelt;
                        s.Aciklama = "TC aynı, isim farklı. İşaretlerseniz PDKS'deki isim muhasebeye göre düzeltilir.";
                    }
                    else
                    {
                        s.Durum = p.Aktif ? "Eşleşti" : "Eşleşti - PDKS'de çıkışlı";
                        if (!p.Aktif) s.Aciklama = "PDKS'de işten çıkış tarihi var (" + p.IstenCikis.Value.ToString("dd.MM.yyyy") + "), muhasebede aktif.";
                    }
                    continue;
                }
                List<Personel> adayi;
                isimIle.TryGetValue(Metin.Anahtar(k.AdSoyad), out adayi);
                var bos = adayi == null ? new List<Personel>() : adayi.Where(p => !kullanilan.Contains(p.Id)).ToList();
                if (bos.Count == 1)
                {
                    var p = bos[0];
                    Bagla(s, p);
                    kullanilan.Add(p.Id);
                    if (Metin.TcGecerli(Metin.SadeceRakam(p.KimlikNo)))
                    {
                        s.Durum = "İsim aynı - TC farklı";
                        s.Aciklama = "PDKS'de bu isimde farklı bir TC kayıtlı. Hangisi doğruysa elle kontrol edin.";
                    }
                    else
                    {
                        s.Durum = "TC eksik - isimle bulundu";
                        s.Islem = Islem.TcYaz;
                        s.Uygula = true;
                        s.Aciklama = "PDKS kaydına muhasebedeki TC yazılacak.";
                    }
                }
                else if (bos.Count > 1)
                {
                    s.Durum = "İsim birden fazla";
                    s.Aciklama = "PDKS'de bu isimde " + bos.Count + " kişi var (id: " + string.Join(", ", bos.Select(x => x.Id)) + "). Doğru kişiye TC'yi eski programda yazın.";
                }
                else
                {
                    s.Durum = "PDKS'de yok";
                    s.Islem = Islem.Ekle;
                    s.Uygula = true;
                    s.Aciklama = "İşaretlerseniz PDKS'ye kart numarasız eklenir; kartı eski programdan tanımlayın.";
                }
            }

            foreach (var p in pdks.Where(x => x.Aktif && !kullanilan.Contains(x.Id)).OrderBy(x => x.AdSoyad))
            {
                var s = new EsitlemeSatiri { Durum = "Muhasebede yok", Islem = Islem.Pasife };
                Bagla(s, p);
                s.Aciklama = "PDKS'de aktif, muhasebe listesinde yok. İşaretlerseniz seçtiğiniz tarihle işten çıkış verilir.";
                sonuc.Add(s);
            }
            return sonuc;
        }

        static void Bagla(EsitlemeSatiri s, Personel p)
        {
            s.Personel = p;
            s.PdksId = p.Id;
            s.PdksAdSoyad = p.AdSoyad;
            s.PdksTc = p.KimlikNo;
            s.PdksKartNo = p.KartNo;
        }

        // TC ile eşleşen kişilerde muhasebedeki dolu alanları PDKS ile karşılaştırır.
        public static List<EsitlemeSatiri> AlanFarklari(List<MuhasebeKisi> kisiler, List<Personel> pdks)
        {
            var tcIle = pdks.Where(p => Metin.TcGecerli(Metin.SadeceRakam(p.KimlikNo)))
                            .GroupBy(p => Metin.SadeceRakam(p.KimlikNo)).Where(g => g.Count() == 1).ToDictionary(g => g.Key, g => g.First());
            var l = new List<EsitlemeSatiri>();
            foreach (var k in kisiler)
            {
                Personel p;
                if (!tcIle.TryGetValue(k.Tc, out p)) continue;
                Fark(l, k, p, "İşe giriş tarihi", T(p.IseGiris), T(k.IseGiris));
                Fark(l, k, p, "İşten çıkış tarihi", T(p.IstenCikis), T(k.IstenCikis));
                Fark(l, k, p, "Ücret", U(p.Ucret), U(k.Ucret));
                Fark(l, k, p, "Sicil no", p.SicilNo, k.SicilNo);
                Fark(l, k, p, "SGK no", p.SgkNo, k.SgkNo);
            }
            return l;
        }

        static string T(DateTime? t) { return t.HasValue ? t.Value.ToString("dd.MM.yyyy") : ""; }
        static string U(decimal? d) { return d.HasValue ? d.Value.ToString("N2", new System.Globalization.CultureInfo("tr-TR")) : ""; }

        static void Fark(List<EsitlemeSatiri> l, MuhasebeKisi k, Personel p, string alan, string pdksDeger, string muhDeger)
        {
            if (string.IsNullOrWhiteSpace(muhDeger)) return; // muhasebede boşsa PDKS'ye dokunma
            if ((pdksDeger ?? "").Trim() == muhDeger.Trim()) return;
            var s = new EsitlemeSatiri
            {
                Kisi = k, Personel = p, PdksId = p.Id, MuhasebeTc = k.Tc, MuhasebeAdSoyad = k.AdSoyad,
                PdksAdSoyad = p.AdSoyad, PdksTc = p.KimlikNo, PdksKartNo = p.KartNo,
                Islem = Islem.AlanGuncelle, Alan = alan, EskiDeger = pdksDeger ?? "", YeniDeger = muhDeger, Uygula = true,
                Durum = alan + " farklı"
            };
            l.Add(s);
        }
    }
}
