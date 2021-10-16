@echo off
setlocal
if '%root%' == '' set root=..

:: Type
set type=Core

:: Settings
call %root%\build\buildenv %*
if ERRORLEVEL 1 goto error
set solution=CafeLib.%type%
set sourcepath=%root%\%type%

:: Setup libraries.
set libs=%solution%
set libs=%libs% %solution%.Buffers 
set libs=%libs% %solution%.Caching 
set libs=%libs% %solution%.Collections
set libs=%libs% %solution%.Collections.DirectedGraph
set libs=%libs% %solution%.Data 
set libs=%libs% %solution%.Dynamic
set libs=%libs% %solution%.Encodings
set libs=%libs% %solution%.Eventing
set libs=%libs% %solution%.FileIO
set libs=%libs% %solution%.IoC
set libs=%libs% %solution%.Logging
set libs=%libs% %solution%.Messaging
set libs=%libs% %solution%.MethodBinding
set libs=%libs% %solution%.Numerics
set libs=%libs% %solution%.Queueing
set libs=%libs% %solution%.Runnable
set libs=%libs% %solution%.Security
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
