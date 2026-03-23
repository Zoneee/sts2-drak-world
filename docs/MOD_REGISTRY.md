# 卡牌注册表

这份文档只记录“当前仓库里真实存在什么”，并明确区分当前实现与设计目标。

## 1. 总览

- 所属卡池：`RegentCardPool`
- 当前卡牌数量：4
- 当前状态：全部已注册，效果未完成

## 2. 卡牌列表

| 中文名   | 类名                | 类型   | 稀有度   | 费用 | 目标     | 本地化键              | 当前实现                  | 设计目标                       |
| -------- | ------------------- | ------ | -------- | ---- | -------- | --------------------- | ------------------------- | ------------------------------ |
| 迅影斩   | `SwiftCut`          | Attack | Common   | 0    | AnyEnemy | `SWIFT_CUT`           | 已注册，`OnPlay()` 未完成 | 作为启动器，提供抽牌或连动能力 |
| 暗焰残页 | `DarkFlameFragment` | Skill  | Common   | 1    | AnyEnemy | `DARK_FLAME_FRAGMENT` | 已注册，`OnPlay()` 未完成 | 弃牌时造成群体伤害             |
| 毒素记录 | `ToxinRecord`       | Skill  | Uncommon | 1    | Self     | `TOXIN_RECORD`        | 已注册，`OnPlay()` 未完成 | 弃牌时施加持续伤害或 debuff    |
| 碎念回响 | `ShatteredEcho`     | Skill  | Rare     | 2    | Self     | `SHATTERED_ECHO`      | 已注册，`OnPlay()` 未完成 | 作为连锁牌扩大弃牌收益         |

## 3. 当前实现状态

| 项目                    | 状态 |
| ----------------------- | ---- |
| 卡牌类存在              | ✅    |
| 已加入 `RegentCardPool` | ✅    |
| 本地化键已补齐          | ✅    |
| `OnPlay()` 真实效果     | ❌    |
| 弃牌触发逻辑            | ❌    |

## 4. 当前与设计目标的边界

当前仓库里，设计说明不能当作功能现状。请按下面理解：

- [DESIGN.md](DESIGN.md) 说明“未来想做成什么”
- 本文说明“现在代码里真实有什么”
- [CHANGELOG.md](CHANGELOG.md) 说明“已经交付了什么”

## 5. 更新规则

新增或修改卡牌时，请同时更新：

1. 本文的卡牌列表
2. `src/localization/eng/cards.json`
3. `Main.cs` 中的注册代码
