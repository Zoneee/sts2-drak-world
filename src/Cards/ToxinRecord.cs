using System;
using DiscardMod.Utils;

namespace DiscardMod.Cards;

/// <summary>
/// 毒记 (Toxin Record)
/// 
/// A discard-trigger card that applies poison for scaling damage.
/// This card cannot be played from hand. When discarded, it applies poison to a random enemy.
/// 
/// Design Purpose:
/// - Status-based output for long-term scaling
/// - Less burst-dependent than Dark Flame; better for extended runs
/// - Enables poison-focused synergy decks
/// - Scales well against bosses with multi-turn fights
/// </summary>
public class ToxinRecord : IDiscardTrigger
{
    public const string CardID = CardRegistry.TOXIN_RECORD_ID;
    public const string CardName = "毒记";
    public const string CardNameEN = "Toxin Record";

    public const int BaseCost = 1;
    public const int BasePoisonLayers = 8;
    public const int UpgradedPoisonLayers = 12;

    public int CurrentPoisonLayers { get; set; } = BasePoisonLayers;

    public ToxinRecord()
    {
        Logger.Log($"Initialized {CardName} (ID: {CardID})");
    }

    public string GetCardId() => CardID;

    /// <summary>
    /// Called when this card is discarded from hand.
    /// Applies poison to a random enemy.
    /// </summary>
    public bool OnDiscard(object card, object player)
    {
        try
        {
            Logger.Log($"{CardName} discarded: Applying {CurrentPoisonLayers} poison to random enemy");

            // TODO (Phase 4): Implement poison application
            // Get random enemy and apply poison status

            return true;
        }
        catch (Exception ex)
        {
            Logger.LogError($"Error in {CardName}.OnDiscard(): {ex.Message}");
            return false;
        }
    }

    public static CardMetadata GetMetadata()
    {
        return new CardMetadata
        {
            ID = CardID,
            Name = CardName,
            NameEN = CardNameEN,
            Cost = BaseCost,
            Rarity = CardRarity.Common,
            Type = CardType.Skill,
            IsPlayable = false,
            BaseDamage = 0,
            UpgradedDamage = 0,
            CharacterID = "Mystic",
            Description = "Discard trigger: Apply 8 poison to a random enemy when discarded.",
            UpgradeDescription = "Discard trigger: Apply 12 poison to a random enemy when discarded."
        };
    }
}
