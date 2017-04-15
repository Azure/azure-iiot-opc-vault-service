@ECHO off
setlocal

:: strlen("\project\scripts\") => 17
SET APP_HOME=%~dp0
SET APP_HOME=%APP_HOME:~0,-17%
cd %APP_HOME%

cd WebService

call dotnet restore
IF NOT ERRORLEVEL 0 GOTO FAIL

call dotnet run

:: - - - - - - - - - - - - - -
goto :END

:FAIL
echo Command failed
endlocal
exit /B 1

:END
endlocal
