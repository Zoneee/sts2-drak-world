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
public class EmberVolley : DiscardModCard
{
    private decimal discardDamage = 12m;

    public override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(10m, ValueProp.Move)];

    public EmberVolley()
        : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, "ember_volley", true)
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
        DynamicVars.Damage.UpgradeValueBy(3m);
        discardDamage += 2m;
    }
}
