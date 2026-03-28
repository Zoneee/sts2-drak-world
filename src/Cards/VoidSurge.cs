using BaseLib.Utils;
using DiscardMod.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscardMod.Cards;

[Pool(typeof(RegentCardPool))]
public class VoidSurge : DiscardModCard
{
    private const decimal DamagePerHit = 3m;
    private const int BaseHits = 2;
    private const int UpgradedHits = 3;

    // DamageVar = damage per hit; CardsVar = hits per discard trigger
    public override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(DamagePerHit, ValueProp.Move), new CardsVar(BaseHits)];

    public VoidSurge()
        : base(2, CardType.Power, CardRarity.Rare, TargetType.Self, "void_surge", true)
    {
    }

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var hits = IsUpgraded ? UpgradedHits : BaseHits;
        LogPlay(cardPlay, $"hitsPerDiscard={hits}; damagePerHit={DamagePerHit}");
        var power = await CommonActions.ApplySelf<VoidSurgePower>(this, DamagePerHit, false);
        if (power != null)
        {
            power.SetHits(hits);
        }
    }

    public override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(UpgradedHits - BaseHits);
    }
}
