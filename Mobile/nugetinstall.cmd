@echo off
setlocal

:: Configuration
set lib=Mobile
set version=0.9.0

:: Settings
set nuget=nuget.exe
::set nuget=dotnet nuget
set configuration=Debug
set libPath=bin\%configuration%
set apikey=
set nugetRepo=C:\Nuget\repo
set sourcepath=C:\Projects\ChrisSolutions\CafeLib\%lib%

@echo on
%nuget% push %sourcepath%\CafeLib.Mobile\%libPath%\CafeLib.Mobile.%version%.nupkg %apikey% -source %nugetRepo%
%nuget% push %sourcepath%\CafeLib.Mobile.Android\%libPath%\CafeLib.Mobile.Android.%version%.nupkg %apikey% -source %nugetRepo%
%nuget% push %sourcepath%\CafeLib.Mobile.iOS\%libPath%\CafeLib.Mobile.iOS.%version%.nupkg %apikey% -source %nugetRepo%
%nuget% push %sourcepath%\CafeLib.Mobile.Test.Core\%libPath%\CafeLib.Mobile.Test.Core.%version%.nupkg %apikey% -source %nugetRepo%

@echo off

endlocal
