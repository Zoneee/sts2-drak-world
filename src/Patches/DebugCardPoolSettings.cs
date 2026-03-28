using DiscardMod.Utils;

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
#if DEBUG
        return true;
#else
        return ModConfig.Instance.StarterDeckEnabled;
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