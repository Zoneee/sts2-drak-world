namespace DiscardMod.Patches;

internal static class DebugCardPoolSettings
{
    internal static bool RestrictRegentPoolToDiscardModCards { get; } = GetDefaultRestriction();

    internal static bool ReplaceStartingDeckWithDiscardModTestDeck { get; } = GetDefaultRestriction();

    internal static bool ReplaceMerchantColorlessCardsWithDiscardModCards { get; } = GetDefaultRestriction();

    private static bool GetDefaultRestriction()
    {
#if DEBUG
        return true;
#else
        return false;
#endif
    }
}