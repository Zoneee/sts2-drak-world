namespace DiscardMod.Utils;

internal sealed class ModConfig
{
    private static ModConfig? _instance;
    internal static ModConfig Instance => _instance ??= Load();

    /// <summary>
    /// When true, the starting deck is replaced with the full discard-mod test deck
    /// at the start of each run. Useful for onboarding. Default: false.
    /// </summary>
    internal bool StarterDeckEnabled { get; private set; } = false;

    private static ModConfig Load()
    {
        var config = new ModConfig();
        try
        {
            var assemblyDir = Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().Location);
            if (assemblyDir is null) return config;

            var configPath = Path.Combine(assemblyDir, "STS2DiscardMod_config.json");
            if (!File.Exists(configPath)) return config;

            using var doc = System.Text.Json.JsonDocument.Parse(File.ReadAllText(configPath));
            var root = doc.RootElement;

            if (root.TryGetProperty("starter_deck_enabled", out var val))
                config.StarterDeckEnabled = val.GetBoolean();
        }
        catch
        {
            // Config read failure is non-fatal; fall back to defaults.
        }
        return config;
    }
}
