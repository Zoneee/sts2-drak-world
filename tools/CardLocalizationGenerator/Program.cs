using System.Text.Json;

if (args.Length != 2)
{
    Console.Error.WriteLine("Usage: CardLocalizationGenerator <catalog-path> <output-directory>");
    return 1;
}

var catalogPath = Path.GetFullPath(args[0]);
var outputDirectory = Path.GetFullPath(args[1]);

if (!File.Exists(catalogPath))
{
    Console.Error.WriteLine($"Card catalog not found: {catalogPath}");
    return 1;
}

using var document = JsonDocument.Parse(File.ReadAllText(catalogPath));
var cards = document.RootElement.GetProperty("cards");
var cardTables = new Dictionary<string, SortedDictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
var powerTables = new Dictionary<string, SortedDictionary<string, string>>(StringComparer.OrdinalIgnoreCase);

foreach (var card in cards.EnumerateArray())
{
    var id = card.GetProperty("id").GetString();
    if (string.IsNullOrWhiteSpace(id))
    {
        continue;
    }

    // Route entries with smartDescription to powers.json; others to cards.json
    var isPower = card.GetProperty("localization").EnumerateObject().Any(
        locale => locale.Value.TryGetProperty("smartDescription", out _));
    var tables = isPower ? powerTables : cardTables;

    foreach (var localeNode in card.GetProperty("localization").EnumerateObject())
    {
        if (!tables.TryGetValue(localeNode.Name, out var table))
        {
            table = new SortedDictionary<string, string>(StringComparer.Ordinal);
            tables[localeNode.Name] = table;
        }

        table[$"{id}.title"] = localeNode.Value.GetProperty("title").GetString() ?? string.Empty;
        table[$"{id}.description"] = localeNode.Value.GetProperty("description").GetString() ?? string.Empty;
        if (localeNode.Value.TryGetProperty("smartDescription", out var smartDescProp))
        {
            table[$"{id}.smartDescription"] = smartDescProp.GetString() ?? string.Empty;
        }
    }
}

Directory.CreateDirectory(outputDirectory);
var options = new JsonSerializerOptions { WriteIndented = true };

foreach (var (locale, table) in cardTables)
{
    var localeDirectory = Path.Combine(outputDirectory, locale);
    Directory.CreateDirectory(localeDirectory);

    var outputPath = Path.Combine(localeDirectory, "cards.json");
    File.WriteAllText(outputPath, JsonSerializer.Serialize(table, options) + Environment.NewLine);
    Console.WriteLine($"Generated {outputPath}");
}

foreach (var (locale, table) in powerTables)
{
    var localeDirectory = Path.Combine(outputDirectory, locale);
    Directory.CreateDirectory(localeDirectory);

    var outputPath = Path.Combine(localeDirectory, "powers.json");
    File.WriteAllText(outputPath, JsonSerializer.Serialize(table, options) + Environment.NewLine);
    Console.WriteLine($"Generated {outputPath}");
}

return 0;