using DiscardMod.Cards;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using System.Linq;

namespace DiscardMod.Patches;

[HarmonyPatch(typeof(ModelDb), nameof(ModelDb.Init))]
public static class ModelDbDiagnosticsPatch
{
    [HarmonyPostfix]
    private static void LogCustomCardPresence()
    {
        LogCardPresence(typeof(DarkFlameFragment));
        LogCardPresence(typeof(SwiftCut));
        LogCardPresence(typeof(ToxinRecord));
        LogCardPresence(typeof(ShatteredEcho));
    }

    private static void LogCardPresence(Type cardType)
    {
        var regentPool = ModelDb.CardPool<RegentCardPool>();
        var cardId = ModelDb.GetId(cardType);
        var cardModel = ModelDb.GetById<CardModel>(cardId);

        if (cardModel == null)
        {
            DiscardModMain.Logger.Info($"ModelDb missing custom card {cardType.Name} ({cardId}) after init");
            return;
        }

        var inRegentPool = regentPool.AllCardIds.Contains(cardId);
        var inGlobalCards = ModelDb.AllCards.Any(card => card.Id == cardId);

        DiscardModMain.Logger.Info(
            $"ModelDb loaded custom card {cardType.Name} as {cardId}; library={cardModel.ShouldShowInCardLibrary}; regentPool={inRegentPool}; allCards={inGlobalCards}");
    }
}