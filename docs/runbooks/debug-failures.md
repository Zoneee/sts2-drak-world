# 调试失败运行手册

状态：有效
负责人：工程团队
最后评审：2026-03-24

## 处理流程
1. 先确认问题属于加载失败、可见性失败、卡图失败、出牌失败还是弃牌触发失败。
2. 搜索 `BepInEx/LogOutput.log` 中的 `STS2DiscardMod`。
3. 按日志阶段定位：初始化 -> `ModelDb` -> 弃牌命令 -> 单卡触发。
4. 缩小到单张牌或单个构建配置后，再做最小修复。
5. 用原始日志信号验证修复是否生效。

## 日志位置
### BepInEx 日志

```text
{game}/BepInEx/LogOutput.log
```

常见路径：
- Linux：`~/.local/share/Steam/steamapps/common/Slay the Spire 2/BepInEx/LogOutput.log`
- Windows：`D:/G_games/steam/steamapps/common/Slay the Spire 2/BepInEx/LogOutput.log`
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

## 关键日志信号
### 初始化阶段
- `STS2 Discard-Trigger Mod loading... discovered ...`
- `STS2 Discard-Trigger Mod loaded!`

用于判断：
- 模组 DLL 是否被加载
- 反射是否发现全部 10 张牌
- Harmony 补丁是否已应用

### `ModelDb` 阶段
- `ModelDb loaded discard-system card ...`

用于判断：
- 卡牌是否真正进入 `ModelDb`
- 卡牌是否进入 `RegentCardPool`

### 弃牌命令阶段
- `[DiscardCmd] single discard requested`
- `[DiscardCmd] batch discard requested`
- `[DiscardCmd] discard-and-draw requested`

用于判断：
- 出牌逻辑是否真的发起了弃牌动作
- 当前被弃掉的是哪几张牌

### 单卡阶段
- `[CardName] play`
- `[CardName] discard-trigger:start`
- `[CardName] discard-trigger:end`

用于判断：
- `OnPlay()` 是否被调用
- 被弃掉的是否正是目标牌本身

## 常见问题
### 模组没有加载
现象：日志里完全没有 `STS2DiscardMod`。

依次检查：
- manifest `id`、DLL 名与 `DiscardModMain.ModId` 是否一致
- live 目录里是否存在 `STS2DiscardMod.dll`
- `0Harmony.dll` 是否已部署
- `BaseLib` 是否已经单独安装到游戏目录

### 卡牌没出现在卡库里
优先检查：
- 初始化日志里是否发现了 10 张牌
- `ModelDb` 日志里是否显示 `regentPool=True`
- 当前存档是否已解锁 `Regent`

### 卡牌能看到，但卡图丢失
若界面或日志出现 `No loader found for resource: res://STS2DiscardMod/images/...`，通常不是注册问题，而是资源挂载失败。

优先检查：
- 是否设置了 `GODOT_CLI_COMMAND`
- live 目录里是否存在 `STS2DiscardMod.pck`
- `.godot/imported/` 资源是否一并部署

### 卡牌能打出，但没有效果
按顺序看：
- 有没有 `[CardName] play`
- 有没有 `[DiscardCmd] ...`
- 有没有 `[CardName] discard-trigger:start`

如果有 `play` 但没有 `DiscardCmd`，通常是 `OnPlay()` 没真正调用弃牌命令。

如果有 `DiscardCmd` 但没有 `discard-trigger:start`，说明被弃掉的不是目标牌。

### 构建报 `STS001`
说明本地化键缺失或不匹配，应回到本地化输入与运行时文本定义补齐对应键。

## Debug 构建专用能力
### Debug 卡池过滤
`Debug` 构建下，`RegentCardPool` 会在 `ModelDb.Init()` 后被裁剪为仅保留本模组卡，便于快速验证拿牌来源。

### Debug 初始牌组替换
`Debug` 构建下，会直接替换 Regent 的初始牌组，使新开局更快进入模组验证路径。

### Debug 商店过滤
`Debug` 构建下，会额外替换普通商店里的 colorless 卡槽，使普通商店更接近“全是模组卡”的测试环境。

### 如何确认当前部署的是哪种构建
打开 `mods/STS2_Discard_Mod/BUILD_FLAVOR.txt`，检查：
- `Configuration=Debug` 或 `Configuration=Release`
- `DebugCardPoolFilter=true/false`
- `DebugStarterDeckReplacement=true/false`
- `DebugMerchantColorlessReplacement=true/false`

## 断点调试
推荐优先设置断点的位置：
- `DiscardModMain.Initialize()`
- `ModelDbDiagnosticsPatch.LogCustomCardPresence()`
- `DiscardModCard.AfterCardDiscarded()`
- 目标卡牌的 `OnPlay()`

如果你在 WSL Remote 中开发、但游戏运行在 Windows 上，WSL 窗口通常不能直接附加 Windows 进程。此时建议保留 WSL 侧构建与部署，在 Windows 本机 VS Code 窗口中完成附加调试。

## 升级路径
- 若怀疑文档与代码不一致，更新对应架构或运行手册文档。
- 若发现规范缺失，补充 `docs/standards/`。
- 若问题反复出现，记录到技术债跟踪并收紧自动检查。
