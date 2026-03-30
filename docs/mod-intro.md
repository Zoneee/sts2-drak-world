# 黑暗手稿 · Dark World Discard Mod

> 为《Slay the Spire 2》奥法师设计的弃牌触发系统，共 13 张牌。
> A discard-trigger system for the Mystic in *Slay the Spire 2*, featuring 13 cards.

---

# 中文版

## 简介

每张牌在两个时刻都有价值：**打出时**产生稳定效果，**被弃掉时**触发另一种效果。
弃牌不是代价——是另一次出手。

这套牌围绕「弃牌触发」设计，通过主动构建弃牌链路，在同一回合叠加多次效果，建立属于奥法师的燃纸得力体系。

---

## 背景故事 — 记录者与暗界

在时间折叠之处，尖塔留下了一道镜像裂隙。那里的规则与现实颠倒：物质不在存在时释放能量，而在消亡时才爆炸性地迸发。这个地方被研究者称为**暗界**（Dark World）。

数百年前，一位名为「**记录者**」（The Archivist）的学者进入暗界边缘，花费数十年撰写了一本前无古人的禁忌手稿。这本书记载了一套「以毁灭换取能量」的施法体系——每一个字在书写时都向暗界释放结构能量，被焚毁时才完全引爆。这本手稿从未被任何人读过。它本来就不是用来阅读的。

记录者最终消失在暗界之中，只留下散落于尖塔各处的孤页残片——**暗焰残页、崩坏手稿、终稿余烬、毒素记录**——每一页都是半完成的咒式，等待被弃掉的那一刻。

奥法师在某次探索中拾到了其中一页。她漫不经心地将它扔进弃牌堆，然后看到了空气中爆出的弧光。

这一刻她领悟了记录者留下的秘密：**写下来不是为了阅读，而是为了被抛弃。**

---

## 设计意图

- **双时点价值**：每张牌在「打出」和「被弃掉」两个节点均有效果。打出时效果稳定，弃牌触发效果往往更强，以抵消主动弃牌的机会成本。
- **主动链路优于孤立爆发**：核心节奏来自构建弃牌链——通过「暗焰残页」「破碎回响」等链路卡持续推进弃牌事件，而不是靠某一张高费单卡孤立爆发。
- **能力牌将弃牌转化为持续红利**：「暗涌动能」「灰烬面纱」「虚空涌动」三张能力牌在放置后，将每次弃牌事件转化为能量、格挡或伤害，使弃牌链路的每一步都产生复合效益。

> ⚠ 触发规则：只有**因卡牌效果触发的弃牌**才会激活弃牌效果。回合结束时被迫丢弃的手牌默认不触发——「褪色公式」是牌面明确标注的显式例外。

---

## 卡牌一览

| 稀有度   | 卡名         | 费用  | 打出效果                                | 弃牌触发效果                                                 |
| -------- | ------------ | :---: | --------------------------------------- | ------------------------------------------------------------ |
| Common   | **暗焰残页** |   1   | 抽 1 弃 2                               | 再弃 1                                                       |
| Common   | **迅影斩**   |   1   | 造成 6 伤                               | 对随机敌人额外造成 7 伤                                      |
| Common   | **毒素记录** |   1   | 施加 5 层中毒                           | 对所有敌人施加 3 层中毒                                      |
| Uncommon | **灰烬庇护** |   2   | 获得 8 格挡，然后弃 1                   | 获得 10 格挡                                                 |
| Uncommon | **崩坏手稿** |   2   | 施加 2 虚弱 + 2 易伤                    | 对所有敌人施加 1 虚弱 + 1 易伤                               |
| Uncommon | **余烬连射** |   2   | 造成 4 伤 × 3 次                        | 对随机敌人造成 4 伤 × 2 次                                   |
| Uncommon | **回收思路** |   2   | 从弃牌堆取回 1 张                       | 取回 1，抽 1 弃 1，置顶 1                                    |
| Rare     | **破碎回响** |   2   | 抽 2 弃 2                               | 抽 1 弃 1（续链）                                            |
| Rare     | **褪色公式** |   3   | 10 伤 + 10 格挡；回合末未打出则自弃触发 | 奇数次弃牌 → 18 伤；偶数次弃牌 → 18 格挡                     |
| Rare     | **终稿余烬** |   2   | 造成 16 伤                              | 对所有敌人 10 伤；本回合第 3 次起每多弃 1 额外 +3（最多 +9） |
| Power    | **暗涌动能** |   1   | 放置能力                                | 每弃 3 张牌 → 获得 1 点能量                                  |
| Power    | **灰烬面纱** |   1   | 放置能力                                | 每次弃牌 → 获得 3 点格挡                                     |
| Power    | **虚空涌动** |   1   | 放置能力                                | 每次弃牌 → 对随机敌人造成 3 伤 × 2                           |

---

## 游玩方式

**启动链路 — 推进弃牌事件**

用「暗焰残页」（抽1弃2，弃牌再弃1）或「破碎回响」（抽2弃2，弃牌再抽1弃1）在单回合内快速积累弃牌次数，为后续更强的效果蓄力。

**能量引擎**

提前打出「暗涌动能」，随后的弃牌链路每触发 3 次即额外获得 1 点能量。能量盈余可以支撑更长的行动序列。

**终结压轴**

「终稿余烬」打出时已造成 16 伤；若当前回合弃牌次数达到阈值（默认第 3 次起），弃牌触发的全体伤害额外 +3（最高 +9）。在链路积累充足后，它是一张合适的收尾牌。

**防御线**

「灰烬面纱」能力 + 「灰烬庇护」打出时弃牌换格挡，可在弃牌链的每一步叠加格挡总量，在进攻节奏中维持生存。

**规则例外 — 褪色公式**

「褪色公式」打出后不会立即消耗——若回合结束时它仍在手牌中，会自动触发弃牌效果。弃牌触发根据本回合整体弃牌奇偶次数分叉：奇数次 → 18 伤，偶数次 → 18 格挡。基于链路位置策略性地使用它。

---

## 安装方式

1. 在发布页下载最新版 ZIP 文件：
   - 原版：`STS2_Discard_Mod_v*.zip`
   - 含预组卡组版（附带初始弃牌卡组）：`STS2_Discard_Mod_StarterDeck_v*.zip`
2. 解压，将 `STS2_Discard_Mod/` 文件夹放入游戏的 `mods/` 目录（通常位于游戏安装目录下）。
3. 启动游戏，在 Mod 选择页面启用「STS2 Discard-Trigger Mod (弃触发)」。

> ZIP 包根目录内含 `RELEASE_NOTES.md`（当前版本变更说明）。

---

## 版本 & Changelog

完整版本记录见仓库根目录 [`CHANGELOG.md`](../CHANGELOG.md)。

当前版本号参见仓库中的 `STS2_Discard_Mod.json`（`version` 字段）。

---
---

# English

## Introduction

Every card has two faces: a **play effect** that triggers when you use it, and a **discard effect** that activates when it leaves your hand. Discarding isn't a cost — it's a second action.

This mod adds a discard-trigger system for the Mystic built around active chaining: stack multiple discard events in a single turn to generate compounding returns across damage, block, and energy.

---

## Lore — The Archivist and the Dark World

Where time folds, the Spire leaves a mirror-rift. There, the rules are inverted: matter does not release energy by existing, but by ceasing to exist — violently, completely, at the moment of destruction. Scholars who have glimpsed it call this place the **Dark World**.

Centuries ago, a scholar known only as **The Archivist** spent decades at the edge of this rift, composing a forbidden manuscript. The book described a spellcasting system built on *destruction as currency* — each word inscribed released structural energy into the Dark World, detonating fully only when the page was burned. The manuscript was never meant to be read.

The Archivist vanished into the rift, leaving only scattered fragments across the Spire: **Dark Flame Fragment, Crippling Manuscript, Final Draft, Toxin Record** — each a half-finished incantation awaiting the moment of discard.

The Mystic found one during an exploration. She tossed it into her discard pile on instinct, then saw the arc of released energy flare across the corridor.

In that moment she understood the secret the Archivist had left behind: **written not to be read, but to be discarded.**

---

## Design Intent

- **Dual-timing value**: Every card has two meaningful effects — a stable play effect and an amplified discard effect that compensates for the opportunity cost of discarding.
- **Active chains over isolated burst**: Core tempo comes from chaining discard events via cards like Dark Flame Fragment and Shattered Echo, rather than relying on a single high-cost card.
- **Power cards convert discards to sustained dividends**: Dark Momentum, Ash Veil, and Void Surge convert each discard event into energy, block, or damage respectively, compounding the value of every step in the chain.

> ⚠ Trigger rule: Only discards **caused by card effects** activate discard triggers. Discards at end of turn do not trigger by default — Fading Formula is the explicitly labeled exception.

---

## Card List

| Rarity   | Card                     | Cost  | Play Effect                                               | Discard Trigger                                                  |
| -------- | ------------------------ | :---: | --------------------------------------------------------- | ---------------------------------------------------------------- |
| Common   | **Dark Flame Fragment**  |   1   | Draw 1, discard 2                                         | Discard 1 more                                                   |
| Common   | **Swift Cut**            |   1   | Deal 6 damage                                             | Deal 7 damage to a random enemy                                  |
| Common   | **Toxin Record**         |   1   | Apply 5 poison                                            | Apply 3 poison to all enemies                                    |
| Uncommon | **Ashen Aegis**          |   2   | Gain 8 Block, then discard 1                              | Gain 10 Block                                                    |
| Uncommon | **Crippling Manuscript** |   2   | Apply 2 Weak + 2 Vulnerable                               | Apply 1 Weak + 1 Vulnerable to all enemies                       |
| Uncommon | **Ember Volley**         |   2   | Deal 4 damage × 3                                         | Deal 4 damage × 2 to a random enemy                              |
| Uncommon | **Recall Surge**         |   2   | Return 1 card from discard pile                           | Return 1, draw 1, discard 1, top-deck 1                          |
| Rare     | **Shattered Echo**       |   2   | Draw 2, discard 2                                         | Draw 1, discard 1 (chain continues)                              |
| Rare     | **Fading Formula**       |   3   | Deal 10 dmg + gain 10 Block; auto-discards at end of turn | Odd discard this turn → 18 dmg; Even → 18 Block                  |
| Rare     | **Final Draft**          |   2   | Deal 16 damage                                            | Deal 10 to all enemies; 3rd+ discard this turn: +3 each (max +9) |
| Power    | **Dark Momentum**        |   1   | Place power                                               | Every 3 discards → gain 1 Energy                                 |
| Power    | **Ash Veil**             |   1   | Place power                                               | Each discard → gain 3 Block                                      |
| Power    | **Void Surge**           |   1   | Place power                                               | Each discard → deal 3 damage × 2 to a random enemy               |

---

## How to Play

**Chain starters — build discard events**

Use **Dark Flame Fragment** (draw 1 discard 2 → discard trigger: discard 1 more) or **Shattered Echo** (draw 2 discard 2 → discard trigger: draw 1 discard 1) to rapidly stack discard events within a single turn, priming stronger effects downstream.

**Energy engine**

Play **Dark Momentum** early. The power converts every 3 discard events into 1 Energy, letting a dense discard turn fund additional plays and extend the action sequence.

**Finisher**

**Final Draft** deals 16 damage on play; if the current turn's discard count reaches the threshold (3rd discard by default), its discard trigger adds +3 damage per additional discard beyond that (up to +9). Land it after a primed chain for maximum output.

**Defense line**

**Ash Veil** (power) + **Ashen Aegis** (discard 1 for block on play) let you stack Block at each link of the discard chain, sustaining survivability while pushing offense.

**Parity exception — Fading Formula**

Fading Formula does not immediately resolve after play — if it remains in your hand at end of turn, it auto-discards and fires its trigger. The trigger branches on whether this is your odd or even discard this turn: odd → 18 damage, even → 18 Block. Position it deliberately in your chain.

---

## Installation

1. Download the latest ZIP from the release page:
   - Vanilla: `STS2_Discard_Mod_v*.zip`
   - With starter deck (pre-built discard deck): `STS2_Discard_Mod_StarterDeck_v*.zip`
2. Extract the archive and place the `STS2_Discard_Mod/` folder inside your game's `mods/` directory (typically inside the game installation folder).
3. Launch the game and enable **"STS2 Discard-Trigger Mod (弃触发)"** in the Mods menu.

> The ZIP root includes `RELEASE_NOTES.md` with the changes for this release.

---

## Version & Changelog

Full version history: [`CHANGELOG.md`](../CHANGELOG.md) in the repository root.

Current version: see the `version` field in `STS2_Discard_Mod.json`.
