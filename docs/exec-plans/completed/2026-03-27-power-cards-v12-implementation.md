# 能力牌新增与卡牌重设计 V1.2 实施计划

## 标题
弃牌体系 V1.2：能力牌新增与卡牌重设计

## 状态
已完成

## 背景
V1.1 已完成基础弃牌触发边界建设与 10 张卡的初始实现。
本阶段（V1.2）进一步聚焦两块工作：

1. **卡牌重设计**：修正 V1.1 中若干卡牌在体验上的问题
   - EmberVolley/RecallSurge 名义与效果对不上
   - FinalDraft 弃牌阈值过低
   - ShatteredEcho 打出成本过高
   - AshenAegis 防御被动感太强
2. **新增 3 张能力牌**：补充弃牌回报维度
   - DarkMomentum（暗涌动能）：能量引擎
   - AshVeil（灰烬面纱）：格挡累积
   - VoidSurge（虚空涌动）：持续输出

## 问题摘要
- EmberVolley 打出只造成单次伤害，不符合"连射"语义
- RecallSurge "抽1弃1"语义与名称"回收思路"偏离；改为从弃牌堆取回一张牌
- FinalDraft 弃牌阈值 3 张在 Common 回合触发概率偏低
- ShatteredEcho 打出弃 3 张入手成本过高，导致玩家不愿主动打出
- AshenAegis 纯格挡无交互，被动体验弱；改为打出时弃牌换取格挡
- 缺乏能量、格挡、持续伤害三类弃牌回报的 Power 牌

## 范围
- 修改 5 张现有牌：EmberVolley, RecallSurge, FinalDraft, ShatteredEcho, AshenAegis
- 新增 3 张 Power 卡：DarkMomentum, AshVeil, VoidSurge
- 新增 3 个 PowerModel：DarkMomentumPower, AshVeilPower, VoidSurgePower
- 扩展 DiscardTriggerRuntime：PowerPendingEvents + ConsumePowerDiscardEvent
- 修正 CardLocalizationGenerator 以生成 powers.json
- 更新 csproj AdditionalFiles 为 glob 模式
- 更新 DebugStartingDeckPatch、cards.catalog.json、card-details.md、mod-registry.md、discard-system.md

## 非范围
- 消散牌、双打消散牌等新机制
- 卡图资源制作
- 能力牌之间的交互优化

## 受影响区域
- `src/Cards/EmberVolley.cs`
- `src/Cards/RecallSurge.cs`
- `src/Cards/FinalDraft.cs`
- `src/Cards/ShatteredEcho.cs`
- `src/Cards/AshenAegis.cs`
- `src/Cards/DarkMomentum.cs`（新增）
- `src/Cards/AshVeil.cs`（新增）
- `src/Cards/VoidSurge.cs`（新增）
- `src/Powers/DarkMomentumPower.cs`（新增）
- `src/Powers/AshVeilPower.cs`（新增）
- `src/Powers/VoidSurgePower.cs`（新增）
- `src/Utils/DiscardTriggerRuntime.cs`
- `src/Data/cards.catalog.json`
- `src/STS2_Discard_Mod.csproj`
- `src/Patches/DebugStartingDeckPatch.cs`
- `tools/CardLocalizationGenerator/Program.cs`
- `src/localization/*/cards.json`（自动生成）
- `src/localization/*/powers.json`（自动生成，新增）
- `docs/product-specs/card-details.md`
- `docs/product-specs/mod-registry.md`
- `docs/design-docs/discard-system.md`

## 约束
- PowerModel 使用 BaseLib `CustomPowerModel`，触发通过 `DiscardTriggerRuntime.ConsumePowerDiscardEvent` 判断
- Power 本地化走 `powers.json`，卡牌本地化走 `cards.json`，通过 catalog 中是否有 `smartDescription` 字段区分
- 力求每次改动最小化，能力牌各自独立实现

## 验收标准
- `dotnet build` 0 编译错误，0 STS001 错误
- DebugStartingDeckPatch 包含全部 13 张牌（10 原有 + 3 新能力牌）
- cards.catalog.json 包含 3 张新卡牌条目与 3 条能力 Power 条目
- 生成文件 `localization/eng/cards.json` 与 `localization/eng/powers.json` 正确拆分
- mod-registry.md 与 discard-system.md 同步更新

## 验证计划
- 执行 `dotnet build src/STS2_Discard_Mod.csproj -p:SkipGodotPackExport=true`
- 检查输出：0 Error(s)

## 风险
- VoidSurgePower.cs 中 `Owner` 空引用（CS8604 nullable warning）；Owner 在 AfterCardDiscarded 中应始终非空，接受现有 warning
- PowerModel 能力叠加逻辑：DarkMomentumPower 的累计计数跨回合累积，需在 `BeforeTurnEnd` 正确清零

## 决策日志
- 2026-03-27：EmberVolley 改为打出 4×3、弃牌 4×2 多段伤害
- 2026-03-27：RecallSurge 改为从弃牌堆取回 1 张牌（玩家选择），弃牌触发保留格挡
- 2026-03-27：ShatteredEcho 打出弃牌数 3→2
- 2026-03-27：FinalDraft 弃牌阈值 3→2
- 2026-03-27：AshenAegis 打出时改为弃 1 换格挡，强化主动交互
- 2026-03-27：PowerPendingEvents 通过标记-消费模式解决 `AfterCardDiscarded` 早于 Power 外部状态更新的时序问题
- 2026-03-27：CardLocalizationGenerator 按 `smartDescription` 字段存在与否将条目分别写入 `powers.json` / `cards.json`
- 2026-03-27：csproj AdditionalFiles 改为 glob `localization/**/*.json` 与 ModTemplate 模式一致

## 进展日志
- 2026-03-27：完成 5 张牌重设计实现
- 2026-03-27：完成 3 张 Power 卡与 3 个 PowerModel 实现
- 2026-03-27：通过反射发现并修复所有 API 调用问题（PowerModel 抽象成员、CreatureCmd、PlayerCmd 签名等）
- 2026-03-27：完成 CardLocalizationGenerator 扩展，生成 powers.json
- 2026-03-27：完成 cards.catalog.json 更新（6 条新条目）
- 2026-03-27：`dotnet build` 验证通过：**0 Error(s)**，1 可接受 nullable warning
- 2026-03-27：同步更新 card-details.md、mod-registry.md、discard-system.md

## 后续事项
- 实机验证 DarkMomentumPower 能量阈值体验（3 次/2 次）
- 实机验证 VoidSurgePower 伤害是否与现有 HP 量级匹配
- 评估 AshVeil + ShatteredEcho 的格挡链路是否过强
