# Biltek PDKS Web

Eski Biltek PDKS (Delphi + MySQL 5.1) sisteminin yerine geçecek, web tabanlı ve çok firmalı kullanıma hazır personel devam kontrol sistemi.

**Teknoloji:** .NET 8, ASP.NET Core, Blazor (Interactive Server), EF Core, MySQL 8 (geliştirmede SQLite).

## Çalıştırma

```bash
dotnet run --project src/BiltekPdks.Web
```

İlk açılışta veritabanı oluşturulur, `FIRMA` kodlu firma ve `admin` kullanıcısı eklenir.
`appsettings.json` → `Kurulum:YoneticiSifresi` boşsa geçici bir şifre üretilir ve konsola yazılır.

### MySQL 8 ile

`appsettings.json` (veya ortam değişkenleri):

```json
"ConnectionStrings": { "Pdks": "Server=localhost;Port=3306;Database=biltek_pdks;User=pdks;Password=...;" },
"Veritabani": { "Saglayici": "MySql", "SunucuSurumu": "8.0.36-mysql" }
```

## Proje yapısı

| Proje | İçerik |
|---|---|
| `BiltekPdks.Domain` | Varlıklar (Firma, Personel, Kullanıcı, tanımlar), iş kuralları |
| `BiltekPdks.Infrastructure` | EF Core bağlamı, firma izolasyonu, şifre/giriş, kurulum |
| `BiltekPdks.Web` | Blazor arayüzü, cookie oturumu |
| `BiltekPdks.Tests` | xUnit testleri (SQLite bellek içi) |

## Güvenlik ilkeleri

- Şifreler PBKDF2 ile hash'lenir (eski sistemde düz metindi); 5 hatalı denemede 15 dk kilit.
- Her firma kaydı `FirmaId` taşır; sorgular otomatik filtrelenir, başka firmaya yazma engellenir.
- Tüm formlar antiforgery korumalı; giriş sonrası yönlendirme yalnızca site içi adreslere.

## Yol haritası

1. **Temel** — giriş/yetki, firma, tanımlar, personel kartı ✅
2. **Çekirdek PDKS** — giriş-çıkış, vardiya planları, izin, resmi tatil, puantaj motoru ✅ (eski sistemden veri aktarımı ve sonuç karşılaştırması sırada)
3. **Raporlar** — günlük/aylık/kişisel puantaj, Excel
4. **Bordro/SGK** ve entegrasyon API'si
5. **Yan modüller** — WC mola, üye, dershane; yemekhane sonraki güncellemede
6. **Cihaz entegrasyonu** — yerel ajan (ZKTeco vb. SDK) / ADMS push

Eski veritabanı yapısı: `docs/eski-sema/data2015_schema.sql`.
