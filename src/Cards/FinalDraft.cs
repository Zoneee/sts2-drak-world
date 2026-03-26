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
    private decimal discardBonusDamage = 4m;
    private const int BonusDiscardThreshold = 2;

    public override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(16m, ValueProp.Move)];

    public FinalDraft()
        : base(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, "final_draft", true)
    {
    }

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        LogPlay(cardPlay, $"damage={DynamicVars.Damage.IntValue}; discardAoeDamage={discardDamage}; bonusThreshold={BonusDiscardThreshold}; bonusDamage={discardBonusDamage}");
        await CommonActions.CardAttack(this, cardPlay).Execute(choiceContext);
    }

    protected override async Task OnSelfDiscarded(PlayerChoiceContext choiceContext, DiscardEventContext discardContext)
    {
        var damage = discardDamage;
        if (discardContext.DiscardCountThisTurn >= BonusDiscardThreshold)
        {
            damage += discardBonusDamage;
        }

        await AttackAllEnemies(choiceContext, damage);
    }

    public override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
        discardBonusDamage += 4m;
    }
}
