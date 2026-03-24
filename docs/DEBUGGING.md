# 调试与故障排查

这份文档只描述当前仓库仍然有效的排查路径。

## 1. 日志看哪里

### BepInEx 日志

```text
{game}/BepInEx/LogOutput.log
```

常见路径：

- Linux：`~/.local/share/Steam/steamapps/common/Slay the Spire 2/BepInEx/LogOutput.log`
- Windows：`D:\G_games\steam\steamapps\common\Slay the Spire 2\BepInEx\LogOutput.log`
- macOS：`~/Library/Application Support/Steam/steamapps/common/Slay the Spire 2/SlayTheSpire2.app/Contents/MacOS/BepInEx/LogOutput.log`

### 过滤本模组日志

Linux / macOS：

```bash
tail -f BepInEx/LogOutput.log | grep 'STS2DiscardMod'
```

Windows PowerShell：

```powershell
Get-Content BepInEx\LogOutput.log -Wait | Select-String 'STS2DiscardMod'
```

## 2. 当前有哪些关键日志

### 初始化阶段

- `STS2 Discard-Trigger Mod loading... discovered ...`
- `ModelDb loaded discard-system card ...`

这两类日志用于判断：

- Harmony 是否已应用
- 反射是否发现全部 10 张牌
- 卡牌是否真正进入 `ModelDb` 和 `RegentCardPool`

### 弃牌阶段

- `[DiscardCmd] single discard requested`
- `[DiscardCmd] batch discard requested`
- `[DiscardCmd] discard-and-draw requested`

这类日志用于判断：

- 卡牌效果是否真的发起了弃牌动作
- 弃掉的是哪几张牌

### 单卡阶段

- `[CardName] play`
- `[CardName] discard-trigger:start`
- `[CardName] discard-trigger:end`

这类日志用于判断：

- `OnPlay()` 是否被调用
- `AfterCardDiscarded()` 是否命中这张牌本身

## 3. 常见问题

### 问题 A：模组没有加载

现象：日志里完全没有 `STS2DiscardMod`。

依次检查：

1. manifest 的 `id` 是否是 `STS2DiscardMod`
2. live 目录里是否存在 `STS2DiscardMod.dll`
3. `BaseLib` 是否已经单独安装到游戏目录
4. DLL 名、manifest `id`、`DiscardModMain.ModId` 是否一致

### 问题 B：卡牌没出现在卡库里

优先看：

1. 初始化日志里是否发现了 10 张牌
2. `ModelDb loaded discard-system card ... regentPool=True`
3. `src/localization/eng/cards.json` 是否补齐对应键

当前仓库使用 `[Pool(typeof(RegentCardPool))]` 自动注册，不再依赖 `Main.cs` 里的手工 `AddModelToPool(...)`。

### 问题 C：卡牌能看到，但卡图丢失

如果日志或界面里出现：

```text
No loader found for resource: res://STS2DiscardMod/images/...
```

通常不是注册问题，而是 `.pck` 没有导出或没有部署。

排查：

1. 是否设置了 `GODOT_CLI_COMMAND`
2. live 目录里是否有 `STS2DiscardMod.pck`
3. 是否重启了游戏重新挂载资源

### 问题 D：卡牌能打出，但没有效果

现在先按日志顺序看：

1. 有没有 `[CardName] play`
2. 有没有 `[DiscardCmd] ...`
3. 有没有 `[CardName] discard-trigger:start`

如果有 `play` 但没有 `DiscardCmd`，说明是出牌逻辑没真正调用弃牌命令。

如果有 `DiscardCmd` 但没有 `discard-trigger:start`，说明被弃掉的不是那张目标卡。

### 问题 E：构建报 `STS001`

说明本地化键缺失。当前格式是：

```json
{
  "DISCARDMOD-YOUR_CARD.title": "标题",
  "DISCARDMOD-YOUR_CARD.description": "描述"
}
```

## 4. VS Code 附加调试器

最稳妥的断点位置：

- `DiscardModMain.Initialize()`
- `ModelDbDiagnosticsPatch.LogCustomCardPresence()`
- `DiscardModCard.AfterCardDiscarded()`
- 目标卡牌的 `OnPlay()`

## 5. 快速测试模式：只出本模组新卡

当前仓库已经加入一个调试专用的卡池过滤：

- `Debug` 构建下，`RegentCardPool` 会在 `ModelDb.Init()` 后被裁剪为仅保留本模组的 10 张卡
- `Release` 构建下，该过滤默认关闭

这意味着在 `Debug` 构建里，基于 `RegentCardPool` 的大部分拿牌来源会优先只出现我们新增的卡，适合快速反复验证：

- 战后奖励
- 其他依赖角色卡池随机生成的拿牌来源
- 部分商店或事件来源

目前这条过滤已经覆盖：

- 普通 `CardReward` 这类直接基于角色卡池的奖励
- `CrystalSphereCardReward` 这类内部仍走角色卡池的特殊奖励

注意边界：

- 初始卡组不会因此自动替换
- 如果某个来源不是走 `RegentCardPool`，它不会被这条调试过滤影响
- 若后续需要连初始卡组也替换，再单独补一个开局注入测试牌组的补丁更合适

相关实现位置：

- `src/Patches/DebugCardPoolSettings.cs`
- `src/Patches/ModelDbDiagnosticsPatch.cs`

## 6. Debug 开局测试牌组

当前仓库在 `Debug` 构建下还会直接替换 Regent 的初始牌组：

- 新开局时，原始初始卡组会被清空
- 改为注入一套仅由本模组卡组成的测试牌组
- `Release` 构建下默认关闭

当前测试牌组包含：

- `SwiftCut`
- `DarkFlameFragment`
- `RecallSurge`
- `ShatteredEcho`
- `EmberVolley`
- `FadingFormula`
- `AshenAegis`
- `ToxinRecord`
- `CripplingManuscript`
- `FinalDraft`

实现位置：

- `src/Patches/DebugStartingDeckPatch.cs`

## 7. Debug 商店过滤

当前仓库在 `Debug` 构建下还会把普通商店里原本的 colorless 卡槽替换成模组卡：

- 角色牌槽本来就会受 `RegentCardPool` 过滤影响
- 额外补上的这一层，主要是覆盖商店里的 colorless 槽位
- 这样普通商店会更接近“全是模组卡”的测试环境

实现位置：

- `src/Patches/DebugMerchantInventoryPatch.cs`

当前边界：

- 主要覆盖普通商店库存
- `SpecialCardReward` 这类固定发某一张指定卡的奖励，不是随机池抽取，因此不会被统一重写

## 8. 如何确认游戏目录里是 Debug 还是 Release

现在构建后会额外生成并部署：

- `mods/STS2_Discard_Mod/BUILD_FLAVOR.txt`

你可以直接打开这个文件确认：

- `Configuration=Debug` 或 `Configuration=Release`
- `BuiltUtc=...`
- `DebugCardPoolFilter=true/false`
- `DebugStarterDeckReplacement=true/false`
- `DebugMerchantColorlessReplacement=true/false`

这比单纯看有没有 `.pdb` 更可靠，因为它能同时告诉你当前调试特性是否真的开启。

## 9. 快速自检命令

重建：

```bash
dotnet clean src/STS2_Discard_Mod.csproj -p:Sts2DataDir=/absolute/path/to/lib && \
  dotnet build src/STS2_Discard_Mod.csproj -p:Sts2DataDir=/absolute/path/to/lib
```

检查 live 模组目录：

```bash
ls -la {game}/mods/STS2_Discard_Mod/
```

## 10. 提交问题时最好附带什么

- 操作系统与游戏版本
- 最新相关日志原文
- `mods/STS2_Discard_Mod/` 目录结构
- 你执行过的构建命令
- 是否设置了 `Sts2DataDir` / `GODOT_CLI_COMMAND`
