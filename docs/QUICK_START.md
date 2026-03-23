# 3 分钟快速上手

这份文档只回答一件事：怎样最快把当前仓库编译并让游戏加载到最新 DLL。

## 第一步：确认 `sts2.dll` 已放到 `lib/`

```bash
ls lib/sts2.dll
```

如果不存在，先从游戏目录复制。完整步骤见 [DEV_GUIDE.md](DEV_GUIDE.md)。

## 第二步：打开项目

```bash
code /home/alphonse/projects/STS2-Dark-World
```

## 第三步：一键构建并自动部署

```text
Ctrl+Shift+B
```

默认会执行 `Build: Release`，成功后会自动把文件部署到：

```text
{游戏目录}/mods/STS2_Discard_Mod/
├── STS2DiscardMod.dll
└── STS2_Discard_Mod.json
```

## 第四步：启动游戏验证

启动游戏后，检查日志中是否出现：

```text
[STS2DiscardMod][INFO] STS2 Discard-Trigger Mod loading...
[STS2DiscardMod][INFO] Registered 4 discard-trigger cards to RegentCardPool
[STS2DiscardMod][INFO] STS2 Discard-Trigger Mod loaded!
```

## 如果自动部署失败

### 情况 1：找不到游戏目录

手动构建：

```bash
dotnet build src/STS2_Discard_Mod.csproj --configuration Release
```

然后把下面两个文件复制到游戏目录：

```text
src/bin/Release/net9.0/STS2DiscardMod.dll
STS2_Discard_Mod.json
```

### 情况 2：DLL 被占用

关闭游戏后重新执行：

```text
Ctrl+Shift+B
```

### 情况 3：`cards.json` 相关报错

确认 live 模组目录里没有这类额外文件：

```text
{游戏目录}/mods/STS2_Discard_Mod/localization/eng/cards.json
```

当前加载器会把它误判为 manifest。

## 下一步

- 想看完整开发流程：见 [DEV_GUIDE.md](DEV_GUIDE.md)
- 想看 VS Code 任务与快捷键：见 [VSCODE_WORKFLOW.md](VSCODE_WORKFLOW.md)
- 想排查日志或附加调试器：见 [DEBUGGING.md](DEBUGGING.md)
