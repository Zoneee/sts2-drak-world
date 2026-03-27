using BaseLib.Abstracts;
using BaseLib.Utils;
using DiscardMod.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using System.Linq;
using System.Threading.Tasks;

namespace DiscardMod.Cards;

[Pool(typeof(RegentCardPool))]
public class RecallSurge : DiscardModCard
{
    private bool upgraded;

    public RecallSurge()
        : base(1, CardType.Skill, CardRarity.Common, TargetType.Self, "recall_surge", true)
    {
    }

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        LogPlay(cardPlay, $"upgraded={upgraded}; action=retrieve-from-discard");
        var retrieved = await RetrieveFromDiscardPile(choiceContext, selectMode: upgraded);
        if (retrieved == null)
        {
            LogLifecycle("play-retrieve:skip", "discard pile empty or selection cancelled");
            return;
        }

        LogLifecycle("play-retrieve", $"card={DescribeCard(retrieved)}");
        await CardPileCmd.Add(retrieved, PileType.Hand, CardPilePosition.Bottom, this);
    }

    protected override async Task OnSelfDiscarded(PlayerChoiceContext choiceContext)
    {
        LogLifecycle("discard-chain:start", "retrieve→draw1→discard1→top-or-keep");

        var retrieved = await RetrieveFromDiscardPile(choiceContext, selectMode: true);
        if (retrieved != null)
        {
            LogLifecycle("discard-retrieve", $"card={DescribeCard(retrieved)}");
            await CardPileCmd.Add(retrieved, PileType.Hand, CardPilePosition.Bottom, this);
        }
        else
        {
            LogLifecycle("discard-retrieve:skip", "discard pile empty or selection cancelled");
        }

        await DrawCards(choiceContext, 1);
        await DiscardFromHand(choiceContext, 1);

        if (!upgraded)
        {
            var hand = CardPile.Get(PileType.Hand, Owner);
            if (hand != null && !hand.IsEmpty)
            {
                var topSelected = await CardSelectCmd.FromHand(choiceContext, Owner,
                    new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, 1), null, this);
                if (topSelected != null && topSelected.Any())
                {
                    var toTop = topSelected.First();
                    LogLifecycle("discard-top", $"card={DescribeCard(toTop)}");
                    await CardPileCmd.Add(toTop, PileType.Draw, CardPilePosition.Top, this);
                }
            }
        }

        LogLifecycle("discard-chain:end", $"upgraded={upgraded}");
    }

    private async Task<CardModel?> RetrieveFromDiscardPile(PlayerChoiceContext choiceContext, bool selectMode)
    {
        var discard = CardPile.Get(PileType.Discard, Owner);
        if (discard == null || discard.IsEmpty)
        {
            return null;
        }

        if (selectMode)
        {
            var selected = await CommonActions.SelectSingleCard(this,
                CardSelectorPrefs.DiscardSelectionPrompt, choiceContext, PileType.Discard);
            return selected;
        }

        return discard.Cards[Random.Shared.Next(discard.Cards.Count)];
    }

    public override void OnUpgrade()
    {
        upgraded = true;
    }
}
