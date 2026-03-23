// Stub attribute for [ModInitializer] - used during development without sts2.dll.
// At runtime, STS2 uses the REAL ModInitializerAttribute from MegaCrit.Sts2.Core.Modding.
//
// HOW TO USE THE REAL API:
//   1. Run: Get-ChildItem "D:\G_games\steam\steamapps\common\Slay the Spire 2" -Filter "sts2.dll" -Recurse
//   2. Copy sts2.dll to this project's lib/ folder
//   3. Update STS2_Discard_Mod.csproj to reference it
//   4. Delete this file and add: using MegaCrit.Sts2.Core.Modding;
//
// The attribute signature is:
//   [ModInitializer("StaticMethodName")]
//   public static class YourModClass { public static void StaticMethodName() { ... } }

using System;

// Only define stub when sts2.dll is not available
#if !STS2_AVAILABLE

/// <summary>
/// Stub for MegaCrit.Sts2.Core.Modding.ModInitializerAttribute.
/// Allows compilation without sts2.dll. The real attribute must be used at runtime.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
internal sealed class ModInitializerAttribute : Attribute
{
    public string EntryMethodName { get; }

    public ModInitializerAttribute(string entryMethodName)
    {
        EntryMethodName = entryMethodName;
    }
}

#endif
