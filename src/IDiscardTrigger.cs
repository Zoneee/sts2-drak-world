namespace DiscardMod;

/// <summary>
/// Interface for cards that trigger effects when discarded.
/// All discard-trigger cards must implement this interface.
/// </summary>
public interface IDiscardTrigger
{
    /// <summary>
    /// Called when this card is discarded.
    /// </summary>
    /// <param name="card">The card being discarded (AbstractCard reference).</param>
    /// <param name="player">The player who owns the card (AbstractPlayer reference).</param>
    /// <returns>True if effect executed successfully; false otherwise.</returns>
    bool OnDiscard(object card, object player);

    /// <summary>
    /// Get the unique ID for this card (e.g., "DiscardMod_DarkFlameFragment").
    /// </summary>
    string GetCardId();
}
