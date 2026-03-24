# 附加调试速查（Godot）

这份文档只保留 VS Code 附加调试器的最短步骤。

完整日志排查、模组加载问题和常见错误，请看 [DEBUGGING.md](DEBUGGING.md)。

## 目标

在 VS Code 中附加到游戏进程，并让 `src/Main.cs` 或 `src/Cards/*.cs` 的断点生效。

## WSL / Windows 边界

如果你是在 WSL Remote 窗口里开发、但游戏实际跑在 Windows 上：

- 当前 `coreclr` 附加配置只能看到“同一侧”的进程
- 也就是说，WSL 窗口里的 `${command:pickProcess}` 只能选到 Linux/WSL 进程，不能直接选到 Windows 的 `Godot.exe`

可行做法是：

1. 继续在 WSL 里构建和部署
2. 另外开一个 Windows 本机 VS Code 窗口，打开同一份代码
3. 在 Windows 窗口里使用 `Attach to Godot (STS2, same host only)` 附加到 `Godot.exe`

如果只在当前 WSL 窗口里操作，这个附加方案不能直接跨到 Windows 进程。

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
├── BUILD_FLAVOR.txt
└── STS2_Discard_Mod.json
```

其中 `BUILD_FLAVOR.txt` 会直接写明：

- 当前部署是 `Debug` 还是 `Release`
- 构建时间
- Debug 专用测试开关是否启用

### 3. 启动游戏并附加调试器

1. 启动 STS2
2. 打开 VS Code“运行和调试”面板
3. 选择“Attach to Process”
4. 找到 STS2 或 Godot 进程并附加

### 4. 设置断点

推荐从这几个位置开始：

- `DiscardModMain.Initialize()`
- `ModelDbDiagnosticsPatch.LogCustomCardPresence()`
- 任意卡牌的 `OnPlay()`

## 常见现象

### 断点没有命中

优先检查：

- 是否构建的是 Debug 版本
- live 模组目录里是否存在 `STS2DiscardMod.pdb`
- live 模组目录里的 `BUILD_FLAVOR.txt` 是否显示 `Configuration=Debug`
- 当前游戏加载的是否是最新 DLL

### 日志有输出但断点无效

通常是附加到了错误进程，或 live 目录里仍是旧的 Release 文件。
