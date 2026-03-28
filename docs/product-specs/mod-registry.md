# 模组卡牌注册表

状态：有效
负责人：产品与工程团队
最后评审：2026-03-28

## 概览
- 所属卡池：`RegentCardPool`
- 当前卡牌数量：13（含 3 张能力牌）
- 注册方式：`[Pool(typeof(RegentCardPool))]`
- 当前状态：全部已实现 `OnPlay()`，能力牌通过 `AfterCardDiscarded` 触发

## 卡牌列表
| 中文名   | 类名                  | 类型   | 稀有度   | 费用 | 目标     | 打出效果                             | 弃牌效果                             |
| -------- | --------------------- | ------ | -------- | ---- | -------- | ------------------------------------ | ------------------------------------ |
| 迅影斩   | `SwiftCut`            | Attack | Common   | 1    | AnyEnemy | 单体造成 6 伤害                      | 对随机敌人造成 7 伤害                |
| 暗焰残页 | `DarkFlameFragment`   | Skill  | Uncommon | 2    | Self     | 抽 1 弃 2                            | 再弃 1 张牌                          |
| 毒素记录 | `ToxinRecord`         | Skill  | Uncommon | 1    | AnyEnemy | 对目标施加 5 层中毒                  | 对所有敌人施加 3 层中毒              |
| 破碎回响 | `ShatteredEcho`       | Skill  | Rare     | 3    | Self     | 抽 2 弃 2                            | 抽 1 弃 1                            |
| 灰烬庇护 | `AshenAegis`          | Skill  | Common   | 1    | Self     | 弃 1 张手牌后获得 8 格挡             | 获得 10 格挡                         |
| 崩坏手稿 | `CripplingManuscript` | Skill  | Uncommon | 1    | AnyEnemy | 对目标施加 2 虚弱和 2 易伤           | 对所有敌人施加 1 虚弱和 1 易伤       |
| 余烬连射 | `EmberVolley`         | Attack | Uncommon | 2    | AnyEnemy | 单体造成 4 伤害 ×3                   | 对随机敌人造成 4 伤害 ×2             |
| 回收思路 | `RecallSurge`         | Skill  | Common   | 1    | Self     | 从弃牌堆取回 1 张牌（玩家选择）      | 获得 7 格挡                          |
| 褪色公式 | `FadingFormula`       | Skill  | Rare     | 3    | AnyEnemy | 对一个敌人造成 10 伤害并获得 10 格挡 | 奇数弃牌造成高伤，偶数弃牌获得高格挡 |
| 终稿余烬 | `FinalDraft`          | Attack | Rare     | 3    | AnyEnemy | 单体高伤                             | 全体高伤，已弃 ≥2 张牌后更高         |
| 暗涌动能 | `DarkMomentum`        | Skill  | Uncommon | 1    | Self     | 施加 1 层暗涌动能（阈值 3）          | ——（能力牌）                         |
| 灰烬面纱 | `AshVeil`             | Skill  | Common   | 1    | Self     | 施加 3 层灰烬面纱                    | ——（能力牌）                         |
| 虚空涌动 | `VoidSurge`           | Attack | Rare     | 2    | Self     | 施加 1 层虚空涌动                    | ——（能力牌）                         |

## 能力（PowerModel）列表
| 中文名   | 类名                | 触发条件                   | 效果                                                    |
| -------- | ------------------- | -------------------------- | ------------------------------------------------------- |
| 暗涌动能 | `DarkMomentumPower` | 每 Amount 次因卡牌效果弃牌 | 获得 1 点能量（累计计数，每回合结束清零）               |
| 灰烬面纱 | `AshVeilPower`      | 每次因卡牌效果弃牌         | 获得 Amount 点格挡                                      |
| 虚空涌动 | `VoidSurgePower`    | 每次因卡牌效果弃牌         | 对随机敌人造成 3 伤害 × Hits 次（基础 2 次，升级 3 次） |

## 运行时诊断面
当前仓库会输出以下关键信号，用于确认注册是否成功：
- 初始化时输出自动发现的卡牌类型列表
- `ModelDb.Init` 后检查每张牌是否进库与进池
- 对 `CardCmd.Discard` 与 `DiscardAndDraw` 统一打日志
- 每张牌在 `OnPlay()` 和弃牌触发时输出上下文日志
- 弃牌来源与本回合弃牌次数

## 规则边界
当前实现下，默认只有因卡牌效果导致的弃牌才会触发弃牌效果及能力牌效果。

能力牌触发通过 `DiscardTriggerRuntime.ConsumePowerDiscardEvent(card)` 判断，与普通弃牌触发规则完全一致。

显式例外：
- `FadingFormula` 在牌面写明，回合结束自弃也会触发。

打出约束：
- 带有打出时弃手牌要求的卡，打出前会校验当前手中是否已有足够其他手牌可弃。
- 待生效的额外弃牌次数也会计入这次校验。

## 使用说明
若后续新增卡牌，应至少同步更新：
- 代码实现
- 本地化输入（cards.catalog.json）
- `DebugStartingDeckPatch.cs`
- 本注册表