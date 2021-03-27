@echo off
setlocal

:: Configuration
set lib=Network
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
%nuget% push %sourcepath%\CafeLib.AspNet.WebSockets\%libPath%\CafeLib.AspNet.WebSockets.%version%.nupkg %apikey% -source %nugetRepo%
%nuget% push %sourcepath%\CafeLib.Web.Request\%libPath%\CafeLib.Web.Request.%version%.nupkg %apikey% -source %nugetRepo%
%nuget% push %sourcepath%\CafeLib.Web.SignalR\%libPath%\CafeLib.Web.SignalR.%version%.nupkg %apikey% -source %nugetRepo%
%nuget% push %sourcepath%\CafeLib.Web.SignalR.Hubs\%libPath%\CafeLib.Web.SignalR.Hubs.%version%.nupkg %apikey% -source %nugetRepo%

@echo off

endlocal
