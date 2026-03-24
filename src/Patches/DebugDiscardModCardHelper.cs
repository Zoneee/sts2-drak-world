using DiscardMod.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace DiscardMod.Patches;

internal static class DebugDiscardModCardHelper
{
    internal static CardModel[] ResolveCanonicalCards()
    {
        return DiscardModCard.AllCardTypes
            .Select(type => ModelDb.GetById<CardModel>(ModelDb.GetId(type)))
            .Where(card => card != null)
            .DistinctBy(card => card!.Id)
            .Cast<CardModel>()
            .ToArray();
    }

    internal static CardModel[] CreateMutableCards(IEnumerable<Type> cardTypes)
    {
        return cardTypes
            .Select(CreateMutableCard)
            .Where(card => card != null)
            .Cast<CardModel>()
            .ToArray();
    }

    internal static bool IsRegentCharacterPool(CardPoolModel cardPool)
    {
        return cardPool is RegentCardPool;
    }

    private static CardModel? CreateMutableCard(Type cardType)
    {
        var cardId = ModelDb.GetId(cardType);
        return ModelDb.GetById<CardModel>(cardId)?.ToMutable();
    }
}