using System;
using DiscardMod.Utils;

namespace DiscardMod.Cards;

/// <summary>
/// 暗焰残页 (Dark Flame Fragment)
/// 
/// A foundational discard-trigger card that demonstrates the core mechanic.
/// This card cannot be played from hand. When discarded, it deals damage to all enemies.
/// 
/// Design Purpose:
/// - Clearest example of "discard-trigger" mechanic
/// - No ambiguity: card is useless when played, powerful when discarded
/// - Stacks multiplicatively with multi-discard (3x = 18 damage)
/// - Core card for burst damage strategies
/// </summary>
public class DarkFlameFragment : IDiscardTrigger
{
    public const string CardID = CardRegistry.DARK_FLAME_ID;
    public const string CardName = "暗焰残页";
    public const string CardNameEN = "Dark Flame Fragment";

    // Card stats (base values; upgrades modify these)
    public const int BaseCost = 1;
    public const int BaseDamage = 6;
    public const int UpgradedDamage = 9;

    /// <summary>
    /// Base damage value for this run (can be modified by relics/effects).
    /// </summary>
    public int CurrentDamage { get; set; } = BaseDamage;

    public DarkFlameFragment()
    {
        Logger.Log($"Initialized {CardName} (ID: {CardID})");
    }

    /// <summary>
    /// Get the unique card ID.
    /// </summary>
    public string GetCardId() => CardID;

    /// <summary>
    /// Called when this card is discarded from hand.
    /// Deals damage to all enemies.
    /// </summary>
    public bool OnDiscard(object card, object player)
    {
        try
        {
            Logger.Log($"{CardName} discarded: Dealing {CurrentDamage} damage to all enemies");

            // TODO (Phase 3): Implement damage action
            // This requires BaseLib integration to access:
            // - AbstractDungeon.actionManager
            // - AbstractPlayer hostile references
            // - DamageInfo / DamageAction construction

            // When integrated with STS2:
            // var action = new DamageAction(enemy, 
            //     new DamageInfo(player, CurrentDamage, DamageType.NORMAL));
            // AbstractDungeon.actionManager.addToBottom(action);

            return true;
        }
        catch (Exception ex)
        {
            Logger.LogError($"Error in {CardName}.OnDiscard(): {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Get card metadata for registration.
    /// </summary>
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
            BaseDamage = BaseDamage,
            UpgradedDamage = UpgradedDamage,
            CharacterID = "Mystic",
            Description = "Discard trigger: Deal 6 damage to all enemies when discarded.",
            UpgradeDescription = "Discard trigger: Deal 9 damage to all enemies when discarded."
        };
    }
}

/// <summary>
/// Metadata for card registration and pooling.
/// </summary>
public class CardMetadata
{
    public string ID { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string NameEN { get; set; } = string.Empty;
    public int Cost { get; set; }
    public CardRarity Rarity { get; set; }
    public CardType Type { get; set; }
    public bool IsPlayable { get; set; }
    public int BaseDamage { get; set; }
    public int UpgradedDamage { get; set; }
    public string CharacterID { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string UpgradeDescription { get; set; } = string.Empty;
}

/// <summary>
/// Card rarity enumeration.
/// </summary>
public enum CardRarity
{
    Common,
    Uncommon,
    Rare
}

/// <summary>
/// Card type enumeration.
/// </summary>
public enum CardType
{
    Skill,
    Power,
    Attack,
    Curse,
    Status
}
