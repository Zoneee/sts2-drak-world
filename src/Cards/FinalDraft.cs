using BaseLib.Abstracts;
using BaseLib.Utils;
using DiscardMod.Utils;
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
    private decimal discardDamage = 10m;
    private decimal discardBonusDamagePerExcess = 3m;
    private int bonusDiscardThreshold = 3;

    public override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(16m, ValueProp.Move)];

    public FinalDraft()
        : base(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, "final_draft", true)
    {
    }

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        LogPlay(cardPlay, $"damage={DynamicVars.Damage.IntValue}; discardAoeDamage={discardDamage}; bonusThreshold={bonusDiscardThreshold}; bonusDamagePerExcess={discardBonusDamagePerExcess}");
        await CommonActions.CardAttack(this, cardPlay).Execute(choiceContext);
    }

    protected override async Task OnSelfDiscarded(PlayerChoiceContext choiceContext, DiscardEventContext discardContext)
    {
        var damage = discardDamage;
        if (discardContext.DiscardCountThisTurn >= bonusDiscardThreshold)
        {
            var excess = discardContext.DiscardCountThisTurn - bonusDiscardThreshold + 1;
            var bonus = Math.Min(excess * discardBonusDamagePerExcess, 9m);
            damage += bonus;
        }

        LogLifecycle("discard-final-draft", $"discardCountThisTurn={discardContext.DiscardCountThisTurn}; threshold={bonusDiscardThreshold}; damage={damage}");
        await AttackAllEnemies(choiceContext, damage);
    }

    public override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
        discardBonusDamagePerExcess += 2m;
    }
}
