@ECHO off
setlocal

:: strlen("\project\scripts\") => 17
SET APP_HOME=%~dp0
SET APP_HOME=%APP_HOME:~0,-17%
cd %APP_HOME%

cd WebService
call dotnet run

endlocal
