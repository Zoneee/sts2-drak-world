namespace DiscardMod.Patches;

internal static class DebugCardPoolSettings
{
    internal static bool RestrictRegentPoolToDiscardModCards { get; } = GetDefaultCardPoolRestriction();

    internal static bool ForceCustomCardsVisibleInCardLibrary { get; } = GetDefaultCardLibraryVisibilityOverride();

    internal static bool ReplaceStartingDeckWithDiscardModTestDeck { get; } = GetDefaultStartingDeckReplacement();

    internal static bool ReplaceMerchantColorlessCardsWithDiscardModCards { get; } = GetDefaultMerchantReplacement();

    private static bool GetDefaultCardPoolRestriction()
    {
        return false;
    }

    private static bool GetDefaultCardLibraryVisibilityOverride()
    {
        return false;
    }

    private static bool GetDefaultStartingDeckReplacement()
    {
#if DEBUG || STARTER_DECK_ENABLED
        return true;
#else
        return false;
#endif
    }

    private static bool GetDefaultMerchantReplacement()
    {
#if DEBUG
        return true;
#else
        return false;
#endif
    }
}