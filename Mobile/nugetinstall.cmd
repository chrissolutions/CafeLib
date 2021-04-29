@echo off
setlocal

:: Type
set type=Mobile

:: Settings
set msbld=msbuild.exe
set nuget=nuget.exe
set configuration=Debug
set libPath=bin\%configuration%
set apikey=
set nugetRepo=C:\Nuget\repo
set sourcepath=.
set solution=CafeLib.%type%
set version=

:: Parse arguments
if '%1' == '' goto usage
:nextarg
set arg=%1
if '%arg%' == '' goto start
if '%arg%' == '-v' set version=%2&&shift&&shift&&goto nextarg
if '%arg%' == '/v' set version=%2&&shift&&shift&&goto nextarg
if '%arg%' == '-c' set configuration=%2&&shift&&shift&&goto nextarg
if '%arg%' == '/c' set configuration=%1&&shift&&shift&&goto nextarg
goto usage

:start
if '%version' == '' goto usage
if '%configuration' == '' goto usage

set libs=%solution%
set libs=%libs% %solution%.Android
set libs=%libs% %solution%.iOS
set libs=%libs% %solution%.Test.Core

echo Create Nuget Package for %solution% ...
@echo on
%msbld% %sourcepath%\%solution%\%solution%.csproj -t:pack -p:PackageVersion=%version% -p:Configuration=%configuration%
%msbld% %sourcepath%\%solution%.Test.Core\%solution%.Test.Core.csproj -t:pack -p:PackageVersion=%version% -p:Configuration=%configuration%
%msbld% %sourcepath%\%solution%.Android\%solution%.Android.csproj -p:PackageVersion=%version% -p:Configuration=%configuration%
%nuget% pack %sourcepath%\%solution%.Android\%solution%.Android.nuspec -Version %version% -Properties Configuration=%configuration% -OutputDirectory %sourcepath%\%solution%.Android\%libPath%
%msbld% %sourcepath%\%solution%.iOS\%solution%.iOS.csproj -p:PackageVersion=%version% -p:Configuration=%configuration%
%nuget% pack %sourcepath%\%solution%.iOS\%solution%.iOS.nuspec -Version %version% -Properties Configuration=%configuration% -OutputDirectory %sourcepath%\%solution%.iOS\%libPath%
@echo off

echo Push Package to Nuget repository ...
for %%X in (%libs%) DO @echo on&&%nuget% push %sourcepath%\%%X\%libPath%\%%X.%version%.nupkg %apikey% -source %nugetRepo%&&@echo off
goto exit

:usage
echo nugetinstall -v ^<version number^> [-c ^<configuration^> Debug is default]
goto exit

:exit
endlocal
