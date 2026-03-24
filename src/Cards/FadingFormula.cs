using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscardMod.Cards;

[Pool(typeof(RegentCardPool))]
public class FadingFormula : DiscardModCard
{
    private decimal discardBlock = 6m;

    public override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(1)];

    public override bool HasTurnEndInHandEffect => true;

    public FadingFormula()
        : base(0, CardType.Skill, CardRarity.Common, TargetType.Self, "shattered_echo", true)
    {
    }

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        LogPlay(cardPlay, $"draw={DynamicVars.Cards.IntValue}; discardBlock={discardBlock}; autoDiscard=endTurn");
        await DrawCards(choiceContext, DynamicVars.Cards.IntValue);
    }

    public override async Task OnTurnEndInHand(PlayerChoiceContext choiceContext)
    {
        LogLifecycle("turn-end-discard", "discarding self from hand");
        await CardCmd.Discard(choiceContext, this);
    }

    protected override async Task OnSelfDiscarded(PlayerChoiceContext choiceContext)
    {
        await GainFlatBlock(discardBlock);
    }

    public override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1m);
        discardBlock += 2m;
    }
}
