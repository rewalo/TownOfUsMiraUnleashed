using BepInEx.Configuration;

namespace MiraUnleashed.Achievements;

/// <summary>
/// Achievement progress that needs to outlive the Achievements API itself (played roles for
/// Jack of All Trades). Bound to the plugin config but intentionally not a local setting,
/// so it never shows up in the settings tab.
/// </summary>
internal static class MuAchievementStorage
{
    private static ConfigEntry<string>? _playedRoles;

    private static ConfigEntry<string> PlayedRolesEntry =>
        _playedRoles ??= MiraUnleashedPlugin.Instance.Config.Bind("Achievements", "PlayedRoles", string.Empty);

    public static HashSet<string> GetPlayedRoles() =>
        PlayedRolesEntry.Value
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToHashSet();

    public static int AddPlayedRole(string roleName)
    {
        var roles = GetPlayedRoles();
        if (roles.Add(roleName))
        {
            PlayedRolesEntry.Value = string.Join(',', roles);
        }

        return roles.Count;
    }
}
