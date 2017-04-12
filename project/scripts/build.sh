#!/usr/bin/env bash

DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && cd .. && cd .. && pwd )/"
source "$DIR/project/scripts/.functions.sh"
cd $DIR

PROJECTS=$(dotnet sln list | grep 'csproj$' | grep -v '^project/project.csproj$')

failed() {
    echo "Build failed"
    exit 1
}

build() {
    for PROJ in $PROJECTS; do
        PROJ=$(dirname "$PROJ")

        cd $PROJ
        if test $(find . -name *.csproj | wc -l) != 0 ; then
            header "Build $PROJ"
            dotnet restore || failed
            dotnet build || failed
        fi
        cd $DIR
    done
}

run_tests() {
    for PROJ in $PROJECTS; do
        PROJ=$(dirname "$PROJ")
        cd $PROJ
        if test $(find . -name *.Test.csproj | wc -l) != 0 ; then
            header "Run $PROJ tests"
            dotnet vstest ./bin/Debug/netcoreapp1.1/$PROJ.dll || failed
        fi
        cd $DIR
    done
}

build
run_tests

