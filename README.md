# STS2 Discard-Trigger Multiplayer Mod

**杀戮尖塔2 - "弃触发"卡牌系统 mod**

A Slay the Spire 2 mod that introduces a "discard-trigger" card system where cards activate powerful effects when discarded (not when played). Designed for 4-player multiplayer gameplay over Steam.

## Quick Start

### Prerequisites
- **.NET SDK 9.0+** ([Download](https://dotnet.microsoft.com/download))
- **Godot 4.5.1+** ([Download](https://godotengine.org/download))
- **Slay the Spire 2** (installed via Steam)
- **Git** with SSH keys configured for GitHub

### Build

```bash
# Clone repository
git clone git@github.com:Zoneee/sts2-drak-world.git
cd sts2-drak-world

# Build the mod
dotnet build

# Output: bin/Release/STS2_Discard_Mod.dll
```

### Install

1. Locate your STS2 mods folder:
   - **Windows**: `C:\Program Files\SteamLibrary\steamapps\common\Slay the Spire 2\mods\`
   - **Linux**: `~/.steam/debian-installation/steamapps/common/Slay the Spire 2/mods/`
   - **macOS**: `~/Library/Application Support/Steam/steamapps/common/Slay the Spire 2/mods/`

2. Copy `bin/Release/STS2_Discard_Mod.dll` to the mods folder

3. Launch STS2 and enable the mod in the main menu

### Multiplayer Testing

1. **Host**: Create a multiplayer game (4-player, local or Steam friends)
2. **All Players**: Ensure identical mod version is installed
3. **Gameplay**: Discard-trigger cards appear in the card pool
4. **Debug**: Open in-game console (`showlogs` command) to check for events

## Core Mechanic: "Discard-Trigger" 

These cards activate **special effects when discarded**, not when played:

- **迅切** (Swift Cut) — 0 cost: Draw 2, discard 1 → Refund 1 energy if discarded card has discard effect
- **暗焰残页** (Dark Flame Fragment) — 1 cost: Cannot play. When discarded: +6 damage to all enemies
- **毒记** (Toxin Record) — 1 cost: Cannot play. When discarded: Apply 8 poison to random enemy
- **碎念回响** (Shattered Echo) — 1 cost: Discard 1 card. If discarded card has discard effect: Draw 2 more cards
- *(Cards 5-8 in development)*

## Development

See [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md) for code structure and implementation details.

### Build & Test Locally

```bash
# Build mod
dotnet build

# Run tests (if implemented)
dotnet test

# Deploy to local STS2 mods folder
cp -r bin/Release/* ~/SteamLibrary/.../Slay\ the\ Spire\ 2/mods/
```

### Debug in-Game

- Open STS2 console: **Input.is_action_pressed("show_logs")**
- Look for `[DiscardMod]` log prefix
- Check [docs/DEBUGGING.md](docs/DEBUGGING.md) for common issues

## Project Status

**v0.1.0-alpha** (In Development)
- [ ] Phase 1: Repository setup (Day 1)
- [ ] Phase 2: Base framework (Days 2-3)
- [ ] Phase 3: First card prototype (Days 4-5)
- [ ] Phase 4: All 8 cards (Days 6-10)
- [ ] Phase 5: Multiplayer testing (Days 11-14)
- [ ] Phase 6: Release packaging (Days 15-16)

See [docs/CHANGELOG.md](docs/CHANGELOG.md) for detailed version history.

## Resources

- [Game Design Doc](docs/DESIGN.md) — Card mechanics and system design
- [Architecture Guide](docs/ARCHITECTURE.md) — Code structure and discard hook implementation
- [Developer Guide](docs/DEV_GUIDE.md) — Setup instructions for contributors
- [Debugging Guide](docs/DEBUGGING.md) — Troubleshooting and logging
- [BaseLib-StS2 Docs](https://alchyr.github.io/BaseLib-Wiki/) — Mod framework reference

## Community

- **Discord**: [Slay the Spire Discord](https://discord.gg/slaythespire) — #modding channel
- **GitHub Issues**: Report bugs or suggest features

## License

See [LICENSE](LICENSE) for details.

---

**Made with ❤️ by alphonse-bot**
