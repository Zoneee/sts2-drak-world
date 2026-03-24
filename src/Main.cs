using DiscardMod.Cards;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using HarmonyLib;
using System.Linq;

namespace DiscardMod;

[ModInitializer(nameof(Initialize))]
public static class DiscardModMain
{
    public const string ModId = "STS2DiscardMod";

    public static Logger Logger { get; } = new(ModId, LogType.Generic);

    public static void Initialize()
    {
        var cardTypes = DiscardModCard.AllCardTypes;
        Logger.Info($"STS2 Discard-Trigger Mod loading... discovered {cardTypes.Count} cards: {string.Join(", ", cardTypes.Select(type => type.Name))}");
        new Harmony(ModId).PatchAll(typeof(DiscardModMain).Assembly);
        Logger.Info("STS2 Discard-Trigger Mod loaded!");
    }
}
