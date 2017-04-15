@ECHO off
setlocal

# quiet|minimal|normal|detailed|diagnostic
SET VERBOSITY=normal
# Debug|Release
SET CONFIGURATION=Release

cd WebService

dotnet restore
IF NOT ERRORLEVEL 0 GOTO FAIL

dotnet publish --output bin/docker --configuration %CONFIGURATION% --verbosity %VERBOSITY%
IF NOT ERRORLEVEL 0 GOTO FAIL

cd bin/docker
docker build --tag "azureiotpcs/microservice-template-dotnet-ws:1.0-SNAPSHOT" --squash --compress --label "Tags=azure,iot,pcs,.NET" .
IF NOT ERRORLEVEL 0 GOTO FAIL

:: - - - - - - - - - - - - - -
goto :END

:FAIL
echo Command failed
endlocal
exit /B 1

:END
endlocal
