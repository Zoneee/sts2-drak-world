using HarmonyLib;
using MegaCrit.Sts2.Core.Localization;
using System.Globalization;
using System.Reflection;

namespace DiscardMod.Patches;

[HarmonyPatch(typeof(LocTable), nameof(LocTable.GetRawText))]
[HarmonyBefore("BaseLib")]
[HarmonyPriority(Priority.First)]
public static class LocalizationRuntimePatch
{
    private static readonly IReadOnlyDictionary<string, string> EnglishCardText = new Dictionary<string, string>(StringComparer.Ordinal)
    {
        ["DISCARDMOD-DARK_FLAME_FRAGMENT.title"] = "Dark Flame Fragment",
        ["DISCARDMOD-DARK_FLAME_FRAGMENT.description"] = "Draw 1 card, then discard 1 card. Discard: Deal 6 damage to all enemies.",
        ["DISCARDMOD-SWIFT_CUT.title"] = "Swift Cut",
        ["DISCARDMOD-SWIFT_CUT.description"] = "Deal 5 damage. Discard: Deal 3 damage to a random enemy.",
        ["DISCARDMOD-TOXIN_RECORD.title"] = "Toxin Record",
        ["DISCARDMOD-TOXIN_RECORD.description"] = "Apply 4 Poison. Discard: Apply 2 Poison to all enemies.",
        ["DISCARDMOD-SHATTERED_ECHO.title"] = "Shattered Echo",
        ["DISCARDMOD-SHATTERED_ECHO.description"] = "Draw 2 cards, then discard 1 card. Discard: Draw 2 cards.",
        ["DISCARDMOD-ASHEN_AEGIS.title"] = "Ashen Aegis",
        ["DISCARDMOD-ASHEN_AEGIS.description"] = "Gain 8 Block. Discard: Gain 5 Block.",
        ["DISCARDMOD-CRIPPLING_MANUSCRIPT.title"] = "Crippling Manuscript",
        ["DISCARDMOD-CRIPPLING_MANUSCRIPT.description"] = "Apply 2 Weak and 2 Vulnerable. Discard: Apply 1 Weak to all enemies.",
        ["DISCARDMOD-EMBER_VOLLEY.title"] = "Ember Volley",
        ["DISCARDMOD-EMBER_VOLLEY.description"] = "Deal 7 damage. Discard: Deal 4 damage to a random enemy and draw 1 card.",
        ["DISCARDMOD-RECALL_SURGE.title"] = "Recall Surge",
        ["DISCARDMOD-RECALL_SURGE.description"] = "Draw 2 cards, then discard 1 card. Discard: Gain 4 Block.",
        ["DISCARDMOD-FADING_FORMULA.title"] = "Fading Formula",
        ["DISCARDMOD-FADING_FORMULA.description"] = "Draw 1 card. If this remains in your hand at end of turn, discard it. Discard: Gain 6 Block.",
        ["DISCARDMOD-FINAL_DRAFT.title"] = "Final Draft",
        ["DISCARDMOD-FINAL_DRAFT.description"] = "Deal 12 damage. Discard: Deal 8 damage to all enemies and draw 1 card."
    };

    private static readonly IReadOnlyDictionary<string, string> ChineseCardText = new Dictionary<string, string>(StringComparer.Ordinal)
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

    private static readonly string[] ChineseLanguageMarkers =
    [
        "zh",
        "zho",
        "chi",
        "chs",
        "cht",
        "cn",
        "schinese",
        "tchinese",
        "chinese",
        "simplified",
        "traditional",
        "中文"
    ];

    public static bool Prefix(string key, string ____name, LocTable __instance, ref string __result)
    {
        if (!string.Equals(____name, "cards", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        var cardText = ResolveCardText(__instance);
        if (!cardText.TryGetValue(key, out var text))
        {
            return true;
        }

        __result = text;
        return false;
    }

    private static IReadOnlyDictionary<string, string> ResolveCardText(LocTable locTable)
    {
        return IsChineseLanguage(ResolveLanguageMarker(locTable)) ? ChineseCardText : EnglishCardText;
    }

    private static bool IsChineseLanguage(string languageMarker)
    {
        return ChineseLanguageMarkers.Any(marker => languageMarker.Contains(marker, StringComparison.OrdinalIgnoreCase));
    }

    private static string ResolveLanguageMarker(LocTable locTable)
    {
        if (TryReadLanguageDescriptor(locTable, out var instanceDescriptor))
        {
            return instanceDescriptor;
        }

        foreach (var type in typeof(LocTable).Assembly.GetTypes())
        {
            if (!TryReadLanguageDescriptor(type, out var staticDescriptor))
            {
                continue;
            }

            return staticDescriptor;
        }

        return string.Join('|',
            CultureInfo.CurrentCulture.Name,
            CultureInfo.CurrentUICulture.Name,
            CultureInfo.CurrentUICulture.ThreeLetterISOLanguageName);
    }

    private static bool TryReadLanguageDescriptor(object source, out string descriptor)
    {
        if (TryReadLanguageDescriptor(source.GetType(), source, out descriptor))
        {
            return true;
        }

        descriptor = string.Empty;
        return false;
    }

    private static bool TryReadLanguageDescriptor(Type type, out string descriptor)
    {
        return TryReadLanguageDescriptor(type, null, out descriptor);
    }

    private static bool TryReadLanguageDescriptor(Type type, object? instance, out string descriptor)
    {
        const BindingFlags instanceFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        const BindingFlags staticFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

        var candidates = new List<string>();
        foreach (var memberName in new[] { "CurrentLanguage", "Language", "LanguageCode", "Locale", "CurrentLocale", "LocalizationTable" })
        {
            var flags = instance == null ? staticFlags : instanceFlags;

            var property = type.GetProperty(memberName, flags);
            if (property != null)
            {
                AppendDescriptor(candidates, property.GetValue(instance));
            }

            var field = type.GetField(memberName, flags);
            if (field != null)
            {
                AppendDescriptor(candidates, field.GetValue(instance));
            }

            var method = type.GetMethod(memberName, flags, []);
            if (method != null)
            {
                AppendDescriptor(candidates, method.Invoke(instance, null));
            }

            var getter = type.GetMethod($"get_{memberName}", flags, []);
            if (getter != null)
            {
                AppendDescriptor(candidates, getter.Invoke(instance, null));
            }
        }

        descriptor = string.Join('|', candidates.Where(value => !string.IsNullOrWhiteSpace(value)).Distinct(StringComparer.OrdinalIgnoreCase));
        return descriptor.Length > 0;
    }

    private static void AppendDescriptor(ICollection<string> candidates, object? value)
    {
        if (value == null)
        {
            return;
        }

        candidates.Add(value.ToString() ?? string.Empty);

        var valueType = value.GetType();
        foreach (var propertyName in new[] { "LanguageCode", "Language", "Name", "Value", "ThreeLetterISOLanguageName" })
        {
            var property = valueType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (property == null)
            {
                continue;
            }

            var propertyValue = property.GetValue(value)?.ToString();
            if (!string.IsNullOrWhiteSpace(propertyValue))
            {
                candidates.Add(propertyValue);
            }
        }
    }
}
