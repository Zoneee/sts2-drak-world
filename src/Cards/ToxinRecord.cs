using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using System.Threading.Tasks;

namespace DiscardMod.Cards;

[Pool(typeof(RegentCardPool))]
public class ToxinRecord : DiscardModCard
{
    private decimal playPoison = 5m;
    private decimal discardPoison = 3m;

    public ToxinRecord()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy, "toxin_record", true)
    {
    }

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var target = GetRequiredTarget(cardPlay);
        LogPlay(cardPlay, $"playPoison={playPoison}; discardPoison={discardPoison}");
        await CommonActions.Apply<PoisonPower>(target, this, playPoison);
    }

    protected override async Task OnSelfDiscarded(PlayerChoiceContext choiceContext)
    {
        await ApplyToAllEnemies<PoisonPower>(discardPoison);
    }

    public override void OnUpgrade()
    {
        playPoison += 2m;
        discardPoison += 1m;
    }
}
