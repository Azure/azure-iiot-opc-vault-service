@ECHO off

:: strlen("\project\scripts\git\") => 21
SET APP_HOME=%~dp0
SET APP_HOME=%APP_HOME:~0,-21%

cd %APP_HOME%

echo Adding pre-commit hook...

mkdir .git\hooks\ 2> nul
del /F .git\hooks\pre-commit 2> nul
copy project\scripts\git\pre-commit-runner.sh .git\hooks\pre-commit

echo Done.
