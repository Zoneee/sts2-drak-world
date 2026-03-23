using System;
using DiscardMod.Utils;

namespace DiscardMod.Cards;

/// <summary>
/// 碎念回响 (Shattered Echo)
/// 
/// An enabler card that chains discard effects together.
/// This card can be played normally. It discards a card from hand,
/// and if that card is a discard-trigger, draws extra cards.
/// 
/// Design Purpose:
/// - Enabler for chaining discard effects
/// - Turbo-charges deck cycles
/// - Enables long combo turns with multiple discards
/// - Consistent card velocity for sustained value
/// </summary>
public class ShatteredEcho : IDiscardTrigger
{
    public const string CardID = CardRegistry.SHATTERED_ECHO_ID;
    public const string CardName = "碎念回响";
    public const string CardNameEN = "Shattered Echo";

    public const int BaseCost = 1;
    public const int BaseDrawAmount = 2;
    public const int UpgradedDrawAmount = 3;

    public int CurrentDrawAmount { get; set; } = BaseDrawAmount;

    public ShatteredEcho()
    {
        Logger.Log($"Initialized {CardName} (ID: {CardID})");
    }

    public string GetCardId() => CardID;

    /// <summary>
    /// Called when this card is discarded from hand.
    /// Note: Shattered Echo is playable, so this fires when another effect discards it.
    /// </summary>
    public bool OnDiscard(object card, object player)
    {
        try
        {
            Logger.Log($"{CardName} discarded");

            // TODO (Phase 4): When played, discard 1 card
            // If discarded card is IDiscardTrigger, draw extra cards

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
            CharacterID = "Mystic",
            Description = "Discard 1 card from hand. If discarded card has discard effect, draw 2 more cards.",
            UpgradeDescription = "Discard 1 card from hand. If discarded card has discard effect, draw 3 more cards."
        };
    }
}
