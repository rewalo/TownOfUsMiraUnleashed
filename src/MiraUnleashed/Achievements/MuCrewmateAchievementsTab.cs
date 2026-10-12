using AchievementsAPI.API;
using MiraUnleashed.Assets;
using UnityEngine;

namespace MiraUnleashed.Achievements;

public sealed class MuCrewmateAchievementsTab : AchievementsTab
{
    /// <inheritdoc/>
    public override string Name => "Mira Unleashed: Crewmate";

    /// <inheritdoc/>
    public override Color GetTabColor() => Palette.CrewmateRoleHeaderBlue;

    /// <inheritdoc/>
    public override Sprite GetIcon() => MiraUnleashedAssets.ForestallerRoleIcon.LoadAsset();

    public MuBaseAchievement NothingLeftToSabotage { get; set; } = new("Nothing Left to Sabotage",
        "Complete all tasks as Forestaller and lock evildoers out of the sabotage menu.",
        MiraUnleashedAssets.ForestallerRoleIcon);

    public MuBaseAchievement SpeedrunLockdown { get; set; } = new("Speedrun Lockdown",
        "Activate the Forestaller lock before the first meeting of the game.",
        MiraUnleashedAssets.ForestallerRoleIcon, 1);

    public MuBaseAchievement Gottem { get; set; } = new("Gottem!",
        "Catch someone interacting with your decoy as Mirage.", MiraUnleashedCrewAssets.DecoyButtonSprite);

    public MuCountAchievement CarnivalOfIllusions { get; set; } = new("Carnival of Illusions",
        "Have your decoys triggered 3 times in a single game.", MiraUnleashedAssets.MirageRoleIcon, 3,
        AchPersistence.ThroughoutRounds);

    public MuBaseAchievement WelcomingCommittee { get; set; } = new("The Welcoming Committee",
        "Snare someone in one of your vent traps as Trapper.", MiraUnleashedAssets.TrapperRoleIcon);

    public MuCountAchievement PestControl { get; set; } = new("Pest Control",
        "Snare 5 players in your vent traps.", MiraUnleashedAssets.TrapperRoleIcon, 5);

    public MuCountAchievement RedHanded { get; set; } = new("Red Handed",
        "Snare 2 players in your vent traps in a single game.", MiraUnleashedAssets.TrapperRoleIcon, 2,
        AchPersistence.ThroughoutRounds, 1);
}
