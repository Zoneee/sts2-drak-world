using BaseLib.Abstracts;
using BaseLib.Utils;
using DiscardMod.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscardMod.Cards;

[Pool(typeof(RegentCardPool))]
public class FadingFormula : DiscardModCard
{
    private decimal oddDiscardDamage = 18m;
    private decimal evenDiscardBlock = 18m;
    private decimal oddDiscardVulnerable;
    private decimal evenDiscardWeak;

    public override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(10m, ValueProp.Move), new BlockVar(10m, ValueProp.Move)];

    public override bool HasTurnEndInHandEffect => true;

    public FadingFormula()
        : base(3, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy, "fading_formula", true)
    {
    }

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        LogPlay(cardPlay, $"damage={DynamicVars.Damage.IntValue}; block={DynamicVars.Block.IntValue}; oddDiscardDamage={oddDiscardDamage}; evenDiscardBlock={evenDiscardBlock}; autoDiscard=endTurn-explicit");
        await CommonActions.CardAttack(this, cardPlay).Execute(choiceContext);
        await GainFlatBlock(DynamicVars.Block.IntValue);
    }

    public override async Task OnTurnEndInHand(PlayerChoiceContext choiceContext)
    {
        LogLifecycle("turn-end-discard", "discarding self from hand");
        using var scope = DiscardTriggerRuntime.BeginExplicitExceptionScope(this);
        await CardCmd.Discard(choiceContext, this);
    }

    protected override async Task OnSelfDiscarded(PlayerChoiceContext choiceContext, DiscardEventContext discardContext)
    {
        if (discardContext.IsOddDiscard)
        {
            var target = GetRandomEnemy();
            if (target == null)
            {
                LogLifecycle("fading-formula:odd-skip", "no hittable enemies");
                return;
            }

            if (oddDiscardVulnerable > 0)
            {
                LogLifecycle("apply-random", $"power={typeof(VulnerablePower).Name}; target={DescribeCreature(target)}; amount={oddDiscardVulnerable}");
                await CommonActions.Apply<VulnerablePower>(target, this, oddDiscardVulnerable);
            }

            LogLifecycle("fading-formula:odd", $"discardCountThisTurn={discardContext.DiscardCountThisTurn}; target={DescribeCreature(target)}; damage={oddDiscardDamage}");
            await CommonActions.CardAttack(this, target, oddDiscardDamage).Execute(choiceContext);
            return;
        }

        if (evenDiscardWeak > 0)
        {
            await ApplyToAllEnemies<WeakPower>(evenDiscardWeak);
        }

        LogLifecycle("fading-formula:even", $"discardCountThisTurn={discardContext.DiscardCountThisTurn}; block={evenDiscardBlock}");
        await GainFlatBlock(evenDiscardBlock);
    }

    public override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
        DynamicVars.Block.UpgradeValueBy(4m);
        oddDiscardDamage += 4m;
        evenDiscardBlock += 4m;
        oddDiscardVulnerable += 1m;
        evenDiscardWeak += 1m;
    }
}
