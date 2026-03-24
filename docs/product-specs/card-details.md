# 卡牌详细设计

状态：有效
负责人：产品与工程团队
最后评审：2026-03-24

## 体系目标
当前套牌的核心不是单纯的“打出收益”，而是让同一张牌在两个时点都具备价值：
- 打出时提供基础节奏
- 被弃掉时提供二次收益

## 卡牌总览
| 卡牌                  | 费用 | 类型   | 稀有度   | 打出效果                   | 弃牌效果                                | 升级方向                          | 设计定位           |
| --------------------- | ---- | ------ | -------- | -------------------------- | --------------------------------------- | --------------------------------- | ------------------ |
| `SwiftCut`            | 0    | Attack | Common   | 对单体造成 5 伤害          | 对随机敌人造成 3 伤害                   | 打出伤害 +2，弃牌伤害 +2          | 低费启动、补刀     |
| `DarkFlameFragment`   | 1    | Skill  | Common   | 抽 1，弃 1                 | 对所有敌人造成 6 伤害                   | 抽牌 +1，弃牌伤害 +3              | 抽弃中继、群攻触发 |
| `ToxinRecord`         | 1    | Skill  | Uncommon | 对单体施加 4 层中毒        | 对所有敌人施加 2 层中毒                 | 打出中毒 +2，弃牌中毒 +1          | 持续压制           |
| `ShatteredEcho`       | 2    | Skill  | Rare     | 抽 2，弃 1                 | 抽 2                                    | 抽牌 +1                           | 引擎核心、滚雪球   |
| `AshenAegis`          | 1    | Skill  | Common   | 获得 8 格挡                | 获得 5 格挡                             | 打出格挡 +3，弃牌格挡 +3          | 防御兜底           |
| `CripplingManuscript` | 1    | Skill  | Uncommon | 对单体施加 2 虚弱和 2 易伤 | 对所有敌人施加 1 虚弱                   | 打出虚弱 +1、易伤 +1，弃牌虚弱 +1 | 群体减益           |
| `EmberVolley`         | 1    | Attack | Common   | 对单体造成 7 伤害          | 对随机敌人造成 4 伤害并抽 1             | 打出伤害 +3，弃牌伤害 +2          | 节奏伤害、过牌     |
| `RecallSurge`         | 1    | Skill  | Uncommon | 抽 2，弃 1                 | 获得 4 格挡                             | 抽牌 +1，弃牌格挡 +2              | 保守型中继         |
| `FadingFormula`       | 0    | Skill  | Common   | 抽 1                       | 回合结束若留在手中会自弃，并获得 6 格挡 | 抽牌 +1，弃牌格挡 +2              | 自动触发、防御缓冲 |
| `FinalDraft`          | 2    | Attack | Rare     | 对单体造成 12 伤害         | 对所有敌人造成 8 伤害并抽 1             | 打出伤害 +4，弃牌伤害 +4          | 收尾、爆发         |

## 分层整理
### 启动与中继
- `SwiftCut`
- `DarkFlameFragment`
- `RecallSurge`
- `ShatteredEcho`

### 输出结算
- `EmberVolley`
- `FinalDraft`
- `DarkFlameFragment`

### 持续压制与减益
- `ToxinRecord`
- `CripplingManuscript`

### 防御兜底
- `AshenAegis`
- `RecallSurge`
- `FadingFormula`

## 重点验证对象
优先级从高到低：
1. `ShatteredEcho`
2. `FinalDraft`
3. `FadingFormula`
4. `DarkFlameFragment`
5. `ToxinRecord`

原因：
- `ShatteredEcho` 最容易形成抽牌回环。
- `FinalDraft` 最容易在稀有位上超出单卡强度。
- `FadingFormula` 的自弃机制天然降低触发门槛。
- `DarkFlameFragment` 决定常见位的群伤手感。
- `ToxinRecord` 在多目标战斗里收益上限较高。

## 当前规则定义
当前版本采用统一规则：只要牌被弃掉，就触发这张牌自身的弃牌效果。这意味着主动弃牌、回合结束弃牌和自弃当前都会触发。

## 后续扩展方向
- 引入区分“主动弃牌”和“回合结束弃牌”的关键词牌。
- 增加更高风险高回报的能量牌或返手牌。
- 为防御轴补一个真正的高费终端牌。
- 强化减益轴与毒、虚弱层数的结算关系。