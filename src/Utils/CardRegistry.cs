using System;
using System.Collections.Generic;
using System.Linq;

namespace DiscardMod.Utils;

/// <summary>
/// Centralized registry for all discard-trigger cards.
/// Single source of truth for card IDs, lookups, and bulk operations.
/// </summary>
public static class CardRegistry
{
    // Card ID constants
    public const string SWIFT_CUT_ID = "DiscardMod_SwiftCut";
    public const string DARK_FLAME_ID = "DiscardMod_DarkFlameFragment";
    public const string TOXIN_RECORD_ID = "DiscardMod_ToxinRecord";
    public const string SHATTERED_ECHO_ID = "DiscardMod_ShatteredEcho";

    private static readonly List<string> RegisteredCardIds = new();

    static CardRegistry()
    {
        // Initialize with known card IDs
        RegisteredCardIds.AddRange(new[]
        {
            SWIFT_CUT_ID,
            DARK_FLAME_ID,
            TOXIN_RECORD_ID,
            SHATTERED_ECHO_ID
        });
    }

    /// <summary>
    /// Register a new card ID (for future cards 5-8).
    /// </summary>
    public static void RegisterCardId(string cardId)
    {
        if (!RegisteredCardIds.Contains(cardId))
        {
            RegisteredCardIds.Add(cardId);
            Logger.Log($"Registered card: {cardId}");
        }
    }

    /// <summary>
    /// Check if a card ID belongs to this mod.
    /// </summary>
    public static bool IsDiscardTriggerCard(string cardId)
    {
        return cardId?.StartsWith("DiscardMod_") ?? false;
    }

    /// <summary>
    /// Check if a card ID is registered in this mod.
    /// </summary>
    public static bool IsRegistered(string cardId)
    {
        return RegisteredCardIds.Contains(cardId);
    }

    /// <summary>
    /// Get all registered card IDs.
    /// </summary>
    public static IEnumerable<string> GetAllCardIds()
    {
        return RegisteredCardIds.AsReadOnly();
    }

    /// <summary>
    /// Get total count of registered cards.
    /// </summary>
    public static int GetCardCount()
    {
        return RegisteredCardIds.Count;
    }

    /// <summary>
    /// Clear registry (for testing purposes).
    /// </summary>
    public static void Clear()
    {
        RegisteredCardIds.Clear();
        Logger.Log("Card registry cleared");
    }
}
