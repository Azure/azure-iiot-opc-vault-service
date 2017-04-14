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
  call dotnet build --verbosity %VERBOSITY% --configuration %CONFIGURATION% %%~nxi
)

for /r %%i in (*.Test.csproj) do (
  call dotnet test --verbosity %VERBOSITY% --configuration %CONFIGURATION% --no-build %%i
)

endlocal
