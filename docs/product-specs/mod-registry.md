# 模组卡牌注册表

状态：有效
负责人：产品与工程团队
最后评审：2026-03-26

## 概览
- 所属卡池：`RegentCardPool`
- 当前卡牌数量：10
- 注册方式：`[Pool(typeof(RegentCardPool))]`
- 当前状态：全部已实现 `OnPlay()`，并具备弃牌触发逻辑

## 卡牌列表
| 中文名   | 类名                  | 类型   | 稀有度   | 费用 | 目标     | 打出效果                             | 弃牌效果                             |
| -------- | --------------------- | ------ | -------- | ---- | -------- | ------------------------------------ | ------------------------------------ |
| 迅影斩   | `SwiftCut`            | Attack | Common   | 1    | AnyEnemy | 单体造成 6 伤害                      | 对随机敌人造成 7 伤害                |
| 暗焰残页 | `DarkFlameFragment`   | Skill  | Uncommon | 2    | Self     | 抽 1 弃 2                            | 再弃 1 张牌                          |
| 毒素记录 | `ToxinRecord`         | Skill  | Uncommon | 1    | AnyEnemy | 对目标施加 5 层中毒                  | 对所有敌人施加 3 层中毒              |
| 破碎回响 | `ShatteredEcho`       | Skill  | Rare     | 3    | Self     | 抽 2 弃 3                            | 抽 1 弃 1                            |
| 灰烬庇护 | `AshenAegis`          | Skill  | Common   | 1    | Self     | 获得 8 格挡                          | 获得 10 格挡                         |
| 崩坏手稿 | `CripplingManuscript` | Skill  | Uncommon | 1    | AnyEnemy | 对目标施加 2 虚弱和 2 易伤           | 对所有敌人施加 1 虚弱和 1 易伤       |
| 余烬连射 | `EmberVolley`         | Attack | Uncommon | 2    | AnyEnemy | 单体造成 10 伤害                     | 对随机敌人造成 12 伤害               |
| 回收思路 | `RecallSurge`         | Skill  | Common   | 1    | Self     | 抽 1 弃 1                            | 获得 7 格挡                          |
| 褪色公式 | `FadingFormula`       | Skill  | Rare     | 3    | AnyEnemy | 对一个敌人造成 10 伤害并获得 10 格挡 | 奇数弃牌造成高伤，偶数弃牌获得高格挡 |
| 终稿余烬 | `FinalDraft`          | Attack | Rare     | 3    | AnyEnemy | 单体高伤                             | 全体高伤，满足弃牌阈值后更高         |

## 运行时诊断面
当前仓库会输出以下关键信号，用于确认注册是否成功：
- 初始化时输出自动发现的卡牌类型列表
- `ModelDb.Init` 后检查每张牌是否进库与进池
- 对 `CardCmd.Discard` 与 `DiscardAndDraw` 统一打日志
- 每张牌在 `OnPlay()` 和弃牌触发时输出上下文日志
- 弃牌来源与本回合弃牌次数

## 规则边界
当前实现下，默认只有因卡牌效果导致的弃牌才会触发弃牌效果。

显式例外：
- `FadingFormula` 在牌面写明，回合结束自弃也会触发。

打出约束：
- 带有打出时弃手牌要求的卡，打出前会校验当前手中是否已有足够其他手牌可弃。
- 待生效的额外弃牌次数也会计入这次校验。

## 使用说明
若后续新增卡牌，应至少同步更新：
- 代码实现
- 本地化输入
- 运行时文本兜底
- 本注册表