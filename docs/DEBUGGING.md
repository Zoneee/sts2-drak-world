# Debugging Guide

## Log File Location

STS2 uses BepInEx. Mod messages appear in the BepInEx log:

```
{game_install_dir}/BepInEx/LogOutput.log
```

Typical paths:
- **Linux**: `~/.local/share/Steam/steamapps/common/Slay the Spire 2/BepInEx/LogOutput.log`
- **Windows**: `D:\G_games\steam\steamapps\common\Slay the Spire 2\BepInEx\LogOutput.log`
- **macOS**: `~/Library/Application Support/Steam/steamapps/common/Slay the Spire 2/SlayTheSpire2.app/Contents/MacOS/BepInEx/LogOutput.log`

### Filtering Mod Logs

All mod messages are prefixed with `[STS2DiscardMod]`:

```bash
# Follow log in real-time (Linux/macOS)
tail -f BepInEx/LogOutput.log | grep '\[STS2DiscardMod\]'

# Windows PowerShell
Get-Content BepInEx\LogOutput.log -Wait | Select-String '\[STS2DiscardMod\]'
```

Expected on successful load:
```
[Info   : STS2DiscardMod] loading...
[Info   : STS2DiscardMod] Registered 4 discard-trigger cards to RegentCardPool
[Info   : STS2DiscardMod] loaded!
```

---

## Logging in Code

The logger is exposed as a static property on `DiscardModMain`:

```csharp
// In any card or helper class:
DiscardModMain.Logger.Info("message");
DiscardModMain.Logger.Debug("verbose details");
DiscardModMain.Logger.Warning("something unexpected");
DiscardModMain.Logger.Error("something failed");
```

`Logger` is `MegaCrit.Sts2.Core.Logging.Logger` with id `"STS2DiscardMod"`.

---

## Common Issues

### Mod Not Loading

**Symptom**: No `[STS2DiscardMod]` lines in `BepInEx/LogOutput.log` after game starts.

1. Verify the mod folder exists and has both files:
   ```
   {game}/mods/STS2_Discard_Mod/
   ├── STS2_Discard_Mod.dll
   └── STS2_Discard_Mod.json
   ```
2. Check that `STS2_Discard_Mod.json` matches the expected format:
   ```json
   {
     "id": "STS2DiscardMod",
     "has_dll": true,
     "has_pck": false,
     "affects_gameplay": true
   }
   ```
3. Rebuild from scratch:
   ```bash
   dotnet clean src/ && dotnet build src/ --configuration Release
   ```
4. Check BepInEx log for `TypeLoadException` or `FileNotFoundException` — these indicate a missing dependency or wrong .NET target.

### Cards Not Appearing in Pool

**Symptom**: Mod loads but 储君 (Regent) card pool doesn't include the new cards.

1. Check the log for: `Registered 4 discard-trigger cards to RegentCardPool`
2. Verify each `AddModelToPool` call is present in `Main.cs`:
   ```csharp
   ModHelper.AddModelToPool(typeof(RegentCardPool), typeof(DarkFlameFragment));
   // (× 4 cards)
   ```
3. Check the localization JSON — a missing key can silently prevent a card from appearing:
   ```
   src/localization/eng/cards.json
   ```
   Required keys: `DARK_FLAME_FRAGMENT.title`, `DARK_FLAME_FRAGMENT.description`, and equivalent for all 4 cards.

### Card Plays But Does Nothing

**Symptom**: Card can be played from hand, but no effect occurs.

This is expected for now — all `OnPlay()` bodies are stubs:
```csharp
public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
{
    DiscardModMain.Logger.Info("SwiftCut played");   // just logs
    await Task.CompletedTask;                         // no effect
}
```

To implement an effect, replace the stub with real game actions. See `docs/DEV_GUIDE.md`.

### Build Error STS001

**Symptom**: Build fails with `STS001: Missing localization key for card 'CARDNAME.title'`.

Add the key to `src/localization/eng/cards.json`:
```json
{
    "CARD_CLASS_NAME": {
        "title": "Card Display Name",
        "description": "Card description text."
    }
}
```

The key must be the **uppercase** class name with underscores.

### Crash on Launch

1. Open `BepInEx/LogOutput.log` and look for `Exception` or `Error` near the top.
2. Common causes:
   - `TypeLoadException` — wrong `net9.0` target or missing reference DLL
   - `MissingMethodException` — game API changed; rebuild with updated `sts2.dll`
3. Try disabling the mod (rename or remove the subfolder) to confirm STS2 itself still launches.

---

## Attaching a Debugger (Optional)

For step-through debugging:

1. Build with Debug configuration:
   ```bash
   dotnet build src/ --configuration Debug
   ```
   This produces `STS2_Discard_Mod.pdb` alongside the DLL (auto-deployed by `CopyToModsFolderOnBuild`).

2. In VS Code, install the **C# Dev Kit** extension.

3. Launch the game, then use **Run and Debug → Attach to Process** and select the STS2/Godot process.

4. Set breakpoints in `src/Cards/*.cs` or `src/Main.cs` — they will be hit when the game executes that code.

---

## Quick Reference

| Action | Command |
| --- | --- |
| Rebuild mod | `dotnet clean src/ && dotnet build src/ --configuration Release` |
| Watch log (Linux) | `tail -f {game}/BepInEx/LogOutput.log \| grep STS2DiscardMod` |
| Find deployed DLL | `find ~/.local/share/Steam -name "STS2_Discard_Mod.dll"` |
| Check mod folder | `ls -la {game}/mods/STS2_Discard_Mod/` |

---

## Reporting Issues

When filing a GitHub Issue, include:

1. OS + STS2 version
2. Exact error text (copy from `BepInEx/LogOutput.log`)
3. The relevant `[STS2DiscardMod]` log section
4. Steps to reproduce
