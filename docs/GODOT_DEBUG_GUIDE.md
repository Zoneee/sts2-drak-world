# STS2 Mod 调试指南

## 方法 1：日志调试（最简单）

修改代码中的日志输出，重新编译部署后从 BepInEx 日志中观察结果。

**日志 API：**

```csharp
// 在卡牌或其他类中使用（Logger 在 DiscardModMain 上）
DiscardModMain.Logger.Info("card played");
DiscardModMain.Logger.Debug("verbose: value = " + someValue);
DiscardModMain.Logger.Warning("unexpected state");
DiscardModMain.Logger.Error("failed to do X");
```

**查看日志：**

```bash
# Linux/macOS — 实时跟踪
tail -f "{game}/BepInEx/LogOutput.log" | grep 'STS2DiscardMod'

# Windows PowerShell
Get-Content "{game}\BepInEx\LogOutput.log" -Wait | Select-String 'STS2DiscardMod'
```

---

## 方法 2：Debug 构建 + 调试器附加

### 步骤 1：编译 Debug 版本

```bash
dotnet build src/ --configuration Debug
```

Debug 版本包含 `.pdb` 符号文件，`CopyToModsFolderOnBuild` 会同时部署 DLL 和 PDB。

部署后：
```
{game}/mods/STS2_Discard_Mod/
├── STS2_Discard_Mod.dll     ← Debug 版本
├── STS2_Discard_Mod.pdb     ← 调试符号
└── STS2_Discard_Mod.json
```

### 步骤 2：启动游戏

正常启动 STS2（通过 Steam 或直接运行 Godot 可执行文件）。

### 步骤 3：在 VS Code 中附加调试器

1. 安装 **C# Dev Kit** 扩展（`ms-dotnettools.csdevkit`）
2. 打开 **运行和调试** 面板（`Ctrl+Shift+D`）
3. 选择 **Attach to Process**
4. 在进程列表中找到 STS2/Godot 进程
5. 在 `src/Cards/*.cs` 或 `src/Main.cs` 中设置断点
6. 在游戏中触发目标逻辑（出牌、弃牌等）
7. 调试器会在断点处暂停，可检查变量和调用栈

### 步骤 4：调试结束后还原

调试完成后，重新编译 Release 版本：
```bash
dotnet build src/ --configuration Release
```

---

## 方法 3：在 Main.cs 中添加临时诊断日志

快速定位问题的最简单方式：

```csharp
public static void Initialize()
{
    Logger.Info("=== MOD LOADING START ===");
    Logger.Info($"Assembly: {typeof(DiscardModMain).Assembly.Location}");

    RegisterCards();

    Logger.Info("=== MOD LOADING COMPLETE ===");
}
```

```csharp
public override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
{
    DiscardModMain.Logger.Info($"DarkFlameFragment OnPlay — ctx={ctx}, cardPlay={cardPlay}");
    await Task.CompletedTask;
}
```

---

## 常见问题诊断

### BepInEx 日志中完全没有 `[STS2DiscardMod]`

1. 确认 `mods/STS2_Discard_Mod/` 目录存在且包含 DLL 和 JSON
2. 确认 `STS2_Discard_Mod.json` 内容正确：
   ```json
   { "id": "STS2DiscardMod", "has_dll": true, "has_pck": false, "affects_gameplay": true }
   ```
3. 检查 BepInEx 日志是否有 `Exception` 或 `Error` 在 mod 加载阶段

### 卡牌不出现在卡池中

1. 确认 `RegisterCards()` 中有 4 次 `ModHelper.AddModelToPool(...)` 调用
2. 确认 `src/localization/eng/cards.json` 包含所有 8 个 key（每张卡 `.title` + `.description`）
3. 构建警告 `STS001` 表示 localization key 缺失

### 编译后日志没变化

确认部署成功 — BepInEx 加载的是上次部署的 DLL。
检查构建时间戳：
```bash
ls -la {game}/mods/STS2_Discard_Mod/STS2_Discard_Mod.dll
```

---

## 调试工作流速查

```
1. 修改代码
        ↓
2. dotnet build src/ -c Debug
        ↓
3. 启动游戏（部署已自动完成）
        ↓
4. 附加调试器 或 查看 BepInEx/LogOutput.log
        ↓
5. 在游戏中触发目标逻辑
        ↓
6. 在断点处检查变量 或 从日志中读取输出
```
