using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BiltekMuhasebeKoprusu
{
    // Bir muhasebe programının puantaj içe aktarma düzeni: sütun başlıkları ve her sütuna PDKS'den ne yazılacağı.
    internal sealed class Sablon
    {
        public string Ad = "", UstSatir = "", SaatBicimi = "Ondalik";
        public List<SablonSutun> Sutunlar = new List<SablonSutun>();
        public string Dosya;
        public override string ToString() { return Ad; }

        public static string Klasor { get { return Path.Combine(Ayarlar.Klasor, "Sablonlar"); } }

        public static List<Sablon> Hepsi()
        {
            Directory.CreateDirectory(Klasor);
            HazirlariYaz();
            return Directory.GetFiles(Klasor, "*.sablon").Select(Oku).Where(s => s != null).OrderBy(s => s.Ad).ToList();
        }

        static Sablon Oku(string yol)
        {
            try
            {
                var s = new Sablon { Dosya = yol };
                foreach (var satir in File.ReadAllLines(yol, Encoding.UTF8))
                {
                    int i = satir.IndexOf('=');
                    if (i <= 0) continue;
                    string k = satir.Substring(0, i).Trim(), v = satir.Substring(i + 1);
                    if (k == "Ad") s.Ad = v.Trim();
                    else if (k == "UstSatir") s.UstSatir = v.Trim();
                    else if (k == "SaatBicimi") s.SaatBicimi = v.Trim();
                    else if (k == "Sutun")
                    {
                        int j = v.LastIndexOf('|');
                        s.Sutunlar.Add(j < 0 ? new SablonSutun { Baslik = v, Kaynak = "OTO" } : new SablonSutun { Baslik = v.Substring(0, j), Kaynak = v.Substring(j + 1) });
                    }
                }
                if (s.Ad.Length == 0) s.Ad = Path.GetFileNameWithoutExtension(yol);
                return s;
            }
            catch { return null; }
        }

        public void Kaydet()
        {
            Directory.CreateDirectory(Klasor);
            string yeni = Path.Combine(Klasor, DosyaAdi(Ad) + ".sablon");
            var sb = new StringBuilder();
            sb.AppendLine("Ad=" + Ad);
            sb.AppendLine("UstSatir=" + UstSatir);
            sb.AppendLine("SaatBicimi=" + SaatBicimi);
            foreach (var c in Sutunlar) sb.AppendLine("Sutun=" + c.Baslik.Replace("\r", " ").Replace("\n", " ") + "|" + c.Kaynak);
            File.WriteAllText(yeni, sb.ToString(), new UTF8Encoding(false));
            if (Dosya != null && !string.Equals(Dosya, yeni, StringComparison.OrdinalIgnoreCase) && File.Exists(Dosya)) File.Delete(Dosya);
            Dosya = yeni;
        }

        static string DosyaAdi(string s)
        {
            foreach (var c in Path.GetInvalidFileNameChars()) s = s.Replace(c, '_');
            return s.Trim().Length == 0 ? "Sablon" : s.Trim();
        }

        // Eski Biltek PDKS'nin Excel klasöründeki Nebim ve Link aktarım dosyalarından alınan düzenler.
        static void HazirlariYaz()
        {
            Hazir("Nebim", "Standart", new[] {
                "İşyeri Kodu|SABIT:1", "Personel Kodu|TC", "Personel Adı|ADSOYAD", "SGK Prim Gün|OTO", "SGK Eksik Gün|OTO",
                "Normal Çalışma (Gün)|OTO", "Normal Çalışma (Saat)|OTO", "Hafta Sonu (Gün)|OTO", "Hafta Sonu (Saat)|OTO",
                "Resmi Tatil (Gün)|OTO", "Resmi Tatil (Saat)|OTO", "Bayram Tatil (Gün)|OTO", "Bayram Tatil (Saat)|OTO",
                "Ücretli İzin (Gün)|OTO", "Ücretli İzin (Saat)|OTO", "Eksik Çalışma Saati|OTO",
                "Fazla Mesai 1 (Saat)|OTO", "Fazla Mesai 2 (Saat)|OTO", "Fazla Mesai 3 (Saat)|OTO",
                "Çocuk Yardımı|", "Çocuk Yardımı (0-6 Yaş)|" });
            Hazir("Link", "", new[] {
                "İŞ YERİ|SABIT:1", "BÖLÜM|", "SİCİL|SICIL", "AY|AY", "YIL|YIL", "BRD NO|SABIT:1",
                "NORMAL GÜN|OTO", "NORMAL SAAT|OTO", "HAFT.TAT.GÜN|OTO", "HAF.TAT.SAAT|OTO", "GEN.TAT.GÜN|OTO", "GEN.TAT.SAAT|OTO",
                "ÜCRETL.İZİN GÜN|OTO", "ÜCRETL.İZİN SAAT|OTO", "MAZERET İZİN GÜN|OTO", "MAZERET.İZİN SAAT|OTO",
                "HASTA GÜN|OTO", "HASTA SAAT|OTO", "FAZ.M.1|OTO", "FAZ.MES.2|OTO", "GECE VARD.|OTO", "EKSİK GÜN KODU|", "EKSİK GÜN|OTO" });
        }

        static void Hazir(string ad, string ustSatir, string[] sutunlar)
        {
            string yol = Path.Combine(Klasor, ad + ".sablon");
            if (File.Exists(yol)) return;
            var s = new Sablon { Ad = ad, UstSatir = ustSatir };
            foreach (var x in sutunlar) { var p = x.Split('|'); s.Sutunlar.Add(new SablonSutun { Baslik = p[0], Kaynak = p[1] }); }
            s.Kaydet();
        }

        public static Sablon OrnektenOlustur(string yol)
        {
            var t = Tablo.Oku(yol);
            var s = new Sablon { Ad = Path.GetFileNameWithoutExtension(yol) };
            foreach (var b in t.Basliklar) s.Sutunlar.Add(new SablonSutun { Baslik = b, Kaynak = "OTO" });
            return s;
        }
    }

    internal sealed class SablonSutun { public string Baslik = "", Kaynak = ""; }

    internal sealed class KaynakSecenek
    {
        public string Kod, Ad;
        public override string ToString() { return Ad; }
    }

    internal static class SablonUygula
    {
        public static List<KaynakSecenek> Secenekler(List<PuantajKolon> kolonlar)
        {
            var l = new List<KaynakSecenek>
            {
                new KaynakSecenek { Kod = "", Ad = "(boş bırak)" },
                new KaynakSecenek { Kod = "TC", Ad = "TC Kimlik No" },
                new KaynakSecenek { Kod = "SICIL", Ad = "Sicil No" },
                new KaynakSecenek { Kod = "KART", Ad = "Kart No" },
                new KaynakSecenek { Kod = "AD", Ad = "Adı" },
                new KaynakSecenek { Kod = "SOYAD", Ad = "Soyadı" },
                new KaynakSecenek { Kod = "ADSOYAD", Ad = "Adı Soyadı" },
                new KaynakSecenek { Kod = "FIRMAKOD", Ad = "Firma kodu (PDKS)" },
                new KaynakSecenek { Kod = "AY", Ad = "Dönem ayı" },
                new KaynakSecenek { Kod = "YIL", Ad = "Dönem yılı" },
                new KaynakSecenek { Kod = "SIRA", Ad = "Sıra no" },
                new KaynakSecenek { Kod = "SABIT", Ad = "Sabit değer (yandaki kutu)" },
            };
            foreach (var k in kolonlar) l.Add(new KaynakSecenek { Kod = "ALAN:" + k.Sql, Ad = "Puantaj: " + k.Baslik });
            return l;
        }

        // Başlıktan, hangi puantaj alanının yazılacağını tahmin eder. Bulamazsa boş.
        public static string Tahmin(string baslik, List<PuantajKolon> kolonlar)
        {
            string a = Metin.Anahtar(baslik);
            if (a.Contains("TCKIMLIK") || a == "TC" || a == "TCNO" || a == "TCKN" || a.Contains("KIMLIKNO")) return "TC";
            if (a.Contains("SICIL") && !a.Contains("SGK")) return "SICIL";
            if (a.Contains("KARTNO")) return "KART";
            if (a.Contains("ADSOYAD") || a.Contains("ADISOYADI") || a == "PERSONELADI" || a == "ADSOYADI") return "ADSOYAD";
            if (a == "SOYAD" || a == "SOYADI") return "SOYAD";
            if (a == "AD" || a == "ADI") return "AD";
            if (a == "AY") return "AY";
            if (a == "YIL") return "YIL";

            string tur = Tur(a);
            if (tur == null) return "";
            bool saat = a.Contains("SAAT") || ((tur.StartsWith("FM") || tur == "GECE") && !a.Contains("GUN"));
            if (tur == "NORMAL" && !saat && kolonlar.Any(k => k.Sql == "ncgun")) return "ALAN:ncgun";
            var aday = kolonlar.FirstOrDefault(k => k.Saat == saat && Tur(Metin.Anahtar(k.Baslik)) == tur);
            return aday == null ? "" : "ALAN:" + aday.Sql;
        }

        static string Tur(string a)
        {
            if (a.Contains("PRIMGUN")) return null;
            for (int n = 1; n <= 5; n++)
            {
                string d = n.ToString();
                if (a.Contains("FAZLAMESAI" + d) || a.Contains("FAZM" + d) || a.Contains("FAZMES" + d) || a.Contains("FM" + d)) return "FM" + d;
            }
            if (a.Contains("FAZLAMESAI") || a.StartsWith("FM")) return "FM1";
            if (a.Contains("GECE")) return "GECE";
            if (a.Contains("BAYRAM")) return "BAYRAM";
            if (a.Contains("RESMI") || a.Contains("GENTAT") || a.Contains("GENELTATIL")) return "RT";
            if (a.Contains("HAFT") || a.StartsWith("HT")) return "HT";
            if (a.Contains("UCRETSIZ")) return "UCRETSIZ";
            if (a.Contains("UCRETL")) return "UIZIN";
            if (a.Contains("YILLIK")) return "UIZIN";
            if (a.Contains("MAZERET")) return "MAZERET";
            if (a.Contains("HASTA") || a.Contains("RAPOR")) return "HASTA";
            if (a.Contains("DEVAMSIZ") || (a.Contains("EKSIK") && a.Contains("GUN"))) return "DEVAMSIZ";
            if (a.Contains("EKSIK")) return "EKSIK";
            if (a.Contains("NORMAL") || a.StartsWith("NC")) return "NORMAL";
            return null;
        }

        public static PuantajSonuc Uygula(Sablon s, PuantajHam h)
        {
            var kaynak = s.Sutunlar.Select(c => c.Kaynak == "OTO" ? Tahmin(c.Baslik, h.Kolonlar) : c.Kaynak).ToList();
            var indeks = h.Kolonlar.Select((k, i) => new { k.Sql, i }).ToDictionary(x => x.Sql, x => x.i, StringComparer.OrdinalIgnoreCase);
            var bas = PdksVeri.Trh(h.Donem.Bas) ?? DateTime.Today;
            var sonuc = new PuantajSonuc { UstSatir = s.UstSatir };
            sonuc.Basliklar.AddRange(s.Sutunlar.Select(c => c.Baslik));
            int sira = 0;
            foreach (var p in h.Kisiler)
            {
                sira++;
                if (!Metin.TcGecerli(p.Tc)) sonuc.TcEksik++;
                var satir = new object[kaynak.Count];
                for (int i = 0; i < kaynak.Count; i++)
                {
                    string k = kaynak[i] ?? "";
                    switch (k)
                    {
                        case "": satir[i] = null; break;
                        case "TC": satir[i] = p.Tc; break;
                        case "SICIL": satir[i] = p.Sicil; break;
                        case "KART": satir[i] = p.Kart; break;
                        case "AD": satir[i] = p.Ad; break;
                        case "SOYAD": satir[i] = p.Soyad; break;
                        case "ADSOYAD": satir[i] = (p.Ad + " " + p.Soyad).Trim(); break;
                        case "FIRMAKOD": satir[i] = p.FirmaKod; break;
                        case "AY": satir[i] = (decimal)bas.Month; break;
                        case "YIL": satir[i] = (decimal)bas.Year; break;
                        case "SIRA": satir[i] = (decimal)sira; break;
                        default:
                            if (k.StartsWith("SABIT:")) { satir[i] = k.Substring(6); break; }
                            int ix;
                            if (k.StartsWith("ALAN:") && indeks.TryGetValue(k.Substring(5), out ix))
                            {
                                decimal v = p.Degerler[ix];
                                if (h.Kolonlar[ix].Saat && s.SaatBicimi == "SaatDakika")
                                {
                                    int dk = (int)Math.Round(Math.Abs(v) * 60m);
                                    satir[i] = (v < 0 ? "-" : "") + (dk / 60).ToString("00") + ":" + (dk % 60).ToString("00");
                                }
                                else satir[i] = v;
                            }
                            break;
                    }
                }
                sonuc.Satirlar.Add(satir);
            }
            return sonuc;
        }
    }

    // Şablon düzenleme penceresi.
    internal sealed class SablonForm : Form
    {
        readonly Sablon _s;
        readonly List<PuantajKolon> _kolonlar;
        readonly List<KaynakSecenek> _secenek;
        readonly TextBox txtAd = new TextBox { Width = 220 }, txtUst = new TextBox { Width = 160 };
        readonly ComboBox cmbSaat = new ComboBox { Width = 170, DropDownStyle = ComboBoxStyle.DropDownList };
        readonly DataGridView g = new DataGridView { Dock = DockStyle.Fill, AllowUserToAddRows = false, RowHeadersVisible = false, BackgroundColor = Color.White, BorderStyle = BorderStyle.None, EditMode = DataGridViewEditMode.EditOnEnter };

        public SablonForm(Sablon s, List<PuantajKolon> kolonlar)
        {
            _s = s; _kolonlar = kolonlar; _secenek = SablonUygula.Secenekler(kolonlar);
            Text = "Muhasebe aktarım şablonu"; Width = 900; Height = 620; StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 9f);

            var ust = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 38, Padding = new Padding(6, 7, 6, 0), WrapContents = false };
            cmbSaat.Items.AddRange(new object[] { "Ondalık saat (7,5)", "Saat:dakika (07:30)" });
            cmbSaat.SelectedIndex = s.SaatBicimi == "SaatDakika" ? 1 : 0;
            txtAd.Text = s.Ad; txtUst.Text = s.UstSatir;
            ust.Controls.AddRange(new Control[] { L("Program adı:"), txtAd, L("Başlık üstü satır:"), txtUst, L("Saatler:"), cmbSaat });

            var alt = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 42, Padding = new Padding(6, 7, 6, 0), FlowDirection = FlowDirection.LeftToRight };
            var bEkle = B("Satır ekle", 85); var bSil = B("Satır sil", 75); var bYukari = B("▲", 35); var bAsagi = B("▼", 35);
            var bOto = B("Boşları otomatik eşleştir", 170);
            var bKaydet = B("Kaydet", 90); bKaydet.BackColor = Color.FromArgb(220, 252, 231);
            var bIptal = B("İptal", 75);
            alt.Controls.AddRange(new Control[] { bEkle, bSil, bYukari, bAsagi, bOto, bKaydet, bIptal });

            var not = new Label { Dock = DockStyle.Bottom, Height = 34, Padding = new Padding(6, 2, 6, 0), ForeColor = Color.DimGray,
                Text = "Sütun başlıkları muhasebe programının beklediği sırada olmalı. Her sütun için PDKS'den yazılacak bilgiyi seçin. " +
                       "Muhasebe sicil yerine TC istiyorsa 'TC Kimlik No' seçin." };

            g.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Muhasebedeki sütun başlığı", Width = 280 });
            var kc = new DataGridViewComboBoxColumn { HeaderText = "PDKS'den yazılacak", Width = 380, DisplayMember = "Ad", ValueMember = "Kod", FlatStyle = FlatStyle.Flat };
            kc.DataSource = _secenek;
            g.Columns.Add(kc);
            g.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Sabit değer", Width = 120 });
            g.DataError += (o, e) => { e.ThrowException = false; };

            foreach (var c in s.Sutunlar) SatirEkle(c.Baslik, c.Kaynak == "OTO" ? SablonUygula.Tahmin(c.Baslik, kolonlar) : c.Kaynak);

            Controls.Add(g); Controls.Add(not); Controls.Add(alt); Controls.Add(ust);
            bEkle.Click += delegate { SatirEkle("Yeni sütun", ""); };
            bSil.Click += delegate { if (g.CurrentRow != null) g.Rows.Remove(g.CurrentRow); };
            bYukari.Click += delegate { Tasi(-1); };
            bAsagi.Click += delegate { Tasi(1); };
            bOto.Click += delegate
            {
                foreach (DataGridViewRow r in g.Rows)
                    if (string.IsNullOrEmpty(r.Cells[1].Value as string)) r.Cells[1].Value = SablonUygula.Tahmin(r.Cells[0].Value as string ?? "", _kolonlar);
            };
            bKaydet.Click += delegate { Kaydet(); };
            bIptal.Click += delegate { DialogResult = DialogResult.Cancel; };
        }

        void SatirEkle(string baslik, string kaynak)
        {
            string sabit = "";
            if (kaynak != null && kaynak.StartsWith("SABIT:")) { sabit = kaynak.Substring(6); kaynak = "SABIT"; }
            if (!_secenek.Any(x => x.Kod == kaynak)) kaynak = "";
            g.Rows.Add(baslik, kaynak, sabit);
        }

        void Tasi(int yon)
        {
            if (g.CurrentRow == null) return;
            int i = g.CurrentRow.Index, j = i + yon;
            if (j < 0 || j >= g.Rows.Count) return;
            var r = g.Rows[i];
            var vals = new object[] { r.Cells[0].Value, r.Cells[1].Value, r.Cells[2].Value };
            g.Rows.RemoveAt(i);
            g.Rows.Insert(j, vals);
            g.CurrentCell = g.Rows[j].Cells[0];
        }

        void Kaydet()
        {
            g.EndEdit();
            if (txtAd.Text.Trim().Length == 0) { MessageBox.Show(this, "Program adını yazın."); return; }
            _s.Ad = txtAd.Text.Trim();
            _s.UstSatir = txtUst.Text.Trim();
            _s.SaatBicimi = cmbSaat.SelectedIndex == 1 ? "SaatDakika" : "Ondalik";
            _s.Sutunlar.Clear();
            foreach (DataGridViewRow r in g.Rows)
            {
                string b = (r.Cells[0].Value as string ?? "").Trim();
                if (b.Length == 0) continue;
                string k = r.Cells[1].Value as string ?? "";
                if (k == "SABIT") k = "SABIT:" + (r.Cells[2].Value as string ?? "").Trim();
                _s.Sutunlar.Add(new SablonSutun { Baslik = b, Kaynak = k });
            }
            try { _s.Kaydet(); DialogResult = DialogResult.OK; }
            catch (Exception ex) { MessageBox.Show(this, ex.Message, "Hata"); }
        }

        static Label L(string s) { return new Label { Text = s, AutoSize = true, Padding = new Padding(4, 4, 0, 0) }; }
        static Button B(string s, int w) { return new Button { Text = s, Width = w, Height = 28 }; }
    }
}
