# 附加调试速查（Godot）

这份文档只保留 VS Code 附加调试器的最短步骤。

完整日志排查、模组加载问题和常见错误，请看 [DEBUGGING.md](DEBUGGING.md)。

## 目标

在 VS Code 中附加到游戏进程，并让 `src/Main.cs` 或 `src/Cards/*.cs` 的断点生效。

## 步骤

### 1. 先构建 Debug 版本

```bash
dotnet build src/STS2_Discard_Mod.csproj --configuration Debug
```

### 2. 确认 Debug 产物已部署

```text
{game}/mods/STS2_Discard_Mod/
├── STS2DiscardMod.dll
├── STS2DiscardMod.pdb
└── STS2_Discard_Mod.json
```

### 3. 启动游戏并附加调试器

1. 启动 STS2
2. 打开 VS Code“运行和调试”面板
3. 选择“Attach to Process”
4. 找到 STS2 或 Godot 进程并附加

### 4. 设置断点

推荐从这几个位置开始：

- `DiscardModMain.Initialize()`
- `DiscardModMain.RegisterCards()`
- 任意卡牌的 `OnPlay()`

## 常见现象

### 断点没有命中

优先检查：

- 是否构建的是 Debug 版本
- live 模组目录里是否存在 `STS2DiscardMod.pdb`
- 当前游戏加载的是否是最新 DLL

### 日志有输出但断点无效

通常是附加到了错误进程，或 live 目录里仍是旧的 Release 文件。
