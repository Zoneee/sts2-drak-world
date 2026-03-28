#!/usr/bin/env bash

set -euo pipefail

source "$(cd "$(dirname "$0")" && pwd)/common.sh"

run_build "Release" "build-only"
print_build_summary "Release"