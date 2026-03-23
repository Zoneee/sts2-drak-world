# Debugging Guide

## In-Game Console Access

### Opening the Console

**Method 1: Input Binding**
```
Press: [Show Logs Keybind]  (default varies by platform)
```

**Method 2: Log File**
- **Windows**: `%APPDATA%\STS2\log.txt` or Steam log directory
- **Linux**: `~/.local/share/STS2/log.txt`
- **macOS**: `~/Library/Logs/STS2/log.txt`

### Console Output

All mod logs are prefixed with `[DiscardMod]` for easy filtering:

```
[DiscardMod] Loaded successfully
[DiscardMod] DarkFlameFragment discarded, dealing 6 damage
[DiscardMod] Shattered Echo: discard-trigger detected, drawing 2 cards
```

---

## Common Issues & Solutions

### Issue 1: Mod Not Loading

**Symptom**: Launch STS2, no `[DiscardMod] Loaded` message in console

**Steps to Debug**:

1. **Check mod file location**:
   - Windows: `C:\Program Files\Steam\steamapps\common\Slay the Spire 2\mods\STS2_Discard_Mod.dll`
   - Linux: `~/.steam/debian-installation/steamapps/common/Slay the Spire 2/mods/STS2_Discard_Mod.dll`
   - macOS: Check Steam library location + `/mods/` folder

2. **Verify file permissions**:
   ```bash
   # Linux/macOS
   ls -la ~/.steam/debian-installation/steamapps/common/Slay\ the\ Spire\ 2/mods/
   # Should show STS2_Discard_Mod.dll with read permissions
   ```

3. **Check for .NET runtime errors**:
   - Open STS2 console, look for:
     - `FileLoadException` — DLL architecture mismatch (32-bit vs 64-bit)
     - `TypeLoadException` — Missing dependency (BaseLib not installed)
     - `DllNotFoundException` — Missing native library

4. **Solutions**:
   - Verify you built Release config: `dotnet build src/ --configuration Release`
   - Ensure BaseLib NuGet package is installed: Check `src/STS2_Discard_Mod.csproj`
   - Try rebuilding from scratch:
     ```bash
     dotnet clean src/
     dotnet build src/ --configuration Release
     cp src/bin/Release/*.dll ~/.steam/.../mods/
     ```

---

### Issue 2: Discard Effects Not Firing

**Symptom**: Card discards but no effect appears (no damage, no poison, etc.)

**Steps to Debug**:

1. **Verify card is actually discarded**:
   - Play another card that discards (e.g., Swift Cut)
   - Check console for: `[DiscardMod] [Card Name] discarded`
   - If no log message: card is not being detected as discarded

2. **Check if card plays normally instead**:
   - Does card appear in hand normally?
   - Can you click to play it?
   - If yes: Card's `CanPlay()` method returns `true` — fix this
   ```csharp
   public override bool CanPlay(AbstractPlayer player) {
       return false;  // Ensure no-play cards return false
   }
   ```

3. **Verify discard hook is registered**:
   - Check Main.cs `RegisterCards()` method
   - Does it call `Logger.Log()` for each card registration?
   - Should see in console: `[DiscardMod] Registered: DarkFlameFragment`

4. **Debug IDiscardTrigger interface**:
   - Add logging inside `OnDiscard()`:
   ```csharp
   public bool OnDiscard(AbstractCard card, AbstractPlayer player) {
       Logger.Log($"OnDiscard fired for {card.name}");
       // ... rest of effect
       return true;
   }
   ```
   - If no log: method not being called
   - Check class implements `IDiscardTrigger` correctly

5. **Solutions** (in order):
   - Rebuild and redeploy DLL
   - Verify card registered in CardRegistry
   - Check if Harmony patch (DiscardHooks.cs) is enabled correctly
   - Add debug breakpoints (if using debugger)

---

### Issue 3: Multiplayer Desync

**Symptom**: Card effects fire on your client but not others (or crash on one client)

**Steps to Debug**:

1. **Check mod versions match**:
   ```bash
   # On both client & host
   ls -la ~/.steam/.../mods/STS2_Discard_Mod.dll
   # Compare file timestamps — should be identical
   ```

2. **Look for mod mismatch in console**:
   - Host console: `[Connection] Client X has different mods`
   - Solution: Both players download identical DLL version

3. **Check for multiplayer-specific conflicts**:
   - Check if card effects use per-player state correctly
   - Verify damage/status actions replicate via STS2's action system
   - Common bug: Card references `AbstractDungeon.player` instead of passed `player` param

4. **Enable verbose logging** (if available):
   - Look for mod settings that increase log verbosity
   - Check if network replication is logged

5. **Solutions**:
   - Rebuild DLL with identical configuration
   - Use `git` to sync exact mod version across machines
   - Report detailed logs to GitHub Issues

---

### Issue 4: Crash on Game Start

**Symptom**: STS2 crashes immediately after launching with mod enabled

**Steps to Debug**:

1. **Check crash message**:
   - STS2 often shows error dialog before closing
   - Write down exact error (copy to text file)

2. **Look for NuGet dependency issues**:
   - Error: `TypeLoadException: BaseLib type not found`
   - Solution: Rebuild NuGet packages:
     ```bash
     cd src/
     dotnet restore
     dotnet build
     ```

3. **Check for circular dependencies**:
   - If mod references other mods, ensure they're compatible
   - Verify STS2_Discard_Mod is not referenced by another DLL

4. **Try minimal mod** (debugging):
   - Comment out all card registrations in `Main.cs`
   - Build & deploy — does STS2 launch?
   - If yes: Problem is in a specific card. Uncomment cards one-by-one to isolate.

5. **Solutions**:
   - Post full crash stack trace to GitHub Issues
   - Check STS2 output_log.txt (usually in game install folder)

---

### Issue 5: Card Appears But Has No Effect When Played

**Symptom**: Card plays from hand normally but should trigger discard effect

**Cause**: Card's `CanPlay()` returns `true` — meant to be no-play only

**Solution**: Update card implementation:

```csharp
public override bool CanPlay(AbstractPlayer player) {
    // Change to false if card should not be playable
    return false;  // No play, discard-only
}

// OR: If card IS playable with weak effect, delegate to OnDiscard:

public override void OnUse(AbstractPlayer player) {
    // Weak play effect (optional)
    Logger.Log("Card played but weakly");
    // Effect is better when discarded
}
```

---

## Logging Best Practices

### Adding Debug Logs

```csharp
// Good: Informative, prefixed with [DiscardMod]
Logger.Log($"DarkFlameFragment discarded. Damage: {dmg}, Enemies: {enemyCount}");

// Bad: Vague, no context
Debug.Log("Effect fired");

// Bad: Spam (don't log every frame)
// ❌ void Update() { Logger.Log("Update called"); }
```

### Filtering Logs in Console

**In-game console** (if searchable):
- Search for `[DiscardMod]` to see only mod logs
- Search for `ERROR` to find problems

**Terminal** (reading log file):
```bash
# Show only DiscardMod logs
grep "\[DiscardMod\]" ~/.steam/.../log.txt

# Show only errors
grep -i "error\|exception" ~/.steam/.../log.txt

# Follow log in real-time (Linux)
tail -f ~/.steam/.../log.txt | grep "\[DiscardMod\]"
```

---

## Advanced: Using a Debugger

If crashes are hard to diagnose, use **Visual Studio Debugger**:

1. **Install Visual Studio 2024** (Community edition free)
2. **Attach debugger to STS2 process**:
   - Open STS2 project in Visual Studio
   - Run → Attach to Process → Select `STS2.exe`
   - Set breakpoints in `src/Cards/*.cs`
3. **Trigger card effect**:
   - Discard the card in-game
   - Debugger pauses at breakpoint
   - Step through code, inspect variables

**Reference**: [Visual Studio Debugging Guide](https://learn.microsoft.com/en-us/visualstudio/debugger/)

---

## Reporting Issues

**When stuck**, post to GitHub Issues with:

1. **Exact error message** (copy from console)
2. **Steps to reproduce**
3. **Console logs** (export via `grep "[DiscardMod]"` or full log file)
4. **Your OS & STS2 version**
5. **Mod version** you're testing with

Example issue:

```
Title: "DarkFlameFragment not triggering on discard"

Description:
- OS: Linux
- STS2 Version: 0.99.2
- Mod Version: v0.1.0-alpha

Steps:
1. Play Swift Cut (discards 1 card)
2. Discard DarkFlameFragment
3. No damage appears

Expected: 6 damage to all enemies
Actual: No effect

Console logs:
[DiscardMod] Loaded successfully
[DiscardMod] Registered: DarkFlameFragment
[DiscardMod] Swift Cut played (draw 2, discard 1)
(no discard log for DarkFlameFragment)
```

---

## Quick Reference: Commands & Paths

| Action           | Command                                                              |
| ---------------- | -------------------------------------------------------------------- |
| Rebuild mod      | `dotnet clean src/ && dotnet build src/ --configuration Release`     |
| View recent logs | `tail -100 ~/.steam/.../log.txt`                                     |
| Find mod file    | `find ~/.steam -name "*DiscardMod*" -o -name "STS2_Discard_Mod.dll"` |
| Copy to mods     | See DEV_GUIDE.md → "Deploy to STS2 (Manual)"                         |
| Check mod loaded | `grep -i "\[DiscardMod\]" ~/.steam/.../log.txt`                      |

---

## Got Stuck?

1. **Check this guide** (you're reading it!)
2. **Review [DEV_GUIDE.md](DEV_GUIDE.md)** — Setup & workflow
3. **Ask on [STS2 Discord](https://discord.gg/slaythespire)** — #modding channel
4. **Open GitHub Issue** with logs (see "Reporting Issues" section)
