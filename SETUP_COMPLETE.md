# 设置检查清单

这份文档用于确认当前仓库的 VS Code 开发环境是否齐全。

## 当前应具备的内容

- `.vscode/tasks.json`
- `.vscode/launch.json`
- `.vscode/settings.json`
- `.vscode/extensions.json`
- `deploy.ps1`
- `docs/runbooks/vscode-workflow.md`
- `docs/runbooks/quick-start.md`

## 推荐验证顺序

### 1. 验证 .NET SDK

```bash
dotnet --version
```

### 2. 验证本地依赖

```bash
ls lib/sts2.dll
```

### 3. 验证构建

```bash
dotnet build src/STS2_Discard_Mod.csproj --configuration Release
```

### 4. 验证 live 模组目录

应至少看到：

```text
mods/STS2_Discard_Mod/
├── STS2DiscardMod.dll
├── STS2DiscardMod.pck
├── 0Harmony.dll
├── BUILD_FLAVOR.txt
└── STS2_Discard_Mod.json
```

## 常见偏差

- 任务名和当前 `.vscode/tasks.json` 不一致
- DLL 名称和当前 manifest 不一致
- live 模组目录里残留 `localization/eng/cards.json`

如果出现这些情况，当前文档应以 `docs/runbooks/local-dev.md`、`docs/runbooks/debug-failures.md` 和 `docs/runbooks/deployment.md` 为准。
