using BaseLib.Utils;
using DiscardMod.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;
using System.Threading.Tasks;

namespace DiscardMod.Cards;

[Pool(typeof(RegentCardPool))]
public class DarkMomentum : DiscardModCard
{
    private const decimal BaseThreshold = 3m;
    private const decimal UpgradedThreshold = 2m;

    public DarkMomentum()
        : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self, "dark_momentum", true)
    {
    }

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var threshold = IsUpgraded ? UpgradedThreshold : BaseThreshold;
        LogPlay(cardPlay, $"threshold={threshold}");
        await CommonActions.ApplySelf<DarkMomentumPower>(this, threshold, false);
    }

    public override void OnUpgrade() { }
}
