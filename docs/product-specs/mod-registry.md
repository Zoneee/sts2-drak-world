# 模组卡牌注册表

状态：有效
负责人：产品与工程团队
最后评审：2026-03-24

## 概览
- 所属卡池：`RegentCardPool`
- 当前卡牌数量：10
- 注册方式：`[Pool(typeof(RegentCardPool))]`
- 当前状态：全部已实现 `OnPlay()`，并具备弃牌触发逻辑

## 卡牌列表
| 中文名   | 类名                  | 类型   | 稀有度   | 费用 | 目标     | 打出效果                   | 弃牌效果           |
| -------- | --------------------- | ------ | -------- | ---- | -------- | -------------------------- | ------------------ |
| 迅影斩   | `SwiftCut`            | Attack | Common   | 0    | AnyEnemy | 单体造成伤害               | 对随机敌人造成伤害 |
| 暗焰残页 | `DarkFlameFragment`   | Skill  | Common   | 1    | Self     | 抽 1 弃 1                  | 对所有敌人造成伤害 |
| 毒素记录 | `ToxinRecord`         | Skill  | Uncommon | 1    | AnyEnemy | 对目标施加中毒             | 对所有敌人施加中毒 |
| 破碎回响 | `ShatteredEcho`       | Skill  | Rare     | 2    | Self     | 抽 2 弃 1                  | 抽牌               |
| 灰烬庇护 | `AshenAegis`          | Skill  | Common   | 1    | Self     | 获得格挡                   | 获得格挡           |
| 崩坏手稿 | `CripplingManuscript` | Skill  | Uncommon | 1    | AnyEnemy | 对目标施加虚弱与易伤       | 对所有敌人施加虚弱 |
| 余烬连射 | `EmberVolley`         | Attack | Common   | 1    | AnyEnemy | 单体造成伤害               | 随机伤害并抽牌     |
| 回收思路 | `RecallSurge`         | Skill  | Uncommon | 1    | Self     | 抽 2 弃 1                  | 获得格挡           |
| 褪色公式 | `FadingFormula`       | Skill  | Common   | 0    | Self     | 抽牌；回合结束在手中会自弃 | 自弃后获得格挡     |
| 终稿余烬 | `FinalDraft`          | Attack | Rare     | 2    | AnyEnemy | 单体高伤                   | 全体伤害并抽牌     |

## 运行时诊断面
当前仓库会输出以下关键信号，用于确认注册是否成功：
- 初始化时输出自动发现的卡牌类型列表
- `ModelDb.Init` 后检查每张牌是否进库与进池
- 对 `CardCmd.Discard` 与 `DiscardAndDraw` 统一打日志
- 每张牌在 `OnPlay()` 和弃牌触发时输出上下文日志

## 规则边界
当前实现下，“这张牌被弃掉”就会触发它的弃牌效果。暂未区分：
- 主动弃牌
- 回合结束弃牌
- 其他来源弃牌

## 使用说明
若后续新增卡牌，应至少同步更新：
- 代码实现
- 本地化输入
- 运行时文本兜底
- 本注册表