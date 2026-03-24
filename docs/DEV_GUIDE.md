# 开发指南

这份文档描述当前仓库的真实开发方式：环境准备、构建部署、卡牌扩展模式，以及这次迭代验证出来的可靠做法。

## 1. 环境准备

### 必要条件

- `.NET SDK 9.0+`
- 已安装的《Slay the Spire 2》
- 可用的 `sts2.dll`

### 获取 `sts2.dll`

推荐两种方式任选其一：

方式 A：复制到当前仓库的 `lib/sts2.dll`

方式 B：构建时直接传路径覆盖

```bash
dotnet build src/STS2_Discard_Mod.csproj \
  -p:Sts2DataDir=/absolute/path/to/lib
```

对于 worktree 开发，方式 B 一般更方便。

## 2. 构建与部署

### 本地构建

```bash
dotnet build src/STS2_Discard_Mod.csproj --configuration Debug
```

或：

```bash
dotnet build src/STS2_Discard_Mod.csproj \
  --configuration Debug \
  -p:Sts2DataDir=/absolute/path/to/lib
```

### 导出 `.pck`

如果需要卡图等 `res://` 资源，必须提供 Godot CLI：

```bash
export GODOT_CLI_COMMAND='/path/to/godot4'
dotnet build src/STS2_Discard_Mod.csproj --configuration Release
```

### 自动部署结果

```text
{游戏目录}/mods/STS2_Discard_Mod/
├── STS2DiscardMod.dll
├── STS2DiscardMod.pck
├── 0Harmony.dll
└── STS2_Discard_Mod.json
```

## 3. 当前代码结构

### `Main.cs`

入口只做两件事：

- 输出初始化日志
- `Harmony.PatchAll()` 应用补丁

卡牌发现改为反射自动完成，不再在 `Main.cs` 里手工维护注册列表。

### `DiscardModCard`

`src/Cards/DiscardModCard.cs` 是当前卡组的核心基类，集中封装了：

- 反射发现所有弃牌体系卡牌类型
- 统一卡图路径
- 统一的打出 / 弃牌触发日志
- 常用 helper：抽牌、弃牌、群体上状态、群体攻击、随机敌人攻击、固定格挡
- 默认的 `AfterCardDiscarded()` 自触发分发

### `Patches/`

当前补丁职责如下：

- `LocalizationRuntimePatch`：运行时卡牌文本兜底
- `CardLibraryVisibilityPatch`：确保自定义卡在卡库可见
- `ModelDbDiagnosticsPatch`：输出每张自定义卡是否成功入库/进池
- `DiscardDiagnosticsPatch`：记录 `CardCmd.Discard` / `DiscardAndDraw` 调用

## 4. 添加新卡牌

### 步骤

1. 在 `src/Cards/` 新建一个继承 `DiscardModCard` 的类
2. 加上 `[Pool(typeof(RegentCardPool))]`
3. 实现 `OnPlay()`
4. 如果有弃牌收益，实现 `OnSelfDiscarded()`
5. 在 `src/localization/eng/cards.json` 添加键
6. 在 `LocalizationRuntimePatch` 添加运行时文本
7. 构建验证

### 模板

```csharp
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;
using System.Threading.Tasks;

namespace DiscardMod.Cards;

[Pool(typeof(RegentCardPool))]
public class YourCard : DiscardModCard
{
    public YourCard()
        : base(1, CardType.Skill, CardRarity.Common, TargetType.Self, "your_asset_name", true)
    {
    }

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        LogPlay(cardPlay, "describe your values here");
        await DrawCards(choiceContext, 1);
    }

    protected override async Task OnSelfDiscarded(PlayerChoiceContext choiceContext)
    {
        await GainFlatBlock(4m);
    }

    public override void OnUpgrade()
    {
    }
}
```

## 5. 本地化格式

当前仓库使用扁平键，格式如下：

```json
{
  "DISCARDMOD-YOUR_CARD.title": "Your Card",
  "DISCARDMOD-YOUR_CARD.description": "Description."
}
```

规则：类名转大写下划线，并带 `DISCARDMOD-` 前缀。

## 6. 成功经验总结

- 用一个强基类统一弃牌体系逻辑，比每张牌各写一份更稳
- build 成功不等于游戏可见；`pck`、manifest 和 `BaseLib` 要一起检查
- 对 `CardCmd.Discard` 打日志，能快速判断问题出在“没弃掉”还是“弃掉了但没触发”
- 用反射发现 `DiscardModCard` 子类，可以避免文档更新了但代码注册表漏改
