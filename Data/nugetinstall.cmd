@echo off
setlocal

:: Type
set type=Data

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
set libs=%libs% %solution%.Mapping 
set libs=%libs% %solution%.SqlGenerator
set libs=%libs% %solution%.Sources 
set libs=%libs% %solution%.Sources.Sqlite
set libs=%libs% %solution%.Sources.SqlServer

echo Create Nuget Package for %solution% ...
for %%X in (%libs%) DO @echo on&&%msbld% %sourcepath%\%%X\%%X.csproj -t:pack -p:PackageVersion=%version% -p:Configuration=%configuration%&&@echo off

echo Push Package to Nuget repository ...
for %%X in (%libs%) DO @echo on&&%nuget% push %sourcepath%\%%X\%libPath%\%%X.%version%.nupkg %apikey% -source %nugetRepo%&&@echo off
goto exit

:usage
echo nugetinstall -v ^<version number^> [-c ^<configuration^> Debug is default]
goto exit

:exit
endlocal
