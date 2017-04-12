#!/usr/bin/env bash

DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && cd .. && cd .. && cd .. && pwd )/"

failed() {
    echo "Git setup failed"
    exit 1
}

cd $DIR/.git || failed
mkdir -p hooks || failed
cd hooks || failed

echo "Adding pre-commit hook..."
rm -f pre-commit
ln -s ../../project/scripts/git/pre-commit-runner.sh pre-commit || failed

echo "Done."
