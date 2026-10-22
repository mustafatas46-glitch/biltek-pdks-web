@echo off
cd /d "%~dp0"
set CSC=%WINDIR%\Microsoft.NET\Framework\v4.0.30319\csc.exe
if not exist "%CSC%" set CSC=%WINDIR%\Microsoft.NET\Framework64\v4.0.30319\csc.exe
"%CSC%" /nologo /target:winexe /platform:x86 /optimize+ /codepage:65001 /define:FIREBIRD /out:BilTekMuhasebeKoprusu.exe /r:System.dll /r:System.Core.dll /r:System.Drawing.dll /r:System.Windows.Forms.dll /r:System.Xml.dll /r:System.IO.Compression.dll /r:System.IO.Compression.FileSystem.dll *.cs > derleme_sonucu.txt 2>&1
if exist BilTekMuhasebeKoprusu.exe (echo DERLEME TAMAM>> derleme_sonucu.txt) else (echo DERLEME HATALI>> derleme_sonucu.txt)
type derleme_sonucu.txt
pause
