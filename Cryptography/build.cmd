@echo off
setlocal
if '%root%' == '' set root=..

:: Type
set type=Cryptography

:: Settings
call %root%\build\buildenv %*
if ERRORLEVEL 1 goto error
set solution=CafeLib.%type%
set sourcepath=%root%\%type%

:: Setup libraries.
rem set libs=%solution%
set libs=
set libs=%libs% %solution%.BouncyCastle
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
