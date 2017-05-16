@ECHO off
setlocal

:: strlen("\scripts\git\") => 13
SET APP_HOME=%~dp0
SET APP_HOME=%APP_HOME:~0,-13%

cd %APP_HOME%

IF "%1"=="--with-sandbox" goto :WITH_SANDBOX
IF "%1"=="--no-sandbox" goto :WITHOUT_SANDBOX
goto :USAGE

:: - - - - - - - - - - - - - -

:WITH_SANDBOX
    echo Adding pre-commit hook (via Docker sandbox)...
    mkdir .git\hooks\ 2> nul
    del /F .git\hooks\pre-commit 2> nul
    copy scripts\git\pre-commit-runner-with-sandbox.sh .git\hooks\pre-commit
    IF NOT ERRORLEVEL 0 GOTO FAIL
    echo Done.
    goto :END

:WITHOUT_SANDBOX
    echo Adding pre-commit hook...
    mkdir .git\hooks\ 2> nul
    del /F .git\hooks\pre-commit 2> nul
    copy scripts\git\pre-commit-runner-no-sandbox.sh .git\hooks\pre-commit
    IF NOT ERRORLEVEL 0 GOTO FAIL
    echo Done.
    goto :END

:USAGE
    echo ERROR: sandboxing mode not specified.
    echo.
    echo The pre-commit hook can run in two different modes:
    echo   With sandbox: the build script runs inside a Docker container so you need to install only Docker
    echo   Without sandbox: the build script runs using binaries like Mono, MSBuild and Nuget from your workstation
    echo.
    echo Usage:
    echo .\scripts\git\setup --with-sandbox
    echo .\scripts\git\setup --no-sandbox
    exit /B 1

:FAIL
    echo Command failed
    endlocal
    exit /B 1

:END
endlocal