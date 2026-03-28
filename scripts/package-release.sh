#!/usr/bin/env bash
# package-release.sh — Build and zip the mod for distribution.
#
# Output:
#   dist/STS2_Discard_Mod_<version>.zip            (原版 — vanilla experience)
#   dist/STS2_Discard_Mod_StarterDeck_<version>.zip (预组卡组版 — starter deck enabled)
#
# Same DLL/PCK/manifest in both ZIPs; only STS2DiscardMod_config.json differs.
#
# Usage:  bash scripts/package-release.sh

set -euo pipefail

source "$(cd "$(dirname "$0")" && pwd)/common.sh"

# ── Verify zip is available ────────────────────────────────────────────────────
if ! command -v zip &>/dev/null; then
    printf 'ERROR: zip command not found. Install it (e.g. sudo apt install zip).\n' >&2
    exit 1
fi

# ── Read version from manifest ─────────────────────────────────────────────────
VERSION=$(python3 -c "
import json
with open('$PROJECT_ROOT/STS2_Discard_Mod.json') as f:
    print(json.load(f)['version'])
")
printf '==> Packaging version %s\n' "$VERSION"

DIST_DIR="$PROJECT_ROOT/dist"
mkdir -p "$DIST_DIR"

# ── Build once ─────────────────────────────────────────────────────────────────
printf '\n==> Building Release...\n'
run_build "Release" "build-only"

RELEASE_OUT="$(build_output_dir "Release")"

# ── Helper: stage shared files into a temp mod folder ─────────────────────────
stage_common_files() {
    local mod_staging="$1"
    mkdir -p "$mod_staging"

    cp "$RELEASE_OUT/STS2DiscardMod.dll" "$mod_staging/"
    cp "$RELEASE_OUT/STS2DiscardMod.pck" "$mod_staging/"
    cp "$RELEASE_OUT/0Harmony.dll"        "$mod_staging/"
    cp "$RELEASE_OUT/BUILD_FLAVOR.txt"    "$mod_staging/"
    cp "$PROJECT_ROOT/STS2_Discard_Mod.json" "$mod_staging/"

    if [[ -d "$PROJECT_ROOT/src/STS2DiscardMod" ]]; then
        cp -a "$PROJECT_ROOT/src/STS2DiscardMod" "$mod_staging/"
    fi

    if [[ -d "$PROJECT_ROOT/src/.godot/imported" ]]; then
        mkdir -p "$mod_staging/.godot/imported"
        cp -a "$PROJECT_ROOT/src/.godot/imported/." "$mod_staging/.godot/imported/"
    fi
}

# ── Package original (starter_deck_enabled: false) ────────────────────────────
printf '\n==> Packaging 原版...\n'
staging_orig="$(mktemp -d)"
trap "rm -rf '$staging_orig'" EXIT
stage_common_files "$staging_orig/STS2_Discard_Mod"
printf '{"starter_deck_enabled": false}\n' > "$staging_orig/STS2_Discard_Mod/STS2DiscardMod_config.json"
ZIP_ORIG="$DIST_DIR/STS2_Discard_Mod_${VERSION}.zip"
(cd "$staging_orig" && zip -r "$ZIP_ORIG" "STS2_Discard_Mod/")
printf '==> Created %s\n' "$ZIP_ORIG"

# ── Package starter-deck variant (starter_deck_enabled: true) ─────────────────
printf '\n==> Packaging 预组卡组版...\n'
staging_sd="$(mktemp -d)"
trap "rm -rf '$staging_orig' '$staging_sd'" EXIT
stage_common_files "$staging_sd/STS2_Discard_Mod"
printf '{"starter_deck_enabled": true}\n' > "$staging_sd/STS2_Discard_Mod/STS2DiscardMod_config.json"
ZIP_SD="$DIST_DIR/STS2_Discard_Mod_StarterDeck_${VERSION}.zip"
(cd "$staging_sd" && zip -r "$ZIP_SD" "STS2_Discard_Mod/")
printf '==> Created %s\n' "$ZIP_SD"

# ── Summary ───────────────────────────────────────────────────────────────────
printf '\n==> Done. Distribution packages:\n'
ls -lh "$ZIP_ORIG" "$ZIP_SD"
