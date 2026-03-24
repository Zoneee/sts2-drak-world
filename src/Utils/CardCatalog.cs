using System.Reflection;
using System.Text.Json;

namespace DiscardMod.Utils;

public static class CardCatalog
{
    private const string CatalogResourceName = "DiscardMod.Data.cards.catalog.json";
    private const string DefaultLocale = "eng";

    private static readonly Lazy<CardCatalogDocument> Catalog = new(LoadCatalog);
    private static readonly Lazy<IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>>> LocalizationTables = new(BuildLocalizationTables);

    public static IReadOnlyDictionary<string, string> GetLocalizationTable(string languageDescriptor)
    {
        var locale = ResolveLocale(languageDescriptor);
        if (LocalizationTables.Value.TryGetValue(locale, out var table))
        {
            return table;
        }

        return LocalizationTables.Value[DefaultLocale];
    }

    public static string ResolveLocale(string languageDescriptor)
    {
        if (string.IsNullOrWhiteSpace(languageDescriptor))
        {
            return DefaultLocale;
        }

        var normalized = languageDescriptor.ToLowerInvariant();
        if (normalized.Contains("zh") ||
            normalized.Contains("zho") ||
            normalized.Contains("chi") ||
            normalized.Contains("chs") ||
            normalized.Contains("cht") ||
            normalized.Contains("cn") ||
            normalized.Contains("schinese") ||
            normalized.Contains("tchinese") ||
            normalized.Contains("chinese") ||
            normalized.Contains("simplified") ||
            normalized.Contains("traditional") ||
            normalized.Contains("中文"))
        {
            return "zhs";
        }

        return DefaultLocale;
    }

    private static IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> BuildLocalizationTables()
    {
        var tables = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);

        foreach (var card in Catalog.Value.Cards)
        {
            foreach (var (locale, entry) in card.Localization)
            {
                if (!tables.TryGetValue(locale, out var table))
                {
                    table = new Dictionary<string, string>(StringComparer.Ordinal);
                    tables[locale] = table;
                }

                table[$"{card.Id}.title"] = entry.Title;
                table[$"{card.Id}.description"] = entry.Description;
            }
        }

        return tables.ToDictionary(
            pair => pair.Key,
            pair => (IReadOnlyDictionary<string, string>)pair.Value,
            StringComparer.OrdinalIgnoreCase);
    }

    private static CardCatalogDocument LoadCatalog()
    {
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(CatalogResourceName)
            ?? throw new InvalidOperationException($"Embedded card catalog '{CatalogResourceName}' was not found.");

        var document = JsonSerializer.Deserialize<CardCatalogDocument>(stream, JsonOptions())
            ?? throw new InvalidOperationException("Embedded card catalog could not be parsed.");

        if (document.Cards.Count == 0)
        {
            throw new InvalidOperationException("Embedded card catalog does not contain any cards.");
        }

        return document;
    }

    private static JsonSerializerOptions JsonOptions()
    {
        return new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    private sealed class CardCatalogDocument
    {
        public List<CardCatalogEntry> Cards { get; set; } = [];
    }

    private sealed class CardCatalogEntry
    {
        public string Id { get; set; } = string.Empty;

        public string AssetName { get; set; } = string.Empty;

        public Dictionary<string, CardLocalizationEntry> Localization { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    }

    private sealed class CardLocalizationEntry
    {
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
    }
}