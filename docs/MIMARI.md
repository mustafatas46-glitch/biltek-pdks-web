# Biltek PDKS — Mimari

Bu belge sistemin nasıl kurulduğunu ve yeni özelliklerin nereye ekleneceğini anlatır.
Kod değiştikçe güncellenir.

## 1. Genel bakış

```
 Tarayıcı (Chrome/Edge, ileride mobil)
        │  https
        ▼
 ┌────────────────────────────── BiltekPdks.Web ──────────────────────────────┐
 │ Blazor ekranları (Components/Pages)   Giriş/çıkış uçları (Endpoints)       │
 │ FirmaDb: oturumdaki firmaya kilitli veritabanı bağlamı                     │
 └──────────────────────────────┬─────────────────────────────────────────────┘
                                ▼
 ┌─────────────────────────── BiltekPdks.Infrastructure ──────────────────────┐
 │ PdksDbContext (+ SQLite / MySQL alt sınıfları ve migration'ları)           │
 │ Servisler: Giriş, Kurulum, Puantaj, Aktarım, (Bordro, Rapor, Yedek …)      │
 └──────────────────────────────┬─────────────────────────────────────────────┘
                                ▼
 ┌───────────────────────────── BiltekPdks.Domain ────────────────────────────┐
 │ Varlıklar (Personel, Hareket, İzin, Vardiya …)                             │
 │ Saf hesap motorları: PuantajMotoru (ileride BordroMotoru)                  │
 └────────────────────────────────────────────────────────────────────────────┘

 Yan araçlar: tools/BiltekPdks.Aktarim (eski Biltek/Azper verisini aktarır)
 İleride:     Cihaz ajanı (terminallerden hareket toplar) · Mobil uygulama · API
```

**Kural:** bağımlılık yalnızca aşağı doğrudur. Domain hiçbir şeye bağlı değildir; hesap kuralları
veritabanı ve ekrandan bağımsız test edilir.

## 2. Katmanlar

| Katman | Ne içerir | Ne içermez |
|---|---|---|
| **Domain** | Varlıklar, enumlar, hesap motorları (girdi → çıktı, yan etkisiz) | EF, HTTP, ekran |
| **Infrastructure** | EF Core bağlamı, tablo ayarları, migration'lar, servisler | Ekran kodu |
| **Web** | Blazor sayfaları, oturum, menü, kısayollar | İş kuralı (servise taşınır) |
| **Tests** | Motor, servis, migration ve uçtan uca testler | — |

## 3. Çok firmalı yapı (kiracı izolasyonu)

- Firmaya ait her tablo `FirmaEntity`'den türer ve `FirmaId` taşır.
- `PdksDbContext` sorgulara otomatik filtre ekler: yalnızca `AktifFirmaId` firmasının kayıtları görünür.
- Yeni kayda `FirmaId` otomatik yazılır; başka firmaya yazma `FirmaIzolasyonHatasi` ile engellenir.
- Ekranlar veritabanını yalnızca `FirmaDb.AcAsync()` ile açar (firma oturumdan gelir).
- Filtreyi kapatmak (`FirmaFiltresiKapali`) yalnızca giriş, kurulum ve aktarım için serbesttir.

Yeni bir firmaya ait tablo eklerken: `FirmaEntity`'den türet → `PdksDbContext`'e `DbSet` ve
`FirmaKurallari<T>()` ekle → yapılandırma sınıfı yaz → migration üret.

## 4. Veritabanı ve sürüm yönetimi

- **SQLite:** tek bilgisayarlık kurulum (Windows paketi, `pdks.db`).
- **MySQL 8 (utf8mb4):** sunucu / bulut, çok kullanıcı.
- Her sağlayıcının kendi migration'ı vardır: `Infrastructure/Migrations/Sqlite` ve `/MySql`.
- Uygulama açılırken `VeritabaniGuncelleyici` bekleyen migration'ları uygular; veri silinmez.
  İlk paketlerle (EnsureCreated) kurulmuş SQLite dosyaları otomatik "ilk şema" sayılır.

**Model değişince migration üretme:**
GitHub → Actions → *build* → *Run workflow* → `migration` alanına ad yaz (örn. `SicilKartiAlanlari`).
İş, iki sağlayıcı için migration'ı üretip `migration/<ad>` dalına koyar; `main`'e birleştirilir.
`MigrationTestleri` model ile migration'lar uyuşmazsa derlemeyi kırar — unutulamaz.

## 5. Güvenlik

- Şifreler PBKDF2 (ASP.NET Identity) ile saklanır; 5 hatalı denemede 15 dk kilit.
- Çerez tabanlı oturum, tüm formlarda antiforgery, yalnızca site içi yönlendirme.
- Roller: Yönetici, İnsan Kaynakları, Bölüm Sorumlusu, Personel. (Ekran bazlı yetki: 1b.) Veri yedeği (Araçlar → Data Yedekle) yalnızca yöneticiye açıktır.
- İşlem kaydı: ekranlardan yapılan her ekleme/değişiklik/silme `islem_kaydi` tablosuna (kullanıcı, zaman, eski→yeni değer) yazılır; şifre gibi gizli alanlar maskelenir. Toplu aktarım kayıt düşmez.
- Elle girilen hareketler silinmez, iptal edilir; açıklama ve kaydeden kullanıcı saklanır.

## 6. Puantaj hesabı

`PuantajMotoru` bir günü hesaplar (giriş, çıkış, plan, mola, tolerans → N.Ç, %50, %100, geç,
erken, eksik). `PuantajServisi` ayın verisini toplar, okutmaları günlere dağıtır (gece vardiyası
dahil) ve motoru çalıştırır. Kurallar şu an vardiya bazlıdır; eski programla birebir sonuç için
**grup hesap kuralları** (mesai eşiği, cumartesi kuralı, hafta tatili kredisi, ceza katsayıları)
1. dalgada eklenecek.

## 7. Modüller ve yol haritası

| Dalga | Modül | Durum |
|---|---|---|
| 0 | Giriş, firma, tanımlar, personel, hareket, izin, vardiya, resmi tatil, puantaj motoru, aktarım, ana ekran, migration | ✅ |
| 1 | Sicil kartı (5 sekme) · ek tanımlar · eski menü düzeni ve sağ tık menüleri · toplu giriş-çıkış / izin / kazanç-kesinti · grup tanımlama (plan, dönem, resmi tatil) · toplu ve kişisel puantaj + Excel · 30 gün hesabı · işlem kaydı · yedekleme | ✅ |
| 1b | Grup hesap kuralları (ceza katsayıları, cumartesi kuralı, yuvarlama) · maaş pusulası ve kişisel bordro · bordro programı aktarımı · kullanıcı ve ekran yetkileri · Windows masaüstü penceresi | sırada |
| 2 | Personel/yönetici self-servis · izin talep/onay · fazla mesai onayı · duyurular | |
| 3 | Mobil ekranlar · dinamik QR · konum doğrulaması | |
| 4 | Cihaz ajanı (ZKTeco vb. SDK) / ADMS · dış API | |

## 8. Doğrulama (CI)

Her gönderimde GitHub Actions:
1. Derler (uyarı sayısını raporlar), 2. tüm testleri çalıştırır (gerçek MySQL 5.5 ile eski şema
okuma testi dahil), 3. Windows paketini üretip gerçek Windows'ta açar, 4. örnek veriyle
ekran görüntüleri alır (`ekran-goruntuleri` dalı).

## 9. Arayüz kuralları

- Menü ve ekran adları eski Biltek PDKS v3 ile aynıdır; kullanıcı alıştığı yerden bulur.
- Her tabloda sağ tık menüsü vardır (`SagTikMenu` bileşeni). Yeni tablo eklerken menüsünü de ekleyin.
- Ana ekran kısayolları: F2 yeni, F3 düzenle, F4 sil, F5 bilgi formu, F8 puantaj, F9 kişisel puantaj, F10 Excel rapor, Esc kapat.
- Toplu işlem ekranları `PersonelCokluSecici` bileşenini kullanır.
- Masaüstü (Windows) sürümü aynı Blazor bileşenlerini kendi penceresinde açacaktır; ekran kodu iki sürümde ortaktır.
