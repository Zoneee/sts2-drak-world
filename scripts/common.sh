#!/usr/bin/env bash

set -euo pipefail

PROJECT_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
PROJECT_FILE="$PROJECT_ROOT/src/STS2_Discard_Mod.csproj"
TARGET_FRAMEWORK="net9.0"
MOD_ID_DIR="STS2_Discard_Mod"
DISABLED_MODS_PATH="/tmp/sts2-mod-disabled"
GODOT_EXPORT_PRESET="STS2 Mod Pack"

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

export_godot_pack() {
    local configuration="$1"
    local godot_cli="$2"
    local output_dir
    local pack_path
    local marker_file

    output_dir="$(build_output_dir "$configuration")"
    pack_path="$output_dir/STS2DiscardMod.pck"
    marker_file="$(mktemp)"

    rm -f "$pack_path"

    printf '==> %s --headless --path %s --export-pack %s %s\n' "$godot_cli" "$PROJECT_ROOT/src" "$GODOT_EXPORT_PRESET" "$pack_path"
    "$godot_cli" --headless --path "$PROJECT_ROOT/src" --export-pack "$GODOT_EXPORT_PRESET" "$pack_path"

    if [[ ! -f "$pack_path" ]]; then
        rm -f "$marker_file"
        printf 'ERROR: Godot export did not create %s\n' "$pack_path" >&2
        return 1
    fi

    if [[ ! "$pack_path" -nt "$marker_file" ]]; then
        rm -f "$marker_file"
        printf 'ERROR: Godot export left a stale %s in place\n' "$pack_path" >&2
        return 1
    fi

    rm -f "$marker_file"
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
        args+=("-p:SkipGodotPackExport=true")
    fi

    if [[ "$deploy_mode" == "build-only" ]]; then
        args+=("-p:ModsPath=$DISABLED_MODS_PATH")
    fi

    printf '==> dotnet %s\n' "${args[*]}"
    dotnet "${args[@]}"

    if [[ -z "$godot_cli" ]]; then
        printf 'ERROR: Godot CLI is required to export a fresh STS2DiscardMod.pck for runtime card art. Configure GODOT_CLI_COMMAND or sts2.godotCliCommand.\n' >&2
        return 1
    fi

    export_godot_pack "$configuration" "$godot_cli"
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

    if [[ -d "$mod_dir/localization" ]]; then
        rm -rf "$mod_dir/localization"
    fi

    copy_if_exists "$output_dir/STS2DiscardMod.dll" "$mod_dir"
    copy_if_exists "$output_dir/STS2DiscardMod.pdb" "$mod_dir"
    copy_if_exists "$output_dir/0Harmony.dll" "$mod_dir"
    copy_if_exists "$output_dir/BUILD_FLAVOR.txt" "$mod_dir"
    copy_if_exists "$output_dir/STS2DiscardMod.pck" "$mod_dir"
    copy_if_exists "$PROJECT_ROOT/STS2_Discard_Mod.json" "$mod_dir"
    copy_if_exists "$PROJECT_ROOT/STS2DiscardMod_config.json" "$mod_dir"

    if [[ -d "$PROJECT_ROOT/src/STS2DiscardMod" ]]; then
        mkdir -p "$mod_dir/STS2DiscardMod"
        cp -a "$PROJECT_ROOT/src/STS2DiscardMod/." "$mod_dir/STS2DiscardMod/"
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