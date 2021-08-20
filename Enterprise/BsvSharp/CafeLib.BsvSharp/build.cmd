@echo off
setlocal

:: Type
set type=BsvSharp

:: Settings
call ..\..\..\build\buildenv %*
if ERRORLEVEL 1 goto error
set sourcepath=.
set solution=CafeLib.%type%

:: Setup libraries.
set libs=%solution%.Identity
set libs=%libs% %solution%.Tokens
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
