#!/usr/bin/env bash

set -euo pipefail

PROJECT_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
PROJECT_FILE="$PROJECT_ROOT/src/STS2_Discard_Mod.csproj"
TARGET_FRAMEWORK="net9.0"
MOD_ID_DIR="STS2_Discard_Mod"
DISABLED_MODS_PATH="/tmp/sts2-mod-disabled"

read_vscode_setting() {
    local key="$1"
    local settings_file="$PROJECT_ROOT/.vscode/settings.json"

    if [[ ! -f "$settings_file" ]]; then
        return 1
    fi

    sed -n "s/.*\"${key}\"[[:space:]]*:[[:space:]]*\"\([^\"]*\)\".*/\1/p" "$settings_file" | head -n 1
}

resolve_godot_cli() {
    if [[ -n "${GODOT_CLI_COMMAND:-}" ]]; then
        printf '%s\n' "$GODOT_CLI_COMMAND"
        return 0
    fi

    read_vscode_setting "sts2.godotCliCommand" || true
}

resolve_mod_dir() {
    if [[ -n "${STS2_MOD_DIR:-}" ]]; then
        printf '%s\n' "$STS2_MOD_DIR"
        return 0
    fi

    if [[ -d "/mnt/d/G_games/steam/steamapps/common/Slay the Spire 2/mods" ]]; then
        printf '%s\n' "/mnt/d/G_games/steam/steamapps/common/Slay the Spire 2/mods/$MOD_ID_DIR"
        return 0
    fi

    if [[ -d "$HOME/.local/share/Steam/steamapps/common/Slay the Spire 2/mods" ]]; then
        printf '%s\n' "$HOME/.local/share/Steam/steamapps/common/Slay the Spire 2/mods/$MOD_ID_DIR"
        return 0
    fi

    return 1
}

build_output_dir() {
    local configuration="$1"
    printf '%s\n' "$PROJECT_ROOT/src/bin/$configuration/$TARGET_FRAMEWORK"
}

run_build() {
    local configuration="$1"
    local deploy_mode="$2"
    local godot_cli
    local -a args

    godot_cli="$(resolve_godot_cli || true)"
    args=(build "$PROJECT_FILE" --configuration "$configuration")

    if [[ -n "$godot_cli" ]]; then
        args+=("-p:GodotCliCommand=$godot_cli")
    fi

    if [[ "$deploy_mode" == "build-only" ]]; then
        args+=("-p:ModsPath=$DISABLED_MODS_PATH")
    fi

    printf '==> dotnet %s\n' "${args[*]}"
    dotnet "${args[@]}"
}

copy_if_exists() {
    local source="$1"
    local destination_dir="$2"

    if [[ -f "$source" ]]; then
        cp -f "$source" "$destination_dir/"
    fi
}

deploy_runtime() {
    local configuration="$1"
    local output_dir
    local mod_dir

    output_dir="$(build_output_dir "$configuration")"
    mod_dir="$(resolve_mod_dir)"

    mkdir -p "$mod_dir"

    copy_if_exists "$output_dir/STS2DiscardMod.dll" "$mod_dir"
    copy_if_exists "$output_dir/STS2DiscardMod.pdb" "$mod_dir"
    copy_if_exists "$output_dir/0Harmony.dll" "$mod_dir"
    copy_if_exists "$output_dir/BUILD_FLAVOR.txt" "$mod_dir"
    copy_if_exists "$output_dir/STS2DiscardMod.pck" "$mod_dir"
    copy_if_exists "$PROJECT_ROOT/STS2_Discard_Mod.json" "$mod_dir"

    if [[ -d "$PROJECT_ROOT/src/STS2DiscardMod" ]]; then
        mkdir -p "$mod_dir/STS2DiscardMod"
        cp -a "$PROJECT_ROOT/src/STS2DiscardMod/." "$mod_dir/STS2DiscardMod/"
    fi

    if [[ -d "$PROJECT_ROOT/src/localization" ]]; then
        mkdir -p "$mod_dir/localization"
        cp -a "$PROJECT_ROOT/src/localization/." "$mod_dir/localization/"
    fi

    if [[ -d "$PROJECT_ROOT/src/.godot/imported" ]]; then
        mkdir -p "$mod_dir/.godot/imported"
        cp -a "$PROJECT_ROOT/src/.godot/imported/." "$mod_dir/.godot/imported/"
    fi

    printf '==> deployed %s to %s\n' "$configuration" "$mod_dir"
}

print_build_summary() {
    local configuration="$1"
    local output_dir
    local marker_file

    output_dir="$(build_output_dir "$configuration")"
    marker_file="$output_dir/BUILD_FLAVOR.txt"

    printf '==> output: %s\n' "$output_dir"
    if [[ -f "$marker_file" ]]; then
        printf '==> build flavor:\n'
        sed -n '1,20p' "$marker_file"
    fi
}