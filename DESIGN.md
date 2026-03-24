# 设计

## 定位
记录当前弃牌触发体系的长期设计目标、规则边界和数值验证重点，避免卡牌扩展只靠临时记忆推进。

## 当前设计原则
- 同一张牌在“打出”和“被弃掉”两个时点都应具备价值
- 优先让玩家通过抽弃链路获得节奏，而不是靠单卡孤立爆发
- 设计验证必须依赖可观察信号，例如日志、卡池可见性和构建标记
- 先用简单规则把体系跑通，再按证据收紧边界

## 当前规则边界
现阶段采用统一规则：只要牌被弃掉，就触发这张牌自身的弃牌效果。该规则尚未区分主动弃牌、回合结束弃牌与其他来源弃牌。

## 重点验证对象
- `ShatteredEcho`
- `FinalDraft`
- `FadingFormula`
- `DarkFlameFragment`
- `ToxinRecord`

## 详细来源
- `docs/design-docs/index.md`
- `docs/design-docs/discard-system.md`
- `docs/product-specs/card-details.md`
