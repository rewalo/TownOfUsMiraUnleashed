using AchievementsAPI.API;
using MiraUnleashed.Assets;
using TownOfUs;
using TownOfUs.Assets;
using UnityEngine;

namespace MiraUnleashed.Achievements;

public sealed class MiraUnleashedAchievementsTab : AchievementsTab
{
    /// <inheritdoc/>
    public override string Name => "Mira Unleashed";

    /// <inheritdoc/>
    public override Color GetTabColor() => MiraUnleashedColors.Trapper;

    /// <inheritdoc/>
    public override Sprite GetIcon() => MiraUnleashedAssets.MirageRoleIcon.LoadAsset();

    // General
    public MuBaseAchievement Unleashed { get; set; } = new("Unleashed",
        "Get assigned a Mira Unleashed role.", MiraUnleashedImpAssets.InjectorRole);

    public MuBaseAchievement DoubleJeopardy { get; set; } = new("Double Jeopardy",
        "Start a game with both the Spiteful and Clueless modifiers.", MiraUnleashedAssets.SpitefulModifierIcon, 2,
        true);

    // Forestaller
    public MuBaseAchievement NothingLeftToSabotage { get; set; } = new("Nothing Left to Sabotage",
        "Complete all tasks as Forestaller and lock evildoers out of the sabotage menu.",
        MiraUnleashedAssets.ForestallerRoleIcon);

    public MuBaseAchievement SpeedrunLockdown { get; set; } = new("Speedrun Lockdown",
        "Activate the Forestaller lock before the first meeting of the game.",
        MiraUnleashedAssets.ForestallerRoleIcon, 1);

    // Mirage
    public MuBaseAchievement Gottem { get; set; } = new("Gottem!",
        "Catch someone interacting with your decoy as Mirage.", MiraUnleashedCrewAssets.DecoyButtonSprite);

    public CountAchievement CarnivalOfIllusions { get; set; } = new MuCountAchievement("Carnival of Illusions",
        "Have your decoys triggered 3 times in a single game.", MiraUnleashedAssets.MirageRoleIcon, 3,
        AchPersistence.ThroughoutRounds);

    // Trapper
    public MuBaseAchievement WelcomingCommittee { get; set; } = new("The Welcoming Committee",
        "Snare someone in one of your vent traps as Trapper.", MiraUnleashedAssets.TrapperRoleIcon);

    public CountAchievement PestControl { get; set; } = new MuCountAchievement("Pest Control",
        "Snare 5 players in your vent traps.", MiraUnleashedAssets.TrapperRoleIcon, 5);

    public CountAchievement RedHanded { get; set; } = new MuCountAchievement("Red Handed",
        "Snare 2 players in your vent traps in a single game.", MiraUnleashedAssets.TrapperRoleIcon, 2,
        AchPersistence.ThroughoutRounds, 1);

    // Charlatan
    public MuBaseAchievement BodyDouble { get; set; } = new("Body Double",
        "Fully conceal a body as Charlatan.", MiraUnleashedImpAssets.ConcealButtonSprite);

    public CountAchievement SmokeAndMirrors { get; set; } = new MuCountAchievement("Smoke and Mirrors",
        "Deceive 3 bodies as Charlatan in a single game.", MiraUnleashedImpAssets.DeceiveButtonSprite, 3,
        AchPersistence.ThroughoutRounds);

    // Hacker
    public CountAchievement DdosAttack { get; set; } = new MuCountAchievement("DDoS Attack",
        "Jam players 3 times as Hacker in a single game.", MiraUnleashedImpAssets.HackerJamButtonSprite, 3,
        AchPersistence.ThroughoutRounds);

    public MuBaseAchievement LowPingHighCrime { get; set; } = new("Low Ping, High Crime",
        "Kill a player while your jam is active.", MiraUnleashedImpAssets.HackerRole, 1);

    // Injector
    public MuBaseAchievement Malpractice { get; set; } = new("Malpractice",
        "Inject a player as Injector.", MiraUnleashedImpAssets.InjectorInjectButtonSprite);

    public MuBaseAchievement DoubleDose { get; set; } = new("Double Dose",
        "Inject the same player twice in one game.", MiraUnleashedImpAssets.InjectorRole, 1);

    public CountAchievement ClinicalTrial { get; set; } = new MuCountAchievement("Clinical Trial",
        "Inject 5 players in a single game.", MiraUnleashedImpAssets.InjectorInjectButtonSprite, 5,
        AchPersistence.ThroughoutRounds);

    // Witch
    public MuBaseAchievement HexMarksTheSpot { get; set; } = new("Hex Marks the Spot",
        "Place a spell on a player as Witch.", MiraUnleashedImpAssets.SpellButtonSprite);

    public CountAchievement BoilAndTrouble { get; set; } = new MuCountAchievement("Boil and Trouble",
        "Spell 10 players as Witch.", MiraUnleashedImpAssets.SpellButtonSprite, 10);

    public MuBaseAchievement BubbleBubble { get; set; } = new("Bubble, Bubble",
        "Kill 2 players with your hexes in a single meeting.", MiraUnleashedAssets.HexedSprite, 2, true);

    // Wraith
    public MuBaseAchievement HitTheGroundScreaming { get; set; } = new("Hit the Ground Screaming",
        "Kill a player while dashing as Wraith.", TouImpAssets.SprintSprite, 1);

    public MuBaseAchievement LamplightLethality { get; set; } = new("Lamplight Lethality",
        "Kill a player while invisible next to your lantern.", MiraUnleashedImpAssets.LanternButtonSprite, 2, true);

    public CountAchievement FadeToBlack { get; set; } = new MuCountAchievement("Fade to Black",
        "Kill 5 players as Wraith.", MiraUnleashedImpAssets.WraithRole, 5);

    // Lawyer
    public MuBaseAchievement CourtAppointed { get; set; } = new("Court Appointed",
        "Get assigned a client as Lawyer.", TouRoleIcons.Lawyer);

    public MuBaseAchievement ObjectionOverruled { get; set; } = new("Objection Overruled",
        "Object to the votes 2 times in a single meeting.", MiraUnleashedAssets.ObjectionButtonSprite, 1);

    public MuBaseAchievement CaseClosed { get; set; } = new("Case Closed",
        "Win a game as the Lawyer.", MiraUnleashedAssets.ObjectionButtonSprite, 3, true);

    // Scavenger
    public CountAchievement CleanPlateClub { get; set; } = new MuCountAchievement("Clean Plate Club",
        "Eat 3 bodies in a single game as Scavenger.", MiraUnleashedAssets.ScavengerEatButtonSprite, 3,
        AchPersistence.ThroughoutRounds);

    public CountAchievement BuffetRegular { get; set; } = new MuCountAchievement("Buffet Regular",
        "Eat 10 bodies as Scavenger.", MiraUnleashedAssets.ScavengerRoleIcon, 10);

    public MuBaseAchievement FullBelly { get; set; } = new("Full Belly",
        "Win a game as the Scavenger.", MiraUnleashedAssets.ScavengerRoleIcon, 1);

    // Serial Killer
    public MuBaseAchievement ColdOpen { get; set; } = new("Cold Open",
        "Claim the first kill of the game as Serial Killer.", TouRoleIcons.SerialKiller);

    public CountAchievement SerialSeason { get; set; } = new MuCountAchievement("Serial Season",
        "Kill 10 players as Serial Killer.", TouRoleIcons.SerialKiller, 10);

    public MuBaseAchievement TheLastScene { get; set; } = new("The Last Scene",
        "Win a game as the Serial Killer.", TouRoleIcons.SerialKiller, 2, true);

    // Spiteful
    public MuBaseAchievement PyrrhicVote { get; set; } = new("Pyrrhic Vote",
        "Get cursed after voting for a Spiteful player.", MiraUnleashedAssets.SpitefulModifierIcon);

    public MuBaseAchievement MiseryLovesCompany { get; set; } = new("Misery Loves Company",
        "Get ejected as a Spiteful player and curse 2 or more voters.", MiraUnleashedAssets.SpitefulModifierIcon, 1);

    // Clueless
    public MuBaseAchievement WingIt { get; set; } = new("Wing It",
        "Complete a task as a Clueless player.", MiraUnleashedAssets.CluelessModifierIcon);

    public CountAchievement MuscleMemory { get; set; } = new MuCountAchievement("Muscle Memory",
        "Complete 5 tasks as a Clueless player in a single game.", MiraUnleashedAssets.CluelessModifierIcon, 5,
        AchPersistence.ThroughoutRounds);

    public MuBaseAchievement PureInstinct { get; set; } = new("Pure Instinct",
        "Win a game as a Clueless player.", MiraUnleashedAssets.CluelessModifierIcon, 2, true);

    // Bad luck
    public MuBaseAchievement UnluckyRole { get; set; } = new("Unlucky (Role)",
        "Get assigned the worst role in the game.", TouRoleIcons.Oracle, 0, true);

    public MuBaseAchievement UnluckyModifier { get; set; } = new("Unlucky (Modifier)",
        "Get assigned the worst modifier in the game.", TouModifierIcons.Crewpostor, 0, true);

    public MuBaseAchievement CursedRole { get; set; } = new("Cursed (Role)",
        "Get assigned a role that should never have existed.", TouRoleIcons.Pestilence, 2, true);

    public MuBaseAchievement CursedModifier { get; set; } = new("Cursed (Modifier)",
        "Get assigned the most despicable modifier in the game.", TouModifierIcons.Bait, 2, true);
}
