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
public class SwiftCut : DiscardModCard
{
    private decimal discardDamage = 7m;

    public override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6m, ValueProp.Move)];

    public SwiftCut()
        : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, "swift_cut", true)
    {
    }

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        LogPlay(cardPlay, $"damage={DynamicVars.Damage.IntValue}; discardDamage={discardDamage}");
        await CommonActions.CardAttack(this, cardPlay).Execute(choiceContext);
    }

    protected override async Task OnSelfDiscarded(PlayerChoiceContext choiceContext)
    {
        await AttackRandomEnemy(choiceContext, discardDamage);
    }

    public override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        discardDamage += 2m;
    }
}
