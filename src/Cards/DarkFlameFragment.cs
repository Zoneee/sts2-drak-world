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
public class DarkFlameFragment : DiscardModCard
{
    private int playDiscardCount = 2;

    protected override int RequiredDiscardableCardsInHandToPlay => playDiscardCount;

    public override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(1)];

    public DarkFlameFragment()
        : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self, "dark_flame_fragment", true)
    {
    }

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        LogPlay(cardPlay, $"draw={DynamicVars.Cards.IntValue}; discard={playDiscardCount}; discardEffect=discard-1");
        await DrawCards(choiceContext, DynamicVars.Cards.IntValue);
        await DiscardFromHand(choiceContext, playDiscardCount);
    }

    protected override async Task OnSelfDiscarded(PlayerChoiceContext choiceContext)
    {
        await DiscardFromHand(choiceContext, 1);
    }

    public override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
