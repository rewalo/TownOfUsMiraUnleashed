using AchievementsAPI.API;
using MiraAPI.Utilities.Assets;
using UnityEngine;

namespace MiraUnleashed.Achievements;

/// <summary>
/// Base achievement class for Mira Unleashed, using MiraAPI loadable assets for icons.
/// </summary>
public class MuBaseAchievement : BaseAchievement
{
    public MuBaseAchievement(string name, string description, LoadableAsset<Sprite> icon, int rarity = 0,
        bool hidden = false)
        : base(name, description, rarity, hidden, false, typeof(MiraUnleashedPlugin).Assembly)
    {
        MiraIcon = icon;
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
    public MuCountAchievement(string name, string description, LoadableAsset<Sprite> icon, int requiredValue,
        AchPersistence persistence = AchPersistence.ThroughoutSessions, int rarity = 0, bool hidden = false)
        : base(name, description, 0, requiredValue, persistence, rarity, hidden)
    {
        MiraIcon = icon;
    }

    /// <summary>
    /// The achievement's icon through MiraAPI.
    /// </summary>
    public LoadableAsset<Sprite> MiraIcon { get; }

    public override Sprite Icon => MiraIcon.LoadAsset();
}
