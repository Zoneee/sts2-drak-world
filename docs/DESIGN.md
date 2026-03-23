# Game Design: Discard-Trigger Card System

## Overview

This mod introduces a novel card mechanic to Slay the Spire 2: **"Discard-Trigger"** cards that activate powerful effects *when discarded*, rather than when played from hand.

## Core Philosophy

**Traditional STS2 Cards**: Play → Get Effect → Discard  
**Discard-Trigger Cards**: Draw/Keep → Discard via Other Effect → Trigger Special Effect

This inverts the normal card economy, allowing players to build synergies around:
1. **Passive card draw** (finding the right cards)
2. **Active discard effects** (triggering them via other cards or effects)
3. **Recycling/recovery** (rebuilding the deck after discard loops)

## Mechanical Framework

### Card Properties

All discard-trigger cards share these properties:

| Property              | Value                          | Notes                                            |
| --------------------- | ------------------------------ | ------------------------------------------------ |
| **Playability**       | Cannot Play or Play Effect = 0 | Player must use other effects to trigger         |
| **Finder-Ability**    | Findable in card pool          | Normal relic/pool access                         |
| **Discard Detection** | Via BaseLib hooks              | Triggers when card leaves hand to discard pile   |
| **Energy Cost**       | 0–1 (variable)                 | Cost is "payment" for draft, not for usage       |
| **Rarity**            | Common–Uncommon                | Prevents powercreep; encourages synergy building |

### Card Types

1. **Direct Damage on Discard** (e.g., "Dark Flame Fragment")
   - Pure offensive; simplest case
   - Enables: Combo with multi-discard effects for burst damage
   - Example: Discard 3 "Dark Flame" → +18 total damage

2. **Debuff Application** (e.g., "Toxin Record")
   - Status-based; enables multi-turn value
   - Enables: Boss scaling, poison-based decks
   - Example: Discard 5 "Toxin" → +40 poison layers

3. **Card Draw / Energy Refund** (e.g., "Swift Cut", "Shattered Echo")
   - Enabler cards; create deck loops
   - Enables: Chaining discard effects, sustained card advantage
   - Example: Discard "Shattered Echo" → Draw 2 more → Chain more discards

4. **Utility** (Future cards 5–8)
   - Block generation, artifact gain, power scaling, etc.
   - TBD based on balance testing

---

## 8 Card Roster

### Tier 1: Foundation (Cards for every deck)

#### 1. **迅切** (Swift Cut)
**Type**: Skill | **Cost**: 0 | **Rarity**: Common | **Upgrade**: ✓

**Base Effect**:
- Draw 2 cards
- Discard 1 card
- If the discarded card is a discard-trigger card, gain 1 Energy

**Upgrade Effect**:
- Draw 3 cards
- Discard 1 card
- If the discarded card is a discard-trigger card, gain 1 Energy

**Design Intent**: 
- Entry point to the system; pure 0-cost card draw
- Refund mechanic incentivizes holding discard-trigger cards
- Synergizes with: All discard-trigger cards

---

### Tier 2: Damage Enablers

#### 2. **暗焰残页** (Dark Flame Fragment)
**Type**: Skill | **Cost**: 1 | **Rarity**: Common | **Upgrade**: ✓
**Playability**: Cannot Play

**Base Effect**:
- When discarded: Deal 6 damage to all enemies

**Upgrade Effect**:
- When discarded: Deal 9 damage to all enemies

**Design Intent**:
- Clearest "discard-trigger" mechanic; no ambiguity
- Stacks multiplicatively with multi-discard (3 discards = 18 dmg)
- Enables: Burst damage combos, early act scaling

---

#### 3. **毒记** (Toxin Record)
**Type**: Skill | **Cost**: 1 | **Rarity**: Common | **Upgrade**: ✓
**Playability**: Cannot Play

**Base Effect**:
- When discarded: Apply 8 Poison to a random enemy

**Upgrade Effect**:
- When discarded: Apply 12 Poison to a random enemy

**Design Intent**:
- Poison scaling provides long-term value
- Less burst-dependent than Dark Flame; better for extended runs
- Enables: Boss-scaling decks, poison-focused synergy

---

### Tier 3: Combo Enablers

#### 4. **碎念回响** (Shattered Echo)
**Type**: Skill | **Cost**: 1 | **Rarity**: Common | **Upgrade**: ✓

**Base Effect**:
- Discard 1 card from hand
- If that card is a discard-trigger card, draw 2 cards

**Upgrade Effect**:
- Discard 1 card from hand
- If that card is a discard-trigger card, draw 3 cards

**Design Intent**:
- Enabler for chaining discard effects
- Turbo-charges deck cycles
- Enables: Long combo turns, consistent card velocity

---

### Tier 4: Advanced Mechanics (Cards 5–8) — TBD

**Candidates** (Placeholder):

- **Card 5**: Artifact/Block generator on discard
- **Card 6**: Power scaling (e.g., +1 strength per discard-trigger)
- **Card 7**: Multi-discard amplifier (e.g., "Discard 2, trigger both")
- **Card 8**: Recovery/Recycle (e.g., "Discard pile → Hand")

*To be designed after Phase 4 balance testing.*

---

## Gameplay Loop

### Early Run (Acts 1–2)

1. **Draft Phase**: Pick up 1–3 discard-trigger cards + "Swift Cut" / "Shattered Echo"
2. **Play Phase**: Use support cards to discard triggers
3. **Output**: Consistent damage + status debuffs
4. **Scaling**: Linear (more copies = more triggers)

### Late Run (Acts 3+)

1. **Synergy Phase**: Build around specific trigger types (e.g., all poison, all damage)
2. **Enabler Phase**: Stack card-draw + discard effects to create mega-turns
3. **Output**: 30–100 damage per turn + heavy debuffs OR infinite loops (if available)
4. **Scaling**: Exponential (exponential if we add recycling mechanics)

---

## Balance Principles

### Design Constraints

1. **No 0-cost triggers** (except Swift Cut): Prevents infinite loops in Act 1
2. **Random targeting where applicable**: Allows RNG to nerf overpowered turns
3. **Requires setup**: Player must play other cards to activate triggers
4. **Tuned for 4-player**: All damage/status values balanced assuming 4 enemies present

### Balance Levers (Tuning Knobs)

- **Damage**: Adjust per-discard value (+6 → +8 → +10)
- **Status**: Adjust layer counts (+8 → +12 poison)
- **Energy Refund**: Remove energy refund if overpowered
- **Draw**: Reduce card draw per trigger if too much velocity

---

## Multiplayer Considerations

### Sync Requirements

- All players must run **identical mod version** (game enforces via mod mismatch detection)
- Discard effects fire **independently per player** (not shared across network)
- Card pool is **identical for all players** (same seed-based generation)

### Balance for 4-Player

- Card costs tuned assuming 4 damage targets (not 1)
- Poison values account for 4-enemy multi-hit scenarios
- Enabling cards (Swift Cut, Shattered Echo) prevent slowdown from interaction overhead

---

## Design Document History

- **v0.1.0-alpha (March 2026)**: Initial 8-card design, ready for prototyping
- **v0.2.0 (TBD)**: Post-playtesting balance adjustments + Cards 5–8 finalization
