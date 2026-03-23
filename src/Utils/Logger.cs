namespace DiscardMod.Utils;

/// <summary>
/// Convenience wrapper — forwards to the mod's MegaCrit Logger instance.
/// Use DiscardModMain.Logger directly for richer API (Debug, Warn, Error).
/// </summary>
public static class Logger
{
    public static void Log(string message) => DiscardModMain.Logger.Info(message);
    public static void LogWarning(string message) => DiscardModMain.Logger.Warn(message);
    public static void LogError(string message) => DiscardModMain.Logger.Error(message);
}
