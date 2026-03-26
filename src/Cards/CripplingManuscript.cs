using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using System.Threading.Tasks;

namespace DiscardMod.Cards;

[Pool(typeof(RegentCardPool))]
public class CripplingManuscript : DiscardModCard
{
    private decimal playWeak = 2m;
    private decimal playVulnerable = 2m;
    private decimal discardWeak = 1m;
    private decimal discardVulnerable = 1m;

    public CripplingManuscript()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy, "crippling_manuscript", true)
    {
    }

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var target = GetRequiredTarget(cardPlay);
        LogPlay(cardPlay, $"weak={playWeak}; vulnerable={playVulnerable}; discardWeakAll={discardWeak}; discardVulnerableAll={discardVulnerable}");
        await CommonActions.Apply<WeakPower>(target, this, playWeak);
        await CommonActions.Apply<VulnerablePower>(target, this, playVulnerable);
    }

    protected override async Task OnSelfDiscarded(PlayerChoiceContext choiceContext)
    {
        await ApplyToAllEnemies<WeakPower>(discardWeak);
        await ApplyToAllEnemies<VulnerablePower>(discardVulnerable);
    }

    public override void OnUpgrade()
    {
        playWeak += 1m;
        playVulnerable += 1m;
        discardWeak += 1m;
        discardVulnerable += 1m;
    }
}
