using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models.CardPools;
using DiscardMod.Cards;

namespace DiscardMod;

[ModInitializer(nameof(Initialize))]
public static class DiscardModMain
{
    public const string ModId = "STS2DiscardMod";

    public static Logger Logger { get; } = new(ModId, LogType.Generic);

    public static void Initialize()
    {
        Logger.Info("STS2 Discard-Trigger Mod loading...");
        RegisterCards();
        Logger.Info("STS2 Discard-Trigger Mod loaded!");
    }

    private static void RegisterCards()
    {
        ModHelper.AddModelToPool(typeof(RegentCardPool), typeof(DarkFlameFragment));
        ModHelper.AddModelToPool(typeof(RegentCardPool), typeof(SwiftCut));
        ModHelper.AddModelToPool(typeof(RegentCardPool), typeof(ToxinRecord));
        ModHelper.AddModelToPool(typeof(RegentCardPool), typeof(ShatteredEcho));
        Logger.Info("Registered 4 discard-trigger cards to RegentCardPool");
    }
}
