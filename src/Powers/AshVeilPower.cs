using BaseLib.Abstracts;
using DiscardMod.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using System.Threading.Tasks;

namespace DiscardMod.Powers;

/// <summary>
/// Each card-effect discard, gain Amount Block immediately.
/// Upgraded: +2 extra block on the 2nd+ discard each turn.
/// </summary>
public class AshVeilPower : CustomPowerModel
{
    private bool upgraded;
    private int discardCountThisTurn;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    internal void SetUpgraded() => upgraded = true;

    public override async Task AfterCardDiscarded(PlayerChoiceContext choiceContext, CardModel card)
    {
        if (!DiscardTriggerRuntime.ConsumePowerDiscardEvent(card))
        {
            return;
        }

        discardCountThisTurn++;
        decimal block = Amount;
        if (upgraded && discardCountThisTurn >= 2)
        {
            block += 2m;
        }

        await CreatureCmd.GainBlock(Owner, block, default(ValueProp), null, false);
    }

    public override Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        discardCountThisTurn = 0;
        return Task.CompletedTask;
    }
}
