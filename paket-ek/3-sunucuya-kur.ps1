# Biltek PDKS - bulut sunucu kurulumu / güncellemesi (Windows Server)
# Yönetici olarak açılmış PowerShell'de çalıştırın:
#   powershell -ExecutionPolicy Bypass -File .\3-sunucuya-kur.ps1
# Tekrar çalıştırmak güvenlidir: hizmet varsa durdurulur, program dosyaları yenilenir, veriler (veri klasörü) korunur.

param(
    [string]$Klasor = "C:\BiltekPdks",
    [int]$Port = 80,
    [string]$YonetimFirmaKodu = "BILTEK",
    [string]$YonetimFirmaAdi = "Biltek Yazilim"
)

$ErrorActionPreference = "Stop"
$hizmet = "BiltekPdks"
$kaynak = Join-Path $PSScriptRoot "web"

$kimlik = [Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()
if (-not $kimlik.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
    Write-Host "Bu dosyayi YONETICI olarak calistirin (PowerShell'e sag tik > Yonetici olarak calistir)." -ForegroundColor Red
    exit 1
}
if (-not (Test-Path (Join-Path $kaynak "BiltekPdks.Web.exe"))) {
    Write-Host "web klasoru bulunamadi. Bu dosyayi paketin icindeki BiltekPdks klasorunden calistirin." -ForegroundColor Red
    exit 1
}

$web = Join-Path $Klasor "web"
$veri = Join-Path $Klasor "veri"
$ayar = Join-Path $web "appsettings.Production.json"
New-Item -ItemType Directory -Force -Path $web, $veri | Out-Null

$var = Get-Service -Name $hizmet -ErrorAction SilentlyContinue
if ($var) {
    Write-Host "Hizmet durduruluyor..."
    Stop-Service -Name $hizmet -Force
    Start-Sleep -Seconds 3
}

# Program dosyalari kopyalanir; sunucu ayari (appsettings.Production.json) korunur.
Write-Host "Program dosyalari kopyalaniyor: $web"
robocopy $kaynak $web /MIR /XF appsettings.Production.json /NFL /NDL /NJH /NJS /NP | Out-Null
if ($LASTEXITCODE -ge 8) { Write-Host "Kopyalama hatasi ($LASTEXITCODE)" -ForegroundColor Red; exit 1 }

$ilkKurulum = -not (Test-Path $ayar)
if ($ilkKurulum) {
    Write-Host ""
    Write-Host "ILK KURULUM. Yonetim firmasi: $YonetimFirmaKodu ($YonetimFirmaAdi)"
    $s1 = Read-Host "Yonetim firmasi 'admin' sifresi (en az 8 karakter, harf ve rakam)" -AsSecureString
    $sifre = [Runtime.InteropServices.Marshal]::PtrToStringAuto([Runtime.InteropServices.Marshal]::SecureStringToBSTR($s1))
    $katalog = (Join-Path $veri "katalog.db")
    $sablon = (Join-Path $veri "{kod}\pdks.db")
    $json = [ordered]@{
        Urls = "http://0.0.0.0:$Port"
        Veritabani = [ordered]@{ Saglayici = "Sqlite"; Mod = "FirmaBasina" }
        ConnectionStrings = [ordered]@{ Katalog = "Data Source=$katalog"; FirmaSablonu = "Data Source=$sablon" }
        Kurulum = [ordered]@{ FirmaKodu = $YonetimFirmaKodu; FirmaAdi = $YonetimFirmaAdi; YoneticiSifresi = $sifre; DemoVeri = $false }
        Sistem = [ordered]@{ YonetimFirmasi = $YonetimFirmaKodu }
    }
    $json | ConvertTo-Json -Depth 5 | Set-Content -Path $ayar -Encoding UTF8
}

if (-not $var) {
    Write-Host "Windows hizmeti olusturuluyor: $hizmet"
    New-Service -Name $hizmet -BinaryPathName "`"$web\BiltekPdks.Web.exe`"" -DisplayName "Biltek PDKS" -StartupType Automatic -Description "Biltek PDKS web programi" | Out-Null
    sc.exe failure $hizmet reset= 86400 actions= restart/5000/restart/5000/restart/30000 | Out-Null
}

if (-not (Get-NetFirewallRule -DisplayName "Biltek PDKS $Port" -ErrorAction SilentlyContinue)) {
    Write-Host "Guvenlik duvarinda $Port portu aciliyor"
    New-NetFirewallRule -DisplayName "Biltek PDKS $Port" -Direction Inbound -Protocol TCP -LocalPort $Port -Action Allow | Out-Null
}

Write-Host "Hizmet baslatiliyor..."
Start-Service -Name $hizmet
$ok = $false
for ($i = 0; $i -lt 60 -and -not $ok; $i++) {
    Start-Sleep -Seconds 2
    try { $r = Invoke-WebRequest -Uri "http://localhost:$Port/hesap/giris" -UseBasicParsing; $ok = $r.StatusCode -eq 200 } catch { }
}
if (-not $ok) {
    Write-Host "Program acilmadi. Olay Gunlugu > Windows Gunlukleri > Uygulama altinda 'BiltekPdks' kayitlarina bakin." -ForegroundColor Red
    exit 1
}

if ($ilkKurulum) {
    # Sifre yalnizca ilk acilista kullanilir; ayar dosyasindan silinir.
    $j = Get-Content $ayar -Raw | ConvertFrom-Json
    $j.Kurulum.YoneticiSifresi = ""
    $j | ConvertTo-Json -Depth 5 | Set-Content -Path $ayar -Encoding UTF8
}

$ip = (Get-NetIPAddress -AddressFamily IPv4 | Where-Object { $_.IPAddress -notlike "127.*" -and $_.IPAddress -notlike "169.254.*" } | Select-Object -First 1).IPAddress
Write-Host ""
Write-Host "KURULUM TAMAM." -ForegroundColor Green
Write-Host "Adres         : http://<sunucu-ip>:$Port   (sunucunun ic IP'si: $ip)"
Write-Host "Yonetim girisi: firma kodu $YonetimFirmaKodu, kullanici admin"
Write-Host "Firma acmak   : Tanimlar > Firma Islemleri (bulut)"
Write-Host "Veriler       : $veri  (her firma ayri klasorde)"
Write-Host ""
Write-Host "ONEMLI: Su an baglanti sifresiz (http). Gercek musteri verisi girmeden once alan adi + HTTPS kurulmali." -ForegroundColor Yellow
