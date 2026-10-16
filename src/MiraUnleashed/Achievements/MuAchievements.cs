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
        => new(key, name, description, icon, subIcon, rarity, hidden, progressGoal, persistence,
            toastOnGameEnd, categoryOverride, idOverride);

    // General
    public static readonly AchievementSpec Unleashed = Define("Unleashed",
        "Get assigned a Mira Unleashed role.", MiraUnleashedImpAssets.InjectorRole,
        subIcon: MiraUnleashedAssets.MirageRoleIcon);
    public static readonly AchievementSpec DoubleJeopardy = Define("Double Jeopardy",
        "Start a game with both the Spiteful and Clueless modifiers.", MiraUnleashedAssets.SpitefulModifierIcon,
        subIcon: MiraUnleashedAssets.CluelessModifierIcon, rarity: 1, hidden: true);
    public static readonly AchievementSpec JackOfAllTrades = Define("Jack of All Trades",
        "Get assigned every Mira Unleashed role at least once.", MiraUnleashedAssets.MirageRoleIcon,
        subIcon: MiraUnleashedAssets.MirageRoleIcon, rarity: 3, hidden: true, progressGoal: 11);

    // Forestaller
    public static readonly AchievementSpec SpeedrunLockdown = Define("Speedrun Lockdown",
        "Activate the Forestaller lock before the first meeting of the game.",
        MiraUnleashedAssets.ForestallerRoleIcon, subIcon: MiraUnleashedAssets.ForestallerRoleIcon,
        rarity: 1, categoryOverride: "crewmate");
    public static readonly AchievementSpec UnderPressure = Define("Under Pressure",
        "Complete all tasks as Forestaller while a Hex Bomb countdown is active.",
        MiraUnleashedAssets.ForestallerRoleIcon, subIcon: MiraUnleashedAssets.ForestallerRoleIcon,
        rarity: 2, categoryOverride: "crewmate");
    public static readonly AchievementSpec EleventhHour = Define("Eleventh Hour",
        "Activate the Forestaller lock while a critical sabotage is counting down.",
        MiraUnleashedAssets.ForestallerRoleIcon, subIcon: MiraUnleashedAssets.ForestallerRoleIcon,
        rarity: 2, categoryOverride: "crewmate");
    public static readonly AchievementSpec Targeted = Define("Targeted",
        "Activate your lock and be the very next player to die.",
        MiraUnleashedAssets.ForestallerRoleIcon, subIcon: MiraUnleashedAssets.ForestallerRoleIcon,
        rarity: 1, categoryOverride: "crewmate");
    public static readonly AchievementSpec Floodgates = Define("Floodgates",
        "Die with your lock active and watch a sabotage get called within 15 seconds.",
        MiraUnleashedAssets.ForestallerRoleIcon, subIcon: MiraUnleashedAssets.ForestallerRoleIcon,
        rarity: 2, hidden: true, categoryOverride: "crewmate");

    // Mirage
    public static readonly AchievementSpec Gottem = Define("Gottem!",
        "Catch someone interacting with your decoy as Mirage.", MiraUnleashedCrewAssets.DecoyButtonSprite,
        subIcon: MiraUnleashedAssets.MirageRoleIcon, categoryOverride: "crewmate");
    public static readonly AchievementSpec CarnivalOfIllusions = Define("Carnival of Illusions",
        "Have your decoys triggered 3 times in a single game.", MiraUnleashedAssets.MirageRoleIcon,
        subIcon: MiraUnleashedAssets.MirageRoleIcon, progressGoal: 3,
        persistence: AchievementPersistence.ThroughoutRounds, rarity: 1, categoryOverride: "crewmate");
    public static readonly AchievementSpec StuntDouble = Define("Stunt Double",
        "Have a decoy wearing your own appearance attacked.", MiraUnleashedAssets.MirageRoleIcon,
        subIcon: MiraUnleashedAssets.MirageRoleIcon, rarity: 1, categoryOverride: "crewmate");
    public static readonly AchievementSpec MistakenIdentity = Define("Mistaken Identity",
        "Catch a player attacking a decoy that looks like them.", MiraUnleashedAssets.MirageRoleIcon,
        subIcon: MiraUnleashedAssets.MirageRoleIcon, rarity: 2, categoryOverride: "crewmate");

    // Trapper
    public static readonly AchievementSpec PestControl = Define("Pest Control",
        "Snare 5 players in your vent traps.", MiraUnleashedAssets.TrapperRoleIcon,
        subIcon: MiraUnleashedAssets.TrapperRoleIcon, progressGoal: 5,
        persistence: AchievementPersistence.ThroughoutSessions, rarity: 1, categoryOverride: "crewmate");
    public static readonly AchievementSpec RevolvingDoor = Define("Revolving Door",
        "Snare the same player twice in one game.", MiraUnleashedAssets.TrapperRoleIcon,
        subIcon: MiraUnleashedAssets.TrapperRoleIcon, rarity: 1, categoryOverride: "crewmate");
    public static readonly AchievementSpec HotPursuit = Define("Hot Pursuit",
        "Snare a player within 10 seconds of them killing someone.", MiraUnleashedAssets.TrapperRoleIcon,
        subIcon: MiraUnleashedAssets.TrapperRoleIcon, rarity: 2, toastOnGameEnd: true,
        categoryOverride: "crewmate");

    // Charlatan
    public static readonly AchievementSpec SmokeAndMirrors = Define("Smoke and Mirrors",
        "Deceive 3 bodies as Charlatan in a single game.", MiraUnleashedImpAssets.DeceiveButtonSprite,
        subIcon: MiraUnleashedImpAssets.CharlatanRole, progressGoal: 3,
        persistence: AchievementPersistence.ThroughoutRounds, rarity: 1, categoryOverride: "impostor");
    public static readonly AchievementSpec Framed = Define("Framed",
        "Deceive-report a body and have a different player ejected in that meeting.",
        MiraUnleashedImpAssets.DeceiveButtonSprite, subIcon: MiraUnleashedImpAssets.CharlatanRole,
        rarity: 2, categoryOverride: "impostor");
    public static readonly AchievementSpec NothingToSeeHere = Define("Nothing to See Here",
        "Have a body you concealed still lie unreported when a meeting is called by button.",
        MiraUnleashedImpAssets.ConcealButtonSprite, subIcon: MiraUnleashedImpAssets.CharlatanRole,
        rarity: 2, categoryOverride: "impostor");
    public static readonly AchievementSpec TheWayMojangIntended = Define("The Way Mojang Intended",
        "Play the role the way Mojang intended. (Deceive-report a body from a different room.)",
        MiraUnleashedImpAssets.DeceiveButtonSprite, subIcon: MiraUnleashedImpAssets.CharlatanRole,
        categoryOverride: "impostor");

    // Hacker
    public static readonly AchievementSpec DdosAttack = Define("DDoS Attack",
        "Jam players 3 times as Hacker in a single game.", MiraUnleashedImpAssets.HackerJamButtonSprite,
        subIcon: MiraUnleashedImpAssets.HackerRole, progressGoal: 3,
        persistence: AchievementPersistence.ThroughoutRounds, rarity: 1, categoryOverride: "impostor");
    public static readonly AchievementSpec DeadAir = Define("Dead Air",
        "Kill a player while your Jam is active as Hacker.", MiraUnleashedImpAssets.HackerRole,
        subIcon: MiraUnleashedImpAssets.HackerRole, rarity: 1, categoryOverride: "impostor",
        idOverride: "MiraUnleashed_Low Ping, High Crime");
    public static readonly AchievementSpec Blackout = Define("Blackout",
        "Activate Jam while lights are sabotaged.", MiraUnleashedImpAssets.HackerJamButtonSprite,
        subIcon: MiraUnleashedImpAssets.HackerRole, rarity: 1, categoryOverride: "impostor");
    public static readonly AchievementSpec BackdoorAccess = Define("Backdoor Access",
        "Activate Jam while a Forestaller has locked out sabotages.", MiraUnleashedImpAssets.HackerRole,
        subIcon: MiraUnleashedImpAssets.HackerRole, rarity: 2, categoryOverride: "impostor");

    // Injector
    public static readonly AchievementSpec Placebo = Define("Placebo",
        "Inject a player with a positive effect.", MiraUnleashedImpAssets.InjectorInjectButtonSprite,
        subIcon: MiraUnleashedImpAssets.InjectorRole, categoryOverride: "impostor");
    public static readonly AchievementSpec DoubleDose = Define("Double Dose",
        "Inject the same player twice in one game.", MiraUnleashedImpAssets.InjectorRole,
        subIcon: MiraUnleashedImpAssets.InjectorRole, rarity: 1, categoryOverride: "impostor");
    public static readonly AchievementSpec ClinicalTrial = Define("Clinical Trial",
        "Inject 5 players in a single game.", MiraUnleashedImpAssets.InjectorInjectButtonSprite,
        subIcon: MiraUnleashedImpAssets.InjectorRole, progressGoal: 5,
        persistence: AchievementPersistence.ThroughoutRounds, rarity: 1, categoryOverride: "impostor");
    public static readonly AchievementSpec TripleThreat = Define("Triple Threat",
        "Inject the same player three times in one game.", MiraUnleashedImpAssets.InjectorRole,
        subIcon: MiraUnleashedImpAssets.InjectorRole, rarity: 2, categoryOverride: "impostor");
    public static readonly AchievementSpec HopelesslyLost = Define("Hopelessly Lost",
        "Inject a Clueless player with Confusion.", MiraUnleashedImpAssets.InjectorInjectButtonSprite,
        subIcon: MiraUnleashedImpAssets.InjectorRole, rarity: 1, categoryOverride: "impostor");

    // Witch
    public static readonly AchievementSpec MarkedForDeath = Define("Marked for Death",
        "Place a spell on a player as Witch.", MiraUnleashedImpAssets.SpellButtonSprite,
        subIcon: MiraUnleashedImpAssets.SpellButtonSprite,
        categoryOverride: "impostor", idOverride: "MiraUnleashed_Hex Marks the Spot");
    public static readonly AchievementSpec PracticedHand = Define("Practiced Hand",
        "Place spells on 10 players as Witch.", MiraUnleashedImpAssets.SpellButtonSprite,
        subIcon: MiraUnleashedImpAssets.SpellButtonSprite,
        progressGoal: 10, persistence: AchievementPersistence.ThroughoutRounds, rarity: 1,
        categoryOverride: "impostor", idOverride: "MiraUnleashed_Boil and Trouble");
    public static readonly AchievementSpec TwinSacrifice = Define("Twin Sacrifice",
        "Kill 2 players with your hexes in a single meeting.", MiraUnleashedAssets.HexedSprite,
        subIcon: MiraUnleashedImpAssets.SpellButtonSprite, rarity: 2,
        toastOnGameEnd: true, categoryOverride: "impostor", idOverride: "MiraUnleashed_Bubble, Bubble");
    public static readonly AchievementSpec FullCoven = Define("Full Coven",
        "Have 5 players under your spell at the same time.", MiraUnleashedAssets.HexedSprite,
        subIcon: MiraUnleashedImpAssets.SpellButtonSprite, rarity: 2,
        categoryOverride: "impostor");
    public static readonly AchievementSpec AndMakeItDouble = Define("And Make It Double",
        "Cast a spell on a player already hexed by a Spellslinger.", MiraUnleashedAssets.HexedSprite,
        subIcon: MiraUnleashedImpAssets.SpellButtonSprite, rarity: 2, toastOnGameEnd: true,
        categoryOverride: "impostor", idOverride: "MiraUnleashed_Double Hex");

    // Wraith
    public static readonly AchievementSpec HitAndRun = Define("Hit and Run",
        "Kill a player, then teleport back to your lantern within 5 seconds.",
        MiraUnleashedImpAssets.LanternButtonSprite, subIcon: MiraUnleashedImpAssets.WraithRole,
        rarity: 1, categoryOverride: "impostor");
    public static readonly AchievementSpec AirtightAlibi = Define("Airtight Alibi",
        "Return to your lantern after a kill and be in a different room when someone else reports the body.",
        MiraUnleashedImpAssets.LanternButtonSprite, subIcon: MiraUnleashedImpAssets.WraithRole,
        rarity: 2, toastOnGameEnd: true, categoryOverride: "impostor");
    public static readonly AchievementSpec LeaveNoTrace = Define("Leave No Trace",
        "Return to your lantern 5 times in one game without ever letting one break.",
        MiraUnleashedImpAssets.LanternButtonSprite, subIcon: MiraUnleashedImpAssets.WraithRole,
        rarity: 1, categoryOverride: "impostor");
    public static readonly AchievementSpec SpeedDemon = Define("Speed Demon",
        "Kill 2 players during a single Dash.", TouImpAssets.SprintSprite,
        subIcon: MiraUnleashedImpAssets.WraithRole, rarity: 2, categoryOverride: "impostor");

    // Lawyer
    public static readonly AchievementSpec ObjectionSustained = Define("Objection Sustained",
        "Object to the votes while your client has at least one vote on them.",
        MiraUnleashedAssets.ObjectionButtonSprite, subIcon: TouRoleIcons.Lawyer,
        rarity: 1, toastOnGameEnd: true, categoryOverride: "neutral");
    public static readonly AchievementSpec DevilsAdvocate = Define("Devil's Advocate",
        "Win as Lawyer while your client is an Impostor or Neutral Killer.", TouRoleIcons.Lawyer,
        subIcon: TouRoleIcons.Lawyer, rarity: 2, categoryOverride: "neutral");
    public static readonly AchievementSpec Overruled = Define("Overruled",
        "Object to the votes and have your client ejected anyway in the same meeting.",
        MiraUnleashedAssets.ObjectionButtonSprite, subIcon: TouRoleIcons.Lawyer,
        rarity: 1, categoryOverride: "neutral");
    public static readonly AchievementSpec Mistrial = Define("Mistrial",
        "Object to the votes and have the revote eject someone other than your client.",
        MiraUnleashedAssets.ObjectionButtonSprite, subIcon: TouRoleIcons.Lawyer,
        rarity: 2, categoryOverride: "neutral");

    // Scavenger
    public static readonly AchievementSpec CleanPlateClub = Define("Clean Plate Club",
        "Eat 3 bodies in a single game as Scavenger.", MiraUnleashedAssets.ScavengerEatButtonSprite,
        subIcon: MiraUnleashedAssets.ScavengerRoleIcon, progressGoal: 3,
        persistence: AchievementPersistence.ThroughoutRounds, rarity: 1, categoryOverride: "neutral");
    public static readonly AchievementSpec BuffetRegular = Define("Buffet Regular",
        "Eat 10 bodies as Scavenger.", MiraUnleashedAssets.ScavengerRoleIcon,
        subIcon: MiraUnleashedAssets.ScavengerRoleIcon, progressGoal: 10,
        persistence: AchievementPersistence.ThroughoutSessions, rarity: 1, categoryOverride: "neutral");
    public static readonly AchievementSpec BottomlessPit = Define("Bottomless Pit",
        "Eat 3 bodies in a single round as Scavenger.", MiraUnleashedAssets.ScavengerEatButtonSprite,
        subIcon: MiraUnleashedAssets.ScavengerRoleIcon, rarity: 2, categoryOverride: "neutral");
    public static readonly AchievementSpec FastFood = Define("Fast Food",
        "Eat a body within 10 seconds of the kill.", MiraUnleashedAssets.ScavengerEatButtonSprite,
        subIcon: MiraUnleashedAssets.ScavengerRoleIcon, rarity: 1, categoryOverride: "neutral");
    public static readonly AchievementSpec AcquiredTaste = Define("Acquired Taste",
        "Eat a body that a Charlatan concealed.", MiraUnleashedAssets.ScavengerEatButtonSprite,
        subIcon: MiraUnleashedAssets.ScavengerRoleIcon, rarity: 1, categoryOverride: "neutral");

    // Serial Killer
    public static readonly AchievementSpec ColdOpen = Define("Cold Open",
        "Claim the first kill of the game as Serial Killer.", TouRoleIcons.SerialKiller,
        subIcon: TouRoleIcons.SerialKiller, toastOnGameEnd: true, categoryOverride: "neutral");
    public static readonly AchievementSpec SerialSeason = Define("Serial Season",
        "Kill 10 players as Serial Killer.", TouRoleIcons.SerialKiller,
        subIcon: TouRoleIcons.SerialKiller, progressGoal: 10,
        persistence: AchievementPersistence.ThroughoutSessions, rarity: 1, categoryOverride: "neutral");
    public static readonly AchievementSpec Massacre = Define("Massacre",
        "Kill 3 players as Serial Killer in a single round.", TouRoleIcons.SerialKiller,
        subIcon: TouRoleIcons.SerialKiller, rarity: 2, categoryOverride: "neutral");
    public static readonly AchievementSpec SnakeInThePipes = Define("Snake in the Pipes",
        "Kill a player inside a vent as Serial Killer.", TouRoleIcons.SerialKiller,
        subIcon: TouRoleIcons.SerialKiller, rarity: 1, categoryOverride: "neutral");
    public static readonly AchievementSpec BuzzerBeater = Define("Buzzer Beater",
        "Make a kill with less than 3 seconds left on your Maniac timer.", TouRoleIcons.SerialKiller,
        subIcon: TouRoleIcons.SerialKiller, rarity: 2, categoryOverride: "neutral");

    // Spiteful
    public static readonly AchievementSpec PyrrhicVote = Define("Pyrrhic Vote",
        "Get cursed after voting for a Spiteful player.", MiraUnleashedAssets.SpitefulModifierIcon,
        subIcon: MiraUnleashedAssets.SpitefulModifierIcon);
    public static readonly AchievementSpec MiseryLovesCompany = Define("Misery Loves Company",
        "Get ejected as a Spiteful player and curse 2 or more voters.",
        MiraUnleashedAssets.SpitefulModifierIcon, subIcon: MiraUnleashedAssets.SpitefulModifierIcon,
        rarity: 1);
    public static readonly AchievementSpec FriendlyFire = Define("Friendly Fire",
        "Get cursed as an Impostor for voting out your own Spiteful teammate.",
        MiraUnleashedAssets.SpitefulModifierIcon, subIcon: MiraUnleashedAssets.SpitefulModifierIcon,
        rarity: 1, toastOnGameEnd: true);

    // Clueless
    public static readonly AchievementSpec WingIt = Define("Wing It",
        "Complete a task as a Clueless player.", MiraUnleashedAssets.CluelessModifierIcon,
        subIcon: MiraUnleashedAssets.CluelessModifierIcon);
    public static readonly AchievementSpec MuscleMemory = Define("Muscle Memory",
        "Complete 5 tasks as a Clueless player in a single game.", MiraUnleashedAssets.CluelessModifierIcon,
        subIcon: MiraUnleashedAssets.CluelessModifierIcon,
        progressGoal: 5, persistence: AchievementPersistence.ThroughoutRounds);
    public static readonly AchievementSpec CluelessCompletionist = Define("Clueless Completionist",
        "Complete all tasks as a Clueless player in a single game.",
        MiraUnleashedAssets.CluelessModifierIcon, subIcon: MiraUnleashedAssets.CluelessModifierIcon,
        rarity: 2);
    public static readonly AchievementSpec BurnTheWitch = Define("Burn the Witch",
        "While under a Witch's spell, vote out the Witch and live to tell about it.",
        MiraUnleashedAssets.HexedSprite, subIcon: MiraUnleashedAssets.CluelessModifierIcon,
        rarity: 1, toastOnGameEnd: true);
    public static readonly AchievementSpec[] All =
    [
        Unleashed, DoubleJeopardy, JackOfAllTrades,
        SpeedrunLockdown, UnderPressure, EleventhHour, Targeted, Floodgates,
        Gottem, CarnivalOfIllusions, StuntDouble, MistakenIdentity,
        PestControl, RevolvingDoor, HotPursuit,
        SmokeAndMirrors, Framed, NothingToSeeHere, TheWayMojangIntended,
        DdosAttack, DeadAir, Blackout, BackdoorAccess,
        Placebo, DoubleDose, ClinicalTrial, TripleThreat, HopelesslyLost,
        MarkedForDeath, PracticedHand, TwinSacrifice, FullCoven, AndMakeItDouble,
        HitAndRun, AirtightAlibi, LeaveNoTrace, SpeedDemon,
        ObjectionSustained, DevilsAdvocate, Overruled, Mistrial,
        CleanPlateClub, BuffetRegular, BottomlessPit, FastFood, AcquiredTaste,
        ColdOpen, SerialSeason, Massacre, SnakeInThePipes, BuzzerBeater,
        PyrrhicVote, MiseryLovesCompany, FriendlyFire,
        WingIt, MuscleMemory, CluelessCompletionist, BurnTheWitch
    ];
}
