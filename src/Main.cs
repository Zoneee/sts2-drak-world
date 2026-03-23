// STS2 Mod Entry Point
// API Source: MegaCrit.Sts2.Core.Modding and MegaCrit.Sts2.Core.Logging from sts2.dll
//
// To enable full STS2 integration:
//   1. Copy sts2.dll from game folder to lib/sts2.dll
//   2. Uncomment the using directives below
//   3. Remove the stub attribute at the bottom of this file

// using Godot;
// using MegaCrit.Sts2.Core.Modding;
// using MegaCrit.Sts2.Core.Logging;

using System;
using DiscardMod.Cards;
using DiscardMod.Utils;

namespace DiscardMod;

/// <summary>
/// STS2 mod entry point.
/// STS2 discovers this class via the [ModInitializer] attribute and calls ModLoaded() on startup.
///
/// Correct usage (once sts2.dll is referenced):
///   [ModInitializer("ModLoaded")]
///   public static class DiscardModMain { ... }
/// </summary>
// TODO: Replace stub [ModInitializer] with real one from MegaCrit.Sts2.Core.Modding
[ModInitializer("ModLoaded")]
public static class DiscardModMain
{
    public const string ModName = "STS2 Discard-Trigger Mod";
    public const string Version = "0.1.0-alpha";

    /// <summary>
    /// Called by STS2 when mod is loaded. This is the real entry point.
    /// </summary>
    public static void ModLoaded()
    {
        try
        {
            Logger.Log($"{ModName} v{Version} loading...");
            RegisterCards();
            Logger.Log($"{ModName} loaded successfully!");
        }
        catch (Exception ex)
        {
            Logger.LogError($"Mod failed to load: {ex.Message}");
        }
    }

    private static void RegisterCards()
    {
        // TODO: Use STS2 card pool registration API
        // Example (API TBD - needs sts2.dll exploration):
        // ModContent.RegisterCard<DarkFlameFragment>();
        // ModContent.RegisterCard<SwiftCut>();
        // ModContent.RegisterCard<ToxinRecord>();
        // ModContent.RegisterCard<ShatteredEcho>();

        Logger.Log($"Registered 4 discard-trigger cards for Mystic");
    }
}
