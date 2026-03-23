using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using System.Threading.Tasks;

namespace DiscardMod.Cards;

/// <summary>
/// 破碎回响 (Shattered Echo)
/// Skill, Rare, 2 energy, target: Self
/// Discard trigger: draw 2 cards (upgraded: 3).
/// </summary>
[Pool(typeof(RegentCardPool))]
public class ShatteredEcho : CustomCardModel
{
    public ShatteredEcho()
        : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self, true) { }

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // TODO: implement play effect (e.g. discard 1 card, draw 1 card)
        DiscardModMain.Logger.Info("ShatteredEcho played");
        await Task.CompletedTask;
    }

    public override void OnUpgrade()
    {
        // Upgrade: discard-trigger draw increases from 2 to 3
    }
}
