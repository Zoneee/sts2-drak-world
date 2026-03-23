# 调试与故障排查

这份文档是当前仓库唯一的完整调试说明，包含日志定位、常见错误和 VS Code 附加调试器流程。

## 1. 先看哪里有日志

### BepInEx 日志

模组加载与运行时输出主要看：

```text
{game}/BepInEx/LogOutput.log
```

常见路径：

- Linux：`~/.local/share/Steam/steamapps/common/Slay the Spire 2/BepInEx/LogOutput.log`
- Windows：`D:\G_games\steam\steamapps\common\Slay the Spire 2\BepInEx\LogOutput.log`
- macOS：`~/Library/Application Support/Steam/steamapps/common/Slay the Spire 2/SlayTheSpire2.app/Contents/MacOS/BepInEx/LogOutput.log`

### 游戏日志过滤

模组日志前缀是 `STS2DiscardMod`。

Linux 或 macOS：

```bash
tail -f BepInEx/LogOutput.log | grep 'STS2DiscardMod'
```

Windows PowerShell：

```powershell
Get-Content BepInEx\LogOutput.log -Wait | Select-String 'STS2DiscardMod'
```

## 2. 代码里怎么打日志

```csharp
DiscardModMain.Logger.Info("普通信息");
DiscardModMain.Logger.Debug("调试细节");
DiscardModMain.Logger.Warning("异常状态");
DiscardModMain.Logger.Error("错误信息");
```

最适合先确认三件事：

1. 模组有没有进入 `Initialize()`
2. `RegisterCards()` 有没有执行
3. 某张卡的 `OnPlay()` 有没有被调用

## 3. 最常见的问题

### 问题 A：模组没有加载

现象：日志里完全没有 `STS2DiscardMod`。

依次检查：

1. live 目录是否正确

```text
{game}/mods/STS2_Discard_Mod/
├── STS2DiscardMod.dll
└── STS2_Discard_Mod.json
```

2. `STS2_Discard_Mod.json` 中是否至少包含：

```json
{
  "id": "STS2DiscardMod",
  "has_dll": true,
  "has_pck": false,
  "affects_gameplay": true
}
```

3. 是否误把 `cards.json` 部署到了 live 模组目录

```text
{game}/mods/STS2_Discard_Mod/localization/eng/cards.json
```

如果存在，删掉再重启游戏。

4. 重新构建：

```bash
dotnet clean src/ && dotnet build src/ --configuration Release
```

### 问题 B：日志报“找不到程序集”

这通常说明 manifest 的 `id` 与最终 DLL 名不一致。

当前仓库的正确组合是：

- `id`: `STS2DiscardMod`
- DLL: `STS2DiscardMod.dll`

### 问题 C：卡牌没出现在卡池里

依次检查：

1. `Main.cs` 中是否调用了 4 次 `ModHelper.AddModelToPool(...)`
2. 日志里是否出现 `Registered 4 discard-trigger cards to RegentCardPool`
3. `src/localization/eng/cards.json` 是否补齐了对应键

### 问题 D：卡牌能打出但没有效果

当前阶段这是预期行为。仓库里大多数 `OnPlay()` 还是占位实现。

### 问题 E：构建报 `STS001`

说明本地化键缺失。当前仓库用扁平键格式：

```json
{
  "YOUR_CARD.title": "标题",
  "YOUR_CARD.description": "描述"
}
```

## 4. VS Code 附加调试器

### 第一步：构建 Debug 版本

```bash
dotnet build src/ --configuration Debug
```

成功后会得到：

```text
{game}/mods/STS2_Discard_Mod/
├── STS2DiscardMod.dll
├── STS2DiscardMod.pdb
└── STS2_Discard_Mod.json
```

### 第二步：附加进程

1. 启动游戏
2. 在 VS Code 打开“运行和调试”
3. 选择“Attach to Process”
4. 选择 STS2 或 Godot 进程
5. 在 `src/Main.cs` 或 `src/Cards/*.cs` 下断点

### 第三步：验证断点命中

最稳妥的断点位置：

- `DiscardModMain.Initialize()`
- `DiscardModMain.RegisterCards()`
- 目标卡牌的 `OnPlay()`

## 5. 快速自检命令

重建：

```bash
dotnet clean src/ && dotnet build src/ --configuration Release
```

查找部署后的 DLL：

```bash
find ~/.local/share/Steam -name "STS2DiscardMod.dll"
```

检查 live 模组目录：

```bash
ls -la {game}/mods/STS2_Discard_Mod/
```

## 6. 提交问题时应附带的信息

如果需要继续排查，请至少提供：

1. 操作系统与游戏版本
2. 最新日志中的错误原文
3. `mods/STS2_Discard_Mod/` 当前目录结构
4. 你执行过的构建命令
