#!/usr/bin/env bash

DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && cd .. && cd .. && pwd )/"
source "$DIR/project/scripts/.functions.sh"
cd $DIR

PROJECTS=$(dotnet sln list | grep 'csproj$')

for PROJ in $PROJECTS; do
    PROJ=$(dirname "$PROJ")
    cd $PROJ
    rm -fR bin/
    rm -fR obj/
    cd $DIR
done
