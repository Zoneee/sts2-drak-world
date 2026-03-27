using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscardMod.Cards;

[Pool(typeof(RegentCardPool))]
public class EmberVolley : DiscardModCard
{
    private const int PlayHits = 3;
    private int discardHits = 2;
    private decimal discardDamage = 4m;

    public override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(4m, ValueProp.Move)];

    public EmberVolley()
        : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, "ember_volley", true)
    {
    }

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        LogPlay(cardPlay, $"damage={DynamicVars.Damage.IntValue}; hits={PlayHits}");
        for (var i = 0; i < PlayHits; i++)
        {
            await AttackRandomEnemy(choiceContext, DynamicVars.Damage.IntValue);
        }
    }

    protected override async Task OnSelfDiscarded(PlayerChoiceContext choiceContext)
    {
        LogLifecycle("discard-volley", $"damage={discardDamage}; hits={discardHits}");
        for (var i = 0; i < discardHits; i++)
        {
            await AttackRandomEnemy(choiceContext, discardDamage);
        }
    }

    public override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1m);
        discardDamage += 1m;
    }
}
