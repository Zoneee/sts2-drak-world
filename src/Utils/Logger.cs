using System;

namespace DiscardMod.Utils;

/// <summary>
/// Centralized logging utility with [DiscardMod] prefix for console output.
/// All mod logs use this to enable easy filtering in STS2 console.
/// 
/// NOTE: Will be updated to use Unity.Debug when integrated with STS2.
/// Currently uses System.Console for build-time compilation.
/// </summary>
public static class Logger
{
    private const string PREFIX = "[DiscardMod]";

    /// <summary>
    /// Log informational message.
    /// </summary>
    public static void Log(string message)
    {
        Console.WriteLine($"{PREFIX} {message}");
    }

    /// <summary>
    /// Log warning message.
    /// </summary>
    public static void LogWarning(string message)
    {
        Console.WriteLine($"{PREFIX} WARNING: {message}");
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
