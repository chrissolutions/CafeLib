@echo off
setlocal

:: Configuration
set lib=Core
set version=0.7.5

:: Settings
set nuget=nuget.exe
::set nuget=dotnet nuget
set configuration=Debug
set libPath=bin\%configuration%
set apikey=
set nugetRepo=C:\Nuget\repo
set sourcepath=C:\Projects\ChrisSolutions\CafeLib\%lib%

@echo on
%nuget% push %sourcepath%\CafeLib.Core\%libPath%\CafeLib.Core.%version%.nupkg %apikey% -source %nugetRepo%
%nuget% push %sourcepath%\CafeLib.Core.Caching\%libPath%\CafeLib.Core.Caching.%version%.nupkg %apikey% -source %nugetRepo%
%nuget% push %sourcepath%\CafeLib.Core.Collections\%libPath%\CafeLib.Core.Collections.%version%.nupkg %apikey% -source %nugetRepo%
%nuget% push %sourcepath%\CafeLib.Core.Data\%libPath%\CafeLib.Core.Data.%version%.nupkg %apikey% -source %nugetRepo%
%nuget% push %sourcepath%\CafeLib.Core.Eventing\%libPath%\CafeLib.Core.Eventing.%version%.nupkg %apikey% -source %nugetRepo%
%nuget% push %sourcepath%\CafeLib.Core.IoC\%libPath%\CafeLib.Core.IoC.%version%.nupkg %apikey% -source %nugetRepo%
%nuget% push %sourcepath%\CafeLib.Core.Logging\%libPath%\CafeLib.Core.Logging.%version%.nupkg %apikey% -source %nugetRepo%
%nuget% push %sourcepath%\CafeLib.Core.Messaging\%libPath%\CafeLib.Core.Messaging.%version%.nupkg %apikey% -source %nugetRepo%
%nuget% push %sourcepath%\CafeLib.Core.Queueing\%libPath%\CafeLib.Core.Queueing.%version%.nupkg %apikey% -source %nugetRepo%
%nuget% push %sourcepath%\CafeLib.Core.Runnable\%libPath%\CafeLib.Core.Runnable.%version%.nupkg %apikey% -source %nugetRepo%

@echo off

endlocal
