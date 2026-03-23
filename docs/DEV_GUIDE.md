# Developer Guide

## First-Time Setup

### Prerequisites

1. **Install .NET SDK 9.0+**
   - [Download](https://dotnet.microsoft.com/download)
   - Verify: `dotnet --version`

2. **Install Godot 4.5.1+**
   - [Download](https://godotengine.org/download)
   - Not required for building mod DLL, but useful for understanding game structure

3. **Install Slay the Spire 2 (via Steam)**
   - Launch once to create base mod folder
   - Locate mods folder (see README.md for path by OS)

4. **Setup Git SSH Keys (for GitHub pull/push)**
   - [GitHub SSH Setup Guide](https://docs.github.com/en/authentication/connecting-to-github-with-ssh)
   - Verify: `ssh -T git@github.com`

### Clone & Build

```bash
# Clone repo
git clone git@github.com:Zoneee/sts2-drak-world.git
cd sts2-drak-world

# Restore NuGet packages + build
dotnet build src/

# Output: src/bin/Release/STS2_Discard_Mod.dll (success!)
```

---

## Project Structure Walkthrough

### **What Goes Where**

| Folder               | Purpose                                      | Example                       |
| -------------------- | -------------------------------------------- | ----------------------------- |
| `src/`               | C# source code + project file                | `Main.cs`, `Cards/*.cs`       |
| `docs/`              | Documentation (design, architecture, guides) | `DESIGN.md`, `DEBUGGING.md`   |
| `design/`            | Original game design notes                   | `draft.md`                    |
| `.github/workflows/` | GitHub Actions CI/CD                         | `build.yml`                   |
| `dist/`              | Compiled release artifacts (auto-generated)  | `.zip` files for distribution |

### **Key Files**

- **`src/STS2_Discard_Mod.csproj`** — NuGet dependencies (BaseLib, Harmony)
- **`src/Main.cs`** — Mod entry point; card registration
- **`src/Cards/`** — Individual card implementations (one file per card)
- **`src/Utils/Logger.cs`** — Logging helper with `[DiscardMod]` prefix
- **`src/Utils/CardRegistry.cs`** — Centralized card ID management

---

## Creating Your First Card

### Template: Copy-Paste to Get Started

**File**: `src/Cards/MyNewCard.cs`

```csharp
using System;
using BaseLib.Cards;
using BaseLib.Modding;
using UnityEngine;

namespace DiscardMod.Cards {
    public class MyNewCard : CustomCard, IDiscardTrigger {
        public static string CardID = "DiscardMod_MyNewCard";
        
        public override void Initialize() {
            // Metadata
            base.SetCardData(new CardData {
                ID = CardID,
                CardName = "My New Card",
                CardText = "When discarded: ...",
                Type = CardType.Skill,
                Rarity = CardRarity.Common,
                Cost = 0,
                Target = CardTarget.Aim,  // or SingleAlly, AllEnemies, None, etc.
                Upgrades = new string[] {
                    "Upgrade Text Here"
                },
            });
        }
        
        public override bool CanPlay(AbstractPlayer player) {
            // Set to false if card should not be playable
            return true;
        }
        
        public bool OnDiscard(AbstractCard card, AbstractPlayer player) {
            // This fires when card is discarded
            Utils.Logger.Log($"MyNewCard discarded!");
            
            // Add your effect logic here
            // Example: Deal damage
            // DamageAction action = new DamageAction(AbstractDungeon.getRandomMonster(), 
            //     new DamageInfo(player, 6, DamageType.NORMAL), CardColor.COLORLESS);
            // AbstractDungeon.actionManager.addToBottom(action);
            
            return true; // Success
        }
    }
}
```

### Steps to Add a Card

1. **Create file** in `src/Cards/YourCardName.cs`
2. **Copy template** above, update:
   - Class name: `YourCardName`
   - CardID: `"DiscardMod_YourCardName"`
   - CardName: Display name in-game
   - OnDiscard() logic
3. **Register in Main.cs**:
   ```csharp
   CardRegistry.Register(typeof(YourCardName));
   ```
4. **Build & test**:
   ```bash
   dotnet build src/
   # Deploy to STS2 mods folder (see README.md)
   ```
5. **Verify in-game**:
   - Launch STS2
   - Check console for `[DiscardMod] Loaded` + card in pool

---

## Building & Testing

### Build Locally

```bash
# Debug build (faster, includes symbols)
dotnet build src/ --configuration Debug

# Release build (optimized, ready to distribute)
dotnet build src/ --configuration Release

# Clean build (remove old artifacts first)
dotnet clean src/ && dotnet build src/ --configuration Release
```

### Deploy to STS2 (Manual)

**Linux/macOS**:
```bash
# Copy DLL to STS2 mods folder
cp src/bin/Release/STS2_Discard_Mod.dll \
  ~/.steam/debian-installation/steamapps/common/Slay\ the\ Spire\ 2/mods/
```

**Windows (PowerShell)**:
```powershell
# Copy DLL to STS2 mods folder
Copy-Item -Path "src\bin\Release\STS2_Discard_Mod.dll" `
  -Destination $env:PROGRAMFILES\Steam\steamapps\common\Slay the Spire 2\mods\
```

### Launch & Test

1. **Launch STS2** from Steam
2. **Open in-game console**: Input.is_action_pressed("show_logs")
3. **Check for mod load message**: `[DiscardMod] Loaded`
4. **Start a run** and look for your card in the card pool
5. **Discard the card** and verify effect fires

---

## Debugging

### Console Logging

All mod logs automatically prefix with `[DiscardMod]` (via Logger.cs):

```csharp
// In your card code
Utils.Logger.Log("Card discarded! Dealing 6 damage.");
Utils.Logger.LogWarning("Something unexpected happened.");
Utils.Logger.LogError("Critical error in OnDiscard()");
```

### In-Game Console Access

**Open console**:
- Press: `Input.is_action_pressed("show_logs")` (depends on STS2 input bindings)
- Alternative: Check STS2 output log file:
  - Windows: `%APPDATA%\STS2\log.txt` or Steam logs folder
  - Linux: `~/.local/share/STS2/log.txt`
  - macOS: `~/Library/Logs/STS2/log.txt`

### Common Issues

See [DEBUGGING.md](DEBUGGING.md) for detailed troubleshooting.

---

## Workflow: Day-to-Day Development

### Typical Workflow

1. **Edit card** in `src/Cards/MyCard.cs`
2. **Build**: `dotnet build src/ --configuration Release`
3. **Copy to STS2 mods folder** (see above)
4. **Launch STS2** and test
5. **Repeat** until satisfied

### Faster Iteration (Optional)

Use a **script to auto-copy** after each build:

**Linux/macOS** (`build_and_deploy.sh`):
```bash
#!/bin/bash
dotnet build src/ --configuration Release
cp src/bin/Release/STS2_Discard_Mod.dll ~/.steam/debian-installation/steamapps/common/"Slay the Spire 2"/mods/
echo "✅ Deployed to STS2"
```

**Windows** (`build_and_deploy.ps1`):
```powershell
dotnet build src --configuration Release
Copy-Item "src\bin\Release\STS2_Discard_Mod.dll" "$env:PROGRAMFILES\Steam\steamapps\common\Slay the Spire 2\mods\"
Write-Host "✅ Deployed to STS2"
```

### Committing Work

```bash
# Stage changes
git add src/Cards/MyNewCard.cs

# Commit with clear message
git commit -m "feat: add MyNewCard discard-trigger card"

# Push to GitHub
git push origin main
```

---

## Useful Commands

| Command               | Purpose                   |
| --------------------- | ------------------------- |
| `dotnet build src/`   | Full build                |
| `dotnet clean src/`   | Remove build artifacts    |
| `dotnet publish src/` | Package for distribution  |
| `git log --oneline`   | View commit history       |
| `git status`          | Check uncommitted changes |
| `git diff src/`       | View code changes         |

---

## Testing Multiplayer (Local Network)

1. **Build mod** on your machine
2. **Deploy** to STS2 mods folder
3. **Launch STS2** as Host (create multiplayer game, 4 players)
4. **Launch STS2** again as Client (via Steam → Invite Friend or Local Network)
5. **Both join same game**
6. **Play and verify**:
   - Do discard effects fire on both clients?
   - Any desyncs or crashes?
   - Console logs appear on both sides?

---

## Resources

- [BaseLib-StS2 API Docs](https://alchyr.github.io/BaseLib-Wiki/) — Card creation reference
- [Harmony Patching Guide](https://harmony.pardeike.net/) — If you need to hook game events
- [ModTemplate-StS2](https://github.com/Alchyr/ModTemplate-StS2) — Reference C# mod skeleton
- [STS2 Console Commands](https://slay-the-spire-wiki.fandom.com/wiki/Console) — In-game debugging

---

## Getting Help

- **Stuck?** Check [DEBUGGING.md](DEBUGGING.md) or [DESIGN.md](DESIGN.md)
- **Questions?** Open GitHub Issue or check [STS2 Discord #modding](https://discord.gg/slaythespire)
- **Found a bug?** Report via GitHub Issues with console logs
