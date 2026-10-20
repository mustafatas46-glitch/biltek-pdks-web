using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace BiltekMuhasebeKoprusu
{
    internal sealed class AnaForm : Form
    {
        static readonly Color Lacivert = Color.FromArgb(30, 41, 59);
        static readonly Color Pembe = Color.FromArgb(253, 242, 248);

        readonly TextBox txtSunucu = new TextBox { Width = 110 }, txtPort = new TextBox { Width = 50 }, txtVt = new TextBox { Width = 110 },
                         txtKullanici = new TextBox { Width = 80 }, txtSifre = new TextBox { Width = 80, UseSystemPasswordChar = true },
                         txtDosya = new TextBox { Width = 430, ReadOnly = true };
        readonly ComboBox cmbFirma = new ComboBox { Width = 260, DropDownStyle = ComboBoxStyle.DropDownList };
        readonly Button btnBaglan = new Button { Text = "Bağlan", Width = 80 }, btnDosya = new Button { Text = "Muhasebe dosyası seç...", Width = 160 };
        readonly Label lblDurum = new Label { AutoSize = true, ForeColor = Color.DimGray };
        readonly TabControl sekmeler = new TabControl { Dock = DockStyle.Fill };

        readonly DataGridView gridEsit = Grid(), gridAlan = Grid(), gridPuantaj = Grid();
        readonly Label lblEsitOzet = Ozet(), lblAlanOzet = Ozet(), lblPuantajOzet = Ozet();
        readonly ComboBox cmbDurumFiltre = new ComboBox { Width = 220, DropDownStyle = ComboBoxStyle.DropDownList };
        readonly DateTimePicker dtCikis = new DateTimePicker { Width = 110, Format = DateTimePickerFormat.Short };
        readonly ComboBox cmbDonem = new ComboBox { Width = 220, DropDownStyle = ComboBoxStyle.DropDownList };
        readonly CheckBox chkBosGizle = new CheckBox { Text = "Boş sütunları gizle", Checked = true, AutoSize = true };

        Db _db;
        Dictionary<string, string> _ayar;
        List<MuhasebeKisi> _kisiler;
        List<EsitlemeSatiri> _esit = new List<EsitlemeSatiri>(), _alan = new List<EsitlemeSatiri>();
        PuantajSonuc _puantaj;

        public AnaForm()
        {
            Text = "Biltek PDKS - Muhasebe Köprüsü";
            Width = 1280; Height = 760; StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Segoe UI", 9f);
            BackColor = Color.White;
            KeyPreview = true;

            var ust = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 36, BackColor = Lacivert, Padding = new Padding(6, 6, 6, 0), WrapContents = false };
            foreach (var c in new Control[] { Etiket("Sunucu"), txtSunucu, Etiket("Port"), txtPort, Etiket("Veritabanı"), txtVt, Etiket("Kullanıcı"), txtKullanici, Etiket("Şifre"), txtSifre, btnBaglan, Etiket("Firma"), cmbFirma })
                ust.Controls.Add(c);
            var ikinci = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 34, BackColor = Pembe, Padding = new Padding(6, 5, 6, 0), WrapContents = false };
            ikinci.Controls.Add(btnDosya); ikinci.Controls.Add(txtDosya); ikinci.Controls.Add(lblDurum);

            sekmeler.TabPages.Add(SekmeEsitleme());
            sekmeler.TabPages.Add(SekmeAlanlar());
            sekmeler.TabPages.Add(SekmePuantaj());

            Controls.Add(sekmeler); Controls.Add(ikinci); Controls.Add(ust);

            btnBaglan.Click += delegate { Baglan(); };
            btnDosya.Click += delegate { DosyaSec(); };
            cmbFirma.SelectedIndexChanged += delegate { FirmaDegisti(); };
            KeyDown += (s, e) => { if (e.KeyCode == Keys.F5) { Yenile(); e.Handled = true; } };
            Shown += delegate { if (_ayar.ContainsKey("Kaynak")) Baglan(); };
            FormClosed += delegate { if (_db != null) _db.Dispose(); AyarKaydet(); };

            _ayar = Ayarlar.Oku();
            txtSunucu.Text = Al("Sunucu", "localhost"); txtPort.Text = Al("Port", "3307"); txtVt.Text = Al("Veritabani", "data2015");
            txtKullanici.Text = Al("Kullanici", "root"); txtSifre.Text = Al("Sifre", "");
            dtCikis.Value = DateTime.Today;
            Durum(_ayar.ContainsKey("Kaynak") ? "Bağlantı bilgileri okundu: " + _ayar["Kaynak"] : "Bağlantı bilgilerini girip Bağlan'a basın.");
        }

        string Al(string k, string varsayilan) { string v; return _ayar.TryGetValue(k, out v) && v.Length > 0 ? v : varsayilan; }

        // ---------------- Sekmeler ----------------

        TabPage SekmeEsitleme()
        {
            var tp = new TabPage("1. Personel Eşitleme");
            var arac = Arac();
            var btnUygula = new Button { Text = "Seçilenleri Uygula", Width = 150, BackColor = Color.FromArgb(220, 252, 231) };
            var btnHepsi = new Button { Text = "Tümünü seç", Width = 90 };
            var btnHicbiri = new Button { Text = "Seçimi kaldır", Width = 95 };
            var btnRapor = new Button { Text = "Excel'e aktar", Width = 100 };
            arac.Controls.AddRange(new Control[] { Etiket2("Göster:"), cmbDurumFiltre, btnHepsi, btnHicbiri, Etiket2("Çıkış tarihi:"), dtCikis, btnUygula, btnRapor });
            cmbDurumFiltre.SelectedIndexChanged += delegate { EsitDoldur(); };
            btnHepsi.Click += delegate { Isaretle(gridEsit, true); };
            btnHicbiri.Click += delegate { Isaretle(gridEsit, false); };
            btnUygula.Click += delegate { EsitUygula(); };
            btnRapor.Click += delegate { GridExcel(gridEsit, "Personel_Esitleme"); };
            Kolonlar(gridEsit, "Durum:150", "Muhasebe Ad Soyad:180", "Muhasebe TC:95", "PDKS Ad Soyad:180", "PDKS TC:95", "Kart No:70", "PDKS Id:55", "Açıklama:400");
            tp.Controls.Add(gridEsit); tp.Controls.Add(lblEsitOzet); tp.Controls.Add(arac);
            return tp;
        }

        TabPage SekmeAlanlar()
        {
            var tp = new TabPage("2. Muhasebeden Bilgi Al");
            var arac = Arac();
            var btnUygula = new Button { Text = "Seçilenleri PDKS'ye yaz", Width = 170, BackColor = Color.FromArgb(220, 252, 231) };
            var btnHepsi = new Button { Text = "Tümünü seç", Width = 90 };
            var btnHicbiri = new Button { Text = "Seçimi kaldır", Width = 95 };
            arac.Controls.AddRange(new Control[] { btnHepsi, btnHicbiri, btnUygula,
                Etiket2("Muhasebe dosyasında dolu olan alanlar (işe giriş, çıkış, ücret, sicil, SGK no) TC ile eşleşen kişilerde karşılaştırılır.") });
            btnHepsi.Click += delegate { Isaretle(gridAlan, true); };
            btnHicbiri.Click += delegate { Isaretle(gridAlan, false); };
            btnUygula.Click += delegate { AlanUygula(); };
            Kolonlar(gridAlan, "Ad Soyad:200", "TC:95", "Kart No:70", "Alan:130", "PDKS'deki:130", "Muhasebedeki:130", "PDKS Id:55");
            tp.Controls.Add(gridAlan); tp.Controls.Add(lblAlanOzet); tp.Controls.Add(arac);
            return tp;
        }

        TabPage SekmePuantaj()
        {
            var tp = new TabPage("3. Puantajı Muhasebeye Gönder");
            var arac = Arac();
            var btnGetir = new Button { Text = "Puantajı getir", Width = 110 };
            var btnExcel = new Button { Text = "Excel (.xlsx) kaydet", Width = 140, BackColor = Color.FromArgb(220, 252, 231) };
            var btnCsv = new Button { Text = "CSV kaydet", Width = 90 };
            arac.Controls.AddRange(new Control[] { Etiket2("Dönem:"), cmbDonem, chkBosGizle, btnGetir, btnExcel, btnCsv });
            btnGetir.Click += delegate { PuantajGetir(); };
            btnExcel.Click += delegate { PuantajKaydet(false); };
            btnCsv.Click += delegate { PuantajKaydet(true); };
            tp.Controls.Add(gridPuantaj); tp.Controls.Add(lblPuantajOzet); tp.Controls.Add(arac);
            return tp;
        }

        // ---------------- Bağlantı ----------------

        void Baglan()
        {
            Islem(delegate
            {
                if (_db != null) { _db.Dispose(); _db = null; }
                int port;
                if (!int.TryParse(txtPort.Text, out port)) throw new Exception("Port sayı olmalı.");
                _db = new Db(txtSunucu.Text.Trim(), port, txtVt.Text.Trim(), txtKullanici.Text.Trim(), txtSifre.Text);
                if (!_db.TabloVar("personel")) throw new Exception(txtVt.Text + " bir Biltek PDKS veritabanı değil (personel tablosu yok).");
                var firmalar = PdksVeri.Firmalar(_db);
                cmbFirma.Items.Clear();
                cmbFirma.Items.Add(new Firma { Id = 0, Ad = "(Tüm firmalar)" });
                foreach (var f in firmalar) cmbFirma.Items.Add(f);
                string kayitli = Al("FirmaId", "");
                int sec = 0;
                for (int i = 0; i < cmbFirma.Items.Count; i++) if (((Firma)cmbFirma.Items[i]).Id.ToString() == kayitli) sec = i;
                if (sec == 0 && firmalar.Count == 1) sec = 1;
                cmbFirma.SelectedIndex = sec;
                Durum("Bağlandı: " + txtVt.Text + " (" + firmalar.Count + " firma)");
                AyarKaydet();
            });
        }

        int? FirmaId { get { var f = cmbFirma.SelectedItem as Firma; return f == null || f.Id == 0 ? (int?)null : f.Id; } }

        void FirmaDegisti()
        {
            Islem(delegate
            {
                cmbDonem.Items.Clear();
                foreach (var d in Puantaj.Donemler(_db, FirmaId)) cmbDonem.Items.Add(d);
                if (cmbDonem.Items.Count > 0) cmbDonem.SelectedIndex = 0;
            });
            Yenile();
        }

        void DosyaSec()
        {
            using (var dlg = new OpenFileDialog { Filter = "Muhasebe / IKYS listesi (*.xlsx;*.csv;*.txt)|*.xlsx;*.csv;*.txt|Tüm dosyalar|*.*", Title = "Muhasebe programından alınan personel listesi" })
            {
                string son = Al("SonDosya", "");
                if (son.Length > 0 && Directory.Exists(Path.GetDirectoryName(son))) dlg.InitialDirectory = Path.GetDirectoryName(son);
                if (dlg.ShowDialog(this) != DialogResult.OK) return;
                txtDosya.Text = dlg.FileName;
                _ayar["SonDosya"] = dlg.FileName;
            }
            Yenile();
        }

        void Yenile()
        {
            if (_db == null || txtDosya.Text.Length == 0) return;
            Islem(delegate
            {
                var tablo = Tablo.Oku(txtDosya.Text);
                _kisiler = Esitleme.Kisiler(tablo);
                var pdks = PdksVeri.Personeller(_db, FirmaId);
                _esit = Esitleme.Karsilastir(_kisiler, pdks);
                _alan = Esitleme.AlanFarklari(_kisiler, pdks);
                var secili = cmbDurumFiltre.SelectedItem as string;
                cmbDurumFiltre.Items.Clear();
                cmbDurumFiltre.Items.Add("(Tümü)");
                cmbDurumFiltre.Items.Add("(Sorunlu olanlar)");
                foreach (var g in _esit.GroupBy(x => x.Durum).OrderBy(g => g.Key)) cmbDurumFiltre.Items.Add(g.Key);
                cmbDurumFiltre.SelectedItem = secili != null && cmbDurumFiltre.Items.Contains(secili) ? secili : "(Sorunlu olanlar)";
                AlanDoldur();
                Durum("Muhasebe: " + _kisiler.Count + " kişi   PDKS (" + cmbFirma.Text + "): " + pdks.Count(p => p.Aktif) + " aktif kişi");
            });
        }

        // ---------------- 1. Eşitleme ----------------

        void EsitDoldur()
        {
            string f = cmbDurumFiltre.SelectedItem as string ?? "(Tümü)";
            gridEsit.Rows.Clear();
            foreach (var s in _esit)
            {
                if (f == "(Sorunlu olanlar)" && s.Durum == "Eşleşti") continue;
                if (f != "(Tümü)" && f != "(Sorunlu olanlar)" && s.Durum != f) continue;
                int i = gridEsit.Rows.Add(s.Islem != BiltekMuhasebeKoprusu.Islem.Yok && s.Uygula, s.Durum, s.MuhasebeAdSoyad, s.MuhasebeTc, s.PdksAdSoyad, s.PdksTc, s.PdksKartNo, s.PdksId, s.Aciklama);
                var row = gridEsit.Rows[i];
                row.Tag = s;
                if (s.Islem == BiltekMuhasebeKoprusu.Islem.Yok) row.Cells[0].ReadOnly = true;
                row.DefaultCellStyle.BackColor = Renk(s.Durum);
            }
            var ozet = _esit.GroupBy(x => x.Durum).OrderByDescending(g => g.Count()).Select(g => g.Key + ": " + g.Count());
            lblEsitOzet.Text = string.Join("    ", ozet);
        }

        static Color Renk(string durum)
        {
            if (durum == "Eşleşti") return Color.White;
            if (durum.StartsWith("Eşleşti")) return Color.FromArgb(254, 249, 195);
            if (durum.StartsWith("TC eksik")) return Color.FromArgb(219, 234, 254);
            if (durum == "PDKS'de yok") return Color.FromArgb(220, 252, 231);
            if (durum == "Muhasebede yok") return Color.FromArgb(243, 232, 255);
            return Color.FromArgb(254, 226, 226);
        }

        void EsitUygula()
        {
            if (_db == null) return;
            gridEsit.EndEdit();
            var secilen = gridEsit.Rows.Cast<DataGridViewRow>().Where(r => true.Equals(r.Cells[0].Value) && r.Tag is EsitlemeSatiri)
                                  .Select(r => (EsitlemeSatiri)r.Tag).Where(s => s.Islem != BiltekMuhasebeKoprusu.Islem.Yok).ToList();
            if (secilen.Count == 0) { Bilgi("Uygulanacak satır seçilmedi."); return; }
            int tc = secilen.Count(s => s.Islem == BiltekMuhasebeKoprusu.Islem.TcYaz), ekle = secilen.Count(s => s.Islem == BiltekMuhasebeKoprusu.Islem.Ekle),
                isim = secilen.Count(s => s.Islem == BiltekMuhasebeKoprusu.Islem.IsimDuzelt), pasif = secilen.Count(s => s.Islem == BiltekMuhasebeKoprusu.Islem.Pasife);

            string grupId = null, durum = null, grupAdi = "";
            if (ekle > 0)
            {
                if (!FirmaId.HasValue) { Bilgi("Yeni personel eklemek için üstten bir firma seçin."); return; }
                grupId = PdksVeri.EnCok(_db, "grupid", FirmaId);
                durum = PdksVeri.EnCok(_db, "durum", FirmaId);
                grupAdi = PdksVeri.GrupAdi(_db, grupId);
            }
            var mesaj = "Eski PDKS veritabanında (" + txtVt.Text + ") şu değişiklikler yapılacak:\r\n\r\n";
            if (tc > 0) mesaj += "• " + tc + " kişiye TC kimlik no yazılacak\r\n";
            if (isim > 0) mesaj += "• " + isim + " kişinin adı soyadı muhasebeye göre düzeltilecek\r\n";
            if (ekle > 0) mesaj += "• " + ekle + " yeni personel eklenecek (firma: " + cmbFirma.Text + ", grup: " + grupAdi + ", kart no boş)\r\n";
            if (pasif > 0) mesaj += "• " + pasif + " kişiye " + dtCikis.Value.ToString("dd.MM.yyyy") + " tarihiyle işten çıkış verilecek\r\n";
            mesaj += "\r\nGeri alma dosyası GeriAlma klasörüne yazılacak. Devam edilsin mi?";
            if (MessageBox.Show(this, mesaj, "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            Islem(delegate
            {
                using (var geri = new GeriAlma("personel_esitleme", txtVt.Text))
                {
                    _db.Yaz("START TRANSACTION");
                    try
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
                                    string ad, soyad;
                                    Metin.AdSoyadBol(s.MuhasebeAdSoyad, out ad, out soyad);
                                    geri.Ekle("UPDATE personel SET ad = " + Db.Q(p.Ad) + ", soyad = " + Db.Q(p.Soyad) + " WHERE id = " + p.Id + ";");
                                    _db.Yaz("UPDATE personel SET ad = " + Db.Q(ad) + ", soyad = " + Db.Q(soyad) + " WHERE id = " + p.Id);
                                    break;
                                case BiltekMuhasebeKoprusu.Islem.Pasife:
                                    geri.Ekle("UPDATE personel SET istencikistarihi = " + Db.QTarih(p.IstenCikis) + " WHERE id = " + p.Id + ";");
                                    _db.Yaz("UPDATE personel SET istencikistarihi = " + Db.QTarih(dtCikis.Value.Date) + " WHERE id = " + p.Id);
                                    break;
                                case BiltekMuhasebeKoprusu.Islem.Ekle:
                                    string a, sy;
                                    Metin.AdSoyadBol(s.MuhasebeAdSoyad, out a, out sy);
                                    var k = s.Kisi;
                                    _db.Yaz("INSERT INTO personel (kartfc, kartno, kartid, ad, soyad, kimlikno, sicilno, firmaid, grupid, durum, isegiristarihi, ucret) VALUES ('', '', '', " +
                                            Db.Q(a) + ", " + Db.Q(sy) + ", " + Db.Q(s.MuhasebeTc) + ", " + Db.Q(k.SicilNo) + ", " + FirmaId.Value + ", " +
                                            (grupId == null ? "NULL" : grupId) + ", " + Db.Q(durum) + ", " + Db.QTarih(k.IseGiris) + ", " + Db.QSayi(k.Ucret) + ")");
                                    geri.Ekle("DELETE FROM personel WHERE id = " + _db.SonId() + ";  -- " + s.MuhasebeAdSoyad);
                                    break;
                            }
                        }
                        _db.Yaz("COMMIT");
                    }
                    catch
                    {
                        _db.Yaz("ROLLBACK");
                        geri.Ekle("-- HATA: islem iptal edildi, veritabani DEGISMEDI. Bu dosyayi calistirmayin.");
                        throw;
                    }
                    Bilgi(secilen.Count + " değişiklik kaydedildi.\r\nGeri alma dosyası: " + geri.Yol);
                }
            });
            Yenile();
        }

        // ---------------- 2. Alanlar ----------------

        void AlanDoldur()
        {
            gridAlan.Rows.Clear();
            foreach (var s in _alan)
            {
                int i = gridAlan.Rows.Add(s.Uygula, s.PdksAdSoyad, s.MuhasebeTc, s.PdksKartNo, s.Alan, s.EskiDeger, s.YeniDeger, s.PdksId);
                gridAlan.Rows[i].Tag = s;
            }
            lblAlanOzet.Text = _alan.Count == 0 ? "Fark yok (ya da muhasebe dosyasında ek bilgi sütunu yok)." :
                string.Join("    ", _alan.GroupBy(x => x.Alan).Select(g => g.Key + ": " + g.Count()));
        }

        void AlanUygula()
        {
            if (_db == null) return;
            gridAlan.EndEdit();
            var secilen = gridAlan.Rows.Cast<DataGridViewRow>().Where(r => true.Equals(r.Cells[0].Value)).Select(r => (EsitlemeSatiri)r.Tag).ToList();
            if (secilen.Count == 0) { Bilgi("Seçili satır yok."); return; }
            if (MessageBox.Show(this, secilen.Count + " alan PDKS'de muhasebedeki değerle güncellenecek. Devam edilsin mi?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            Islem(delegate
            {
                using (var geri = new GeriAlma("bilgi_guncelleme", txtVt.Text))
                {
                    _db.Yaz("START TRANSACTION");
                    try
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
                        _db.Yaz("COMMIT");
                    }
                    catch { _db.Yaz("ROLLBACK"); geri.Ekle("-- HATA: islem iptal edildi, veritabani DEGISMEDI. Bu dosyayi calistirmayin."); throw; }
                    Bilgi(geri.Sayi + " alan güncellendi.\r\nGeri alma dosyası: " + geri.Yol);
                }
            });
            Yenile();
        }

        // ---------------- 3. Puantaj ----------------

        void PuantajGetir()
        {
            var d = cmbDonem.SelectedItem as Donem;
            if (_db == null || d == null) { Bilgi("Önce bağlanın ve dönem seçin. Dönem listesi boşsa eski programda puantajı hesaplatın."); return; }
            Islem(delegate
            {
                _puantaj = Puantaj.Getir(_db, d, FirmaId, chkBosGizle.Checked);
                gridPuantaj.Columns.Clear();
                gridPuantaj.Rows.Clear();
                foreach (var b in _puantaj.Basliklar) gridPuantaj.Columns.Add(b, b);
                foreach (DataGridViewColumn c in gridPuantaj.Columns) { c.SortMode = DataGridViewColumnSortMode.Automatic; c.ReadOnly = true; }
                foreach (var r in _puantaj.Satirlar)
                {
                    int i = gridPuantaj.Rows.Add(r);
                    if (((string)r[r.Length - 1]).Length > 0) gridPuantaj.Rows[i].DefaultCellStyle.BackColor = Color.FromArgb(254, 226, 226);
                }
                lblPuantajOzet.Text = _puantaj.Satirlar.Count + " kişi" + (_puantaj.TcEksik > 0 ? "   •   " + _puantaj.TcEksik + " kişinin TC'si eksik/hatalı (kırmızı) - 1. sekmeden tamamlayın" : "   •   Tüm kişilerin TC'si tamam") +
                                       "   •   Saatler ondalık (7:30 = 7,5)";
            });
        }

        void PuantajKaydet(bool csv)
        {
            if (_puantaj == null) PuantajGetir();
            if (_puantaj == null) return;
            var d = (Donem)cmbDonem.SelectedItem;
            string ad = "Puantaj_" + Temiz(cmbFirma.Text) + "_" + d.ToString().Replace(" - ", "_").Replace(".", "");
            using (var dlg = new SaveFileDialog { FileName = ad + (csv ? ".csv" : ".xlsx"), Filter = csv ? "CSV (*.csv)|*.csv" : "Excel (*.xlsx)|*.xlsx" })
            {
                if (dlg.ShowDialog(this) != DialogResult.OK) return;
                Islem(delegate
                {
                    if (csv) CsvYaz(dlg.FileName, _puantaj.Basliklar, _puantaj.Satirlar);
                    else XlsxYazici.Yaz(dlg.FileName, "Puantaj", _puantaj.Basliklar, _puantaj.Satirlar);
                    Durum("Kaydedildi: " + dlg.FileName);
                    if (MessageBox.Show(this, "Dosya kaydedildi. Açılsın mı?", "Puantaj", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        System.Diagnostics.Process.Start(dlg.FileName);
                });
            }
        }

        static void CsvYaz(string yol, IList<string> basliklar, IEnumerable<object[]> satirlar)
        {
            var tr = new System.Globalization.CultureInfo("tr-TR");
            using (var w = new StreamWriter(yol, false, new System.Text.UTF8Encoding(true)))
            {
                w.WriteLine(string.Join(";", basliklar.Select(Csv)));
                foreach (var r in satirlar)
                    w.WriteLine(string.Join(";", r.Select(v => v is decimal ? ((decimal)v).ToString("0.##", tr) : Csv(v == null ? "" : v.ToString()))));
            }
        }

        static string Csv(string s) { return s.IndexOfAny(new[] { ';', '"', '\n' }) >= 0 ? "\"" + s.Replace("\"", "\"\"") + "\"" : s; }

        void GridExcel(DataGridView g, string ad)
        {
            using (var dlg = new SaveFileDialog { FileName = ad + "_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx", Filter = "Excel (*.xlsx)|*.xlsx" })
            {
                if (dlg.ShowDialog(this) != DialogResult.OK) return;
                Islem(delegate
                {
                    var bas = g.Columns.Cast<DataGridViewColumn>().Skip(1).Select(c => c.HeaderText).ToList();
                    var sat = g.Rows.Cast<DataGridViewRow>().Select(r => r.Cells.Cast<DataGridViewCell>().Skip(1).Select(c => c.Value == null ? null : (object)c.Value.ToString()).ToArray());
                    XlsxYazici.Yaz(dlg.FileName, "Rapor", bas, sat);
                    Durum("Kaydedildi: " + dlg.FileName);
                });
            }
        }

        // ---------------- Yardımcılar ----------------

        void Islem(Action a)
        {
            Cursor = Cursors.WaitCursor;
            try { a(); }
            catch (DllNotFoundException) { Hata("libmySQL.dll bulunamadı. Bu programı eski Biltek PDKS klasörüne (biltek.exe'nin yanına) kopyalayın."); }
            catch (BadImageFormatException) { Hata("libmySQL.dll ile program uyumsuz (32/64 bit). Programı 32 bit derleyin."); }
            catch (Exception ex) { Hata(ex.Message); }
            finally { Cursor = Cursors.Default; }
        }

        void AyarKaydet()
        {
            _ayar["Sunucu"] = txtSunucu.Text; _ayar["Port"] = txtPort.Text; _ayar["Veritabani"] = txtVt.Text; _ayar["Kullanici"] = txtKullanici.Text;
            if (FirmaId.HasValue) _ayar["FirmaId"] = FirmaId.Value.ToString();
            Ayarlar.Kaydet(_ayar);
        }

        void Durum(string s) { lblDurum.Text = s; }
        void Bilgi(string s) { MessageBox.Show(this, s, "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        void Hata(string s) { MessageBox.Show(this, s, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning); Durum("Hata: " + s.Split('\n')[0]); }

        static string Temiz(string s)
        {
            foreach (var c in Path.GetInvalidFileNameChars()) s = s.Replace(c, '_');
            return s.Replace(" ", "_").Replace("(", "").Replace(")", "");
        }

        static DataGridView Grid()
        {
            var g = new DataGridView
            {
                Dock = DockStyle.Fill, AllowUserToAddRows = false, AllowUserToDeleteRows = false, RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect, BackgroundColor = Color.White, BorderStyle = BorderStyle.None,
                AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None, EnableHeadersVisualStyles = false
            };
            g.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(241, 245, 249);
            g.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            g.DefaultCellStyle.SelectionBackColor = Color.FromArgb(191, 219, 254);
            g.DefaultCellStyle.SelectionForeColor = Color.Black;
            var menu = new ContextMenuStrip();
            menu.Items.Add("Hücreyi kopyala", null, delegate { if (g.CurrentCell != null && g.CurrentCell.Value != null) Clipboard.SetText(g.CurrentCell.Value.ToString()); });
            menu.Items.Add("Seçili satırları işaretle", null, delegate { foreach (DataGridViewRow r in g.SelectedRows) if (g.Columns.Count > 0 && g.Columns[0] is DataGridViewCheckBoxColumn && !r.Cells[0].ReadOnly) r.Cells[0].Value = true; });
            menu.Items.Add("Seçili satırların işaretini kaldır", null, delegate { foreach (DataGridViewRow r in g.SelectedRows) if (g.Columns.Count > 0 && g.Columns[0] is DataGridViewCheckBoxColumn) r.Cells[0].Value = false; });
            g.ContextMenuStrip = menu;
            return g;
        }

        static void Kolonlar(DataGridView g, params string[] tanimlar)
        {
            g.Columns.Add(new DataGridViewCheckBoxColumn { HeaderText = "Uygula", Width = 55 });
            foreach (var t in tanimlar)
            {
                var p = t.Split(':');
                g.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = p[0], Width = int.Parse(p[1]), ReadOnly = true });
            }
        }

        static void Isaretle(DataGridView g, bool deger)
        {
            foreach (DataGridViewRow r in g.Rows) if (!r.Cells[0].ReadOnly) r.Cells[0].Value = deger;
        }

        static FlowLayoutPanel Arac() { return new FlowLayoutPanel { Dock = DockStyle.Top, Height = 36, Padding = new Padding(4, 5, 4, 0), WrapContents = false }; }
        static Label Ozet() { return new Label { Dock = DockStyle.Bottom, Height = 26, Padding = new Padding(6, 5, 0, 0), BackColor = Color.FromArgb(248, 250, 252) }; }
        static Label Etiket(string s) { return new Label { Text = s, AutoSize = true, ForeColor = Color.White, Padding = new Padding(4, 4, 0, 0) }; }
        static Label Etiket2(string s) { return new Label { Text = s, AutoSize = true, Padding = new Padding(4, 5, 0, 0) }; }
    }

    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new AnaForm());
        }
    }
}
