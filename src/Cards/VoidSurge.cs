using BaseLib.Utils;
using DiscardMod.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;
using System.Threading.Tasks;

namespace DiscardMod.Cards;

[Pool(typeof(RegentCardPool))]
public class VoidSurge : DiscardModCard
{
    public VoidSurge()
        : base(2, CardType.Power, CardRarity.Rare, TargetType.Self, "void_surge", true)
    {
    }

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var hits = IsUpgraded ? 3 : 2;
        LogPlay(cardPlay, $"hitsPerDiscard={hits}; damagePerHit=3");
        var power = await CommonActions.ApplySelf<VoidSurgePower>(this, 3m, false);
        if (power != null)
        {
            power.SetHits(hits);
        }
    }

    public override void OnUpgrade() { }
}
