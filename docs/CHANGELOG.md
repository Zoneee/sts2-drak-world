# Changelog

All notable changes to the STS2 Discard-Trigger Mod will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---

## [Unreleased]

### In Progress
- `OnPlay()` implementations for all 4 cards (currently empty stubs)
- Discard trigger detection via Harmony patch

---

## [0.1.0-alpha] — TBD

### Added

#### Cards (4 cards, `RegentCardPool` / 储君角色)

| 中文名 | English | Class | Type | Cost | Rarity | Target |
| --- | --- | --- | --- | --- | --- | --- |
| 迅影斩 | Swift Cut | `SwiftCut` | Attack | 0 | Common | AnyEnemy |
| 暗焰残页 | Dark Flame Fragment | `DarkFlameFragment` | Skill | 1 | Common | AnyEnemy |
| 毒素记录 | Toxin Record | `ToxinRecord` | Skill | 1 | Uncommon | Self |
| 碎念回响 | Shattered Echo | `ShatteredEcho` | Skill | 2 | Rare | Self |

All cards are registered via `ModHelper.AddModelToPool(typeof(RegentCardPool), ...)`.  
All `OnPlay()` bodies are placeholder stubs pending effect implementation.

#### Framework

- `[ModInitializer(nameof(Initialize))]` entry point on static class `DiscardModMain`
- `MegaCrit.Sts2.Core.Logging.Logger` with mod ID `STS2DiscardMod`
- `ModHelper.AddModelToPool` for card pool registration
- `src/localization/eng/cards.json` with 8 localization keys (satisfies STS001)
- `STS2_Discard_Mod.json` manifest at project root

#### Build & CI/CD

- `Microsoft.NET.Sdk`, `net9.0`, `BepInEx.AssemblyPublicizer.MSBuild v0.4.3`
- `Alchyr.Sts2.ModAnalyzers` — STS003 warnings (acceptable), STS001 enforces localization
- `CopyToModsFolderOnBuild` MSBuild target: auto-deploys DLL + JSON to `mods/STS2_Discard_Mod/`
- GitHub Actions (`build.yml`): builds on push to `main`, creates Release on `v*` tags
- Requires `STS2_DLL_B64` repository secret for CI builds

#### Documentation

- `README.md` — Installation, card list, CI instructions
- `docs/DEV_GUIDE.md` — Developer setup, card template, build workflow
- `docs/QUICK_START.md` — Quick install and first run
- `docs/ARCHITECTURE.md` — Code structure, API reference
- `docs/DEBUGGING.md` — BepInEx log location, common issues
- `docs/CHANGELOG.md` — This file

### Known Issues

- All `OnPlay()` methods are empty — cards appear in pool but have no effects yet
- Discard trigger mechanic not implemented (no Harmony patches yet)
- `STS003` warnings on card constructors (acceptable per ModAnalyzers docs)
- In-game appearance unverified (no actual test run completed)

---

## [0.2.0] — TBD

### Planned

- `OnPlay()` implementations for all 4 cards
- Discard trigger via Harmony patch on STS2 discard event
- In-game playtesting and balance tuning
- Additional cards (5–8) based on playtesting feedback

---

## Development History

### Setup Phase (Completed)
- [x] Repository initialized: `git@github.com:Zoneee/sts2-drak-world.git`
- [x] `.gitignore` for C#/.NET/Godot/STS2 artifacts
- [x] Correct project structure matching `erasels/StS2-Quick-Restart`
- [x] `nuget.config`, `src/project.godot`, assembly references

### Framework Phase (Completed)
- [x] `Main.cs` with `[ModInitializer]` + `ModHelper.AddModelToPool`
- [x] 4 × `CardModel` subclasses in `src/Cards/`
- [x] `src/localization/eng/cards.json` with all 8 keys
- [x] `STS2_Discard_Mod.json` manifest
- [x] Build passing: 0 errors, 4 STS003 warnings
- [x] CI/CD fixed and working (`build.yml`)

### Pending
- [ ] Implement `OnPlay()` for each card
- [ ] Research STS2 API for discard events
- [ ] Implement discard trigger via Harmony patch
- [ ] In-game testing (cards visible in pool, effects fire on discard)
- [ ] Balance tuning

---

## Acknowledgments

- **STS2 Mod API**: MegaCrit / `sts2.dll`
- **Reference mod**: [erasels/StS2-Quick-Restart](https://github.com/erasels/StS2-Quick-Restart)
- **ModAnalyzers**: [Alchyr](https://github.com/Alchyr/)
