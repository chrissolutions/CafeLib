@echo off
setlocal
if '%root%' == '' set root=..

:: Type
set type=Mobile

:: Settings
call ..\build\buildenv %*
if ERRORLEVEL 1 goto error
set solution=CafeLib.%type%
set sourcepath=%root%\%type%

:: Setup libraries.
set libs=%solution%
set libs=%libs% %solution%.Android
set libs=%libs% %solution%.iOS
set libs=%libs% %solution%.Test.Core

echo Build %solution% ...
echo %msbuild% %sourcepath%\%solution%\%solution%.csproj -p:Version=%version% -p:PackageVersion=%version% -p:Configuration=%configuration%
%msbuild% %sourcepath%\%solution%\%solution%.csproj -p:Version=%version% -p:PackageVersion=%version% -p:Configuration=%configuration%

echo %msbuild% %sourcepath%\%solution%.Test.Core\%solution%.Test.Core.csproj -p:Version=%version% -p:PackageVersion=%version% -p:Configuration=%configuration%
%msbuild% %sourcepath%\%solution%.Test.Core\%solution%.Test.Core.csproj -p:Version=%version% -p:PackageVersion=%version% -p:Configuration=%configuration%

echo %msbuild% %sourcepath%\%solution%.Android\%solution%.Android.csproj -p:Version=%version% -p:PackageVersion=%version% -p:Configuration=%configuration%
%msbuild% %sourcepath%\%solution%.Android\%solution%.Android.csproj -p:Version=%version% -p:PackageVersion=%version% -p:Configuration=%configuration%

echo %msbuild% %sourcepath%\%solution%.iOS\%solution%.iOS.csproj -p:Version=%version% -p:PackageVersion=%version% -p:Configuration=%configuration%
%msbuild% %sourcepath%\%solution%.iOS\%solution%.iOS.csproj -p:Version=%version% -p:PackageVersion=%version% -p:Configuration=%configuration%

echo Create Nuget Packages for %solution% ...
echo %pack% %sourcepath%\%solution%\%solution%.csproj -p:Version=%version% -p:PackageVersion=%version% -p:Configuration=%configuration%
%pack% %sourcepath%\%solution%\%solution%.csproj -p:Version=%version% -p:PackageVersion=%version% -p:Configuration=%configuration%

echo %pack% %sourcepath%\%solution%.Test.Core\%solution%.Test.Core.csproj -p:Version=%version% -p:PackageVersion=%version% -p:Configuration=%configuration%
%pack% %sourcepath%\%solution%.Test.Core\%solution%.Test.Core.csproj -p:Version=%version% -p:PackageVersion=%version% -p:Configuration=%configuration%

echo %nugetpack% %sourcepath%\%solution%.Android\%solution%.Android.nuspec -Version %version% -Properties Configuration=%configuration% -OutputDirectory %sourcepath%\%solution%.Android\%libPath%
%nugetpack% %sourcepath%\%solution%.Android\%solution%.Android.nuspec -Version %version% -Properties Configuration=%configuration% -OutputDirectory %sourcepath%\%solution%.Android\%libPath%

echo %nugetpack% %sourcepath%\%solution%.iOS\%solution%.iOS.nuspec -Version %version% -Properties Configuration=%configuration% -OutputDirectory %sourcepath%\%solution%.iOS\%libPath%
%nugetpack% %sourcepath%\%solution%.iOS\%solution%.iOS.nuspec -Version %version% -Properties Configuration=%configuration% -OutputDirectory %sourcepath%\%solution%.iOS\%libPath%

echo Push Packages to Nuget repository ...
for %%X in (%libs%) do (
    echo %nuget% push %sourcepath%\%%X\%libPath%\%%X.%version%.nupkg %apiswitch% -s %nugetServer% %skipdup%
    if '%debug%' == '' %nuget% push %sourcepath%\%%X\%libPath%\%%X.%version%.nupkg %apiswitch% -s %nugetServer% %skipdup%
    if ERRORLEVEL 1 goto error
)

:exit
endlocal
exit /b 0

:error
endlocal
exit /b 1
