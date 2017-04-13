#!/usr/bin/env bash

APP_HOME="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && cd .. && cd .. && cd .. && pwd )/"

failed() {
    echo "Git hooks setup failed"
    exit 1
}

cd $APP_HOME/.git || failed
mkdir -p hooks || failed
cd hooks || failed

echo "Adding pre-commit hook..."
rm -f pre-commit
cp -p $APP_HOME/project/scripts/git/pre-commit-runner.sh ./pre-commit || failed

echo "Done."
