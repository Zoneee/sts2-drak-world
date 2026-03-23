using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using System.Threading.Tasks;

namespace DiscardMod.Cards;

/// <summary>
/// 毒素记录 (Toxin Record)
/// Skill, Uncommon, 1 energy, target: Self
/// Discard trigger: apply 2 Poison to all enemies (upgraded: 3).
/// </summary>
public class ToxinRecord : CardModel
{
    public ToxinRecord()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, true) { }

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // TODO: implement play effect (e.g. apply poison or draw a card)
        DiscardModMain.Logger.Info("ToxinRecord played");
        await Task.CompletedTask;
    }

    public override void OnUpgrade()
    {
        // Upgrade: discard-trigger poison stacks increase from 2 to 3
    }
}
