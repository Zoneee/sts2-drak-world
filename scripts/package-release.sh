#!/usr/bin/env bash
# package-release.sh — Build both distribution variants and zip them for upload.
#
# Output:
#   dist/STS2_Discard_Mod_<version>.zip            (原版 — vanilla experience)
#   dist/STS2_Discard_Mod_StarterDeck_<version>.zip (预组卡组版 — onboarding starter deck)
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
import json, sys
with open('$PROJECT_ROOT/STS2_Discard_Mod.json') as f:
    print(json.load(f)['version'])
")
printf '==> Packaging version %s\n' "$VERSION"

DIST_DIR="$PROJECT_ROOT/dist"
mkdir -p "$DIST_DIR"

# ── Build Release (dotnet compile + Godot PCK export) ─────────────────────────
printf '\n==> [1/2] Building Release...\n'
run_build "Release" "build-only"

RELEASE_OUT="$(build_output_dir "Release")"
STARTER_OUT="$(build_output_dir "StarterDeck")"

# ── Build StarterDeck DLL (reuse PCK from Release — same Godot assets) ─────────
printf '\n==> [2/2] Building StarterDeck...\n'
dotnet build "$PROJECT_FILE" \
    --configuration "StarterDeck" \
    -p:ModsPath="$DISABLED_MODS_PATH" \
    -p:SkipGodotPackExport=true

# Copy the shared PCK into the StarterDeck output directory.
cp -f "$RELEASE_OUT/STS2DiscardMod.pck" "$STARTER_OUT/STS2DiscardMod.pck"

# ── Helper: stage files and create ZIP for one variant ────────────────────────
package_variant() {
    local config="$1"       # Release | StarterDeck
    local manifest="$2"     # path to manifest JSON to include
    local zip_name="$3"     # output zip filename (no path)

    local out_dir staging_dir zip_path
    out_dir="$(build_output_dir "$config")"
    staging_dir="$(mktemp -d)"
    zip_path="$DIST_DIR/$zip_name"

    # Ensure cleanup on exit from this function.
    # shellcheck disable=SC2064
    trap "rm -rf '$staging_dir'" RETURN

    local mod_staging="$staging_dir/STS2_Discard_Mod"
    mkdir -p "$mod_staging"

    # Core binaries (no .pdb — debug symbols not needed by end users).
    cp "$out_dir/STS2DiscardMod.dll"  "$mod_staging/"
    cp "$out_dir/STS2DiscardMod.pck"  "$mod_staging/"
    cp "$out_dir/0Harmony.dll"         "$mod_staging/"
    cp "$out_dir/BUILD_FLAVOR.txt"     "$mod_staging/"

    # Manifest — always named STS2_Discard_Mod.json inside the ZIP so the
    # loader finds it regardless of which variant is installed.
    cp "$manifest" "$mod_staging/STS2_Discard_Mod.json"

    # Card art (loose images).
    if [[ -d "$PROJECT_ROOT/src/STS2DiscardMod" ]]; then
        cp -a "$PROJECT_ROOT/src/STS2DiscardMod" "$mod_staging/"
    fi

    # Godot texture cache — required for runtime portrait resolution.
    if [[ -d "$PROJECT_ROOT/src/.godot/imported" ]]; then
        mkdir -p "$mod_staging/.godot/imported"
        cp -a "$PROJECT_ROOT/src/.godot/imported/." "$mod_staging/.godot/imported/"
    fi

    (cd "$staging_dir" && zip -r "$zip_path" "STS2_Discard_Mod/")
    printf '==> Created %s\n' "$zip_path"
}

# ── Package both variants ──────────────────────────────────────────────────────
printf '\n==> Packaging...\n'

package_variant "Release" \
    "$PROJECT_ROOT/STS2_Discard_Mod.json" \
    "STS2_Discard_Mod_${VERSION}.zip"

package_variant "StarterDeck" \
    "$PROJECT_ROOT/STS2_Discard_Mod_StarterDeck.json" \
    "STS2_Discard_Mod_StarterDeck_${VERSION}.zip"

# ── Summary ───────────────────────────────────────────────────────────────────
printf '\n==> Done. Distribution packages:\n'
ls -lh "$DIST_DIR/"*.zip
