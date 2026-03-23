using System;
using DiscardMod.Utils;

namespace DiscardMod.Cards;

/// <summary>
/// 迅切 (Swift Cut)
/// 
/// A foundational card that enables the discard-trigger system through card draw.
/// This card can be played normally and combines card draw with active discarding.
/// If the discarded card is a discard-trigger, refund 1 energy.
/// 
/// Design Purpose:
/// - Entry point to the system for new players
/// - 0-cost card draw enables turn acceleration
/// - Energy refund incentivizes holding discard-trigger cards
/// - Synergizes with all other discard-trigger cards
/// </summary>
public class SwiftCut : IDiscardTrigger
{
    public const string CardID = CardRegistry.SWIFT_CUT_ID;
    public const string CardName = "迅切";
    public const string CardNameEN = "Swift Cut";
    
    public const int BaseCost = 0;
    public const int BaseDrawAmount = 2;
    public const int UpgradedDrawAmount = 3;
    public const int EnergyRefund = 1;

    public int CurrentDrawAmount { get; set; } = BaseDrawAmount;

    public SwiftCut()
    {
        Logger.Log($"Initialized {CardName} (ID: {CardID})");
    }

    public string GetCardId() => CardID;

    /// <summary>
    /// Called when this card is discarded from hand.
    /// Note: Swift Cut is playable, so this fires when another effect discards it.
    /// </summary>
    public bool OnDiscard(object card, object player)
    {
        try
        {
            Logger.Log($"{CardName} discarded: Checking for energy refund");
            
            // TODO (Phase 4): Check if discarded card is IDiscardTrigger
            // If yes, refund 1 energy
            
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
            IsPlayable = true,
            BaseDamage = 0,
            UpgradedDamage = 0,
            Description = "Draw 2 cards, then discard 1. If discarded card has discard effect, gain 1 energy.",
            UpgradeDescription = "Draw 3 cards, then discard 1. If discarded card has discard effect, gain 1 energy."
        };
    }
}
