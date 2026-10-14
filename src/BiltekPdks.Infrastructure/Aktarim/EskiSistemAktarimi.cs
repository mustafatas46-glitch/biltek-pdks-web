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
        var vardiyalar = await VardiyalariAktarAsync(db, eski, rapor, ct);
        var gruplar = await GruplariAktarAsync(db, eski.Gruplar, rapor, ct);
        await GrupPlanlariniAktarAsync(db, eski, gruplar, vardiyalar, rapor, ct);
        var altGruplar = await AltGruplariAktarAsync(db, eski, gruplar, vardiyalar, rapor, ct);
        await DonemleriAktarAsync(db, eski.Donemler, gruplar, altGruplar, rapor, ct);
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

    // ---- Günlük çalışma saatleri (vardiya), alt gruplar, haftalık planlar, dönemler ----

    private static async Task<Dictionary<int, int>> VardiyalariAktarAsync(PdksDbContext db, EskiVeri eski, AktarimRaporu rapor, CancellationToken ct)
    {
        var mevcut = await db.Vardiyalar.Where(x => x.EskiId != null).ToDictionaryAsync(x => x.EskiId!.Value, ct);
        foreach (var e in eski.PuantajTanimlari)
        {
            if (mevcut.ContainsKey(e.Id)) continue;
            var ad = string.IsNullOrWhiteSpace(e.Ad) ? $"Günlük plan {e.Id}" : e.Ad;
            var satirlar = eski.CalismaParametreleri.Where(x => x.PuantajId == e.Id).ToList();
            // Normal çalışma satırının giriş-çıkış saati vardiyanın saatidir; yoksa ilk satır alınır.
            var normal = satirlar.FirstOrDefault(x => DegerCozucu.Anahtar(x.BordroAd).Contains("NORMAL")) ?? satirlar.FirstOrDefault();
            var bas = EskiSaat(normal?.Igs);
            var bit = EskiSaat(normal?.Dcs);
            if (bas is null || bit is null)
            {
                rapor.Atla("Günlük çalışma saati: giriş/çıkış saati okunamadı");
                rapor.Uyarilar.Add($"Günlük çalışma saati \"{ad}\" (eski id {e.Id}) aktarılmadı: giriş/çıkış saati yok. Bu planı kullanan günler varsayılan plana bağlandı.");
                continue;
            }

            var molalar = new List<VardiyaMola>();
            foreach (var m in eski.Molalar.Where(x => x.PuantajId == e.Id))
            {
                if (EskiSaat(m.Baslama) is not { } mb || EskiSaat(m.Bitis) is not { } mbit || mb == mbit) continue;
                var tip = DegerCozucu.Anahtar(m.Tip);
                molalar.Add(new VardiyaMola
                {
                    Ad = Kisalt(string.IsNullOrWhiteSpace(m.Ad) ? "Mola" : m.Ad, 60)!,
                    Baslangic = mb, Bitis = mbit,
                    Ucretli = tip.Contains("UCRETLI") && !tip.Contains("UCRETSIZ"),
                });
            }

            var sure = (int)(bit.Value - bas.Value).TotalMinutes; // TimeOnly farkı gece yarısını kendisi aşar
            var gunluk = SureDakika(e.GunlukSaat);
            // Mola tanımı yoksa mola, vardiya süresi ile günlük çalışma süresinin farkıdır.
            var mola = molalar.Count > 0 ? molalar.Where(x => !x.Ucretli).Sum(x => x.Dakika)
                : gunluk is int g && g > 0 && g < sure ? sure - g : 0;

            var v = new Vardiya { EskiId = e.Id, Ad = Kisalt(ad, 120)!, Baslangic = bas.Value, Bitis = bit.Value, MolaDakika = mola, Molalar = molalar };
            db.Vardiyalar.Add(v);
            mevcut[e.Id] = v;
            rapor.Ekle("Günlük çalışma saati");
        }
        await db.SaveChangesAsync(ct);
        return mevcut.ToDictionary(x => x.Key, x => x.Value.Id);
    }

    /// <summary>Eski g1…g7 (pazartesi…pazar) planını yeni plana çevirir. Bilinmeyen plan numarası varsayılan vardiyaya bağlanır.</summary>
    private static Dictionary<DayOfWeek, int?> HaftalikPlan(int?[] gunler, Dictionary<int, int> vardiyalar, int? varsayilan, ref int bilinmeyen)
    {
        var plan = new Dictionary<DayOfWeek, int?>();
        for (var i = 0; i < 7; i++)
        {
            var gun = i == 6 ? DayOfWeek.Sunday : (DayOfWeek)(i + 1);
            int? vid = null;
            if (i < gunler.Length && gunler[i] is int eskiId && eskiId > 0)
            {
                if (vardiyalar.TryGetValue(eskiId, out var yeni)) vid = yeni;
                else { vid = varsayilan; bilinmeyen++; }
            }
            plan[gun] = vid;
        }
        return plan;
    }

    private static async Task GrupPlanlariniAktarAsync(PdksDbContext db, EskiVeri eski, Dictionary<int, int> gruplar,
        Dictionary<int, int> vardiyalar, AktarimRaporu rapor, CancellationToken ct)
    {
        var altIdler = eski.AltGruplar.Select(a => a.Id).ToHashSet();
        var planlar = eski.GunParametreleri
            .Where(g => !altIdler.Contains(g.AltGrupId) && g.GrupId is int gid && gruplar.ContainsKey(gid))
            .GroupBy(g => g.GrupId!.Value).Select(g => g.Last()).ToList();
        if (planlar.Count == 0) return;

        var varsayilan = await db.Vardiyalar.Where(v => v.Aktif && v.EskiId == null).OrderBy(v => v.Id).Select(v => (int?)v.Id).FirstOrDefaultAsync(ct);
        var bilinmeyen = 0;
        foreach (var e in planlar)
        {
            var grupId = gruplar[e.GrupId!.Value];
            var plan = HaftalikPlan(e.Gunler, vardiyalar, varsayilan, ref bilinmeyen);
            if (plan.Values.All(v => v is null)) continue;

            var mevcut = await db.CalismaGrubuGunleri.Where(x => x.CalismaGrubuId == grupId).ToListAsync(ct);
            if (mevcut.Count == 7 && mevcut.All(x => plan[x.Gun] == x.VardiyaId)) continue; // zaten aktarılmış
            // Elle değiştirilmiş planlara dokunulmaz; yalnızca boş ya da aktarımın verdiği varsayılan plan değiştirilir.
            var varsayilanPlan = mevcut.Count == 0 || mevcut.All(x =>
                x.Gun is DayOfWeek.Saturday or DayOfWeek.Sunday ? x.VardiyaId is null : x.VardiyaId == varsayilan);
            if (!varsayilanPlan)
            {
                rapor.Atla("Grup çalışma planı: yeni sistemde değiştirilmiş, dokunulmadı");
                continue;
            }
            foreach (var (gun, vid) in plan)
            {
                var k = mevcut.FirstOrDefault(x => x.Gun == gun);
                if (k is null) { k = new CalismaGrubuGunu { CalismaGrubuId = grupId, Gun = gun }; db.CalismaGrubuGunleri.Add(k); }
                k.VardiyaId = vid;
            }
            rapor.Ekle("Grup çalışma planı");
        }
        await db.SaveChangesAsync(ct);
        if (bilinmeyen > 0)
            rapor.Uyarilar.Add($"Haftalık planlarda {bilinmeyen} gün, aktarılamayan bir günlük çalışma saatine bağlıydı; varsayılan plana bağlandı.");
    }

    private static async Task<Dictionary<int, int>> AltGruplariAktarAsync(PdksDbContext db, EskiVeri eski, Dictionary<int, int> gruplar,
        Dictionary<int, int> vardiyalar, AktarimRaporu rapor, CancellationToken ct)
    {
        var mevcut = await db.AltGruplar.Where(x => x.EskiId != null).ToDictionaryAsync(x => x.EskiId!.Value, ct);
        var grupSureleri = await db.CalismaGruplari.ToDictionaryAsync(g => g.Id, g => g.GunlukCalismaDakika, ct);
        var yeniler = new List<(EskiAltGrup Eski, AltGrup Yeni)>();
        foreach (var e in eski.AltGruplar)
        {
            if (mevcut.ContainsKey(e.Id)) continue;
            if (!gruplar.TryGetValue(e.GrupId, out var grupId)) { rapor.Atla("Alt grup: grubu aktarılmamış"); continue; }
            if (string.IsNullOrWhiteSpace(e.Ad)) { rapor.Atla("Alt grup: adı boş"); continue; }
            var a = new AltGrup
            {
                EskiId = e.Id, CalismaGrubuId = grupId, Ad = Kisalt(e.Ad, 120)!,
                GunlukCalismaDakika = SureDakika(e.GunlukSaat) ?? grupSureleri.GetValueOrDefault(grupId, 450),
            };
            db.AltGruplar.Add(a);
            mevcut[e.Id] = a;
            yeniler.Add((e, a));
            rapor.Ekle("Alt grup");
        }
        await db.SaveChangesAsync(ct);

        var varsayilan = await db.Vardiyalar.Where(v => v.Aktif && v.EskiId == null).OrderBy(v => v.Id).Select(v => (int?)v.Id).FirstOrDefaultAsync(ct);
        var bilinmeyen = 0;
        foreach (var (e, a) in yeniler)
        {
            var gp = eski.GunParametreleri.LastOrDefault(x => x.AltGrupId == e.Id);
            if (gp is null) { rapor.Uyarilar.Add($"Alt grup \"{a.Ad}\" için eski programda haftalık plan yok; Grup Tanımlama ekranından girin."); continue; }
            foreach (var (gun, vid) in HaftalikPlan(gp.Gunler, vardiyalar, varsayilan, ref bilinmeyen))
                db.AltGrupGunleri.Add(new AltGrupGunu { AltGrupId = a.Id, Gun = gun, VardiyaId = vid });
        }
        await db.SaveChangesAsync(ct);
        if (bilinmeyen > 0)
            rapor.Uyarilar.Add($"Alt grup planlarında {bilinmeyen} gün, aktarılamayan bir günlük çalışma saatine bağlıydı; varsayılan plana bağlandı.");
        return mevcut.ToDictionary(x => x.Key, x => x.Value.Id);
    }

    private static async Task DonemleriAktarAsync(PdksDbContext db, List<EskiDonem> eski, Dictionary<int, int> gruplar,
        Dictionary<int, int> altGruplar, AktarimRaporu rapor, CancellationToken ct)
    {
        if (eski.Count == 0) return;
        var mevcut = (await db.DonemTanimlari.Select(d => new { d.CalismaGrubuId, d.AltGrupId, d.Baslangic }).ToListAsync(ct))
            .Select(d => (d.CalismaGrubuId, d.AltGrupId, d.Baslangic)).ToHashSet();
        var grupSureleri = await db.CalismaGruplari.ToDictionaryAsync(g => g.Id, g => g.GunlukCalismaDakika, ct);

        foreach (var e in eski)
        {
            if (!gruplar.TryGetValue(e.GrupId, out var grupId)) { rapor.Atla("Dönem: grubu aktarılmamış"); continue; }
            int? altId = null;
            if (e.AltGrupId is int ea && ea > 0)
            {
                if (!altGruplar.TryGetValue(ea, out var yeniAlt)) { rapor.Atla("Dönem: alt grubu aktarılmamış"); continue; }
                altId = yeniAlt;
            }
            var bas = Tarih(e.Baslangic);
            var bit = Tarih(e.Bitis);
            if (bas is null || bit is null || bit < bas) { rapor.Atla("Dönem: tarih yok ya da hatalı"); continue; }
            if (!mevcut.Add((grupId, altId, bas.Value))) continue;

            var d = new DonemTanimi
            {
                CalismaGrubuId = grupId, AltGrupId = altId, Baslangic = bas.Value, Bitis = bit.Value,
                GirisSaati = e.GirisSaati is { } g && g < TimeSpan.FromDays(1) ? TimeOnly.FromTimeSpan(g) : null,
                CikisSaati = e.CikisSaati is { } c && c < TimeSpan.FromDays(1) ? TimeOnly.FromTimeSpan(c) : null,
            };
            d.GunHesapla(grupSureleri.GetValueOrDefault(grupId, 450));
            // Eski programda kayıtlı değerler varsa onlar esas alınır.
            if (e.GunFarki is int gf && gf > 0) d.GunFarki = gf;
            if (e.EksikGun is int eg) d.EksikGun = Math.Max(0, eg);
            if (SureDakika(e.EksikSaat) is int es) d.EksikDakika = Math.Max(0, es);
            if (e.FazlaGun is int fg) d.FazlaGun = Math.Max(0, fg);
            if (SureDakika(e.FazlaSaat) is int fs) d.FazlaDakika = Math.Max(0, fs);
            db.DonemTanimlari.Add(d);
            rapor.Ekle("Dönem");
        }
        await db.SaveChangesAsync(ct);
    }

    /// <summary>Eski saat metni ("08:00", "8:00", "24:00") → saat. "24:00" gece yarısıdır.</summary>
    public static TimeOnly? EskiSaat(string? metin)
    {
        if (string.IsNullOrWhiteSpace(metin)) return null;
        var m = metin.Trim().Replace('.', ':');
        if (m is "24:00" or "24:00:00") return TimeOnly.MinValue;
        return TimeOnly.TryParseExact(m, new[] { "H:mm", "HH:mm", "H:mm:ss", "HH:mm:ss" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out var t) ? t : null;
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
