using HarmonyLib;
using MegaCrit.Sts2.Core.Localization;

namespace DiscardMod.Patches;

[HarmonyPatch(typeof(LocTable), nameof(LocTable.GetRawText))]
[HarmonyBefore("BaseLib")]
[HarmonyPriority(Priority.First)]
public static class LocalizationRuntimePatch
{
    private static readonly Dictionary<string, string> CardText = new(StringComparer.Ordinal)
    {
        ["DISCARDMOD-DARK_FLAME_FRAGMENT.title"] = "暗焰残页",
        ["DISCARDMOD-DARK_FLAME_FRAGMENT.description"] = "弃牌触发：对所有敌人造成 6 点伤害。",
        ["DISCARDMOD-SWIFT_CUT.title"] = "迅影斩",
        ["DISCARDMOD-SWIFT_CUT.description"] = "弃牌触发：对随机敌人造成 3 点伤害。",
        ["DISCARDMOD-TOXIN_RECORD.title"] = "毒素记录",
        ["DISCARDMOD-TOXIN_RECORD.description"] = "弃牌触发：对所有敌人施加 2 层中毒。",
        ["DISCARDMOD-SHATTERED_ECHO.title"] = "碎念回响",
        ["DISCARDMOD-SHATTERED_ECHO.description"] = "弃牌触发：抽 2 张牌。"
    };

    public static bool Prefix(string key, string ____name, ref string __result)
    {
        if (!string.Equals(____name, "cards", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (!CardText.TryGetValue(key, out var text))
        {
            return true;
        }

        __result = text;
        return false;
    }
}