@echo off
setlocal
if '%root%' == '' set root=..

:: Type
set type=Network

:: Settings
call %root%\build\buildenv %*
if ERRORLEVEL 1 goto error
set solution=CafeLib.%type%
set sourcepath=%root%\%type%

:: Setup libraries.
set libs=CafeLib.AspNet.WebSockets
set libs=%libs% CafeLib.Web.Request 
set libs=%libs% CafeLib.Web.SignalR
set libs=%libs% CafeLib.Web.SignalR.Hubs 
::

:: Run script to build the libraries
call %root%\build\buildlibs
if ERRORLEVEL 1 goto error

:exit
endlocal
exit /b 0

:error
endlocal
exit /b 1
