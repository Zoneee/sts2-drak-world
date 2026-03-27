using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscardMod.Cards;

[Pool(typeof(RegentCardPool))]
public class AshenAegis : DiscardModCard
{
    private decimal discardBlock = 10m;

    public override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(8m, ValueProp.Move)];

    public AshenAegis()
        : base(1, CardType.Skill, CardRarity.Common, TargetType.Self, "ashen_aegis", true)
    {
    }

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        LogPlay(cardPlay, $"block={DynamicVars.Block.IntValue}; discardBlock={discardBlock}; trigger-discard=1");
        await CommonActions.CardBlock(this, cardPlay);
        await DiscardFromHand(choiceContext, 1);
    }

    protected override async Task OnSelfDiscarded(PlayerChoiceContext choiceContext)
    {
        await GainFlatBlock(discardBlock);
    }

    public override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
        discardBlock += 3m;
    }
}
