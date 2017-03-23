@echo off

if not EXIST build mkdir build
cd build

if not EXIST Hes mkdir Hes
cd Hes

if not EXIST localization mkdir localization
if not EXIST plugins mkdir plugins
if not EXIST logs mkdir logs

if not EXIST bin mkdir bin
cd bin

cd ../../../HellionExtendedServer\bin\Debug

REM Binaries and config
xcopy HellionExtendedServer.exe ..\..\..\build\ /y
xcopy HellionExtendedServer.exe.config ..\..\..\build\ /y

REM Hes\bin
xcopy HellionExtendedServer.Common.dll ..\..\..\build\hes\bin\ /y
xcopy NLog.dll ..\..\..\build\hes\bin\ /y

cd ../../../build

echo current directory structure of HellionExtendedServer was created in the "build" folder where this was ran from, you can drop the contents of the build folder directly to your test server.
echo.
echo ------------- STRUCTURE -----------
tree /f /a 
echo -----------------------------------
echo.
echo hit any key to close!
pause