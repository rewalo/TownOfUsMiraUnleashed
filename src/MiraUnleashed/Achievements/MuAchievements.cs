using System.Reflection;
using System.Runtime.CompilerServices;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using MiraUnleashed.Assets;
using TownOfUs.Achievements;
using TownOfUs.Assets;
using UnityEngine;

namespace MiraUnleashed.Achievements;

/// <summary>
/// Mira Unleashed's achievement tab descriptor.
/// </summary>
internal static class MuAchievementGroups
{
    private static readonly Assembly MuAssembly = typeof(MiraUnleashedPlugin).Assembly;

    public static readonly AchievementTabSpec Tab = new(
        MuAssembly, "Mira Unleashed", "Mira Unleashed", "general",
        TabLocaleKey: "MiraUnleashed.Achievements.Tab",
        LocalePrefix: "MiraUnleashed.Achievement",
        TabIcon: MiraUnleashedAssets.MirageRoleIcon, ModIcon: MiraUnleashedAssets.MirageRoleIcon,
        TabColor: MiraUnleashedColors.Trapper);
}

/// <summary>
/// All Mira Unleashed achievements as data. The <see cref="Key"/> matches both the old tab property
/// names (so locale keys are unchanged) and the static member names via
/// <see cref="CallerMemberNameAttribute"/>.
/// </summary>
public static class MuAchievements
{
    private static AchievementSpec Define(string name, string description, LoadableAsset<Sprite> icon,
        LoadableAsset<Sprite>? subIcon = null, int rarity = 0, bool hidden = false, int progressGoal = 0,
        AchievementPersistence persistence = AchievementPersistence.ThroughoutSessions,
        bool toastOnGameEnd = false, string? categoryOverride = null, string? idOverride = null,
        [CallerMemberName] string key = "")
        => new(key, name, description, icon, subIcon, rarity, hidden, progressGoal, persistence, toastOnGameEnd);

    // General
    public static readonly AchievementSpec Unleashed = Define("Unleashed",
        "Get assigned a Mira Unleashed role.", MiraUnleashedImpAssets.InjectorRole);
    public static readonly AchievementSpec DoubleJeopardy = Define("Double Jeopardy",
        "Start a game with both the Spiteful and Clueless modifiers.", MiraUnleashedAssets.SpitefulModifierIcon,
        rarity: 1, hidden: true);

    // Forestaller
    public static readonly AchievementSpec NothingLeftToSabotage = Define("Nothing Left to Sabotage",
        "Complete all tasks as Forestaller and lock evildoers out of the sabotage menu.",
        MiraUnleashedAssets.ForestallerRoleIcon, categoryOverride: "crewmate");
    public static readonly AchievementSpec SpeedrunLockdown = Define("Speedrun Lockdown",
        "Activate the Forestaller lock before the first meeting of the game.",
        MiraUnleashedAssets.ForestallerRoleIcon, rarity: 1, categoryOverride: "crewmate");
    public static readonly AchievementSpec UnderPressure = Define("Under Pressure",
        "Complete all tasks as Forestaller while a Hex Bomb countdown is active.",
        MiraUnleashedAssets.ForestallerRoleIcon, rarity: 2, categoryOverride: "crewmate");
    public static readonly AchievementSpec EleventhHour = Define("Eleventh Hour",
        "Activate the Forestaller lock while a critical sabotage is counting down.",
        MiraUnleashedAssets.ForestallerRoleIcon, rarity: 2, categoryOverride: "crewmate");

    // Mirage
    public static readonly AchievementSpec Gottem = Define("Gottem!",
        "Catch someone interacting with your decoy as Mirage.", MiraUnleashedCrewAssets.DecoyButtonSprite,
        categoryOverride: "crewmate");
    public static readonly AchievementSpec CarnivalOfIllusions = Define("Carnival of Illusions",
        "Have your decoys triggered 3 times in a single game.", MiraUnleashedAssets.MirageRoleIcon,
        progressGoal: 3, persistence: AchievementPersistence.ThroughoutRounds, rarity: 1,
        categoryOverride: "crewmate");
    public static readonly AchievementSpec MirrorMatch = Define("Mirror Match",
        "Trigger a decoy on a player who is currently morphed.", MiraUnleashedAssets.MirageRoleIcon,
        rarity: 2, toastOnGameEnd: true, categoryOverride: "crewmate");

    // Trapper
    public static readonly AchievementSpec WelcomingCommittee = Define("The Welcoming Committee",
        "Snare someone in one of your vent traps as Trapper.", MiraUnleashedAssets.TrapperRoleIcon,
        categoryOverride: "crewmate");
    public static readonly AchievementSpec PestControl = Define("Pest Control",
        "Snare 5 players in your vent traps.", MiraUnleashedAssets.TrapperRoleIcon,
        progressGoal: 5, persistence: AchievementPersistence.ThroughoutSessions, rarity: 1,
        categoryOverride: "crewmate");
    public static readonly AchievementSpec RedHanded = Define("Red Handed",
        "Snare 2 players in your vent traps in a single game.", MiraUnleashedAssets.TrapperRoleIcon,
        progressGoal: 2, persistence: AchievementPersistence.ThroughoutRounds, rarity: 1,
        categoryOverride: "crewmate");
    public static readonly AchievementSpec CaughtInTheAct = Define("Caught in the Act",
        "Snare a player who was invisible when they entered your trapped vent.",
        MiraUnleashedAssets.TrapperRoleIcon, rarity: 2, toastOnGameEnd: true,
        categoryOverride: "crewmate");
    public static readonly AchievementSpec CaughtAndConvicted = Define("Caught and Convicted",
        "Have a player you snared get ejected in the very next meeting.",
        MiraUnleashedAssets.TrapperRoleIcon, rarity: 1, categoryOverride: "crewmate");

    // Charlatan
    public static readonly AchievementSpec BodyDouble = Define("Body Double",
        "Fully conceal a body as Charlatan.", MiraUnleashedImpAssets.ConcealButtonSprite,
        categoryOverride: "impostor");
    public static readonly AchievementSpec SmokeAndMirrors = Define("Smoke and Mirrors",
        "Deceive 3 bodies as Charlatan in a single game.", MiraUnleashedImpAssets.DeceiveButtonSprite,
        progressGoal: 3, persistence: AchievementPersistence.ThroughoutRounds, rarity: 1,
        categoryOverride: "impostor");
    public static readonly AchievementSpec Framed = Define("Framed",
        "Deceive-report a body and have a different player ejected in that meeting.",
        MiraUnleashedImpAssets.DeceiveButtonSprite, rarity: 2, categoryOverride: "impostor");

    // Hacker
    public static readonly AchievementSpec DdosAttack = Define("DDoS Attack",
        "Jam players 3 times as Hacker in a single game.", MiraUnleashedImpAssets.HackerJamButtonSprite,
        progressGoal: 3, persistence: AchievementPersistence.ThroughoutRounds, rarity: 1,
        categoryOverride: "impostor");
    public static readonly AchievementSpec LowPingHighCrime = Define("Low Ping, High Crime",
        "Kill a player while your jam is active.", MiraUnleashedImpAssets.HackerRole, rarity: 1,
        categoryOverride: "impostor");
    public static readonly AchievementSpec Blackout = Define("Blackout",
        "Activate Jam while lights are sabotaged.", MiraUnleashedImpAssets.HackerJamButtonSprite, rarity: 1,
        categoryOverride: "impostor");
    public static readonly AchievementSpec BackdoorAccess = Define("Backdoor Access",
        "Activate Jam while a Forestaller has locked out sabotages.", MiraUnleashedImpAssets.HackerRole,
        rarity: 2, categoryOverride: "impostor");

    // Injector
    public static readonly AchievementSpec Malpractice = Define("Malpractice",
        "Inject a player as Injector.", MiraUnleashedImpAssets.InjectorInjectButtonSprite,
        categoryOverride: "impostor");
    public static readonly AchievementSpec DoubleDose = Define("Double Dose",
        "Inject the same player twice in one game.", MiraUnleashedImpAssets.InjectorRole, rarity: 1,
        categoryOverride: "impostor");
    public static readonly AchievementSpec ClinicalTrial = Define("Clinical Trial",
        "Inject 5 players in a single game.", MiraUnleashedImpAssets.InjectorInjectButtonSprite,
        progressGoal: 5, persistence: AchievementPersistence.ThroughoutRounds, rarity: 1,
        categoryOverride: "impostor");
    public static readonly AchievementSpec TripleThreat = Define("Triple Threat",
        "Inject the same player three times in one game.", MiraUnleashedImpAssets.InjectorRole, rarity: 2,
        categoryOverride: "impostor");
    public static readonly AchievementSpec BadMedicine = Define("Bad Medicine",
        "Inject a player who is under a Witch's spell.", MiraUnleashedImpAssets.InjectorInjectButtonSprite,
        rarity: 1, categoryOverride: "impostor");
    public static readonly AchievementSpec LabRat = Define("Lab Rat",
        "Have a player you injected get killed by a teammate before the next meeting.",
        MiraUnleashedImpAssets.InjectorRole, rarity: 1, categoryOverride: "impostor");

    // Witch
    public static readonly AchievementSpec MarkedForDeath = Define("Marked for Death",
        "Place a spell on a player as Witch.", MiraUnleashedImpAssets.SpellButtonSprite,
        categoryOverride: "impostor", idOverride: "MiraUnleashed_Hex Marks the Spot");
    public static readonly AchievementSpec PracticedHand = Define("Practiced Hand",
        "Place spells on 10 players as Witch.", MiraUnleashedImpAssets.SpellButtonSprite,
        progressGoal: 10, persistence: AchievementPersistence.ThroughoutRounds, rarity: 1,
        categoryOverride: "impostor", idOverride: "MiraUnleashed_Boil and Trouble");
    public static readonly AchievementSpec TwinSacrifice = Define("Twin Sacrifice",
        "Kill 2 players with your hexes in a single meeting.", MiraUnleashedAssets.HexedSprite, rarity: 2,
        toastOnGameEnd: true, categoryOverride: "impostor", idOverride: "MiraUnleashed_Bubble, Bubble");
    public static readonly AchievementSpec FullCoven = Define("Full Coven",
        "Have 5 players under your spell at the same time.", MiraUnleashedAssets.HexedSprite, rarity: 2,
        categoryOverride: "impostor");
    public static readonly AchievementSpec DoubleHex = Define("Double Hex",
        "Cast a spell on a player already hexed by a Spellslinger.", MiraUnleashedAssets.HexedSprite,
        rarity: 2, toastOnGameEnd: true, categoryOverride: "impostor");

    // Wraith
    public static readonly AchievementSpec HitTheGroundScreaming = Define("Hit the Ground Screaming",
        "Kill a player while dashing as Wraith.", TouImpAssets.SprintSprite, rarity: 1,
        categoryOverride: "impostor");
    public static readonly AchievementSpec LamplightLethality = Define("Lamplight Lethality",
        "Kill a player while invisible next to your lantern.", MiraUnleashedImpAssets.LanternButtonSprite, rarity: 1,
        categoryOverride: "impostor");
    public static readonly AchievementSpec FadeToBlack = Define("Fade to Black",
        "Kill 5 players as Wraith.", MiraUnleashedImpAssets.WraithRole, progressGoal: 5,
        persistence: AchievementPersistence.ThroughoutSessions, rarity: 1, categoryOverride: "impostor");
    public static readonly AchievementSpec LanternGuided = Define("Lantern Guided",
        "Teleport back to your lantern and kill a player within 3 seconds.",
        MiraUnleashedImpAssets.LanternButtonSprite, rarity: 2, categoryOverride: "impostor");

    // Lawyer
    public static readonly AchievementSpec CourtAppointed = Define("Court Appointed",
        "Get assigned a client as Lawyer.", TouRoleIcons.Lawyer, categoryOverride: "neutral");
    public static readonly AchievementSpec ObjectionSustained = Define("Objection Sustained",
        "Object to the votes while your client has at least one vote on them.",
        MiraUnleashedAssets.ObjectionButtonSprite, rarity: 1, toastOnGameEnd: true,
        categoryOverride: "neutral");
    public static readonly AchievementSpec DevilsAdvocate = Define("Devil's Advocate",
        "Win as Lawyer while your client is an Impostor or Neutral Killer.", TouRoleIcons.Lawyer,
        rarity: 2, categoryOverride: "neutral");

    // Scavenger
    public static readonly AchievementSpec CleanPlateClub = Define("Clean Plate Club",
        "Eat 3 bodies in a single game as Scavenger.", MiraUnleashedAssets.ScavengerEatButtonSprite,
        progressGoal: 3, persistence: AchievementPersistence.ThroughoutRounds, rarity: 1,
        categoryOverride: "neutral");
    public static readonly AchievementSpec BuffetRegular = Define("Buffet Regular",
        "Eat 10 bodies as Scavenger.", MiraUnleashedAssets.ScavengerRoleIcon, progressGoal: 10,
        persistence: AchievementPersistence.ThroughoutSessions, rarity: 1, categoryOverride: "neutral");
    public static readonly AchievementSpec BottomlessPit = Define("Bottomless Pit",
        "Eat 3 bodies in a single round as Scavenger.", MiraUnleashedAssets.ScavengerEatButtonSprite,
        rarity: 2, categoryOverride: "neutral");
    public static readonly AchievementSpec FastFood = Define("Fast Food",
        "Eat a body within 10 seconds of the kill.", MiraUnleashedAssets.ScavengerEatButtonSprite,
        rarity: 1, categoryOverride: "neutral");

    // Serial Killer
    public static readonly AchievementSpec ColdOpen = Define("Cold Open",
        "Claim the first kill of the game as Serial Killer.", TouRoleIcons.SerialKiller,
        toastOnGameEnd: true, categoryOverride: "neutral");
    public static readonly AchievementSpec SerialSeason = Define("Serial Season",
        "Kill 10 players as Serial Killer.", TouRoleIcons.SerialKiller, progressGoal: 10,
        persistence: AchievementPersistence.ThroughoutSessions, rarity: 1, categoryOverride: "neutral");
    public static readonly AchievementSpec Massacre = Define("Massacre",
        "Kill 3 players as Serial Killer in a single round.", TouRoleIcons.SerialKiller, rarity: 2,
        categoryOverride: "neutral");
    public static readonly AchievementSpec SnakeInThePipes = Define("Snake in the Pipes",
        "Kill a player inside a vent as Serial Killer.", TouRoleIcons.SerialKiller, rarity: 1,
        categoryOverride: "neutral");

    // Spiteful
    public static readonly AchievementSpec PyrrhicVote = Define("Pyrrhic Vote",
        "Get cursed after voting for a Spiteful player.", MiraUnleashedAssets.SpitefulModifierIcon);
    public static readonly AchievementSpec MiseryLovesCompany = Define("Misery Loves Company",
        "Get ejected as a Spiteful player and curse 2 or more voters.",
        MiraUnleashedAssets.SpitefulModifierIcon, rarity: 1);
    public static readonly AchievementSpec PartingGift = Define("Parting Gift",
        "Get ejected as a Spiteful player and curse everyone who voted for you.",
        MiraUnleashedAssets.SpitefulModifierIcon, rarity: 2);

    // Clueless
    public static readonly AchievementSpec WingIt = Define("Wing It",
        "Complete a task as a Clueless player.", MiraUnleashedAssets.CluelessModifierIcon);
    public static readonly AchievementSpec MuscleMemory = Define("Muscle Memory",
        "Complete 5 tasks as a Clueless player in a single game.", MiraUnleashedAssets.CluelessModifierIcon,
        progressGoal: 5, persistence: AchievementPersistence.ThroughoutRounds);
    public static readonly AchievementSpec CluelessCompletionist = Define("Clueless Completionist",
        "Complete all tasks as a Clueless player in a single game.",
        MiraUnleashedAssets.CluelessModifierIcon, rarity: 2);
    public static readonly AchievementSpec BurnTheWitch = Define("Burn the Witch",
        "While under a Witch's spell, vote out the Witch and live to tell about it.",
        MiraUnleashedAssets.HexedSprite, rarity: 1, toastOnGameEnd: true);
    public static readonly AchievementSpec BlindJustice = Define("Blind Justice",
        "Vote out an Impostor or Neutral Killer while Clueless.",
        MiraUnleashedAssets.CluelessModifierIcon, rarity: 1, toastOnGameEnd: true);

    public static readonly AchievementSpec[] All =
    [
        Unleashed, DoubleJeopardy,
        NothingLeftToSabotage, SpeedrunLockdown, UnderPressure, EleventhHour,
        Gottem, CarnivalOfIllusions, MirrorMatch,
        WelcomingCommittee, PestControl, RedHanded, CaughtInTheAct, CaughtAndConvicted,
        BodyDouble, SmokeAndMirrors, Framed,
        DdosAttack, LowPingHighCrime, Blackout, BackdoorAccess,
        Malpractice, DoubleDose, ClinicalTrial, TripleThreat, BadMedicine, LabRat,
        MarkedForDeath, PracticedHand, TwinSacrifice, FullCoven, DoubleHex,
        HitTheGroundScreaming, LamplightLethality, FadeToBlack, LanternGuided,
        CourtAppointed, ObjectionSustained, DevilsAdvocate,
        CleanPlateClub, BuffetRegular, BottomlessPit, FastFood,
        ColdOpen, SerialSeason, Massacre, SnakeInThePipes,
        PyrrhicVote, MiseryLovesCompany, PartingGift,
        WingIt, MuscleMemory, CluelessCompletionist, BurnTheWitch, BlindJustice
    ];
}
