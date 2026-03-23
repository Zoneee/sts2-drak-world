using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Logging;

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
        // TODO: Use STS2 card pool registration API once documented
        // ModContent.RegisterCard<DarkFlameFragment>();
        // ModContent.RegisterCard<SwiftCut>();
        // ModContent.RegisterCard<ToxinRecord>();
        // ModContent.RegisterCard<ShatteredEcho>();
        Logger.Info("Card registration placeholder (API TBD)");
    }
}
