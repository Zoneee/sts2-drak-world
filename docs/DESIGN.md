# 卡组设计文档

这份文档描述当前已经落地的弃牌体系，以及下一轮值得继续推进的设计方向。

## 1. 设计目标

这套卡组的核心体验不是“打出才有收益”，而是：

1. 通过抽牌找到关键牌
2. 通过弃牌动作把收益牌送进弃牌堆
3. 在弃牌瞬间得到伤害、抽牌、防御或 debuff 收益
4. 用更多抽弃牌动作把收益继续放大

## 2. 当前已落地的 10 张牌

### 原有 4 张核心牌

- `SwiftCut`：低费启动器，打出是伤害，弃掉也有补刀
- `DarkFlameFragment`：抽弃中继，弃掉时提供群体伤害
- `ToxinRecord`：单体上毒 + 群体上毒的持续收益牌
- `ShatteredEcho`：抽弃引擎，弃掉后继续补牌

### 本轮新增 6 张扩展牌

- `AshenAegis`：防御向弃牌收益
- `CripplingManuscript`：群体 debuff 向弃牌收益
- `EmberVolley`：伤害 + 过牌的节奏牌
- `RecallSurge`：偏保守的抽弃中继
- `FadingFormula`：会在回合结束自动弃掉自身的自触发牌
- `FinalDraft`：偏收尾的高质量伤害牌

## 3. 当前体系分层

### 启动

- `SwiftCut`
- `DarkFlameFragment`
- `ShatteredEcho`
- `RecallSurge`

### 伤害结算

- `DarkFlameFragment`
- `EmberVolley`
- `FinalDraft`

### 持续压制

- `ToxinRecord`
- `CripplingManuscript`

### 防御兜底

- `AshenAegis`
- `RecallSurge`
- `FadingFormula`

## 4. 当前规则取舍

本轮为了先把体系跑通，采用了最直接的规则：

> 只要这张牌被弃掉，就触发它的弃牌效果。

优点：

- 实现稳定
- 调试简单
- 玩家容易理解

代价：

- 还没有区分“主动弃牌”和“回合结束弃牌”
- 某些牌在未来可能需要更严格的触发边界

## 5. 下一轮最值得做的事

- 给“弃牌触发”补一个正式关键词或规则词
- 评估是否只允许“因卡牌效果弃牌”时触发
- 增加更多与弃牌联动的防御牌、力量牌或能量牌
- 补完整卡图资源导出流程，减少 `.pck` 缺失导致的假问题
- 做实机数值验证，尤其是 `FinalDraft`、`ShatteredEcho` 这类容易滚雪球的牌
