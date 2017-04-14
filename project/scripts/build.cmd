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

call dotnet restore --verbosity %VERBOSITY% *.sln
call dotnet build --configuration %CONFIGURATION% --verbosity %VERBOSITY% *.sln

for /r %%i in (*.Test.csproj) do (
  call dotnet test --no-build --verbosity %VERBOSITY% %%i
)

endlocal
