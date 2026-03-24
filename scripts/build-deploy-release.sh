#!/usr/bin/env bash

set -euo pipefail

source "$(cd "$(dirname "$0")" && pwd)/common.sh"

run_build "Release" "build-only"
deploy_runtime "Release"
print_build_summary "Release"