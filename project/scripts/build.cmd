@ECHO off
setlocal

:: Debug|Release
SET CONFIGURATION=Release

:: strlen("\project\scripts\") => 17
SET APP_HOME=%~dp0
SET APP_HOME=%APP_HOME:~0,-17%
cd %APP_HOME%

call dotnet restore *.sln
call dotnet build --configuration %CONFIGURATION% *.sln

for /r %%i in (*.Test.csproj) do (
  call dotnet test --no-build %%i
)

endlocal
