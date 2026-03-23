using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using System.Threading.Tasks;

namespace DiscardMod.Cards;

/// <summary>
/// 迅影斩 (Swift Cut)
/// Attack, Common, 0 energy, target: AnyEnemy
/// Discard trigger: deal 3 damage to a random enemy (upgraded: 5).
/// </summary>
public class SwiftCut : CardModel
{
    public SwiftCut()
        : base(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, true) { }

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // TODO: deal damage to targeted enemy (normal play effect)
        DiscardModMain.Logger.Info("SwiftCut played");
        await Task.CompletedTask;
    }

    public override void OnUpgrade()
    {
        // Upgrade: discard-trigger damage increases from 3 to 5
    }
}
