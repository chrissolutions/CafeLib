@echo off
setlocal
if '%root%' == '' set root=..

:: Type
set type=Data

:: Settings
call ..\build\buildenv %*
if ERRORLEVEL 1 goto error
set solution=CafeLib.%type%
set sourcepath=%root%\%type%

:: Setup libraries.
set libs=%solution%
set libs=%libs% %solution%.Mapping 
set libs=%libs% %solution%.SqlGenerator
set libs=%libs% %solution%.Sources 
set libs=%libs% %solution%.Sources.Sqlite
set libs=%libs% %solution%.Sources.SqlServer
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
