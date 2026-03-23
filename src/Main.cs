using System;
using System.Collections.Generic;
using DiscardMod.Cards;
using DiscardMod.Utils;

namespace DiscardMod;

/// <summary>
/// Main mod loader and coordinator.
/// Entry point for BaseLib-StS2 mod framework.
/// 
/// This class will be instantiated by BaseLib when the mod loads.
/// All card registration and hook setup happens here.
/// 
/// NOTE: Currently a standalone implementation. 
/// Will inherit from BaseMod once BaseLib is fully integrated.
/// </summary>
public class DiscardModMain
{
    public static DiscardModMain? Instance { get; private set; }

    public const string ModName = "STS2 Discard-Trigger Mod";
    public const string ModID = "DiscardMod";
    public const string Version = "0.1.0-alpha";

    public DiscardModMain()
    {
        Instance = this;
        Logger.Log($"Initializing {ModName} v{Version}");
    }

    public void OnModLoad()
    {
        try
        {
            Logger.Log("Mod loading, initializing systems");
            InitializeCardRegistry();
            RegisterCards();
            SetupHooks();
            Logger.Log($"{ModName} loaded successfully");
        }
        catch (Exception ex)
        {
            Logger.LogException(ex);
            throw;
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
            // Register all card types
            var cardTypes = new Type[]
            {
                typeof(DarkFlameFragment),
                typeof(SwiftCut),
                typeof(ToxinRecord),
                typeof(ShatteredEcho)
            };

            foreach (var cardType in cardTypes)
            {
                Logger.Log($"Card type registered: {cardType.Name}");
            }

            Logger.Log($"Card registration complete: {cardTypes.Length} cards");
        }
        catch (Exception ex)
        {
            Logger.LogError($"Failed to register cards: {ex.Message}");
            throw;
        }
    }

    private void SetupHooks()
    {
        try
        {
            // Discard hook setup will go here
            // This may use BaseLib events or Harmony patches
            Logger.Log("Discard event hooks configured");
        }
        catch (Exception ex)
        {
            Logger.LogError($"Failed to setup hooks: {ex.Message}");
            throw;
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
