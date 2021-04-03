@echo off
setlocal

if '%1' == '' echo missing version && goto exit

:: Configuration
set lib=Mobile
set version=%1

:: Settings
set msbld=msbuild.exe
set nuget=nuget.exe
set configuration=Debug
set libPath=bin\%configuration%
set apikey=
set nugetRepo=C:\Nuget\repo
set sourcepath=C:\Projects\ChrisSolutions\CafeLib\%lib%
set solution=CafeLib.%lib%

set libs=%solution%
set libs=%libs% %solution%.Android
set libs=%libs% %solution%.iOS
set libs=%libs% %solution%.Test.Core

echo Create Nuget Package for %solution% ...
@echo on
for %%X in (%libs%) DO %msbld% %sourcepath%\%%X\%%X.csproj -t:pack -p:PackageVersion=%version%
@echo off

echo Push Package to Nuget repository ...
@echo on
for %%X in (%libs%) DO %nuget% push %sourcepath%\%%X\%libPath%\%%X.%version%.nupkg %apikey% -source %nugetRepo%
@echo off

:exit
endlocal
