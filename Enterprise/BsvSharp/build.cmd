@echo on
setlocal
if '%root%' == '' set root=..\..

:: Type
set type=BsvSharp
set location=Enterprise\%type%

:: Settings
call %root%\build\buildenv %*
if ERRORLEVEL 1 goto error
set solution=CafeLib.%type%
set sourcepath=%root%\%location%

:: Setup libraries.
set libs=%solution%
rem set libs=%libs% %solution%.Tokens
::

:: Run script to build the libraries
call %root%\build\buildlibs
if ERRORLEVEL 1 goto error

:: Package Secp256k1 to Nuget.
set solution=CafeLib.Secp256k1
set sourcepath=%sourcepath%\libs\%solution%
echo %nuget% push %sourcepath%\%libPath%\%solution%.%version%.nupkg %apiswitch% -s %nugetServer% %skipdup%
%nuget% push %sourcepath%\%libPath%\%solution%.%version%.nupkg %apiswitch% -s %nugetServer% %skipdup%
if ERRORLEVEL 1 goto error

:exit
endlocal
exit /b 0

:error
endlocal
exit /b 1
