using BaseLib.Abstracts;
using DiscardMod.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using System.Linq;
using System.Threading.Tasks;

namespace DiscardMod.Powers;

/// <summary>
/// Every N card-effect discards this turn, immediately gain 1 energy.
/// Amount = threshold (3 base, 2 upgraded). Rolling counter, resets each turn.
/// </summary>
public class DarkMomentumPower : CustomPowerModel
{
    private int discardsSinceLastTrigger;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomPackedIconPath =>
        $"res://{DiscardModMain.ModId}/images/powers/dark_momentum_power_64.png";
    public override string? CustomBigIconPath =>
        $"res://{DiscardModMain.ModId}/images/powers/dark_momentum_power.png";

    public override async Task AfterCardDiscarded(PlayerChoiceContext choiceContext, CardModel card)
    {
        if (!DiscardTriggerRuntime.ConsumePowerDiscardEvent(card))
        {
            return;
        }

        discardsSinceLastTrigger++;
        var threshold = (int)Amount;
        if (threshold <= 0)
        {
            threshold = 3;
        }

        if (discardsSinceLastTrigger >= threshold)
        {
            discardsSinceLastTrigger -= threshold;
            var player = Owner?.CombatState?.Players.FirstOrDefault();
            if (player != null)
            {
                await PlayerCmd.GainEnergy(1m, player);
            }
        }
    }

    public override Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        discardsSinceLastTrigger = 0;
        return Task.CompletedTask;
    }
}
