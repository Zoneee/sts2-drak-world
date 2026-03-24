using HarmonyLib;
using DiscardMod.Utils;
using MegaCrit.Sts2.Core.Localization;
using System.Globalization;
using System.Reflection;

namespace DiscardMod.Patches;

[HarmonyPatch(typeof(LocTable), nameof(LocTable.GetRawText))]
[HarmonyBefore("BaseLib")]
[HarmonyPriority(Priority.First)]
public static class LocalizationRuntimePatch
{
    public static bool Prefix(string key, string ____name, LocTable __instance, ref string __result)
    {
        if (!string.Equals(____name, "cards", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (!CardCatalog.TryGetLocalizedText(ResolveLanguageMarker(__instance), key, out var text))
        {
            return true;
        }

        __result = text;
        return false;
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
