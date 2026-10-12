using AchievementsAPI.API;
using MiraUnleashed.Assets;
using UnityEngine;

namespace MiraUnleashed.Achievements;

/// <summary>
/// Thin typed layer over the Achievements API. THIS CLASS MUST ONLY BE USED THROUGH
/// <see cref="AchievementsIntegration"/>: every type referenced here lives in the optional
/// AchievementsAPI assembly, so nothing may trigger loading this class (or JITing its
/// members) while the mod is absent.
/// </summary>
internal static class AchievementsRuntime
{
    private static readonly Dictionary<string, BaseAchievement> AchievementMap = new(StringComparer.Ordinal);
    private static readonly List<string> TabNames = [];
    private static readonly List<Color> TabColors = [];
    private static readonly List<Func<Sprite>> TabIcons = [];

    private static bool _initialized;

    // Entry points for the dynamically emitted tab types (see DynamicTabFactory).
    public static string GetTabName(int tabIndex) => TabNames[tabIndex];

    public static Color GetTabColor(int tabIndex) => TabColors[tabIndex];

    public static Sprite GetTabIcon(int tabIndex) => TabIcons[tabIndex]();

    /// <summary>
    /// Builds and registers the Mira Unleashed achievement tabs. Called at plugin load when
    /// the Achievements API is installed.
    /// </summary>
    public static void Initialize()
    {
        if (_initialized)
        {
            return;
        }

        _initialized = true;

        RegisterTab("Mira Unleashed: General", MiraUnleashedColors.Trapper,
            () => MiraUnleashedAssets.MirageRoleIcon.LoadAsset());
        RegisterTab("Mira Unleashed: Crewmate", Palette.CrewmateRoleHeaderBlue,
            () => MiraUnleashedAssets.ForestallerRoleIcon.LoadAsset());
        RegisterTab("Mira Unleashed: Impostor", Palette.ImpostorRoleHeaderRed,
            () => MiraUnleashedImpAssets.CharlatanRole.LoadAsset());
        RegisterTab("Mira Unleashed: Neutral", TownOfUs.TownOfUsColors.Neutral,
            () => TownOfUs.Assets.TouRoleIcons.SerialKiller.LoadAsset());
        RegisterTab("Mira Unleashed: Modifiers", new Color32(255, 100, 0, 255),
            () => MiraUnleashedAssets.CluelessModifierIcon.LoadAsset());
    }

    private static void RegisterTab(string name, Color color, Func<Sprite> icon)
    {
        var tabIndex = TabNames.Count;
        TabNames.Add(name);
        TabColors.Add(color);
        TabIcons.Add(icon);

        var kind = (MuAchievementTab)tabIndex;
        var achievements = new List<(string PropName, BaseAchievement Achievement)>();

        foreach (var def in MuAchievements.All)
        {
            if (def.Tab != kind)
            {
                continue;
            }

            var achievement = CreateAchievement(def);
            AchievementMap[def.Name] = achievement;

            // Property names must be valid identifiers.
            achievements.Add((def.Name.Replace(" ", string.Empty).Replace(",", string.Empty), achievement));
        }

        var tabInstance = DynamicTabFactory.CreateTab(tabIndex, achievements);
        AchievementsManager.Tabs.Add(tabInstance);
        Info($"Registered '{name}' achievements tab with {achievements.Count} achievements.");
    }

    private static BaseAchievement CreateAchievement(MuAchievementDefinition def)
    {
        return def.IsCounted
            ? new CountAchievement(def.Name, def.Description, def.Icon(), 0, def.RequiredCount,
                (AchPersistence)(int)def.Persistence, def.Rarity, def.Hidden)
            : new BaseAchievement(def.Name, def.Description, def.Icon(), def.Rarity, def.Hidden,
                assembly: typeof(MiraUnleashedPlugin).Assembly);
    }

    public static void Unlock(string key)
    {
        if (AchievementMap.TryGetValue(key, out var achievement) && !achievement.Unlocked)
        {
            achievement.Unlock();
        }
    }

    public static void Increment(string key)
    {
        if (AchievementMap.TryGetValue(key, out var achievement) && achievement is CountAchievement count)
        {
            count.Increment(1);
        }
    }
}
