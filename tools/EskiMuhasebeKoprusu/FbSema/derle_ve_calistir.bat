@echo off
cd /d "%~dp0"
set CSC=%WINDIR%\Microsoft.NET\Framework\v4.0.30319\csc.exe
"%CSC%" /nologo /target:exe /platform:x86 /codepage:65001 /out:FbSema.exe /r:System.dll /r:System.Core.dll FbSema.cs ..\Fb.cs > derleme_sonucu.txt 2>&1
if not exist FbSema.exe (echo DERLEME HATALI>> derleme_sonucu.txt & type derleme_sonucu.txt & pause & exit /b 1)
echo DERLEME TAMAM>> derleme_sonucu.txt
echo Veritabani inceleniyor, lutfen bekleyin...
FbSema.exe
echo Bitti. sema_raporu.txt olustu.
pause
