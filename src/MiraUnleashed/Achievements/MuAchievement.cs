using System.Runtime.CompilerServices;
using AchievementsAPI.API;
using MiraAPI.Utilities.Assets;
using UnityEngine;

namespace MiraUnleashed.Achievements;

/// <summary>
/// Base achievement class for Mira Unleashed, using MiraAPI loadable assets for icons.
/// </summary>
public class MuBaseAchievement : BaseAchievement
{
    /// <summary>
    /// Creates an achievement whose name and description are resolved through the Mira
    /// locale manager as <c>MiraUnleashed.Achievement.{memberName}.Name/Description</c>
    /// (falling back to the provided text when no key exists).
    /// </summary>
    public MuBaseAchievement(string name, string description, LoadableAsset<Sprite> icon, int rarity = 0,
        bool hidden = false, [CallerMemberName] string memberName = "")
        : base(MuAchievementLocale.Key(memberName, name, "Name"), MuAchievementLocale.Key(memberName, description, "Description"),
            rarity, hidden, false, typeof(MiraUnleashedPlugin).Assembly)
    {
        MiraIcon = icon;
        Id = Assembly.GetName().Name + "_" + name;
    }

    /// <summary>
    /// The achievement's icon through MiraAPI.
    /// </summary>
    public LoadableAsset<Sprite> MiraIcon { get; }

    public override Sprite Icon => MiraIcon.LoadAsset();
}

/// <summary>
/// Achievement class for Mira Unleashed achievements that can increment.
/// </summary>
public class MuCountAchievement : CountAchievement
{
    /// <inheritdoc cref="MuBaseAchievement(string, string, LoadableAsset{Sprite}, int, bool, string)"/>
    public MuCountAchievement(string name, string description, LoadableAsset<Sprite> icon, int requiredValue,
        AchPersistence persistence = AchPersistence.ThroughoutSessions, int rarity = 0, bool hidden = false,
        [CallerMemberName] string memberName = "")
        : base(MuAchievementLocale.Key(memberName, name, "Name"), MuAchievementLocale.Key(memberName, description, "Description"),
            0, requiredValue, persistence, rarity, hidden)
    {
        MiraIcon = icon;
        Id = Assembly.GetName().Name + "_" + name;
    }

    /// <summary>
    /// The achievement's icon through MiraAPI.
    /// </summary>
    public LoadableAsset<Sprite> MiraIcon { get; }

    public override Sprite Icon => MiraIcon.LoadAsset();
}

internal static class MuAchievementLocale
{
    /// <summary>
    /// Builds a translatable <see cref="StringNames"/> keyed
    /// <c>MiraUnleashed.Achievement.{memberName}.{part}</c>. When no member name is known
    /// (non-property call sites), the display text itself is used as the key so English
    /// still resolves correctly.
    /// </summary>
    public static StringNames Key(string memberName, string text, string part) =>
        MiraLocaleManager.GetOrCreateLocaleString(memberName.Length == 0 ? text : $"MiraUnleashed.Achievement.{memberName}.{part}");
}
