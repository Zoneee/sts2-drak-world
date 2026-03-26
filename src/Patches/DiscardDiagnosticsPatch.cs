using DiscardMod.Cards;
using DiscardMod.Utils;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace DiscardMod.Patches;

[HarmonyPatch(typeof(CardCmd))]
public static class DiscardDiagnosticsPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(CardCmd.Discard), typeof(PlayerChoiceContext), typeof(CardModel))]
    private static void LogSingleDiscard(PlayerChoiceContext choiceContext, CardModel card)
    {
        DiscardTriggerRuntime.RegisterDiscards(choiceContext, card == null ? [] : [card]);
        DiscardModMain.Logger.Info($"[DiscardCmd] single discard requested | source={DiscardTriggerRuntime.CurrentScopeDescription}; card={DescribeCard(card)}; tracked={card is DiscardModCard}");
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(CardCmd.Discard), typeof(PlayerChoiceContext), typeof(IEnumerable<CardModel>))]
    private static void LogMultiDiscard(PlayerChoiceContext choiceContext, IEnumerable<CardModel> cards)
    {
        var array = cards?.ToArray() ?? [];
        DiscardTriggerRuntime.RegisterDiscards(choiceContext, array);
        DiscardModMain.Logger.Info($"[DiscardCmd] batch discard requested | source={DiscardTriggerRuntime.CurrentScopeDescription}; count={array.Length}; cards={string.Join(", ", array.Select(DescribeCard))}");
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(CardCmd.DiscardAndDraw), typeof(PlayerChoiceContext), typeof(IEnumerable<CardModel>), typeof(int))]
    private static void LogDiscardAndDraw(PlayerChoiceContext choiceContext, IEnumerable<CardModel> cardsToDiscard, int cardsToDraw)
    {
        var array = cardsToDiscard?.ToArray() ?? [];
        DiscardModMain.Logger.Info($"[DiscardCmd] discard-and-draw requested | discardCount={array.Length}; draw={cardsToDraw}; cards={string.Join(", ", array.Select(DescribeCard))}");
    }

    private static string DescribeCard(CardModel? card)
    {
        return card == null ? "<null>" : $"{card.GetType().Name}";
    }
}
