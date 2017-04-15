@ECHO off
setlocal

:: quiet|minimal|normal|detailed|diagnostic
SET VERBOSITY=normal
:: Debug|Release
SET CONFIGURATION=Release

:: strlen("\project\scripts\") => 17
SET APP_HOME=%~dp0
SET APP_HOME=%APP_HOME:~0,-17%
cd %APP_HOME%

for %%i in (*.sln) do (
  call dotnet restore --verbosity %VERBOSITY% %%~nxi
  IF NOT ERRORLEVEL 0 GOTO FAIL
  call dotnet build --verbosity %VERBOSITY% --configuration %CONFIGURATION% %%~nxi
  IF NOT ERRORLEVEL 0 GOTO FAIL
)

for /r %%i in (*.Test.csproj) do (
  call dotnet test --verbosity %VERBOSITY% --configuration %CONFIGURATION% --no-build %%i
  IF NOT ERRORLEVEL 0 GOTO FAIL
)

:: - - - - - - - - - - - - - -
goto :END

:FAIL
echo Command failed
endlocal
exit /B 1

:END
endlocal
