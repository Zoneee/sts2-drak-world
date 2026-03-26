using HarmonyLib;
using DiscardMod.Utils;
using MegaCrit.Sts2.Core.Localization;
using System.Globalization;
using System.Diagnostics;
using System.Reflection;
using System.Threading;

namespace DiscardMod.Patches;

[HarmonyPatch(typeof(LocTable), nameof(LocTable.GetRawText))]
[HarmonyBefore("BaseLib")]
[HarmonyPriority(Priority.First)]
public static class LocalizationRuntimePatch
{
    private const int StatsLogThreshold = 256;
    private const int MarkerLogMaxLength = 160;

    private static string? cachedLanguageMarker;
    private static int statsLogged;
    private static long cardsTableLookups;
    private static long fastRejectedLookups;
    private static long modKeyLookups;
    private static long localizedHits;
    private static long localizedMisses;
    private static long languageResolutions;
    private static long languageResolutionTicks;

    public static bool Prefix(string key, string ____name, LocTable __instance, ref string __result)
    {
        if (!string.Equals(____name, "cards", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        Interlocked.Increment(ref cardsTableLookups);

        if (!CardCatalog.IsDiscardModKey(key))
        {
            Interlocked.Increment(ref fastRejectedLookups);
            MaybeLogStats();
            return true;
        }

        Interlocked.Increment(ref modKeyLookups);

        if (!CardCatalog.TryGetLocalizedText(GetLanguageMarker(__instance), key, out var text))
        {
            Interlocked.Increment(ref localizedMisses);
            MaybeLogStats();
            return true;
        }

        Interlocked.Increment(ref localizedHits);
        __result = text;
        MaybeLogStats();
        return false;
    }

    private static string GetLanguageMarker(LocTable locTable)
    {
        var cached = Volatile.Read(ref cachedLanguageMarker);
        if (!string.IsNullOrWhiteSpace(cached))
        {
            return cached;
        }

        var startedAt = Stopwatch.GetTimestamp();
        var resolved = ResolveLanguageMarker(locTable);
        var elapsedTicks = Stopwatch.GetTimestamp() - startedAt;

        Interlocked.Increment(ref languageResolutions);
        Interlocked.Add(ref languageResolutionTicks, elapsedTicks);
        Interlocked.CompareExchange(ref cachedLanguageMarker, resolved, null);

        return Volatile.Read(ref cachedLanguageMarker) ?? resolved;
    }

    private static string ResolveLanguageMarker(LocTable locTable)
    {
        var candidates = new List<string>();
        AppendCultureDescriptors(candidates);
        AppendLanguageDescriptors(candidates, locTable);

        foreach (var type in typeof(LocTable).Assembly.GetTypes())
        {
            AppendLanguageDescriptors(candidates, type);
        }

        var descriptor = string.Join('|',
            candidates
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Distinct(StringComparer.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(descriptor))
        {
            return descriptor;
        }

        return string.Join('|',
            CultureInfo.CurrentCulture.Name,
            CultureInfo.CurrentUICulture.Name,
            CultureInfo.CurrentUICulture.ThreeLetterISOLanguageName);
    }

    private static void MaybeLogStats()
    {
        if (Volatile.Read(ref statsLogged) != 0)
        {
            return;
        }

        var totalLookups = Interlocked.Read(ref cardsTableLookups);
        if (totalLookups < StatsLogThreshold)
        {
            return;
        }

        if (Interlocked.Exchange(ref statsLogged, 1) != 0)
        {
            return;
        }

        var resolutionMs = TimeSpan.FromSeconds(Interlocked.Read(ref languageResolutionTicks) / (double)Stopwatch.Frequency).TotalMilliseconds;
        var resolvedMarker = Volatile.Read(ref cachedLanguageMarker) ?? string.Empty;
        var resolvedLocale = CardCatalog.ResolveLocale(resolvedMarker);
        DiscardModMain.Logger.Info(
            $"[LocalizationRuntimePatch] cardsLookups={totalLookups}; fastRejected={Interlocked.Read(ref fastRejectedLookups)}; modKeys={Interlocked.Read(ref modKeyLookups)}; localizedHits={Interlocked.Read(ref localizedHits)}; localizedMisses={Interlocked.Read(ref localizedMisses)}; languageResolutions={Interlocked.Read(ref languageResolutions)}; languageResolutionMs={resolutionMs:F2}; resolvedLocale={resolvedLocale}; languageMarker={FormatMarkerForLog(resolvedMarker)}");
    }

    private static void AppendCultureDescriptors(ICollection<string> candidates)
    {
        foreach (var culture in new[] { CultureInfo.CurrentUICulture, CultureInfo.CurrentCulture })
        {
            candidates.Add(culture.Name);
            candidates.Add(culture.EnglishName);
            candidates.Add(culture.NativeName);
            candidates.Add(culture.ThreeLetterISOLanguageName);
            candidates.Add(culture.TwoLetterISOLanguageName);
        }
    }

    private static void AppendLanguageDescriptors(ICollection<string> candidates, object source)
    {
        if (TryReadLanguageDescriptor(source, out var descriptor))
        {
            candidates.Add(descriptor);
        }
    }

    private static void AppendLanguageDescriptors(ICollection<string> candidates, Type type)
    {
        if (TryReadLanguageDescriptor(type, out var descriptor))
        {
            candidates.Add(descriptor);
        }
    }

    private static string FormatMarkerForLog(string marker)
    {
        if (string.IsNullOrWhiteSpace(marker))
        {
            return "<empty>";
        }

        return marker.Length <= MarkerLogMaxLength
            ? marker
            : $"{marker[..MarkerLogMaxLength]}...";
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
