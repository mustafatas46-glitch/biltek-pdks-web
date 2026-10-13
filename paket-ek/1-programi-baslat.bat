@echo off
chcp 65001 >nul
cd /d "%~dp0web"
set ASPNETCORE_URLS=http://localhost:5180
set ConnectionStrings__Pdks=Data Source=%~dp0pdks.db
set Veritabani__Saglayici=Sqlite
echo Biltek PDKS baslatiliyor: http://localhost:5180
echo Ilk calistirmada yonetici sifresi asagida yazar. Kapatmak icin bu pencereyi kapatin.
start "" http://localhost:5180
BiltekPdks.Web.exe
pause
