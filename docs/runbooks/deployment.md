# 部署运行手册

状态：有效
负责人：工程团队
最后评审：2026-03-24

## 目标
说明本仓库如何把构建产物部署到《Slay the Spire 2》的 live 模组目录，以及遇到路径或资源问题时的排查方式。

## 部署产物
正确的 live 目录结构应包含：

```text
{game}/mods/STS2_Discard_Mod/
├── STS2DiscardMod.dll
├── STS2DiscardMod.pck
├── 0Harmony.dll
├── BUILD_FLAVOR.txt
├── STS2_Discard_Mod.json
├── STS2DiscardMod/images/...
└── .godot/imported/...
```

说明：
- `STS2DiscardMod.pck` 负责挂载 `res://STS2DiscardMod/...` 资源。
- `STS2DiscardMod/images/...` 与 `.godot/imported/...` 用于本地调试时的松散资源与导入缓存。
- `BUILD_FLAVOR.txt` 用于确认当前部署是 `Debug` 还是 `Release`，以及调试过滤是否开启。

## 自动部署
### VS Code 任务
- `Build + Deploy: Release`
- `Build + Deploy: Debug`
- `Deploy: Release Runtime`
- `Deploy: Debug Runtime`

### 命令行构建后自动部署
项目文件会在构建后尝试把 DLL、manifest、`.pck`、Harmony 依赖和 Godot 导入资源复制到 `ModsPath` 指向的 live 模组目录。

Linux / WSL 默认优先探测：
- `/mnt/d/G_games/steam/steamapps/common/Slay the Spire 2/mods/`
- `~/.local/share/Steam/steamapps/common/Slay the Spire 2/mods/`

Windows 默认优先探测：
- Steam 注册表路径对应的 `steamapps`
- 兜底 `D:/G_games/steam/steamapps`

macOS 默认路径：
- `~/Library/Application Support/Steam/steamapps/common/Slay the Spire 2/SlayTheSpire2.app/Contents/MacOS/mods/`

## 手动部署
如果自动探测失效，可以先构建，再手动复制 `src/bin/<Configuration>/net9.0/` 中的关键产物到 live 模组目录。

至少需要复制：
- `STS2DiscardMod.dll`
- `STS2DiscardMod.pck`（如果有）
- `0Harmony.dll`
- `BUILD_FLAVOR.txt`
- `STS2_Discard_Mod.json`

如果需要调试卡图，还应同步复制：
- `src/STS2DiscardMod/images/...`
- `src/.godot/imported/*.ctex`
- `src/.godot/imported/*.md5`

## 必要依赖
- `BaseLib` 必须作为独立 mod 已安装在游戏目录中。
- manifest `id`、`AssemblyName` 和运行时 `ModId` 必须一致，当前都应为 `STS2DiscardMod`。

## 部署后验证
1. 打开 live 模组目录，确认关键文件齐全。
2. 查看 `BUILD_FLAVOR.txt`，确认构建类型与调试开关。
3. 启动游戏，搜索 `BepInEx/LogOutput.log` 中的 `STS2DiscardMod`。
4. 若卡牌可见但卡图缺失，优先检查 `.pck` 是否导出与部署。

## 常见故障
### DLL 无法覆盖
通常是游戏仍在运行。关闭游戏后重新执行部署。

### 卡图缺失但卡牌存在
优先检查：
- `GODOT_CLI_COMMAND` 是否设置
- `STS2DiscardMod.pck` 是否存在于 live 模组目录
- `.godot/imported/` 资源是否一并部署

### 模组不加载
优先检查：
- `STS2DiscardMod.dll`
- `0Harmony.dll`
- `STS2_Discard_Mod.json`
- `BaseLib` 是否已安装

### 额外 JSON 导致加载异常
不要把 `src/localization/eng/cards.json` 或其他本地化 JSON 复制到 live 模组目录。