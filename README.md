# STS2 Discard-Trigger Multiplayer Mod

**杀戮尖塔2 - "弃触发"卡牌系统 mod**

A Slay the Spire 2 mod that introduces a "discard-trigger" card system where cards activate powerful effects when discarded (not when played). Designed for 4-player multiplayer gameplay over Steam.

## Quick Start

### Prerequisites
- **.NET SDK 9.0+** ([Download](https://dotnet.microsoft.com/download))
- **Godot 4.5.1+** ([Download](https://godotengine.org/download))
- **Slay the Spire 2** (installed via Steam)
- **Git** with SSH keys configured for GitHub

### Development Setup (VS Code)

```bash
# Clone repository
git clone git@github.com:Zoneee/sts2-drak-world.git
cd sts2-drak-world

# Open in VS Code
code .
```

**One-Click Build + Deploy:**

```
Press: Ctrl+Shift+B

Automatically compiles and deploys DLL to your STS2 mods folder!
```

See [docs/QUICK_START.md](docs/QUICK_START.md) for complete workflow.

### Build (Manual)

```bash
# Build Release version
dotnet build src/ --configuration Release

# Output: src/bin/Release/net9.0/STS2_Discard_Mod.dll
```

### Install

1. Locate your STS2 mods folder:
   - **Windows**: `D:\G_games\steam\steamapps\common\Slay the Spire 2\mods\` (customize in `.vscode/tasks.json`)
   - **Linux**: `~/.steam/debian-installation/steamapps/common/Slay the Spire 2/mods/`
   - **macOS**: `~/Library/Application Support/Steam/steamapps/common/Slay the Spire 2/mods/`

2. Copy files to mods folder:
   - `src/bin/Release/net9.0/STS2_Discard_Mod.dll`
   - `modInfo.json` (from project root)

3. Launch STS2 and create a Mystic character to see discard-trigger cards

**⭐ Tip:** Using VS Code? Just press `Ctrl+Shift+B` to automate this entire process!

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

## Development Workflow

### VS Code Auto-Deploy (Recommended)

**One-command everything:**
```
Ctrl+Shift+B  →  Compile + Deploy automatically!
```

**Features:**
- ✅ Compiles your code
- ✅ Copies DLL to game directory
- ✅ Copies modInfo.json
- ✅ Real-time error display
- ✅ Integrated terminal output

See [docs/VSCODE_WORKFLOW.md](docs/VSCODE_WORKFLOW.md) for complete guide.

### Debugging in Visual Studio Code

```
1. Press Ctrl+Shift+B to build and deploy Debug version
2. Launch STS2 and create a Mystic game
3. Press F5 → Attach to Process → Select Godot.exe
4. Set breakpoints in VS Code (click line number)
5. Trigger card effects in-game, code pauses at breakpoints
```

See [docs/GODOT_DEBUG_GUIDE.md](docs/GODOT_DEBUG_GUIDE.md) for detailed debugging strategies.

### Manual Build & Test

```bash
# Build mod
dotnet build src/ --configuration Release

# Deploy manually (Windows)
Copy-Item src/bin/Release/net9.0/STS2_Discard_Mod.dll -Destination "D:/G_games/steam/steamapps/common/Slay the Spire 2/mods/" -Force

# Or use Python script for automation
python deploy.ps1 -Configuration Release
```

### Debug in-Game

- Look for **`[DiscardMod]`** log prefix in console
- Check [docs/DEBUGGING.md](docs/DEBUGGING.md) for common issues
- Enable detailed logging in [src/Utils/Logger.cs](src/Utils/Logger.cs)

## Documentation

| Document | Purpose |
|----------|---------|
| [QUICK_START.md](docs/QUICK_START.md) 🟢 | **Start here!** One-click deploy setup |
| [VSCODE_WORKFLOW.md](docs/VSCODE_WORKFLOW.md) | Complete VS Code automation guide |
| [GODOT_DEBUG_GUIDE.md](docs/GODOT_DEBUG_GUIDE.md) | Debugging strategies for Godot/STS2 |
| [WINDOWS_DEPLOYMENT.md](docs/WINDOWS_DEPLOYMENT.md) | Windows-specific installation guide |
| [DESIGN.md](docs/DESIGN.md) | Card mechanics and balance framework |
| [ARCHITECTURE.md](docs/ARCHITECTURE.md) | Code structure and mod framework |
| [DEV_GUIDE.md](docs/DEV_GUIDE.md) | How to create new cards |
| [DEBUGGING.md](docs/DEBUGGING.md) | Troubleshooting and log analysis |

## Project Status

**v0.1.0-alpha** (In Development)
- [x] Phase 1: Repository setup ✅
- [x] Phase 2: Base framework ✅
- [x] Phase 3: Core 4 cards ✅
- [x] Phase 4: VS Code automation ✅ (NEW!)
- [ ] Phase 5: Remaining 4 cards
- [ ] Phase 6: Multiplayer testing
- [ ] Phase 7: Release packaging

See [docs/CHANGELOG.md](docs/CHANGELOG.md) for detailed version history.

## Community

- **Discord**: [Slay the Spire Discord](https://discord.gg/slaythespire) — #modding channel
- **GitHub Issues**: Report bugs or suggest features

## License

See [LICENSE](LICENSE) for details.

---

**Made with ❤️ by alphonse-bot**
