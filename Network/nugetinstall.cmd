@echo off
setlocal

if '%1' == '' echo missing version && goto exit

:: Configuration
set lib=Network
set version=%1

:: Settings
set msbld=msbuild.exe
set nuget=nuget.exe
set configuration=Debug
set libPath=bin\%configuration%
set apikey=
set nugetRepo=C:\Nuget\repo
set sourcepath=C:\Projects\ChrisSolutions\CafeLib\%lib%

set libs=CafeLib.AspNet.WebSockets
set libs=%libs% CafeLib.Web.Request 
set libs=%libs% CafeLib.Web.SignalR
set libs=%libs% CafeLib.Web.SignalR.Hubs 

echo Create Nuget Package for %lib% ...
@echo on
for %%X in (%libs%) DO %msbld% %sourcepath%\%%X\%%X.csproj -t:pack -p:PackageVersion=%version%
@echo off

echo Push Package to Nuget repository ...
@echo on
for %%X in (%libs%) DO %nuget% push %sourcepath%\%%X\%libPath%\%%X.%version%.nupkg %apikey% -source %nugetRepo%
@echo off

:exit
endlocal
