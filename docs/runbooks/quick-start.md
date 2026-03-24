# 快速上手运行手册

状态：有效
负责人：工程团队
最后评审：2026-03-24

## 目标
用最短路径完成一次可运行的构建和部署，并在游戏日志中确认当前模组已加载。

## 前置条件
- 已安装 `.NET SDK 9.0+`
- 已安装《Slay the Spire 2》
- `lib/sts2.dll` 已存在，或你准备在构建时显式传入 `Sts2DataDir`
- 若需要卡图资源，已设置 `GODOT_CLI_COMMAND`

## 最短路径
1. 确认 `lib/sts2.dll` 存在：`ls lib/sts2.dll`
2. 在 VS Code 中打开仓库。
3. 使用 `Ctrl+Shift+B` 运行默认任务 `Build: Release`。
4. 如果需要显式部署，再运行 `Build + Deploy: Release` 或 `Deploy: Release Runtime`。
5. 启动游戏，检查 `BepInEx/LogOutput.log` 中是否出现 `STS2DiscardMod` 相关日志。

## 命令行方式
只构建：

```bash
dotnet build src/STS2_Discard_Mod.csproj --configuration Release
```

指定 `sts2.dll` 所在目录：

```bash
dotnet build src/STS2_Discard_Mod.csproj \
  --configuration Release \
  -p:Sts2DataDir=/absolute/path/to/lib
```

带 Godot 资源导出：

```bash
export GODOT_CLI_COMMAND='/path/to/godot4'
dotnet build src/STS2_Discard_Mod.csproj --configuration Release
```

## 成功信号
构建或部署成功后，游戏目录中的 live 模组结构应接近：

```text
{game}/mods/STS2_Discard_Mod/
├── STS2DiscardMod.dll
├── STS2DiscardMod.pck
├── 0Harmony.dll
├── BUILD_FLAVOR.txt
└── STS2_Discard_Mod.json
```

日志中至少应出现：

```text
[STS2DiscardMod][INFO] STS2 Discard-Trigger Mod loading... discovered 10 cards: ...
[STS2DiscardMod][INFO] STS2 Discard-Trigger Mod loaded!
```

## 常见失败
### 缺少 `sts2.dll`
构建会因引用缺失失败。优先把游戏目录中的 `sts2.dll` 复制到 `lib/`，或使用 `-p:Sts2DataDir=...`。

### 只有 DLL，没有卡图
代码可以构建成功，但如果没有设置 `GODOT_CLI_COMMAND`，`.pck` 不会导出，`res://STS2DiscardMod/...` 资源会缺失。

### 自动部署没有发生
优先检查 `ModsPath` 指向的游戏目录是否存在，必要时改用 `Build + Deploy: Release`、`Deploy: Release Runtime` 或参见 `deployment.md` 手动部署。

### live 模组目录里出现额外 `cards.json`
不要把 `src/localization/eng/cards.json` 复制到 live 模组目录，当前加载器可能把它误判为 manifest。

## 延伸阅读
- `local-dev.md`：完整开发与构建说明
- `deployment.md`：部署与平台差异
- `debug-failures.md`：日志与断点调试路径
- `vscode-workflow.md`：VS Code 任务与推荐工作流