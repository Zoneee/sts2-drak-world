# Mod Card Registry

**Complete catalog of all cards in the STS2 Discard-Trigger Mod**

> Last Updated: Framework phase complete (build passing, OnPlay stubs pending)
> Mod Version: v0.1.0-alpha (pre-release)

---

## Card Database

All cards belong to `RegentCardPool` (储君 character).

### 迅影斩 (Swift Cut)

| Property | Value |
| --- | --- |
| Class | `SwiftCut` |
| Cost | 0 |
| Type | Attack |
| Rarity | Common |
| Target | AnyEnemy |
| Localization key | `SWIFT_CUT` |
| Status | ✅ Registered · 🚧 OnPlay stub |

---

### 暗焰残页 (Dark Flame Fragment)

| Property | Value |
| --- | --- |
| Class | `DarkFlameFragment` |
| Cost | 1 |
| Type | Skill |
| Rarity | Common |
| Target | AnyEnemy |
| Localization key | `DARK_FLAME_FRAGMENT` |
| Status | ✅ Registered · 🚧 OnPlay stub |

---

### 毒素记录 (Toxin Record)

| Property | Value |
| --- | --- |
| Class | `ToxinRecord` |
| Cost | 1 |
| Type | Skill |
| Rarity | Uncommon |
| Target | Self |
| Localization key | `TOXIN_RECORD` |
| Status | ✅ Registered · 🚧 OnPlay stub |

---

### 碎念回响 (Shattered Echo)

| Property | Value |
| --- | --- |
| Class | `ShatteredEcho` |
| Cost | 2 |
| Type | Skill |
| Rarity | Rare |
| Target | Self |
| Localization key | `SHATTERED_ECHO` |
| Status | ✅ Registered · 🚧 OnPlay stub |

---

## Card Pool Statistics

| Metric | Value |
| --- | --- |
| Total Cards | 4 |
| Attack | 1 (迅影斩) |
| Skill | 3 (暗焰残页, 毒素记录, 碎念回响) |
| Common | 2 |
| Uncommon | 1 |
| Rare | 1 |
| Pool | RegentCardPool |

---

## Implementation Status

| Card | Registered | Localized | OnPlay Effect | Discard Trigger |
| --- | --- | --- | --- | --- |
| 迅影斩 | ✅ | ✅ | 🚧 TODO | 🚧 TODO |
| 暗焰残页 | ✅ | ✅ | 🚧 TODO | 🚧 TODO |
| 毒素记录 | ✅ | ✅ | 🚧 TODO | 🚧 TODO |
| 碎念回响 | ✅ | ✅ | 🚧 TODO | 🚧 TODO |

---

## Planned Card Effects (from design/draft.md)

| Card | Intended Play Effect | Intended Discard Trigger |
| --- | --- | --- |
| 迅影斩 | Deal damage to enemy | Deal bonus damage |
| 暗焰残页 | (Skill utility) | +6 damage to all enemies |
| 毒素记录 | (Skill utility) | Apply 8 poison to random enemy |
| 碎念回响 | (Skill utility) | Discard 1 → Draw 2 if trigger |

These are design targets, not yet implemented. See `design/draft.md` for details.

---

## Future Expansions

- Cards 5–8 (design pending playtesting feedback from v0.1.0-alpha)
- Possible relic support for amplifying discard triggers

---

*This registry is manually maintained. Update when card metadata changes.*
