@echo off
setlocal

if '%1' == '' echo missing version && goto exit

:: Configuration
set lib=Core
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
set libs=%libs% %solution%.Caching 
set libs=%libs% %solution%.Collections
set libs=%libs% %solution%.Data 
set libs=%libs% %solution%.Dynamic
set libs=%libs% %solution%.Eventing
set libs=%libs% %solution%.FileIO
set libs=%libs% %solution%.Hashing
set libs=%libs% %solution%.IoC
set libs=%libs% %solution%.Logging
set libs=%libs% %solution%.Messaging
set libs=%libs% %solution%.MethodBinding
set libs=%libs% %solution%.Queueing
set libs=%libs% %solution%.Runnable

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
