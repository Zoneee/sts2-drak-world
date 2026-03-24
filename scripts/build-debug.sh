#!/usr/bin/env bash

set -euo pipefail

source "$(cd "$(dirname "$0")" && pwd)/common.sh"

run_build "Debug" "build-only"
print_build_summary "Debug"