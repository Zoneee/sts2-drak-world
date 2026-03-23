#!/usr/bin/env powershell
<#
.SYNOPSIS
    STS2 Mod 自动编译和部署脚本

.DESCRIPTION
    编译 mod 并自动部署到游戏目录。支持 Release 和 Debug 两种模式。

.PARAMETER Configuration
    编译配置: Release 或 Debug (默认: Release)

.PARAMETER NoClean
    跳过清理编译输出 (默认: 清理)

.PARAMETER Watch
    启用监听模式，文件变化时自动编译部署 (实验性)

.EXAMPLE
    .\deploy.ps1                    # Release 编译 + 部署
    .\deploy.ps1 -Configuration Debug    # Debug 编译 + 部署
    .\deploy.ps1 -Watch            # 监听模式自动编译

.NOTES
    需要 .NET SDK 9.0 和 PowerShell 5.1+
#>

param(
    [ValidateSet("Release", "Debug")]
    [string]$Configuration = "Release",

    [switch]$NoClean = $false,

    [switch]$Watch = $false
)

# ==================== 配置 ====================
$projectRoot = $PSScriptRoot
$gameModsDir = "D:\G_games\steam\steamapps\common\Slay the Spire 2\mods\STS2_Discard_Mod"
$dllSourceDir = "$projectRoot\src\bin\$Configuration\net9.0"
$dllName = "STS2DiscardMod.dll"
$pdbName = "STS2DiscardMod.pdb"
$pckName = "STS2DiscardMod.pck"
$harmonyName = "0Harmony.dll"
$modInfoSource = "$projectRoot\STS2_Discard_Mod.json"

# ==================== 函数 ====================

function Write-Header {
    param([string]$Text)
    Write-Host "`n" -NoNewline
    Write-Host "╔" + ("═" * 50) + "╗" -ForegroundColor Cyan
    Write-Host "║ $Text".PadRight(52) + "║" -ForegroundColor Cyan
    Write-Host "╚" + ("═" * 50) + "╝" -ForegroundColor Cyan
}

function Write-Success {
    param([string]$Text)
    Write-Host "  ✅ $Text" -ForegroundColor Green
}

function Write-Error-Custom {
    param([string]$Text)
    Write-Host "  ❌ $Text" -ForegroundColor Red
}

function Write-Warning-Custom {
    param([string]$Text)
    Write-Host "  ⚠️  $Text" -ForegroundColor Yellow
}

function Write-Info {
    param([string]$Text)
    Write-Host "  ℹ️  $Text" -ForegroundColor Cyan
}

function Verify-Paths {
    Write-Header "验证路径"

    if (-not (Test-Path $projectRoot)) {
        Write-Error-Custom "项目目录不存在: $projectRoot"
        exit 1
    }
    Write-Success "项目目录: $projectRoot"

    if (-not (Test-Path $gameModsDir)) {
        Write-Warning-Custom "mods 目录不存在，将创建: $gameModsDir"
        New-Item -ItemType Directory -Force -Path $gameModsDir | Out-Null
    }
    Write-Success "游戏 mods 目录: $gameModsDir"
}

function Build-Project {
    Write-Header "编译项目 ($Configuration)"

    Push-Location $projectRoot

    try {
        Write-Info "执行: dotnet build src/ --configuration $Configuration"
        $buildOutput = dotnet build src/ --configuration $Configuration 2>&1

        if ($LASTEXITCODE -ne 0) {
            Write-Error-Custom "编译失败！"
            Write-Host $buildOutput -ForegroundColor Red
            return $false
        }

        Write-Success "编译成功"
        return $true
    }
    finally {
        Pop-Location
    }
}

function Deploy-Files {
    Write-Header "部署文件"

    $dllSource = Join-Path $dllSourceDir $dllName
    $pckSource = Join-Path $dllSourceDir $pckName
    $harmonySource = Join-Path $dllSourceDir $harmonyName

    # 验证 DLL 文件
    if (-not (Test-Path $dllSource)) {
        Write-Error-Custom "DLL 文件不存在: $dllSource"
        return $false
    }

    # 复制 DLL
    Write-Info "复制 $dllName..."
    Copy-Item -Path $dllSource -Destination $gameModsDir -Force -Verbose | Out-Host
    Write-Success "DLL 已部署"

    if (Test-Path $harmonySource) {
        Write-Info "复制 $harmonyName..."
        Copy-Item -Path $harmonySource -Destination $gameModsDir -Force -Verbose | Out-Host
        Write-Success "Harmony 运行库已部署"
    }

    if (Test-Path $pckSource) {
        Write-Info "复制 $pckName..."
        Copy-Item -Path $pckSource -Destination $gameModsDir -Force -Verbose | Out-Host
        Write-Success "PCK 已部署"
    }
    else {
        Write-Warning-Custom "$pckName 不存在：卡图资源不会被 Godot 挂载。请先配置 GODOT_CLI_COMMAND 或 GodotCliCommand 再执行构建。"
    }

    # 复制 PDB (Debug 时)
    if ($Configuration -eq "Debug") {
        $pdbSource = Join-Path $dllSourceDir $pdbName
        if (Test-Path $pdbSource) {
            Write-Info "复制 $pdbName..."
            Copy-Item -Path $pdbSource -Destination $gameModsDir -Force -Verbose | Out-Host
            Write-Success "PDB 已部署"
        }
    }

    # 复制 manifest
    if (Test-Path $modInfoSource) {
        Write-Info "复制 STS2_Discard_Mod.json..."
        Copy-Item -Path $modInfoSource -Destination $gameModsDir -Force -Verbose | Out-Host
        Write-Success "STS2_Discard_Mod.json 已部署"
    }
    else {
        Write-Warning-Custom "STS2_Discard_Mod.json 不存在: $modInfoSource"
    }

    return $true
}

function Get-FileSize {
    param([string]$Path)
    $file = Get-Item $Path
    $size = $file.Length
    if ($size -lt 1KB) { return "$size B" }
    if ($size -lt 1MB) { return "{0:N2} KB" -f ($size / 1KB) }
    return "{0:N2} MB" -f ($size / 1MB)
}

function Show-Summary {
    Write-Header "部署完成"

    $dllFile = Join-Path $gameModsDir $dllName
    $modInfoFile = Join-Path $gameModsDir "STS2_Discard_Mod.json"
    $pckFile = Join-Path $gameModsDir $pckName

    if (Test-Path $dllFile) {
        $size = Get-FileSize $dllFile
        $timestamp = (Get-Item $dllFile).LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss")
        Write-Success "$dllName ($size) - $timestamp"
    }

    if (Test-Path $modInfoFile) {
        Write-Success "STS2_Discard_Mod.json - 已更新"
    }

    if (Test-Path $pckFile) {
        $size = Get-FileSize $pckFile
        Write-Success "$pckName ($size) - 已更新"
    }

    Write-Info "部署位置: $gameModsDir"
    Write-Info "配置: $Configuration"

    if ($Configuration -eq "Debug") {
        Write-Warning-Custom "Debug 模式: 包含调试符号，可在 VS Code 中设置断点"
    }
}

function Watch-Changes {
    Write-Header "监听模式（实验性）"
    Write-Info "监听源文件变化，自动编译部署..."
    Write-Info "按 Ctrl+C 退出"

    $watcher = New-Object System.IO.FileSystemWatcher
    $watcher.Path = "$projectRoot\src"
    $watcher.Filter = "*.cs"
    $watcher.IncludeSubdirectories = $true
    $watcher.EnableRaisingEvents = $true

    $lastRun = [DateTime]::MinValue
    $debounceMs = 500

    $action = {
        $now = [DateTime]::Now
        if (($now - $lastRun).TotalMilliseconds -lt $debounceMs) {
            return
        }
        $lastRun = $now

        Write-Host "`n🔄 文件变化检测到，重新编译..." -ForegroundColor Yellow

        if (Build-Project) {
            Deploy-Files
            Show-Summary
        }
    }

    $onChanged = Register-ObjectEvent -InputObject $watcher -EventName "Changed" -Action $action

    try {
        while ($true) {
            Start-Sleep -Seconds 1
        }
    }
    finally {
        Unregister-Event -SourceIdentifier $onChanged.Name
        $watcher.Dispose()
    }
}

# ==================== 主流程 ====================

try {
    Verify-Paths

    if (-not $NoClean) {
        # 可选: 清理旧编译
        # Write-Header "清理旧编译"
        # dotnet clean src/ | Out-Null
    }

    if (Build-Project) {
        if (Deploy-Files) {
            Show-Summary

            if ($Watch) {
                Write-Host ""
                Watch-Changes
            }
            else {
                Write-Host "`n💡 提示: 添加 -Watch 参数以启用自动编译模式" -ForegroundColor Cyan
            }
        }
    }
}
catch {
    Write-Error-Custom "发生错误: $_"
    exit 1
}
