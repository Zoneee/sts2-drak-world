using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscardMod.Cards;

[Pool(typeof(RegentCardPool))]
public class RecallSurge : DiscardModCard
{
    private decimal discardBlock = 7m;

    protected override int RequiredDiscardableCardsInHandToPlay => 1;

    public override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(1)];

    public RecallSurge()
        : base(1, CardType.Skill, CardRarity.Common, TargetType.Self, "recall_surge", true)
    {
    }

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        LogPlay(cardPlay, $"draw={DynamicVars.Cards.IntValue}; discard=1; discardBlock={discardBlock}");
        await DrawCards(choiceContext, DynamicVars.Cards.IntValue);
        await DiscardFromHand(choiceContext, 1);
    }

    protected override async Task OnSelfDiscarded(PlayerChoiceContext choiceContext)
    {
        await GainFlatBlock(discardBlock);
    }

    public override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1m);
        discardBlock += 3m;
    }
}
