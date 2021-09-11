@echo off
setlocal
if '%root%' == '' set root=..\..

:: Type
set type=BsvSharp
set location=Enterprise\%type%

:: Settings
call %root%\build\buildenv %*
if ERRORLEVEL 1 goto error
set solution=CafeLib.%type%
set sourcepath=%root%\%location%

:: Setup libraries.
set libs=%solution%
::

:: Run script to build the libraries
call %root%\build\buildlibs
if ERRORLEVEL 1 goto error

:: Package BouncyCastle.
set solution=CafeLib.%type%.BouncyCastle
set sourcepath=%root%\%location%\libs\%solution%

echo Create Nuget Packages for %solution% ...
echo %pack% %sourcepath%\%solution%.csproj -p:Version=%version% -p:PackageVersion=%version% -p:Configuration=%configuration%
%pack% %sourcepath%\%solution%.csproj -p:Version=%version% -p:PackageVersion=%version% -p:Configuration=%configuration%
if ERRORLEVEL 1 goto error

echo Push Packages to Nuget repository ...
echo %nuget% push %sourcepath%\%libPath%\%solution%.%version%.nupkg %apiswitch% -s %nugetServer% %skipdup%
if '%debug%' == '' %nuget% push %sourcepath%\%libPath%\%solution%.%version%.nupkg %apiswitch% -s %nugetServer% %skipdup%
if ERRORLEVEL 1 goto error

:: Package Secp256k1 to Nuget.
set solution=CafeLib.Secp256k1
set sourcepath=%root%\%location%\libs\%solution%

echo %nugetpack% %sourcepath%\%solution%.nuspec -Version %version% -Properties Configuration=%configuration% -OutputDirectory %sourcepath%\%libPath%
%nugetpack% %sourcepath%\%solution%.nuspec -Version %version% -Properties Configuration=%configuration% -OutputDirectory %sourcepath%\%libPath%

echo %nuget% push %sourcepath%\%libPath%\%solution%.%version%.nupkg %apiswitch% -s %nugetServer% %skipdup%
if '%debug%' == '' %nuget% push %sourcepath%\%libPath%\%solution%.%version%.nupkg %apiswitch% -s %nugetServer% %skipdup%
if ERRORLEVEL 1 goto error

:exit
endlocal
exit /b 0

:error
endlocal
exit /b 1
