using BaseLib.Abstracts;
using BaseLib.Utils;
using DiscardMod.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscardMod.Cards;

[Pool(typeof(RegentCardPool))]
public class ShatteredEcho : DiscardModCard
{
    private int playDiscardCount = 3;
    private int discardDrawCount = 1;
    private int discardCount = 1;
    private int bonusNextDiscardCount;

    protected override int RequiredDiscardableCardsInHandToPlay => playDiscardCount;

    public override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2)];

    public ShatteredEcho()
        : base(3, CardType.Skill, CardRarity.Rare, TargetType.Self, "shattered_echo", true)
    {
    }

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        LogPlay(cardPlay, $"draw={DynamicVars.Cards.IntValue}; discard={playDiscardCount}; discardDraw={discardDrawCount}; discardDiscard={discardCount}; bonusNextDiscard={bonusNextDiscardCount}");
        await DrawCards(choiceContext, DynamicVars.Cards.IntValue);
        await DiscardFromHand(choiceContext, playDiscardCount);
    }

    protected override async Task OnSelfDiscarded(PlayerChoiceContext choiceContext)
    {
        await DrawCards(choiceContext, discardDrawCount);
        await DiscardFromHand(choiceContext, discardCount);

        if (bonusNextDiscardCount > 0)
        {
            DiscardTriggerRuntime.GrantBonusDiscard(Owner, bonusNextDiscardCount);
            LogLifecycle("discard-bonus-grant", $"bonusNextDiscard={bonusNextDiscardCount}");
        }
    }

    public override void OnUpgrade()
    {
        bonusNextDiscardCount = 1;
    }
}
