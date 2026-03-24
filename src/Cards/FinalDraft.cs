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
public class FinalDraft : DiscardModCard
{
    private decimal discardDamage = 8m;

    public override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(12m, ValueProp.Move)];

    public FinalDraft()
        : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, "dark_flame_fragment", true)
    {
    }

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        LogPlay(cardPlay, $"damage={DynamicVars.Damage.IntValue}; discardAoeDamage={discardDamage}; discardDraw=1");
        await CommonActions.CardAttack(this, cardPlay).Execute(choiceContext);
    }

    protected override async Task OnSelfDiscarded(PlayerChoiceContext choiceContext)
    {
        await AttackAllEnemies(choiceContext, discardDamage);
        await DrawCards(choiceContext, 1);
    }

    public override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
        discardDamage += 4m;
    }
}
