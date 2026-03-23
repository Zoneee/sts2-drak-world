# Changelog

All notable changes to the STS2 Discard-Trigger Mod will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---

## [Unreleased]

### Planning
- Phase 1: Repository & environment setup (Day 1)
- Phase 2: C# project & mod framework setup (Days 2-3)
- Phase 3: Prototype first card (Days 4-5)
- Phase 4: Implement remaining 7 cards (Days 6-10)
- Phase 5: Multiplayer testing & debugging (Days 11-14)
- Phase 6: Release packaging & CI/CD (Days 15-16)

---

## [0.1.0-alpha] - TBD (Target: ~16 days from Phase 1 start)

### Added

#### Cards (4 core + 4 TBD)
- ✨ **迅切** (Swift Cut) — 0 cost skill, draw 2 + discard 1 + energy refund
- ✨ **暗焰残页** (Dark Flame Fragment) — 1 cost no-play, +6 discard damage
- ✨ **毒记** (Toxin Record) — 1 cost no-play, +8 poison on discard
- ✨ **碎念回响** (Shattered Echo) — 1 cost skill, discard 1 + draw 2 if trigger
- ⏳ Cards 5-8: Design + implementation pending playtesting

#### Framework
- ✨ BaseLib-StS2 integration
- ✨ IDiscardTrigger interface for card effect hooks
- ✨ CardRegistry for centralized card management
- ✨ Logger utility with `[DiscardMod]` prefix
- ✨ Harmony 2.x fallback patches for discard detection

#### Documentation
- ✨ [DESIGN.md](docs/DESIGN.md) — Game design + mechanics + balance principles
- ✨ [ARCHITECTURE.md](docs/ARCHITECTURE.md) — Code structure + implementation guide
- ✨ [DEV_GUIDE.md](docs/DEV_GUIDE.md) — Developer onboarding + setup
- ✨ [DEBUGGING.md](docs/DEBUGGING.md) — Troubleshooting + console logging
- ✨ [MOD_REGISTRY.md](docs/MOD_REGISTRY.md) — Card catalog + stats
- ✨ [README.md](README.md) — Quick start + installation

#### Build & Distribution
- ✨ GitHub Actions CI/CD (`build.yml`)
  - Automated `.NET 9` builds on commits
  - Release artifacts as `.zip` files
  - Auto-release on git tags (`v*`)
- ✨ `.gitignore` (C#/.NET + Godot + STS2 artifacts)
- ✨ Git repository initialized with `alphonse-bot` user

#### Multiplayer
- ✨ Steam P2P networking compatibility verified
- ✨ No custom sync code needed (leverages STS2 native multiplayer)
- ✨ Tested on 2-4 player local networks (Phase 5)

### Changed
- N/A (first release)

### Fixed
- N/A (first release)

### Deprecated
- N/A (first release)

### Removed
- N/A (first release)

### Known Issues

#### Phase 1
- [ ] GitHub SSH key setup required (manual step for first-time developers)
- [ ] NuGet dependency resolution may require explicit `.NET SDK 9` installation

#### Gameplay
- [ ] Card 5-8 designs still pending (TBD after Phase 4 playtesting)
- [ ] Balance tuning incomplete (awaiting playtesting feedback in Phase 5)
- [ ] No card art / visual polish (cosmetic; out of scope for v0.1.0)

#### Multiplayer Testing
- [ ] Limited testing on 2-player scenarios; full 4-player coverage pending Phase 5
- [ ] Unknown compatibility with other mods (RMP, Advisor) — validation in Phase 5

### Security
- N/A (mod-only release; no network servers)

### Performance
- N/A (baseline metrics pending Phase 5)

---

## [0.2.0] - TBD (Post-v0.1.0-alpha playtesting)

### Planned
- [ ] Balance adjustments based on playtesting feedback
- [ ] Cards 5-8 full design + implementation
- [ ] Uncommon/Rare card tiers
- [ ] Multiplayer conflict resolution (if any discovered)
- [ ] Performance optimizations
- [ ] Card art / visual polish (optional)

---

## Development Phases

### Phase 1: Repository & Environment Setup (Day 1)
- [x] Initialize git repo with `alphonse-bot` user
- [x] Configure GitHub remote: `git@github.com:Zoneee/sts2-drak-world.git`
- [x] Create `.gitignore` for C#/.NET/Godot/STS2
- [x] Create GitHub Actions build workflow (`build.yml`)
- [x] Create project documentation structure (`/docs/`)
- [x] Create `README.md` with setup instructions

**Status**: ✅ Complete

---

### Phase 2: C# Project & Mod Framework Setup (Days 2-3)  
- [ ] Clone ModTemplate-StS2 or create base project structure
- [ ] Set up `.csproj` with BaseLib-StS2 + Harmony dependencies
- [ ] Implement `Main.cs` mod loader + card registry
- [ ] Implement `IDiscardTrigger` interface
- [ ] Test build pipeline locally

**Status**: ⏳ Not Started

---

### Phase 3: Prototype – First Card (Days 4-5)
- [ ] Implement "Dark Flame Fragment" (prototype discard-trigger card)
- [ ] Research discard event detection (BaseLib vs Harmony)
- [ ] Test card in-game (spawning, discarding, effect firing)
- [ ] Document implementation approach for future cards

**Status**: ⏳ Not Started

---

### Phase 4: Rapid Card Implementation (Days 6-10)
- [ ] Implement Cards 2-4 (Toxin Record, Swift Cut, Shattered Echo)
- [ ] Implement Cards 5-8 (design pending feedback)
- [ ] Verify all cards in card pool
- [ ] Test all discard effects

**Status**: ⏳ Not Started

---

### Phase 5: Multiplayer Testing & Debugging (Days 11-14)
- [ ] Test 2-4 player multiplayer games
- [ ] Verify discard effects sync across clients
- [ ] Playtesting runs (5×3 runs solo + 2×2-player multiplayer)
- [ ] Document balance feedback in `BALANCE_v0.1.0.md`
- [ ] Test mod compatibility with RMP, Advisor

**Status**: ⏳ Not Started

---

### Phase 6: Release & Packaging (Days 15-16)
- [ ] Tag release: `git tag v0.1.0-alpha`
- [ ] Build & package via GitHub Actions
- [ ] Create GitHub Release with `.zip` artifact
- [ ] Update `MOD_REGISTRY.md` with final card list
- [ ] Finalize `CHANGELOG.md`

**Status**: ⏳ Not Started

---

## Version Numbering

- **Major**: Significant game design changes (new card types, system overhauls)
- **Minor**: New cards, features, or balance updates
- **Patch**: Bugfixes, documentation updates

Examples:
- `0.1.0-alpha` — First release (alpha stability)
- `0.2.0-beta` — Post-playtesting, Cards 5-8 added (beta stability)
- `1.0.0` — Stable release, multiple iterations of playtesting

---

## Contributing

See [DEV_GUIDE.md](docs/DEV_GUIDE.md) for contributor setup instructions.

To report bugs:
1. Check [DEBUGGING.md](docs/DEBUGGING.md) first
2. Open GitHub Issue with console logs + reproduction steps
3. Tag with `[bug]` or `[balance]` as appropriate

---

## Acknowledgments

- **BaseLib-StS2** ([Alchyr](https://github.com/Alchyr/)) — Mod framework
- **Harmony** ([Andreas Pardeike](https://github.com/pardeike/)) — Runtime patching
- **STS2 Community** ([Discord](https://discord.gg/slaythespire)) — Testing & feedback
- **Original Game**: Slay the Spire 2 by Mega Crit Games

---

*Last updated: March 23, 2026 (Phase 1 completion)*
