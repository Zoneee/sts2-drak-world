# 开发指南

## 首次设置

### 环境要求

1. **.NET SDK 9.0+** — 验证：`dotnet --version`
2. **Slay the Spire 2**（Steam 安装）
3. `lib/sts2.dll` — 见下方步骤

### 获取 sts2.dll

`sts2.dll` 是游戏专有文件，不提交到 git。需手动从游戏目录复制：

**Windows（PowerShell）：**
```powershell
$src = "D:\G_games\steam\steamapps\common\Slay the Spire 2\data_sts2_windows_x86_64\sts2.dll"
$dst = "\\wsl$\Ubuntu\home\alphonse\projects\STS2-Dark-World\lib\sts2.dll"
Copy-Item $src $dst
```

**Linux：**
```bash
cp ~/.local/share/Steam/steamapps/common/Slay\ the\ Spire\ 2/data_sts2_linuxbsd_x86_64/sts2.dll \
   ~/projects/STS2-Dark-World/lib/
```

### 克隆 & 构建

```bash
git clone git@github.com:Zoneee/sts2-drak-world.git
cd sts2-drak-world
dotnet build src/
# Build succeeded. 0 Error(s), 4 Warning(s)  ← STS003 警告，正常
```

---

## 项目结构

```
src/
├── Main.cs                      # [ModInitializer] 入口，调用 RegisterCards()
├── Cards/
│   ├── DarkFlameFragment.cs     # 暗焰残页 — Skill/Common/1-cost
│   ├── SwiftCut.cs              # 迅影斩  — Attack/Common/0-cost
│   ├── ToxinRecord.cs           # 毒素记录 — Skill/Uncommon/1-cost
│   └── ShatteredEcho.cs         # 破碎回响 — Skill/Rare/2-cost
├── Utils/
│   └── Logger.cs                # MegaCrit Logger 封装（可选）
├── localization/
│   └── eng/cards.json           # 卡牌标题与描述（STS001 分析器要求）
├── project.godot                # Godot 工程引用（STS2 需要）
└── STS2_Discard_Mod.csproj      # 项目文件
```

**根目录：**
```
STS2_Discard_Mod.json    # mod 清单（id/has_dll/has_pck/dependencies/affects_gameplay）
lib/sts2.dll             # 游戏 DLL（gitignored）
nuget.config             # 指向本地 packages/ 目录
```

---

## 关键 STS2 API

### 卡牌基类：`CardModel`

所有卡牌继承自 `MegaCrit.Sts2.Core.Models.CardModel`：

```csharp
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using System.Threading.Tasks;

namespace DiscardMod.Cards;

public class MyCard : CardModel
{
    // 构造函数：(费用, 类型, 稀有度, 目标, 显示在卡库中)
    public MyCard()
        : base(1, CardType.Skill, CardRarity.Common, TargetType.Self, true) { }

    // 打出时触发
    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        DiscardModMain.Logger.Info("MyCard played");
        await Task.CompletedTask;
    }

    // 升级时触发
    public override void OnUpgrade() { }
}
```

**可用枚举值：**

| 枚举 | 可用值 |
|------|--------|
| `CardType` | `None, Attack, Skill, Power, Status, Curse, Quest` |
| `CardRarity` | `None, Basic, Common, Uncommon, Rare, Ancient, Event, Token, Status, Curse, Quest` |
| `TargetType` | `None, Self, AnyEnemy, AllEnemies, RandomEnemy, AnyPlayer, AnyAlly, AllAllies, TargetedNoCreature, Osty` |

### 卡牌注册：`ModHelper.AddModelToPool`

在 `Main.cs` 的 `RegisterCards()` 中注册：

```csharp
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models.CardPools;

ModHelper.AddModelToPool(typeof(RegentCardPool), typeof(MyCard));
```

**可用角色池：**
`IroncladCardPool`, `SilentCardPool`, `DefectCardPool`, `NecrobinderCardPool`,  
`RegentCardPool`, `ColorlessCardPool`, `CurseCardPool`, `StatusCardPool`, 等

### 本地化

每张 `CardModel` 子类都需要在 `localization/eng/cards.json` 中有对应条目，否则 STS001 报错：

```json
{
  "MY_CARD.title": "My Card",
  "MY_CARD.description": "Card description here."
}
```

键名规则：类名转大写下划线（`MyCard` → `MY_CARD`）。

> STS003 警告（建议继承 `BaseLib.Abstracts.CustomCardModel`）：当前直接继承 `CardModel` 即可正常使用，该警告不阻止构建或运行。

---

## 添加新卡牌

### 步骤

1. **创建卡牌文件** `src/Cards/YourCard.cs`（复制下方模板）
2. **添加本地化条目** 到 `src/localization/eng/cards.json`
3. **在 `Main.cs` 注册** — 调用 `ModHelper.AddModelToPool`
4. **构建验证** — `dotnet build src/` → 0 errors

### 完整模板

```csharp
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using System.Threading.Tasks;

namespace DiscardMod.Cards;

/// <summary>
/// 卡牌中文名 (EnglishName)
/// Type/Rarity/Cost/Target
/// 效果说明
/// </summary>
public class YourCard : CardModel
{
    public YourCard()
        : base(/*cost*/ 1, CardType.Skill, CardRarity.Common, TargetType.Self, true) { }

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        DiscardModMain.Logger.Info("YourCard played");
        // TODO: 实现效果
        await Task.CompletedTask;
    }

    public override void OnUpgrade()
    {
        // TODO: 升级效果
    }
}
```

### 对应的本地化条目

```json
"YOUR_CARD.title": "卡牌名",
"YOUR_CARD.description": "效果描述。"
```

### 对应的注册代码（Main.cs）

```csharp
ModHelper.AddModelToPool(typeof(RegentCardPool), typeof(YourCard));
```

---

## 构建与部署

### 本地构建

```bash
# Debug 构建（含调试符号）
dotnet build src/ --configuration Debug

# Release 构建（优化，用于分发）
dotnet build src/ --configuration Release

# 全量重建
dotnet clean src/ && dotnet build src/ --configuration Release
```

输出文件：`src/bin/Release/net9.0/STS2_Discard_Mod.dll`

### 部署到 STS2

csproj 内置 MSBuild target（`CopyToModsFolderOnBuild`），构建后自动将 DLL 和 `STS2_Discard_Mod.json` 部署到正确位置：

```
{游戏目录}/mods/STS2_Discard_Mod/
├── STS2_Discard_Mod.dll
└── STS2_Discard_Mod.json
```

**前提条件**：游戏已安装，csproj 中的路径能找到 `mods/` 目录。

**手动部署（Linux）：**
```bash
MOD_DIR="$HOME/.local/share/Steam/steamapps/common/Slay the Spire 2/mods/STS2_Discard_Mod"
mkdir -p "$MOD_DIR"
cp src/bin/Release/net9.0/STS2_Discard_Mod.dll "$MOD_DIR/"
cp STS2_Discard_Mod.json "$MOD_DIR/"
cp -r src/localization "$MOD_DIR/"
```

**手动部署（Windows PowerShell）：**
```powershell
$modDir = "D:\G_games\steam\steamapps\common\Slay the Spire 2\mods\STS2_Discard_Mod"
New-Item -ItemType Directory -Force -Path $modDir
Copy-Item src\bin\Release\net9.0\STS2_Discard_Mod.dll $modDir
Copy-Item STS2_Discard_Mod.json $modDir
Copy-Item src\localization $modDir -Recurse
```

### VS Code 一键部署

```
Ctrl+Shift+B  →  编译 Release + 自动部署
```

详见 [QUICK_START.md](QUICK_START.md)。

---

## 调试

### 日志查看

所有 mod 日志通过 `DiscardModMain.Logger`（`MegaCrit.Sts2.Core.Logging.Logger`）：

```csharp
DiscardModMain.Logger.Info("正常消息");
DiscardModMain.Logger.Warn("警告消息");
DiscardModMain.Logger.Error("错误消息");
DiscardModMain.Logger.Debug("调试消息");
```

**STS2 日志文件位置：**
- Windows: `%APPDATA%\Roaming\STS2\logs\`
- Linux: `~/.local/share/STS2/logs/`

过滤 mod 日志：搜索 `STS2DiscardMod`。

### 常见问题

**问题：mod 不加载（无 `Registered 4 discard-trigger cards` 日志）**
1. 确认 `mods/STS2_Discard_Mod/STS2_Discard_Mod.json` 存在且 `"has_dll": true`
2. 确认 `mods/STS2_Discard_Mod/STS2_Discard_Mod.dll` 存在
3. 重新构建：`dotnet clean src/ && dotnet build src/ --configuration Release`

**问题：构建报错 "STS2.dll not found"**
- `lib/sts2.dll` 不存在。按首次设置步骤从游戏目录复制。

**问题：STS001 分析器报错 "Localization X.title not found"**
- 在 `src/localization/eng/cards.json` 中添加对应的 `CARD_NAME.title` 和 `CARD_NAME.description` 条目。

**问题：构建报错 "MSB3270 (arch mismatch)"**
- 已在 csproj 中通过 `NoWarn` 抑制，正常可忽略。

---

## manifest 格式（STS2_Discard_Mod.json）

```json
{
  "id": "STS2DiscardMod",
  "name": "显示名称",
  "author": "作者",
  "description": "描述",
  "version": "v0.1.0",
  "has_pck": false,
  "has_dll": true,
  "dependencies": [],
  "affects_gameplay": true
}
```

`id` 必须与 `[ModInitializer]` 类中的 `ModId` 常量一致。
