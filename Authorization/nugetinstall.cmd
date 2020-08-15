@echo off
setlocal

:: Configuration
set lib=Authorization
set version=0.8.0

:: Settings
set nuget=nuget.exe
::set nuget=dotnet nuget
set configuration=Debug
set libPath=bin\%configuration%
set apikey=
set nugetRepo=C:\Nuget\repo
set sourcepath=C:\Projects\ChrisSolutions\CafeLib\%lib%

@echo on
%nuget% push %sourcepath%\CafeLib.%lib%.Hash\%libPath%\CafeLib.%lib%.Hash.%version%.nupkg %apikey% -source %nugetRepo%
%nuget% push %sourcepath%\CafeLib.%lib%.Identity\%libPath%\CafeLib.%lib%.Identity.%version%.nupkg %apikey% -source %nugetRepo%
%nuget% push %sourcepath%\CafeLib.%lib%.Tokens\%libPath%\CafeLib.%lib%.Tokens.%version%.nupkg %apikey% -source %nugetRepo%

@echo off

endlocal
