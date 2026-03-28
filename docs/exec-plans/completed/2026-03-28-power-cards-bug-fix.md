# 能力牌四项 Bug 修复

## 标题
弃牌体系 V1.2 后：能力牌本地化、图标与升级描述修复

## 状态
已完成

## 背景
V1.2 实现了三张能力牌（AshVeil、DarkMomentum、VoidSurge）后，在游戏内测试中发现 4 个关联 bug。本计划记录根因分析、修复方案与验证证据。

## Bug 清单

### Bug #1 — 升级预览不更新描述
- **症状**：升级选择界面能力牌的 "升级后" 描述与升级前相同，数值无变化
- **根本原因**：三张 Power 卡的 `OnUpgrade()` 方法为空（仅 `{}`），且未声明 `CanonicalVars`，导致升级对比 UI 没有 DynamicVar 可追踪
- **修复**：在 `AshVeil.cs`、`DarkMomentum.cs`、`VoidSurge.cs` 添加 `CanonicalVars` 声明和 `UpgradeValueBy()` 调用

### Bug #2 — 打出动画文字显示 ID
- **症状**：打出能力牌时系统动画显示 `DISCARDMOD-DARK_MOMENTUM_POWER` 而非 `暗涌动能`
- **根本原因**：`LocalizationRuntimePatch` 仅拦截 `cards` 表，`powers` 表查询直接落到 BaseLib 未登记的路径，返回原始 key
- **修复**：扩展 `LocalizationRuntimePatch`，同时拦截 `powers` 表

### Bug #3 — 玩家状态栏缺少能力图标
- **症状**：打出能力牌后状态栏显示空白或默认图标，日志出现 `[WARN] AtlasResourceLoader: Missing sprite 'discardmod-ash_veil_power' in power_atlas`
- **根本原因**：三个 Power 类未重写 `CustomPackedIconPath`/`CustomBigIconPath`，BaseLib 回退到 atlas 路径（该路径不存在）
- **修复**：生成 6 张 PNG 占位图标，在三个 Power 类中重写路径属性

### Bug #4 — 状态栏 tooltip 描述错误
- **症状**：鼠标悬停能力图标时，tooltip 显示 key 原文或空白
- **根本原因**：同 Bug #2，`powers` 表文本查询未被拦截
- **修复**：同 Bug #2 的修复（统一修复）

## 范围
- `src/Cards/AshVeil.cs`
- `src/Cards/DarkMomentum.cs`
- `src/Cards/VoidSurge.cs`
- `src/Powers/AshVeilPower.cs`
- `src/Powers/DarkMomentumPower.cs`
- `src/Powers/VoidSurgePower.cs`
- `src/Patches/LocalizationRuntimePatch.cs`
- `src/STS2DiscardMod/images/powers/`（新增 6 张 PNG）

## 修改详情

### LocalizationRuntimePatch.cs
```csharp
// 修改前：仅处理 cards 表
if (!string.Equals(____name, "cards", StringComparison.OrdinalIgnoreCase)) { return true; }

// 修改后：同时处理 cards 和 powers 表
var isCardsTable = string.Equals(____name, "cards", StringComparison.OrdinalIgnoreCase);
var isPowersTable = string.Equals(____name, "powers", StringComparison.OrdinalIgnoreCase);
if (!isCardsTable && !isPowersTable) { return true; }
```

### AshVeil.cs
- 新增：`CanonicalVars => [new BlockVar(BaseBlock, ValueProp.Move)]`
- `OnUpgrade()` 改为：`DynamicVars.Block.UpgradeValueBy(UpgradedBlock - BaseBlock);`

### DarkMomentum.cs
- 新增：`CanonicalVars => [new CardsVar((int)BaseThreshold)]`
- `OnUpgrade()` 改为：`DynamicVars.Cards.UpgradeValueBy(UpgradedThreshold - BaseThreshold);`

### VoidSurge.cs
- 新增常量：`DamagePerHit = 3m`, `BaseHits = 2`, `UpgradedHits = 3`
- 新增：`CanonicalVars => [new DamageVar(DamagePerHit, ValueProp.Move), new CardsVar(BaseHits)]`
- `OnUpgrade()` 改为：`DynamicVars.Cards.UpgradeValueBy(UpgradedHits - BaseHits);`

### 三个 Power 类
各自新增：
```csharp
public override string? CustomPackedIconPath =>
    $"res://{DiscardModMain.ModId}/images/powers/{name}_64.png";
public override string? CustomBigIconPath =>
    $"res://{DiscardModMain.ModId}/images/powers/{name}.png";
```

### 新增图标文件
```
src/STS2DiscardMod/images/powers/
    ash_veil_power.png        (256×256，橙色圆形占位)
    ash_veil_power_64.png     (64×64，橙色圆形占位)
    dark_momentum_power.png   (256×256，蓝色圆形占位)
    dark_momentum_power_64.png(64×64，蓝色圆形占位)
    void_surge_power.png      (256×256，紫色圆形占位)
    void_surge_power_64.png   (64×64，紫色圆形占位)
```

## 验证证据
- `dotnet build src/STS2_Discard_Mod.csproj` 输出：`0 Error(s)`
- 1 个 CS8604 nullable warning（VoidSurgePower.Owner，已登记为 TD-001，可接受）
- 1 个 PCK skip warning（未设置 GODOT_CLI_COMMAND，已登记为 TD-003，可接受）

## 风险与后续事项
- 能力图标为占位占位符，需美术资源后替换（TD-002）
- 图标替换后需重新导出 PCK（TD-003）
- Bug #3 在游戏内的最终验证依赖 PCK 导出；本地化和升级描述修复（Bug #1/#2/#4）可在下次游戏启动时验证

## 决策日志
- 2026-03-28：`CanonicalVars` 中 AshVeil 使用 `BlockVar(ValueProp.Move)`，DarkMomentum 使用 `CardsVar(int)`（需显式转型），VoidSurge 使用 `DamageVar + CardsVar`
- 2026-03-28：图标路径不加 `res://` 前缀被确认为错误；修正为带 `res://` 前缀（与游戏 Godot 资源路径约定一致）
