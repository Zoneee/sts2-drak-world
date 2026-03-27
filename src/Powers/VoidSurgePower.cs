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
/// Each card-effect discard, attack a random enemy Amount times for 3 damage each.
/// Upgraded: 3 hits instead of 2 (Amount stays 3 dmg, hits controlled by IsUpgraded logic in card).
/// </summary>
public class VoidSurgePower : CustomPowerModel
{
    private decimal damagePerHit = 3m;
    private int hits = 2;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    internal void SetHits(int hitCount) => hits = hitCount;

    public override async Task AfterCardDiscarded(PlayerChoiceContext choiceContext, CardModel card)
    {
        if (!DiscardTriggerRuntime.ConsumePowerDiscardEvent(card))
        {
            return;
        }

        var enemies = Owner?.CombatState?.HittableEnemies;
        if (enemies == null || enemies.Count == 0)
        {
            return;
        }

        for (var i = 0; i < hits; i++)
        {
            var target = enemies[Random.Shared.Next(enemies.Count)];
            await CreatureCmd.Damage(choiceContext, target, damagePerHit, default(ValueProp), Owner);
        }
    }
}
