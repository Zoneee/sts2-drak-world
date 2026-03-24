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
        ["DISCARDMOD-DARK_FLAME_FRAGMENT.description"] = "抽 1 张牌，然后弃 1 张牌。弃牌触发：对所有敌人造成 6 点伤害。",
        ["DISCARDMOD-SWIFT_CUT.title"] = "迅影斩",
        ["DISCARDMOD-SWIFT_CUT.description"] = "造成 5 点伤害。弃牌触发：对随机敌人造成 3 点伤害。",
        ["DISCARDMOD-TOXIN_RECORD.title"] = "毒素记录",
        ["DISCARDMOD-TOXIN_RECORD.description"] = "施加 4 层中毒。弃牌触发：对所有敌人施加 2 层中毒。",
        ["DISCARDMOD-SHATTERED_ECHO.title"] = "破碎回响",
        ["DISCARDMOD-SHATTERED_ECHO.description"] = "抽 2 张牌，然后弃 1 张牌。弃牌触发：抽 2 张牌。",
        ["DISCARDMOD-ASHEN_AEGIS.title"] = "灰烬庇护",
        ["DISCARDMOD-ASHEN_AEGIS.description"] = "获得 8 点格挡。弃牌触发：获得 5 点格挡。",
        ["DISCARDMOD-CRIPPLING_MANUSCRIPT.title"] = "崩坏手稿",
        ["DISCARDMOD-CRIPPLING_MANUSCRIPT.description"] = "施加 2 层虚弱和 2 层易伤。弃牌触发：对所有敌人施加 1 层虚弱。",
        ["DISCARDMOD-EMBER_VOLLEY.title"] = "余烬连射",
        ["DISCARDMOD-EMBER_VOLLEY.description"] = "造成 7 点伤害。弃牌触发：对随机敌人造成 4 点伤害并抽 1 张牌。",
        ["DISCARDMOD-RECALL_SURGE.title"] = "回收思路",
        ["DISCARDMOD-RECALL_SURGE.description"] = "抽 2 张牌，然后弃 1 张牌。弃牌触发：获得 4 点格挡。",
        ["DISCARDMOD-FADING_FORMULA.title"] = "褪色公式",
        ["DISCARDMOD-FADING_FORMULA.description"] = "抽 1 张牌。回合结束时若仍在手牌中，将其弃掉。弃牌触发：获得 6 点格挡。",
        ["DISCARDMOD-FINAL_DRAFT.title"] = "终稿余烬",
        ["DISCARDMOD-FINAL_DRAFT.description"] = "造成 12 点伤害。弃牌触发：对所有敌人造成 8 点伤害并抽 1 张牌。"
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
