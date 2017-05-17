@ECHO off
setlocal enableextensions enabledelayedexpansion

:: strlen("\scripts\") => 9
SET APP_HOME=%~dp0
SET APP_HOME=%APP_HOME:~0,-9%
cd %APP_HOME%

:: Check dependencies
docker version > NUL
IF NOT ERRORLEVEL 0 GOTO MISSING_DOCKER

:: Create cache folders to speed up future executions
mkdir .cache\sandbox\.config 2>NUL
mkdir .cache\sandbox\.dotnet 2>NUL
mkdir .cache\sandbox\.nuget 2>NUL

:: Start the sandbox and execute the build script
docker run ^
    -v %APP_HOME%\.cache\sandbox\.config:/root/.config ^
    -v %APP_HOME%\.cache\sandbox\.dotnet:/root/.dotnet ^
    -v %APP_HOME%\.cache\sandbox\.nuget:/root/.nuget ^
    -v %APP_HOME%:/opt/code ^
    azureiotpcs/code-builder-dotnet:1.0 /opt/scripts/build

:: Error 125 typically triggers on Windows if the drive is not shared
IF ERRORLEVEL 125 GOTO DOCKER_SHARE
IF NOT ERRORLEVEL 0 GOTO FAIL

:: - - - - - - - - - - - - - -
goto :END

:DOCKER_SHARE
    echo ERROR: the drive containing the source code cannot be mounted.
    echo Open Docker settings from the tray icon, and fix the settings under 'Shared Drives'.
    exit /B 1

:MISSING_DOCKER
    echo ERROR: 'docker' command not found.
    echo Install Docker and make sure the 'docker' command is in the PATH.
    echo Docker installation: https://store.docker.com/editions/community/docker-ce-desktop-windows
    exit /B 1

:FAIL
    echo Command failed
    endlocal
    exit /B 1

:END
endlocal