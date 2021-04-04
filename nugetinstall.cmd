@echo off
setlocal

set configuration=Debug
set version=

:: Parse arguments
if '%1' == '' goto usage
:nextarg
set arg=%1
if '%arg%' == '' goto start
if '%arg%' == '-v' set version=%2&&shift&&shift&&goto nextarg
if '%arg%' == '/v' set version=%2&&shift&&shift&&goto nextarg
if '%arg%' == '-c' set configuration=%2&&shift&&shift&&goto nextarg
if '%arg%' == '/c' set configuration=%1&&shift&&shift&&goto nextarg
goto usage

:start
if '%version' == '' goto usage
if '%configuration' == '' goto usage

set libs=
set libs=%libs% Core
set libs=%libs% Data
set libs=%libs% Authorization
set libs=%libs% Network
set libs=%libs% Mobile

for %%X in (%libs%) DO @echo on&&pushd %%X&&cd&&call nugetinstall.cmd -v %version% -c %configuration%&&popd&&@echo off
goto exit

:usage
echo nugetinstall -v ^<version number^> [-c ^<configuration^> Debug is default]
goto exit

:exit
endlocal
