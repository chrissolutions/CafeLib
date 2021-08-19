@echo off
setlocal

:: Type
set type=Network

:: Settings
call ..\build\buildenv %*
if ERRORLEVEL 1 goto error
set sourcepath=.
set solution=CafeLib.%type%

:: Setup libraries.
set libs=CafeLib.AspNet.WebSockets
set libs=%libs% CafeLib.Web.Request 
set libs=%libs% CafeLib.Web.SignalR
set libs=%libs% CafeLib.Web.SignalR.Hubs 
::

:: Run script to build the libraries
call ..\build\buildlibs
if ERRORLEVEL 1 goto error

:exit
endlocal
exit /b 0

:error
endlocal
exit /b 1
