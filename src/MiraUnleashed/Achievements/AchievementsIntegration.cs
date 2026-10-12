using System.Runtime.CompilerServices;
using BepInEx.Unity.IL2CPP;

namespace MiraUnleashed.Achievements;

/// <summary>
/// Optional Achievements API entry point. Every method touching AchievementsAPI types is
/// kept behind a no-inline bridge so Mira Unleashed loads cleanly when the Achievements
/// API mod is not installed (the reference is compile-time only; the assembly is neither
/// shipped nor required at runtime).
/// </summary>
public static class AchievementsIntegration
{
    public static bool IsActive => IL2CPPChainloader.Instance.Plugins.ContainsKey(AchievementsAPI.AchievementsAPIPlugin.Id);

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void Initialize()
    {
        if (!IsActive)
        {
            return;
        }

        AchievementsRuntime.Initialize();
        Info("Achievements API detected, Mira Unleashed achievements registered.");
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void Unlock(string key)
    {
        if (IsActive)
        {
            AchievementsRuntime.Unlock(key);
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void Increment(string key)
    {
        if (IsActive)
        {
            AchievementsRuntime.Increment(key);
        }
    }
}
