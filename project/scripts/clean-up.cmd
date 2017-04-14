@ECHO off
setlocal

:: strlen("\project\scripts\") => 17
SET APP_HOME=%~dp0
SET APP_HOME=%APP_HOME:~0,-17%

cd %APP_HOME%

rmdir /s /q .\Services\bin
rmdir /s /q .\Services\obj
rmdir /s /q .\Services.Test\bin
rmdir /s /q .\Services.Test\obj

rmdir /s /q .\WebService\bin
rmdir /s /q .\WebService\obj
rmdir /s /q .\WebService.Test\bin
rmdir /s /q .\WebService.Test\obj

echo Done.

endlocal
