# Mod Card Registry

**Complete catalog of all cards in the STS2 Discard-Trigger Mod**

> Last Updated: Phase 6 (TBD)  
> Mod Version: v0.1.0-alpha

---

## Card Database

### Type: Skill (Discard-Trigger)

#### 1. **迅切** (Swift Cut)
- **Cost**: 0
- **Rarity**: Common
- **Effect**: Draw 2 cards, discard 1. If discarded is discard-trigger, gain 1 energy.
- **Upgrade**: Draw 3 (instead of 2)
- **ID**: `DiscardMod_SwiftCut`
- **Status**: ✅ Implemented (Phase 4)

---

#### 2. **暗焰残页** (Dark Flame Fragment)
- **Cost**: 1
- **Rarity**: Common
- **Playability**: Cannot play
- **Effect**: When discarded: +6 damage to all enemies
- **Upgrade**: +9 damage (instead of 6)
- **ID**: `DiscardMod_DarkFlameFragment`
- **Status**: ✅ Implemented (Phase 3 Prototype)

---

#### 3. **毒记** (Toxin Record)
- **Cost**: 1
- **Rarity**: Common
- **Playability**: Cannot play
- **Effect**: When discarded: Apply 8 poison to random enemy
- **Upgrade**: Apply 12 poison (instead of 8)
- **ID**: `DiscardMod_ToxinRecord`
- **Status**: ✅ Implemented (Phase 4)

---

#### 4. **碎念回响** (Shattered Echo)
- **Cost**: 1
- **Rarity**: Common
- **Effect**: Discard 1 card. If discarded is discard-trigger, draw 2 more cards.
- **Upgrade**: Draw 3 more (instead of 2)
- **ID**: `DiscardMod_ShatteredEcho`
- **Status**: ✅ Implemented (Phase 4)

---

### Cards 5–8 (TBD)

**Status**: Placeholder - to be designed during Phase 4 playtesting

- **Card 5**: [Design TBD]
- **Card 6**: [Design TBD]
- **Card 7**: [Design TBD]
- **Card 8**: [Design TBD]

See [DESIGN.md - Tier 4](DESIGN.md#tier-4-advanced-mechanics-cards-5-8--tbd) for candidate ideas.

---

## Card Pool Statistics

| Metric         | Value                         |
| -------------- | ----------------------------- |
| Total Cards    | 4 (8 planned)                 |
| Common         | 4                             |
| Uncommon       | 0                             |
| Rare           | 0                             |
| Average Cost   | 0.75                          |
| No-Play Cards  | 2 (Dark Flame, Toxin)         |
| Playable Cards | 2 (Swift Cut, Shattered Echo) |

---

## Synergy Groups

### **Damage Strategy** (Burst)
- **Core Cards**: Dark Flame Fragment (×3)
- **Enablers**: Swift Cut, Shattered Echo
- **Goal**: Chain discards for burst damage (3×6 = 18 dmg per turn)

### **Poison Strategy** (Scaling)
- **Core Cards**: Toxin Record (×3)
- **Enablers**: Swift Cut, Shattered Echo
- **Goal**: Stack poison for long-term damage (8×5 = 40 poison layers)

### **Draw Strategy** (Combo)
- **Core Cards**: Shattered Echo (×2), Swift Cut (×1)
- **Enablers**: Other discard-trigger cards
- **Goal**: Generate infinite card draw on discard loops

---

## Card Interactions

### **With Swift Cut**
- Discards a card → Can trigger other discard-trigger effects
- If discarded card is discard-trigger, refunds 1 energy

### **With Shattered Echo**
- Discards a card → Chains to other discard-trigger effects
- If discarded is discard-trigger, draws 2 more (enables combo turns)

### **Between Triggers**
- Multiple discard-trigger cards stack multiplicatively
- Example: 3× Dark Flame Fragment discarded = 3 × 6 dmg = 18 total
- No diminishing returns

---

## Balance History

### **v0.1.0-alpha** (Initial Release - TBD)

| Card           | Cost | Base Effect                   | Notes                                             |
| -------------- | ---- | ----------------------------- | ------------------------------------------------- |
| Swift Cut      | 0    | Draw 2, Discard 1, Refund 1 E | Base case; no playtesting adjustments             |
| Dark Flame     | 1    | +6 all enemies                | Initial damage value; may tune up/down            |
| Toxin          | 1    | +8 poison random              | Initial poison value; likely balanced for Acts 3+ |
| Shattered Echo | 1    | Discard 1, Draw 2 if trigger  | Initial draw value; combo-enabling                |

**Known Issues**: None for v0.1.0-alpha (first release)  
**Balance Notes**: Playtesting pending Phase 5

### **v0.2.0** (Post-Playtesting - TBD)

TBD based on actual player feedback from v0.1.0-alpha.

---

## Mod Compatibility

**Known Working With**:
- Slay the Spire 2 v0.99+ 
- BaseLib-StS2 v1.8.0+
- RMP (Expanded lobby mod) — ✅ Confirmed (Phase 5)
- Advisor — ✅ Confirmed (Phase 5)

**Known Conflicts**: None reported yet

---

## Installation & Usage

See [README.md](../README.md) for installation.

To use mod:
1. Install STS2_Discard_Mod.dll into STS2 mods folder
2. Enable in main menu
3. Start a run — cards appear in card pool naturally
4. Draft discard-trigger cards to build synergies

---

## Future Expansions

**Post-v0.2.0 Ideas**:
- [ ] Tier 2 (Uncommon rarity) discard-trigger cards
- [ ] Rare cards with complex interactions
- [ ] New card types (Powers, Curses that synergize)
- [ ] Relics that amplify discard triggers
- [ ] Balance tuning for higher ascension levels

---

## Data Export

*Generated for mod data mining / analytics (if enabled):*

```json
{
  "mod_version": "0.1.0-alpha",
  "total_cards": 4,
  "cards": [
    {
      "id": "DiscardMod_SwiftCut",
      "name": "迅切",
      "cost": 0,
      "rarity": "Common",
      "type": "Skill"
    },
    ...
  ]
}
```

---

## Card Art / Visuals (TBD)

Placeholder images for each card (to be created in Phase 6 or later if desired).

| Card           | Preview       | Artist |
| -------------- | ------------- | ------ |
| Swift Cut      | [Placeholder] | TBD    |
| Dark Flame     | [Placeholder] | TBD    |
| Toxin Record   | [Placeholder] | TBD    |
| Shattered Echo | [Placeholder] | TBD    |

---

*This registry auto-updates on each Phase release. Last sync: v0.1.0-alpha planning phase.*
