using AchievementsAPI.API;
using MiraUnleashed.Assets;
using UnityEngine;

namespace MiraUnleashed.Achievements;

public sealed class MuGeneralAchievementsTab : AchievementsTab
{
    /// <inheritdoc/>
    public override string Name => "Mira Unleashed: General";

    /// <inheritdoc/>
    public override Color GetTabColor() => MiraUnleashedColors.Trapper;

    /// <inheritdoc/>
    public override Sprite GetIcon() => MiraUnleashedAssets.MirageRoleIcon.LoadAsset();

    public MuBaseAchievement Unleashed { get; set; } = new("Unleashed",
        "Get assigned a Mira Unleashed role.", MiraUnleashedImpAssets.InjectorRole);

    public MuBaseAchievement DoubleJeopardy { get; set; } = new("Double Jeopardy",
        "Start a game with both the Spiteful and Clueless modifiers.", MiraUnleashedAssets.SpitefulModifierIcon, 2,
        true);
}
