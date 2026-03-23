using DiscardMod.Cards;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.UI;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Screens.CardLibrary;

namespace DiscardMod.Patches;

[HarmonyPatch(typeof(NCardLibraryGrid), "GetCardVisibility")]
public static class CardLibraryVisibilityPatch
{
    [HarmonyPostfix]
    private static void ForceCustomRegentCardsVisible(CardModel card, ref ModelVisibility __result)
    {
        if (!IsDiscardModCard(card))
        {
            return;
        }

        __result = ModelVisibility.Visible;
    }

    private static bool IsDiscardModCard(CardModel card)
    {
        return card is DarkFlameFragment
            or SwiftCut
            or ToxinRecord
            or ShatteredEcho;
    }
}