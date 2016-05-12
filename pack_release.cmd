@echo off
if [%1]==[] goto missingRevision

echo Packaging revision %1...
rmdir /s /q packaging_temp 1>NUL 2>NUL
mkdir packaging_temp 1>NUL 2>NUL
cd packaging_temp
mkdir ThomasJepp.SaintsRow-rev%1 1>NUL 2>NUL
mkdir ThomasJepp.SaintsRow-rev%1\x64 1>NUL 2>NUL
copy ..\license.txt ThomasJepp.SaintsRow-rev%1\ 1>NUL 2>NUL
copy ..\bin\Release\*.exe ThomasJepp.SaintsRow-rev%1\ 1>NUL 2>NUL
copy ..\bin\Release\*.dll ThomasJepp.SaintsRow-rev%1\ 1>NUL 2>NUL
copy ..\bin\Release\*.cmd ThomasJepp.SaintsRow-rev%1\ 1>NUL 2>NUL
copy ..\bin\x64\Release\*.exe ThomasJepp.SaintsRow-rev%1\x64\ 1>NUL 2>NUL
copy ..\bin\x64\Release\*.dll ThomasJepp.SaintsRow-rev%1\x64\ 1>NUL 2>NUL
copy ..\bin\x64\Release\*.cmd ThomasJepp.SaintsRow-rev%1\x64\ 1>NUL 2>NUL
mkdir ThomasJepp.SaintsRow-rev%1\external 1>NUL 2>NUL
copy ..\bin\Release\external\*.* ThomasJepp.SaintsRow-rev%1\external\ 1>NUL 2>NUL
mkdir ThomasJepp.SaintsRow-rev%1\templates 1>NUL 2>NUL
copy ..\bin\Release\templates\*.* ThomasJepp.SaintsRow-rev%1\templates\ 1>NUL 2>NUL
"C:\Program Files\7-Zip\7z.exe" a ..\ThomasJepp.SaintsRow-rev%1.zip ThomasJepp.SaintsRow-rev%1
cd ..
rmdir /s /q packaging_temp 1>NUL 2>NUL
echo Done!

GOTO end

:missingRevision
echo No revision number specified.

:end 