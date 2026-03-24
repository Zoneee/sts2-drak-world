using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Merchant;
using MegaCrit.Sts2.Core.Models;

namespace DiscardMod.Patches;

[HarmonyPatch(typeof(MerchantInventory), "PopulateColorlessCardEntries")]
public static class DebugMerchantInventoryPatch
{
    private static readonly AccessTools.FieldRef<MerchantInventory, List<MerchantCardEntry>> ColorlessEntriesField =
        AccessTools.FieldRefAccess<MerchantInventory, List<MerchantCardEntry>>("_colorlessCardEntries");

    [HarmonyPostfix]
    private static void ReplaceColorlessEntriesForDebug(MerchantInventory __instance)
    {
        if (!DebugCardPoolSettings.ReplaceMerchantColorlessCardsWithDiscardModCards)
        {
            return;
        }

        if (!DebugDiscardModCardHelper.IsRegentCharacterPool(__instance.Player.Character.CardPool))
        {
            return;
        }

        var customCards = DebugDiscardModCardHelper.ResolveCanonicalCards();
        if (customCards.Length == 0)
        {
            DiscardModMain.Logger.Warn("Debug merchant filter requested, but no discard mod cards were resolved from ModelDb.");
            return;
        }

        var colorlessEntries = ColorlessEntriesField(__instance);
        var originalCards = colorlessEntries
            .Select(entry => entry.CreationResult?.Card)
            .Where(card => card != null)
            .Cast<CardModel>()
            .ToArray();
        var rarities = originalCards.Select(card => card.Rarity).DefaultIfEmpty(CardRarity.Uncommon).ToArray();

        colorlessEntries.Clear();

        foreach (var rarity in rarities)
        {
            var entry = new MerchantCardEntry(__instance.Player, __instance, customCards, rarity);
            entry.Populate();
            colorlessEntries.Add(entry);
        }

        DiscardModMain.Logger.Warn(
            $"DEBUG ONLY: merchant colorless entries replaced with discard mod cards. original={string.Join(", ", originalCards.Select(card => card.GetType().Name))}; replacement={string.Join(", ", colorlessEntries.Select(entry => entry.CreationResult?.Card.GetType().Name ?? "<null>"))}");
    }
}