#!/bin/bash

set -e

SCRIPT_DIR=$(readlink -e "$(dirname "$0")")

if [[ "$OSTYPE" != "linux-gnu"* ]]; then
  DOCKER_SOCKET='//var/run/docker.sock'
else
  DOCKER_SOCKET='/var/run/docker.sock'
fi

docker build --file "$SCRIPT_DIR/Dockerfile" --target 'test' --tag 'tmc-test' "$SCRIPT_DIR/.."

function cleanup() {
  docker images --filter=reference='tmc-test' --quiet | xargs -r docker rmi --force
}
trap cleanup EXIT

docker run --rm --name 'tmc-test' --volume "$DOCKER_SOCKET:/var/run/docker.sock" 'tmc-test'
