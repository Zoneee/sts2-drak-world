using System;
using DiscardMod.Utils;

namespace DiscardMod;

/// <summary>
/// Main mod loader and coordinator.
/// Entry point for BaseLib-StS2 mod framework.
/// 
/// NOTE: This is a placeholder that will be updated once BaseLib dependency is resolved.
/// The mod framework will instantiate this class when loading the mod.
/// </summary>
public class DiscardModMain
{
    public static DiscardModMain? Instance { get; private set; }

    public const string ModName = "STS2 Discard-Trigger Mod";
    public const string ModID = "DiscardMod";
    public const string Version = "0.1.0-alpha";
    public const string Description = "Introduces discard-trigger card mechanics with 8 custom cards for multiplayer gameplay";
    public const string Author = "alphonse-bot";

    public DiscardModMain()
    {
        Instance = this;
        Logger.Log($"Initializing {ModName} v{Version}");
    }

    public void OnGameLoad()
    {
        try
        {
            Logger.Log("Game loaded, initializing mod systems");
            InitializeCardRegistry();
            RegisterCards();
            SetupHooks();
            Logger.Log("Mod systems initialized successfully");
        }
        catch (Exception ex)
        {
            Logger.LogException(ex);
        }
    }

    private void InitializeCardRegistry()
    {
        Logger.Log($"Card registry initialized with {CardRegistry.GetCardCount()} cards");
    }

    private void RegisterCards()
    {
        try
        {
            // Cards will be registered here during Phase 3-4
            // This is a placeholder for the card registration system
            Logger.Log("Card registration system ready (awaiting Phase 3 card implementations)");
        }
        catch (Exception ex)
        {
            Logger.LogError($"Failed to register cards: {ex.Message}");
        }
    }

    private void SetupHooks()
    {
        try
        {
            // Discard hook setup will go here
            // This may use BaseLib events or Harmony patches
            Logger.Log("Discard event hooks configured (if applicable)");
        }
        catch (Exception ex)
        {
            Logger.LogError($"Failed to setup hooks: {ex.Message}");
        }
    }

    public void OnGameStart()
    {
        Logger.Log("Game started, mod is active");
    }

    public void OnGameEnd()
    {
        Logger.Log("Game ended");
    }
}
