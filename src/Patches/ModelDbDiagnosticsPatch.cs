using DiscardMod.Cards;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using System.Collections.Generic;
using System.Linq;

namespace DiscardMod.Patches;

[HarmonyPatch(typeof(ModelDb), nameof(ModelDb.Init))]
public static class ModelDbDiagnosticsPatch
{
    private static readonly AccessTools.FieldRef<CardPoolModel, CardModel[]?> AllCardsField =
        AccessTools.FieldRefAccess<CardPoolModel, CardModel[]?>("_allCards");

    private static readonly AccessTools.FieldRef<CardPoolModel, HashSet<ModelId>?> AllCardIdsField =
        AccessTools.FieldRefAccess<CardPoolModel, HashSet<ModelId>?>("_allCardIds");

    [HarmonyPostfix]
    private static void LogCustomCardPresence()
    {
        foreach (var cardType in DiscardModCard.AllCardTypes)
        {
            LogCardPresence(cardType);
        }

        RestrictRegentPoolForDebugTesting();
    }

    private static void LogCardPresence(Type cardType)
    {
        var regentPool = ModelDb.CardPool<RegentCardPool>();
        var cardId = ModelDb.GetId(cardType);
        var cardModel = ModelDb.GetById<CardModel>(cardId);

        if (cardModel == null)
        {
            DiscardModMain.Logger.Warn($"ModelDb missing discard-system card {cardType.Name} ({cardId}) after init");
            return;
        }

        var inRegentPool = regentPool.AllCardIds.Contains(cardId);
        var inGlobalCards = ModelDb.AllCards.Any(card => card.Id == cardId);

        DiscardModMain.Logger.Info(
            $"ModelDb loaded discard-system card {cardType.Name} as {cardId}; library={cardModel.ShouldShowInCardLibrary}; regentPool={inRegentPool}; allCards={inGlobalCards}");
    }

    private static void RestrictRegentPoolForDebugTesting()
    {
        if (!DebugCardPoolSettings.RestrictRegentPoolToDiscardModCards)
        {
            return;
        }

        var regentPool = ModelDb.CardPool<RegentCardPool>();
        var originalCount = regentPool.AllCardIds.Count();
        var customCards = DiscardModCard.AllCardTypes
            .Select(type => ModelDb.GetById<CardModel>(ModelDb.GetId(type)))
            .Where(card => card != null)
            .DistinctBy(card => card!.Id)
            .Cast<CardModel>()
            .ToArray();

        if (customCards.Length == 0)
        {
            DiscardModMain.Logger.Warn("Debug card-pool filter requested, but no discard mod cards were resolved from ModelDb.");
            return;
        }

        AllCardsField(regentPool) = customCards;
        AllCardIdsField(regentPool) = customCards.Select(card => card.Id).ToHashSet();

        DiscardModMain.Logger.Warn(
            $"DEBUG ONLY: RegentCardPool restricted to discard mod cards for rapid testing. kept={customCards.Length}; removed={originalCount - customCards.Length}");
    }
}
