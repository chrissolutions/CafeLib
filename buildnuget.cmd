@echo on
setlocal

set apikey=oy2didvqmsgou2rjhdxamlnijbmytzlefgg4jmyd453iwa
rem set nugetServer=https://api.nuget.org/v3/index.json
set nugetServer=C:/Nuget/repo
set debugswitch=--nugetDebug=true

dotnet cake --build=Publish --config=Release --nugetKey=%apikey% --nugetServer=%nugetServer% --nugetSkipDup=true %debugswitch%
if ERRORLEVEL 1 goto error

endlocal
exit /b 0

:error
endlocal
exit /b 1
