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
该能力已默认关闭。

原因：此前通过在 `ModelDb.Init()` 后直接改写 `RegentCardPool` 内部缓存来实现过滤，这会破坏卡牌库排序与过滤依赖的卡池不变量，并在打开图鉴时触发 `MockCardPool` / `You monster!` 异常。

当前建议的 Debug 快速验证路径：
- 使用 Debug 初始牌组替换，快速在开局拿到模组卡。
- 使用 Debug 商店过滤，在普通商店里更快遇到模组卡。

### Debug 图鉴强制可见
该能力已默认关闭。

原因：对 `NCardLibraryGrid.GetCardVisibility()` 的后置补丁会越过游戏原生的图鉴可见性判定。当前在进入百科大全的卡牌总览页面时，这条 override 仍是高风险链路；在获得新的运行时证据前，不再允许默认启用。

若后续仍需要在未解锁 Regent 的档位里验证图鉴展示，必须先找到不会破坏图鉴筛选/排序状态的安全挂点，并补充新的日志证据后再恢复。

### 卡图资源路径
自定义卡的 `PortraitPath` / `BetaPortraitPath` / `CustomPortraitPath` 必须使用 Godot 资源路径格式，也就是固定使用 `/` 分隔。

不要用 `Path.Join(...)` 或其他会在 Windows 上产出 `\` 分隔符的 API 来拼接这些路径。百科大全中的卡牌总览会批量读取卡图资源，这类路径格式错误可能在 `Debug` 和 `Release` 下都表现为进入页面后卡住，但日志里没有明确异常栈。

### 卡图导入参数
当前自定义卡图应统一使用更适合运行时批量加载的导入参数：
- `compress/mode=2`
- `compress/high_quality=true`
- `mipmaps/generate=true`

原因：百科大全卡牌总览和局内抽牌都会同步读取模组卡图。此前所有 small/big 卡图都使用未压缩纹理且关闭 mipmaps，容易在首次批量访问时把纹理解压、上传和缩放成本堆在主线程上，表现为明显卡顿甚至看起来像“卡死”。

### 卡图生成脚本与 catalog 必须一一对应
自定义卡图的生成来源以 `scripts/generate_card_art.py` 为准。该脚本现在必须覆盖 `src/Data/cards.catalog.json` 中全部 10 个 `assetName`。

检查要点：
- 运行脚本时应输出全部 10 张牌名，而不是只生成部分文件。
- 若 `cards.catalog.json` 中新增/修改了 `assetName`，而脚本规格未同步，脚本应直接失败，而不是继续保留旧图片。
- 不要继续混用不同日期、不同来源批次的卡图文件；这会让“卡图错配”变成无法重现的脏状态问题。

当前已知历史教训：源目录曾同时存在两批来源不同的卡图，前 4 张和后 6 张来自不同时间点，导致即使路径映射正确，也会继续怀疑运行时“拿错图”。

### 卡牌总览仍卡顿，但可见的是非 Mod 卡
若用户明确反馈“即使看的不是储君卡池或 Mod 卡，也会明显卡顿”，不要继续只沿卡图资源路径排查。

优先检查：
- `LocalizationRuntimePatch` 是否仍在对所有 `cards` 表查询做工作。
- 日志里是否出现一次性聚合统计：`[LocalizationRuntimePatch] cardsLookups=...; fastRejected=...; modKeys=...`。
- `fastRejected` 是否远高于 `modKeys`，以及 `languageResolutions` 是否接近 1，而不是随查询数增长。

当前实现约束：
- 运行时本地化只应对 `DISCARDMOD-` key 生效。
- 运行时本地化需要同时兼容 `CARD.DISCARDMOD-*` 这类完整模型 ID 形态，否则在 `zh_CN` 下也可能回退成英文。
- 非模组卡的 `cards` 查询必须在语言解析前被快速拒绝。
- 语言标记应在首次解析后缓存复用，不允许在卡牌总览批量渲染时反复扫描 `LocTable` 反射成员。

如果这些信号都正常，但卡牌总览仍明显卡顿，再回到“是否枚举了全部卡并触发 PortraitPath / CustomPortraitPath”的方向继续定位。

### Debug 初始牌组替换
`Debug` 构建下，会直接替换 Regent 的初始牌组，使新开局更快进入模组验证路径。

### Debug 商店过滤
`Debug` 构建下，会额外替换普通商店里的 colorless 卡槽，使普通商店更接近“全是模组卡”的测试环境。

### Debug 快速验证能力的边界
当前默认保留且已验证安全的 Debug 快速验证能力只有两条：
- 起始牌组替换
- 商店 colorless 卡槽替换

不要重新启用以下旧方案：
- `ModelDb.Init()` 后修改 `RegentCardPool` 内部缓存
- `NCardLibraryGrid.GetCardVisibility()` 强制把模组卡设为可见

如果后续希望恢复更强的“只出模组卡”调试能力，必须先找到奖励生成、卡牌提供或进度层的安全挂点；当前最新日志仍显示 `Progress parse: Unknown card ID: CARD.DISCARDMOD-*`，说明还没有可验证的安全“已发现卡牌”恢复路径。

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
