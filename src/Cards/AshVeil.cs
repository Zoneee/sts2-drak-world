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
public class AshVeil : DiscardModCard
{
    private const decimal BaseBlock = 3m;
    private const decimal UpgradedBlock = 5m;

    public override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(BaseBlock, ValueProp.Move)];

    public AshVeil()
        : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self, "ash_veil", true)
    {
    }

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var block = IsUpgraded ? UpgradedBlock : BaseBlock;
        LogPlay(cardPlay, $"block={block}; upgraded={IsUpgraded}");
        var power = await CommonActions.ApplySelf<AshVeilPower>(this, block, false);
        if (power != null && IsUpgraded)
        {
            power.SetUpgraded();
        }
    }

    public override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(UpgradedBlock - BaseBlock);
    }
}
