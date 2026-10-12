using AchievementsAPI.API;
using MiraUnleashed.Assets;
using UnityEngine;

namespace MiraUnleashed.Achievements;

public sealed class MuModifierAchievementsTab : AchievementsTab
{
    /// <inheritdoc/>
    public override string Name => "Mira Unleashed: Modifiers";

    /// <inheritdoc/>
    public override Color GetTabColor() => new Color32(255, 100, 0, 255);

    /// <inheritdoc/>
    public override Sprite GetIcon() => MiraUnleashedAssets.CluelessModifierIcon.LoadAsset();

    public MuBaseAchievement PyrrhicVote { get; set; } = new("Pyrrhic Vote",
        "Get cursed after voting for a Spiteful player.", MiraUnleashedAssets.SpitefulModifierIcon);

    public MuBaseAchievement MiseryLovesCompany { get; set; } = new("Misery Loves Company",
        "Get ejected as a Spiteful player and curse 2 or more voters.", MiraUnleashedAssets.SpitefulModifierIcon, 1);

    public MuBaseAchievement WingIt { get; set; } = new("Wing It",
        "Complete a task as a Clueless player.", MiraUnleashedAssets.CluelessModifierIcon);

    public MuCountAchievement MuscleMemory { get; set; } = new("Muscle Memory",
        "Complete 5 tasks as a Clueless player in a single game.", MiraUnleashedAssets.CluelessModifierIcon, 5,
        AchPersistence.ThroughoutRounds);

    public MuBaseAchievement PureInstinct { get; set; } = new("Pure Instinct",
        "Win a game as a Clueless player.", MiraUnleashedAssets.CluelessModifierIcon, 2, true);
}
