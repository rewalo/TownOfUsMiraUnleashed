using MiraAPI.Utilities.Assets;
using MiraUnleashed.Assets;
using UnityEngine;

namespace MiraUnleashed.Achievements;

/// <summary>
/// Which achievements tab an achievement shows up in.
/// </summary>
internal enum MuAchievementTab
{
    General,
    Crewmate,
    Impostor,
    Neutral,
    Modifiers,
}

/// <summary>
/// How the progress of a counting achievement persists. Mirrors AchievementsAPI's
/// persistence semantics without referencing the (optional) assembly.
/// </summary>
internal enum MuAchievementPersistence
{
    ThroughoutSessions,
    ThroughoutRounds,
    ResetOnRoundStart,
    ResetOnMeetingStart,
}

/// <summary>
/// Declarative description of an achievement. Purely managed data with no references
/// to the optional Achievements API, so it is always safe to load.
/// </summary>
internal sealed record MuAchievementDefinition(
    string Name,
    string Description,
    MuAchievementTab Tab,
    Func<Sprite> Icon,
    int RequiredCount = 0,
    MuAchievementPersistence Persistence = MuAchievementPersistence.ThroughoutSessions,
    int Rarity = 0,
    bool Hidden = false)
{
    public bool IsCounted => RequiredCount > 0;
}

/// <summary>
/// Achievement keys and definitions for every Mira Unleashed achievement, grouped by tab.
/// </summary>
internal static class MuAchievements
{
    private static Sprite Load(LoadableAsset<Sprite> asset) => asset.LoadAsset();

    // General
    public const string Unleashed = "Unleashed";
    public const string DoubleJeopardy = "Double Jeopardy";

    // Forestaller
    public const string NothingLeftToSabotage = "Nothing Left to Sabotage";
    public const string SpeedrunLockdown = "Speedrun Lockdown";

    // Mirage
    public const string Gottem = "Gottem!";
    public const string CarnivalOfIllusions = "Carnival of Illusions";

    // Trapper
    public const string WelcomingCommittee = "The Welcoming Committee";
    public const string PestControl = "Pest Control";
    public const string RedHanded = "Red Handed";

    // Charlatan
    public const string BodyDouble = "Body Double";
    public const string SmokeAndMirrors = "Smoke and Mirrors";

    // Hacker
    public const string DdosAttack = "DDoS Attack";
    public const string LowPingHighCrime = "Low Ping, High Crime";

    // Injector
    public const string Malpractice = "Malpractice";
    public const string DoubleDose = "Double Dose";
    public const string ClinicalTrial = "Clinical Trial";

    // Witch
    public const string HexMarksTheSpot = "Hex Marks the Spot";
    public const string BoilAndTrouble = "Boil and Trouble";
    public const string BubbleBubble = "Bubble, Bubble";

    // Wraith
    public const string HitTheGroundScreaming = "Hit the Ground Screaming";
    public const string LamplightLethality = "Lamplight Lethality";
    public const string FadeToBlack = "Fade to Black";

    // Lawyer
    public const string CourtAppointed = "Court Appointed";
    public const string ObjectionOverruled = "Objection Overruled";
    public const string CaseClosed = "Case Closed";

    // Scavenger
    public const string CleanPlateClub = "Clean Plate Club";
    public const string BuffetRegular = "Buffet Regular";
    public const string FullBelly = "Full Belly";

    // Serial Killer
    public const string ColdOpen = "Cold Open";
    public const string SerialSeason = "Serial Season";
    public const string TheLastScene = "The Last Scene";

    // Spiteful
    public const string PyrrhicVote = "Pyrrhic Vote";
    public const string MiseryLovesCompany = "Misery Loves Company";

    // Clueless
    public const string WingIt = "Wing It";
    public const string MuscleMemory = "Muscle Memory";
    public const string PureInstinct = "Pure Instinct";

    public static readonly IReadOnlyList<MuAchievementDefinition> All =
    [
        // General
        new(Unleashed, "Get assigned a Mira Unleashed role.", MuAchievementTab.General,
            () => Load(MiraUnleashedImpAssets.InjectorRole)),
        new(DoubleJeopardy, "Start a game with both the Spiteful and Clueless modifiers.", MuAchievementTab.General,
            () => Load(MiraUnleashedAssets.SpitefulModifierIcon), Rarity: 2, Hidden: true),

        // Crewmate - Forestaller
        new(NothingLeftToSabotage, "Complete all tasks as Forestaller and lock evildoers out of the sabotage menu.",
            MuAchievementTab.Crewmate, () => Load(MiraUnleashedAssets.ForestallerRoleIcon)),
        new(SpeedrunLockdown, "Activate the Forestaller lock before the first meeting of the game.",
            MuAchievementTab.Crewmate, () => Load(MiraUnleashedAssets.ForestallerRoleIcon), Rarity: 1),

        // Crewmate - Mirage
        new(Gottem, "Catch someone interacting with your decoy as Mirage.", MuAchievementTab.Crewmate,
            () => Load(MiraUnleashedCrewAssets.DecoyButtonSprite)),
        new(CarnivalOfIllusions, "Have your decoys triggered 3 times in a single game.", MuAchievementTab.Crewmate,
            () => Load(MiraUnleashedAssets.MirageRoleIcon), 3, MuAchievementPersistence.ThroughoutRounds),

        // Crewmate - Trapper
        new(WelcomingCommittee, "Snare someone in one of your vent traps as Trapper.", MuAchievementTab.Crewmate,
            () => Load(MiraUnleashedAssets.TrapperRoleIcon)),
        new(PestControl, "Snare 5 players in your vent traps.", MuAchievementTab.Crewmate,
            () => Load(MiraUnleashedAssets.TrapperRoleIcon), 5),
        new(RedHanded, "Snare 2 players in your vent traps in a single game.", MuAchievementTab.Crewmate,
            () => Load(MiraUnleashedAssets.TrapperRoleIcon), 2, MuAchievementPersistence.ThroughoutRounds, Rarity: 1),

        // Impostor - Charlatan
        new(BodyDouble, "Fully conceal a body as Charlatan.", MuAchievementTab.Impostor,
            () => Load(MiraUnleashedImpAssets.ConcealButtonSprite)),
        new(SmokeAndMirrors, "Deceive 3 bodies as Charlatan in a single game.", MuAchievementTab.Impostor,
            () => Load(MiraUnleashedImpAssets.DeceiveButtonSprite), 3, MuAchievementPersistence.ThroughoutRounds),

        // Impostor - Hacker
        new(DdosAttack, "Jam players 3 times as Hacker in a single game.", MuAchievementTab.Impostor,
            () => Load(MiraUnleashedImpAssets.HackerJamButtonSprite), 3, MuAchievementPersistence.ThroughoutRounds),
        new(LowPingHighCrime, "Kill a player while your jam is active.", MuAchievementTab.Impostor,
            () => Load(MiraUnleashedImpAssets.HackerRole), Rarity: 1),

        // Impostor - Injector
        new(Malpractice, "Inject a player as Injector.", MuAchievementTab.Impostor,
            () => Load(MiraUnleashedImpAssets.InjectorInjectButtonSprite)),
        new(DoubleDose, "Inject the same player twice in one game.", MuAchievementTab.Impostor,
            () => Load(MiraUnleashedImpAssets.InjectorRole), Rarity: 1),
        new(ClinicalTrial, "Inject 5 players in a single game.", MuAchievementTab.Impostor,
            () => Load(MiraUnleashedImpAssets.InjectorInjectButtonSprite), 5, MuAchievementPersistence.ThroughoutRounds),

        // Impostor - Witch
        new(HexMarksTheSpot, "Place a spell on a player as Witch.", MuAchievementTab.Impostor,
            () => Load(MiraUnleashedImpAssets.SpellButtonSprite)),
        new(BoilAndTrouble, "Spell 10 players as Witch.", MuAchievementTab.Impostor,
            () => Load(MiraUnleashedImpAssets.SpellButtonSprite), 10),
        new(BubbleBubble, "Kill 2 players with your hexes in a single meeting.", MuAchievementTab.Impostor,
            () => Load(MiraUnleashedAssets.HexedSprite), Rarity: 2, Hidden: true),

        // Impostor - Wraith
        new(HitTheGroundScreaming, "Kill a player while dashing as Wraith.", MuAchievementTab.Impostor,
            () => Load(TownOfUs.Assets.TouImpAssets.SprintSprite), Rarity: 1),
        new(LamplightLethality, "Kill a player while invisible next to your lantern.", MuAchievementTab.Impostor,
            () => Load(MiraUnleashedImpAssets.LanternButtonSprite), Rarity: 2, Hidden: true),
        new(FadeToBlack, "Kill 5 players as Wraith.", MuAchievementTab.Impostor,
            () => Load(MiraUnleashedImpAssets.WraithRole), 5),

        // Neutral - Lawyer
        new(CourtAppointed, "Get assigned a client as Lawyer.", MuAchievementTab.Neutral,
            () => Load(TownOfUs.Assets.TouRoleIcons.Lawyer)),
        new(ObjectionOverruled, "Object to the votes 2 times in a single meeting.", MuAchievementTab.Neutral,
            () => Load(MiraUnleashedAssets.ObjectionButtonSprite), Rarity: 1),
        new(CaseClosed, "Win a game as the Lawyer.", MuAchievementTab.Neutral,
            () => Load(MiraUnleashedAssets.ObjectionButtonSprite), Rarity: 3, Hidden: true),

        // Neutral - Scavenger
        new(CleanPlateClub, "Eat 3 bodies in a single game as Scavenger.", MuAchievementTab.Neutral,
            () => Load(MiraUnleashedAssets.ScavengerEatButtonSprite), 3, MuAchievementPersistence.ThroughoutRounds),
        new(BuffetRegular, "Eat 10 bodies as Scavenger.", MuAchievementTab.Neutral,
            () => Load(MiraUnleashedAssets.ScavengerRoleIcon), 10),
        new(FullBelly, "Win a game as the Scavenger.", MuAchievementTab.Neutral,
            () => Load(MiraUnleashedAssets.ScavengerRoleIcon), Rarity: 1),

        // Neutral - Serial Killer
        new(ColdOpen, "Claim the first kill of the game as Serial Killer.", MuAchievementTab.Neutral,
            () => Load(TownOfUs.Assets.TouRoleIcons.SerialKiller)),
        new(SerialSeason, "Kill 10 players as Serial Killer.", MuAchievementTab.Neutral,
            () => Load(TownOfUs.Assets.TouRoleIcons.SerialKiller), 10),
        new(TheLastScene, "Win a game as the Serial Killer.", MuAchievementTab.Neutral,
            () => Load(TownOfUs.Assets.TouRoleIcons.SerialKiller), Rarity: 2, Hidden: true),

        // Modifiers - Spiteful
        new(PyrrhicVote, "Get cursed after voting for a Spiteful player.", MuAchievementTab.Modifiers,
            () => Load(MiraUnleashedAssets.SpitefulModifierIcon)),
        new(MiseryLovesCompany, "Get ejected as a Spiteful player and curse 2 or more voters.", MuAchievementTab.Modifiers,
            () => Load(MiraUnleashedAssets.SpitefulModifierIcon), Rarity: 1),

        // Modifiers - Clueless
        new(WingIt, "Complete a task as a Clueless player.", MuAchievementTab.Modifiers,
            () => Load(MiraUnleashedAssets.CluelessModifierIcon)),
        new(MuscleMemory, "Complete 5 tasks as a Clueless player in a single game.", MuAchievementTab.Modifiers,
            () => Load(MiraUnleashedAssets.CluelessModifierIcon), 5, MuAchievementPersistence.ThroughoutRounds),
        new(PureInstinct, "Win a game as a Clueless player.", MuAchievementTab.Modifiers,
            () => Load(MiraUnleashedAssets.CluelessModifierIcon), Rarity: 2, Hidden: true),
    ];

}
