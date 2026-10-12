using AchievementsAPI.API;
using MiraUnleashed.Assets;
using TownOfUs;
using TownOfUs.Assets;
using UnityEngine;

namespace MiraUnleashed.Achievements;

public sealed class MuNeutralAchievementsTab : AchievementsTab
{
    /// <inheritdoc/>
    public override string Name => "Mira Unleashed: Neutral";

    /// <inheritdoc/>
    public override Color GetTabColor() => TownOfUsColors.Neutral;

    /// <inheritdoc/>
    public override Sprite GetIcon() => TouRoleIcons.SerialKiller.LoadAsset();

    public MuBaseAchievement CourtAppointed { get; set; } = new("Court Appointed",
        "Get assigned a client as Lawyer.", TouRoleIcons.Lawyer);

    public MuBaseAchievement ObjectionOverruled { get; set; } = new("Objection Overruled",
        "Object to the votes 2 times in a single meeting.", MiraUnleashedAssets.ObjectionButtonSprite, 1);

    public MuBaseAchievement CaseClosed { get; set; } = new("Case Closed",
        "Win a game as the Lawyer.", MiraUnleashedAssets.ObjectionButtonSprite, 3, true);

    public MuCountAchievement CleanPlateClub { get; set; } = new("Clean Plate Club",
        "Eat 3 bodies in a single game as Scavenger.", MiraUnleashedAssets.ScavengerEatButtonSprite, 3,
        AchPersistence.ThroughoutRounds);

    public MuCountAchievement BuffetRegular { get; set; } = new("Buffet Regular",
        "Eat 10 bodies as Scavenger.", MiraUnleashedAssets.ScavengerRoleIcon, 10);

    public MuBaseAchievement FullBelly { get; set; } = new("Full Belly",
        "Win a game as the Scavenger.", MiraUnleashedAssets.ScavengerRoleIcon, 1);

    public MuBaseAchievement ColdOpen { get; set; } = new("Cold Open",
        "Claim the first kill of the game as Serial Killer.", TouRoleIcons.SerialKiller);

    public MuCountAchievement SerialSeason { get; set; } = new("Serial Season",
        "Kill 10 players as Serial Killer.", TouRoleIcons.SerialKiller, 10);

    public MuBaseAchievement TheLastScene { get; set; } = new("The Last Scene",
        "Win a game as the Serial Killer.", TouRoleIcons.SerialKiller, 2, true);
}
