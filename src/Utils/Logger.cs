// Logger wraps MegaCrit.Sts2.Core.Logging.Log
// Once sts2.dll is referenced, replace Console.WriteLine with:
//   global::MegaCrit.Sts2.Core.Logging.Log.Info(...);
//   global::MegaCrit.Sts2.Core.Logging.Log.Warn(...);
//   global::MegaCrit.Sts2.Core.Logging.Log.Error(...);

using System;

namespace DiscardMod.Utils;

/// <summary>
/// Logging wrapper for STS2 mod.
/// Uses MegaCrit.Sts2.Core.Logging.Log when available (sts2.dll).
/// Falls back to Console.WriteLine for compilation without sts2.dll.
/// </summary>
public static class Logger
{
    private const string PREFIX = "[DiscardMod]";

    public static void Log(string message)
    {
        // TODO: Replace with Log.Info($"{PREFIX} {message}"); once sts2.dll is referenced
        Console.WriteLine($"{PREFIX} {message}");
    }

    public static void LogWarning(string message)
    {
        // TODO: Replace with Log.Warn($"{PREFIX} {message}"); once sts2.dll is referenced
        Console.WriteLine($"{PREFIX} WARN: {message}");
    }

    /// <summary>
    /// Log error message.
    /// </summary>
    public static void LogError(string message)
    {
        Console.Error.WriteLine($"{PREFIX} ERROR: {message}");
    }

    /// <summary>
    /// Log exception with full stack trace.
    /// </summary>
    public static void LogException(Exception ex)
    {
        Console.Error.WriteLine($"{PREFIX} EXCEPTION: {ex.Message}\n{ex.StackTrace}");
    }
}
