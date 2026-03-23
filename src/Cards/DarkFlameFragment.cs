using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using System.Threading.Tasks;

namespace DiscardMod.Cards;

/// <summary>
/// 暗焰残页 (Dark Flame Fragment)
/// Skill, Common, 1 energy, target: AnyEnemy
/// Discard trigger: deal 6 damage to all enemies (upgraded: 9).
/// </summary>
[Pool(typeof(RegentCardPool))]
public class DarkFlameFragment : DiscardModCard
{
    public DarkFlameFragment()
        : base(1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy, "dark_flame_fragment", true) { }

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // TODO: implement play effect once combat action API is explored
        DiscardModMain.Logger.Info("DarkFlameFragment played");
        await Task.CompletedTask;
    }

    public override void OnUpgrade()
    {
        // Upgrade: discard-trigger damage increases from 6 to 9
    }
}
