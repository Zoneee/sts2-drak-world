using DiscardMod.Cards;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace DiscardMod.Patches;

[HarmonyPatch(typeof(Player), "PopulateStartingDeck")]
public static class DebugStartingDeckPatch
{
    private static readonly Type[] TestDeckCardTypes =
    [
        typeof(SwiftCut),
        typeof(DarkFlameFragment),
        typeof(RecallSurge),
        typeof(ShatteredEcho),
        typeof(EmberVolley),
        typeof(FadingFormula),
        typeof(AshenAegis),
        typeof(ToxinRecord),
        typeof(CripplingManuscript),
        typeof(FinalDraft),
        typeof(DarkMomentum),
        typeof(AshVeil),
        typeof(VoidSurge)
    ];

    [HarmonyPostfix]
    private static void ReplaceStartingDeckForDebug(Player __instance)
    {
        if (!DebugCardPoolSettings.ReplaceStartingDeckWithDiscardModTestDeck)
        {
            return;
        }

        if (!DebugDiscardModCardHelper.IsRegentCharacterPool(__instance.Character.CardPool))
        {
            return;
        }

        var testDeck = DebugDiscardModCardHelper.CreateMutableCards(TestDeckCardTypes);

        if (testDeck.Length == 0)
        {
            DiscardModMain.Logger.Warn("Debug starting-deck replacement requested, but no discard mod cards were resolved from ModelDb.");
            return;
        }

        __instance.Deck.Clear(silent: true);

        foreach (var card in testDeck)
        {
            card.FloorAddedToDeck = 1;
            __instance.Deck.AddInternal(card, -1, silent: true);
        }

        __instance.Deck.InvokeContentsChanged();
        __instance.Deck.InvokeCardAddFinished();

        DiscardModMain.Logger.Warn(
            $"DEBUG ONLY: starting deck replaced with discard mod test deck for {__instance.Character.Id}. cards={string.Join(", ", testDeck.Select(card => card.GetType().Name))}");
    }
}