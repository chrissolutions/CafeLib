@echo off
setlocal

:: Configuration
set lib=Data
set version=0.7.0

:: Settings
set nuget=nuget.exe
::set nuget=dotnet nuget
set configuration=Debug
set libPath=bin\%configuration%
set apikey=
set nugetRepo=C:\Nuget\repo
set sourcepath=C:\Projects\ChrisSolutions\CafeLib\%lib%

@echo on
%nuget% push %sourcepath%\CafeLib.Data\%libPath%\CafeLib.Data.%version%.nupkg %apikey% -source %nugetRepo%
%nuget% push %sourcepath%\CafeLib.Data.Dto\%libPath%\CafeLib.Data.Dto.%version%.nupkg %apikey% -source %nugetRepo%
%nuget% push %sourcepath%\CafeLib.Data.Expressions\%libPath%\CafeLib.Data.Expressions.%version%.nupkg %apikey% -source %nugetRepo%
%nuget% push %sourcepath%\CafeLib.Data.Mapping\%libPath%\CafeLib.Data.Mapping.%version%.nupkg %apikey% -source %nugetRepo%

@echo off

endlocal
