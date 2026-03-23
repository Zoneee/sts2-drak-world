using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Logging;
using HarmonyLib;

namespace DiscardMod;

[ModInitializer(nameof(Initialize))]
public static class DiscardModMain
{
    public const string ModId = "STS2DiscardMod";

    public static Logger Logger { get; } = new(ModId, LogType.Generic);

    public static void Initialize()
    {
        Logger.Info("STS2 Discard-Trigger Mod loading...");
        new Harmony(ModId).PatchAll(typeof(DiscardModMain).Assembly);
        Logger.Info("STS2 Discard-Trigger Mod loaded!");
    }
}
