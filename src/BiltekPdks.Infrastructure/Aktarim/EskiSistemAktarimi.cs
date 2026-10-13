using System.Globalization;
using BiltekPdks.Domain.Entities;
using BiltekPdks.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BiltekPdks.Infrastructure.Aktarim;

public sealed class AktarimRaporu
{
    public Dictionary<string, int> Eklenen { get; } = new();
    public Dictionary<string, int> Atlanan { get; } = new();
    public List<string> Uyarilar { get; } = new();

    internal void Ekle(string tur, int adet = 1) => Eklenen[tur] = Eklenen.GetValueOrDefault(tur) + adet;
    internal void Atla(string neden, int adet = 1) => Atlanan[neden] = Atlanan.GetValueOrDefault(neden) + adet;

    public override string ToString()
    {
        var s = new System.Text.StringBuilder();
        s.AppendLine("Eklenen:");
        foreach (var (k, v) in Eklenen.OrderBy(x => x.Key)) s.AppendLine($"  {k,-28} {v,8}");
        if (Atlanan.Count > 0)
        {
            s.AppendLine("Atlanan:");
            foreach (var (k, v) in Atlanan.OrderBy(x => x.Key)) s.AppendLine($"  {k,-48} {v,8}");
        }
        if (Uyarilar.Count > 0)
        {
            s.AppendLine("Uyarılar:");
            foreach (var u in Uyarilar.Take(50)) s.AppendLine("  - " + u);
            if (Uyarilar.Count > 50) s.AppendLine($"  ... ve {Uyarilar.Count - 50} uyarı daha");
        }
        return s.ToString();
    }
}

/// <summary>
/// Eski Biltek verisini yeni sisteme, verilen firmaya aktarır.
/// Tekrar çalıştırılabilir: daha önce aktarılan kayıtlar (EskiId / aynı tarih-saat) yeniden eklenmez.
/// </summary>
public static class EskiSistemAktarimi
{
    private const int Parti = 2000;

    public static async Task<AktarimRaporu> AktarAsync(PdksDbContext db, int firmaId, EskiVeri eski, CancellationToken ct = default)
    {
        if (db.AktifFirmaId != firmaId)
            throw new InvalidOperationException("Hedef bağlam aktarılacak firmaya ayarlanmalı.");

        var rapor = new AktarimRaporu();

        var subeler = await TanimlariAktarAsync(db, db.Subeler, eski.Subeler, () => new Sube(), "Şube", rapor, ct);
        var bolumler = await TanimlariAktarAsync(db, db.Bolumler, eski.Bolumler, () => new Bolum(), "Bölüm", rapor, ct);
        var gorevler = await TanimlariAktarAsync(db, db.Gorevler, eski.Gorevler, () => new Gorev(), "Görev", rapor, ct);
        var gruplar = await GruplariAktarAsync(db, eski.Gruplar, rapor, ct);
        var izinTurleri = await IzinTurleriniAktarAsync(db, eski.IzinTurleri, rapor, ct);
        var personeller = await PersonelleriAktarAsync(db, eski.Personeller, subeler, bolumler, gorevler, gruplar, rapor, ct);
        await TatilleriAktarAsync(db, eski.Tatiller, rapor, ct);
        await IzinleriAktarAsync(db, eski.Izinler, personeller, izinTurleri, rapor, ct);
        await HareketleriAktarAsync(db, eski.GirisCikislar, personeller, rapor, ct);

        return rapor;
    }

    private static async Task<Dictionary<int, int>> TanimlariAktarAsync<T>(
        PdksDbContext db, DbSet<T> set, List<EskiTanim> eski, Func<T> yeni, string tur, AktarimRaporu rapor, CancellationToken ct)
        where T : TanimEntity
    {
        var mevcut = await set.Where(x => x.EskiId != null).ToDictionaryAsync(x => x.EskiId!.Value, ct);
        foreach (var e in eski)
        {
            if (mevcut.ContainsKey(e.Id)) continue;
            if (string.IsNullOrWhiteSpace(e.Ad) || e.Ad == "0") { rapor.Atla($"{tur}: adı boş"); continue; }
            var t = yeni();
            t.EskiId = e.Id;
            t.Kod = Kisalt(e.Kod, 20);
            t.Ad = Kisalt(e.Ad, 120)!;
            set.Add(t);
            mevcut[e.Id] = t;
            rapor.Ekle(tur);
        }
        await db.SaveChangesAsync(ct);
        return mevcut.ToDictionary(x => x.Key, x => x.Value.Id);
    }

    private static async Task<Dictionary<int, int>> GruplariAktarAsync(PdksDbContext db, List<EskiGrup> eski, AktarimRaporu rapor, CancellationToken ct)
    {
        var mevcut = await db.CalismaGruplari.Where(x => x.EskiId != null).ToDictionaryAsync(x => x.EskiId!.Value, ct);
        var yeniGruplar = new List<CalismaGrubu>();
        foreach (var e in eski)
        {
            if (mevcut.ContainsKey(e.Id)) continue;
            if (string.IsNullOrWhiteSpace(e.Ad)) { rapor.Atla("Çalışma grubu: adı boş"); continue; }
            var g = new CalismaGrubu
            {
                EskiId = e.Id,
                Ad = Kisalt(e.Ad, 120)!,
                GunlukCalismaDakika = SureDakika(e.GunlukSaat) ?? 450,
                DonemlikCalismaDakika = SureDakika(e.DonemlikSaat) ?? 2700,
            };
            db.CalismaGruplari.Add(g);
            mevcut[e.Id] = g;
            yeniGruplar.Add(g);
            rapor.Ekle("Çalışma grubu");
        }
        await db.SaveChangesAsync(ct);

        // Eski sistemin vardiya kuralları birebir aktarılmaz; yeni gruplara hafta içi varsayılan plan verilir.
        if (yeniGruplar.Count > 0)
        {
            var vardiya = await db.Vardiyalar.Where(v => v.Aktif).OrderBy(v => v.Id).FirstOrDefaultAsync(ct);
            if (vardiya is null)
            {
                rapor.Uyarilar.Add("Aktif vardiya yok; aktarılan çalışma gruplarına haftalık plan verilmedi.");
            }
            else
            {
                foreach (var g in yeniGruplar)
                    foreach (var gun in Enum.GetValues<DayOfWeek>())
                        db.CalismaGrubuGunleri.Add(new CalismaGrubuGunu
                        {
                            CalismaGrubuId = g.Id, Gun = gun,
                            VardiyaId = gun is DayOfWeek.Saturday or DayOfWeek.Sunday ? null : vardiya.Id,
                        });
                await db.SaveChangesAsync(ct);
                rapor.Uyarilar.Add($"{yeniGruplar.Count} çalışma grubuna hafta içi '{vardiya.Ad}' planı verildi; gerçek vardiyaları 'Haftalık çalışma planı' ekranından kontrol edin.");
            }
        }
        return mevcut.ToDictionary(x => x.Key, x => x.Value.Id);
    }

    private static async Task<Dictionary<int, IzinTuru>> IzinTurleriniAktarAsync(PdksDbContext db, List<EskiIzinTuru> eski, AktarimRaporu rapor, CancellationToken ct)
    {
        var mevcut = await db.IzinTurleri.Where(x => x.EskiId != null).ToDictionaryAsync(x => x.EskiId!.Value, ct);
        foreach (var e in eski)
        {
            if (mevcut.ContainsKey(e.Id)) continue;
            var ad = !string.IsNullOrWhiteSpace(e.Aciklama) ? e.Aciklama : e.Ad;
            if (string.IsNullOrWhiteSpace(ad)) { rapor.Atla("İzin türü: adı boş"); continue; }
            var tip = (e.Tip ?? "").ToLowerInvariant();
            var t = new IzinTuru
            {
                EskiId = e.Id,
                Kod = Kisalt(e.Kod, 20),
                Ad = Kisalt(ad, 120)!,
                Ucretli = !(tip.Contains("ücretsiz") || tip.Contains("ucretsiz") || ad.ToLowerInvariant().Contains("ücretsiz")),
                YillikIzindenDuser = ad.ToLowerInvariant().Contains("yıllık") || ad.ToLowerInvariant().Contains("yillik"),
            };
            db.IzinTurleri.Add(t);
            mevcut[e.Id] = t;
            rapor.Ekle("İzin türü");
        }
        await db.SaveChangesAsync(ct);
        return mevcut;
    }

    private static async Task<Dictionary<long, int>> PersonelleriAktarAsync(
        PdksDbContext db, List<EskiPersonel> eski,
        Dictionary<int, int> subeler, Dictionary<int, int> bolumler, Dictionary<int, int> gorevler, Dictionary<int, int> gruplar,
        AktarimRaporu rapor, CancellationToken ct)
    {
        var mevcut = await db.Personeller.ToListAsync(ct);
        var eskiIdIle = mevcut.Where(p => p.EskiId != null).ToDictionary(p => (long)p.EskiId!.Value, p => p.Id);
        var kartlar = mevcut.Select(p => p.KartNo).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var eklenenler = new List<(long EskiId, Personel P)>();

        foreach (var e in eski)
        {
            if (eskiIdIle.ContainsKey(e.Id)) continue;

            var kart = (e.KartNo ?? "").Trim();
            if (kart.Length == 0) { rapor.Atla("Personel: kart no boş"); continue; }
            if (kartlar.Contains(kart))
            {
                rapor.Atla("Personel: kart no başka personelde var");
                rapor.Uyarilar.Add($"Kart {kart} (eski id {e.Id}, {e.Ad} {e.Soyad}) atlandı: aynı kart numarası zaten var.");
                continue;
            }
            if (e.IseGirisTarihi is null)
                rapor.Uyarilar.Add($"Kart {kart}: işe giriş tarihi yok, 01.01.2000 yazıldı.");

            var tc = e.KimlikNo?.Trim();
            if (tc is not null && !TcKimlik.GecerliMi(tc))
            {
                rapor.Uyarilar.Add($"Kart {kart}: T.C. kimlik no geçersiz ({tc.Length} hane), boş bırakıldı.");
                tc = null;
            }

            var p = new Personel
            {
                EskiId = checked((int)e.Id),
                KartNo = Kisalt(kart, 20)!,
                SicilNo = Kisalt(e.SicilNo, 30),
                Ad = Kisalt(string.IsNullOrWhiteSpace(e.Ad) ? "?" : e.Ad, 60)!,
                Soyad = Kisalt(string.IsNullOrWhiteSpace(e.Soyad) ? "?" : e.Soyad, 60)!,
                TcKimlikNo = tc,
                Cinsiyet = CinsiyetCevir(e.Cinsiyet),
                DogumTarihi = Tarih(e.DogumTarihi),
                IseGirisTarihi = Tarih(e.IseGirisTarihi) ?? new DateOnly(2000, 1, 1),
                IstenCikisTarihi = Tarih(e.IstenCikisTarihi),
                IstenAyrilmaNedeni = Kisalt(e.IstenAyrilmaNedeni, 500),
                CepTelefonu = Kisalt(e.CepTel, 30),
                Eposta = Kisalt(e.Email, 150),
                Adres = Kisalt(e.Adres, 500),
                SubeId = Esle(subeler, e.SubeId),
                BolumId = Esle(bolumler, e.BolumId),
                GorevId = Esle(gorevler, e.GorevId),
                CalismaGrubuId = Esle(gruplar, e.GrupId),
            };
            db.Personeller.Add(p);
            kartlar.Add(kart);
            eklenenler.Add((e.Id, p));
            rapor.Ekle("Personel");
        }
        await db.SaveChangesAsync(ct);

        foreach (var (eskiId, p) in eklenenler) eskiIdIle[eskiId] = p.Id;
        return eskiIdIle;
    }

    private static async Task TatilleriAktarAsync(PdksDbContext db, List<EskiTatil> eski, AktarimRaporu rapor, CancellationToken ct)
    {
        var mevcut = (await db.ResmiTatiller.Select(t => t.Tarih).ToListAsync(ct)).ToHashSet();
        foreach (var e in eski.GroupBy(t => DateOnly.FromDateTime(t.Tarih)).Select(g => g.First()))
        {
            var tarih = DateOnly.FromDateTime(e.Tarih);
            if (!mevcut.Add(tarih)) continue;
            db.ResmiTatiller.Add(new ResmiTatil { Tarih = tarih, Ad = Kisalt(string.IsNullOrWhiteSpace(e.Ad) ? "Resmi tatil" : e.Ad, 120)! });
            rapor.Ekle("Resmi tatil");
        }
        await db.SaveChangesAsync(ct);
    }

    private static async Task IzinleriAktarAsync(PdksDbContext db, List<EskiIzin> eski,
        Dictionary<long, int> personeller, Dictionary<int, IzinTuru> turler, AktarimRaporu rapor, CancellationToken ct)
    {
        var mevcut = (await db.Izinler.Select(i => new { i.PersonelId, i.Baslangic, i.IzinTuruId }).ToListAsync(ct))
            .Select(i => (i.PersonelId, i.Baslangic, i.IzinTuruId)).ToHashSet();

        foreach (var e in eski)
        {
            if (!personeller.TryGetValue(e.PersonelId, out var pid)) { rapor.Atla("İzin: personel aktarılmamış"); continue; }
            if (e.IzinTurId is not int eskiTur || !turler.TryGetValue(eskiTur, out var tur)) { rapor.Atla("İzin: izin türü bulunamadı"); continue; }
            var bas = Tarih(e.Baslangic);
            if (bas is null) { rapor.Atla("İzin: başlangıç tarihi yok"); continue; }
            var bit = Tarih(e.Bitis) ?? bas.Value;

            // Aynı gün içinde saat aralığı olan kayıtlar saatlik izindir; bu sürümde gün bazlı izin desteklenir.
            var saatlik = bit == bas && e.BaslangicSaati is { } s1 && e.BitisSaati is { } s2
                          && s1 > TimeSpan.Zero && s2 > s1 && (s2 - s1) < TimeSpan.FromHours(7);
            if (saatlik) { rapor.Atla("İzin: saatlik izin (sonraki sürümde)"); continue; }
            if (bit < bas) { rapor.Atla("İzin: bitiş başlangıçtan önce"); continue; }

            if (!mevcut.Add((pid, bas.Value, tur.Id))) continue;
            db.Izinler.Add(new Izin
            {
                PersonelId = pid, IzinTuruId = tur.Id, Baslangic = bas.Value, Bitis = bit,
                Aciklama = Kisalt(e.Aciklama, 250),
            });
            rapor.Ekle("İzin");
        }
        await db.SaveChangesAsync(ct);
    }

    private static async Task HareketleriAktarAsync(PdksDbContext db, List<EskiGirisCikis> eski,
        Dictionary<long, int> personeller, AktarimRaporu rapor, CancellationToken ct)
    {
        var mevcut = (await db.Hareketler.Select(h => new { h.PersonelId, h.Zaman }).ToListAsync(ct))
            .Select(h => (h.PersonelId, h.Zaman)).ToHashSet();
        var bekleyen = 0;

        void Ekle(int pid, DateTime? tarih, TimeSpan? saat, string? elle, string? terminal)
        {
            if (tarih is null || saat is null) return;
            var zaman = tarih.Value.Date + saat.Value;
            if (!mevcut.Add((pid, zaman))) { rapor.Atla("Hareket: zaten var"); return; }
            var elleMi = !string.IsNullOrWhiteSpace(elle) && elle != "0" && !elle.Equals("H", StringComparison.OrdinalIgnoreCase);
            db.Hareketler.Add(new Hareket
            {
                PersonelId = pid,
                Zaman = zaman,
                Kaynak = elleMi ? HareketKaynagi.Elle : HareketKaynagi.Aktarim,
                TerminalKodu = Kisalt(terminal, 20),
                Aciklama = elleMi ? "Eski sistemde elle girilmiş" : null,
            });
            rapor.Ekle("Hareket");
            bekleyen++;
        }

        foreach (var e in eski)
        {
            if (!personeller.TryGetValue(e.PersonelId, out var pid)) { rapor.Atla("Hareket: personel aktarılmamış"); continue; }
            if (e.GirisSaati is null && e.CikisSaati is null) { rapor.Atla("Hareket: saat yok (izin/boş satır)"); continue; }

            Ekle(pid, e.GirisTarihi, e.GirisSaati, e.ElleGiris, e.GirisTerminal);
            Ekle(pid, e.CikisTarihi ?? e.GirisTarihi, e.CikisSaati, e.ElleCikis, e.CikisTerminal);

            if (bekleyen >= Parti)
            {
                await db.SaveChangesAsync(ct);
                db.ChangeTracker.Clear();
                bekleyen = 0;
            }
        }
        await db.SaveChangesAsync(ct);
    }

    /// <summary>"07:30" ya da "7.5" gibi eski süre metnini dakikaya çevirir.</summary>
    public static int? SureDakika(string? metin)
    {
        if (string.IsNullOrWhiteSpace(metin)) return null;
        metin = metin.Trim();
        var parca = metin.Split(':');
        if (parca.Length == 2 && int.TryParse(parca[0], out var sa) && int.TryParse(parca[1], out var dk))
            return sa * 60 + dk;
        if (decimal.TryParse(metin.Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out var ondalik))
            return (int)Math.Round(ondalik * 60);
        return null;
    }

    public static Cinsiyet CinsiyetCevir(string? metin)
    {
        var m = (metin ?? "").Trim().ToLowerInvariant();
        if (m is "bayan" or "kadın" or "kadin" or "k" or "f" or "2") return Cinsiyet.Kadin;
        if (m is "bay" or "erkek" or "e" or "m" or "1") return Cinsiyet.Erkek;
        return Cinsiyet.Belirtilmemis;
    }

    private static DateOnly? Tarih(DateTime? d) => d is { } t && t.Year >= 1900 ? DateOnly.FromDateTime(t) : null;

    private static int? Esle(Dictionary<int, int> harita, int? eskiId) =>
        eskiId is int id && harita.TryGetValue(id, out var yeni) ? yeni : null;

    private static string? Kisalt(string? s, int uzunluk)
    {
        if (string.IsNullOrWhiteSpace(s)) return null;
        s = s.Trim();
        return s.Length <= uzunluk ? s : s[..uzunluk];
    }
}
