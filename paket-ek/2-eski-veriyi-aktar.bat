@echo off
chcp 65001 >nul
cd /d "%~dp0"
echo Eski Biltek veritabani (localhost:3307 / data2015) SALT OKUNUR okunacak ve yeni sisteme aktarilacak.
echo Once 1-programi-baslat.bat ile programi en az bir kez calistirmis olmalisiniz.
echo.
set /p DENEME=Once deneme yapilsin mi (hicbir sey kaydetmez)? [E/H]: 
set EK=
if /I "%DENEME%"=="E" set EK=--deneme
aktarim\pdks-aktarim.exe --eski "Server=localhost;Port=3307;Database=data2015;User=root;Password=root" --yeni "Data Source=%~dp0pdks.db" --saglayici Sqlite --firma FIRMA %EK%
pause
