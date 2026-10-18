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
        LoadableAsset<Sprite>? subIcon = null, string? subIconTooltip = null, int rarity = 0,
        bool hidden = false, int progressGoal = 0,
        AchievementPersistence persistence = AchievementPersistence.ThroughoutSessions,
        bool toastOnGameEnd = false, string? categoryOverride = null, string? idOverride = null,
        bool hideProgress = false, string[]? tags = null, [CallerMemberName] string key = "")
        => new(key, name, description, icon, subIcon, rarity, hidden, progressGoal, persistence,
            toastOnGameEnd, categoryOverride, idOverride, HideProgress: hideProgress, Tags: tags,
            SubIconTooltip: subIconTooltip);

    // General
    public static readonly AchievementSpec Unleashed = Define("Unleashed",
        "Get assigned a Mira Unleashed role.", MiraUnleashedImpAssets.InjectorRole,
        subIcon: MiraUnleashedAssets.MirageRoleIcon, subIconTooltip: "MiraUnleashed.Role.Mirage", tags: new[] { "Role", "Assignment" });
    public static readonly AchievementSpec DoubleJeopardy = Define("Double Jeopardy",
        "Start a game with both the Spiteful and Clueless modifiers.", MiraUnleashedAssets.SpitefulModifierIcon,
        subIcon: MiraUnleashedAssets.CluelessModifierIcon, subIconTooltip: "MiraUnleashed.Modifier.Clueless", rarity: 1, hidden: true, tags: new[] { "Hidden", "Modifier", "Spiteful", "Clueless" });
    public static readonly AchievementSpec JackOfAllTrades = Define("Jack of All Trades",
        "Get assigned every Mira Unleashed role at least once.", MiraUnleashedAssets.MirageRoleIcon, rarity: 3, hidden: true, progressGoal: 11,
        hideProgress: true, tags: new[] { "Hidden", "Role", "All", "Multi" });

    // Forestaller
    public static readonly AchievementSpec SpeedrunLockdown = Define("Speedrun Lockdown",
        "Activate the Forestaller lock before the first meeting of the game.",
        MiraUnleashedAssets.SpeedrunLockdownAchievementSprite, subIcon: MiraUnleashedAssets.ForestallerRoleIcon, subIconTooltip: "MiraUnleashed.Role.Forestaller",
        rarity: 1, categoryOverride: "crewmate", tags: new[] { "Forestaller", "Lock", "Sabotage", "Meeting" });
    public static readonly AchievementSpec UnderPressure = Define("Under Pressure",
        "Complete all tasks as Forestaller while a Hex Bomb countdown is active.",
        MiraUnleashedAssets.ForestallerRoleIcon, subIcon: MiraUnleashedAssets.ForestallerRoleIcon, subIconTooltip: "MiraUnleashed.Role.Forestaller",
        rarity: 2, categoryOverride: "crewmate", tags: new[] { "Forestaller", "Task", "Hex", "Bomb" });
    public static readonly AchievementSpec EleventhHour = Define("Eleventh Hour",
        "Activate the Forestaller lock while a critical sabotage is counting down.",
        MiraUnleashedAssets.ForestallerRoleIcon, subIcon: MiraUnleashedAssets.ForestallerRoleIcon, subIconTooltip: "MiraUnleashed.Role.Forestaller",
        rarity: 2, categoryOverride: "crewmate", tags: new[] { "Forestaller", "Lock", "Sabotage", "Critical" });
    public static readonly AchievementSpec Targeted = Define("Targeted",
        "Activate your lock and be the very next player to die.",
        MiraUnleashedAssets.ForestallerRoleIcon, subIcon: MiraUnleashedAssets.ForestallerRoleIcon, subIconTooltip: "MiraUnleashed.Role.Forestaller",
        rarity: 1, categoryOverride: "crewmate", tags: new[] { "Forestaller", "Lock", "Death" });
    public static readonly AchievementSpec Floodgates = Define("Floodgates",
        "Die with your lock active and watch a sabotage get called within 15 seconds.",
        MiraUnleashedAssets.ForestallerRoleIcon, subIcon: MiraUnleashedAssets.ForestallerRoleIcon, subIconTooltip: "MiraUnleashed.Role.Forestaller",
        rarity: 2, hidden: true, categoryOverride: "crewmate", tags: new[] { "Forestaller", "Hidden", "Lock", "Death", "Sabotage" });

    // Mirage
    public static readonly AchievementSpec Gottem = Define("Gottem!",
        "Catch someone interacting with your decoy as Mirage.", MiraUnleashedCrewAssets.DecoyButtonSprite,
        subIcon: MiraUnleashedAssets.MirageRoleIcon, subIconTooltip: "MiraUnleashed.Role.Mirage", categoryOverride: "crewmate", tags: new[] { "Mirage", "Decoy", "Interact" });
    public static readonly AchievementSpec CarnivalOfIllusions = Define("Carnival of Illusions",
        "Have your decoys triggered 3 times in a single game.", MiraUnleashedAssets.MirageRoleIcon,
        subIcon: MiraUnleashedAssets.MirageRoleIcon, subIconTooltip: "MiraUnleashed.Role.Mirage", progressGoal: 3,
        persistence: AchievementPersistence.ThroughoutRounds, rarity: 1, categoryOverride: "crewmate", tags: new[] { "Mirage", "Decoy", "Multi" });
    public static readonly AchievementSpec StuntDouble = Define("Stunt Double",
        "Have a decoy wearing your own appearance attacked.", MiraUnleashedAssets.MirageRoleIcon,
        subIcon: MiraUnleashedAssets.MirageRoleIcon, subIconTooltip: "MiraUnleashed.Role.Mirage", rarity: 1, categoryOverride: "crewmate", tags: new[] { "Mirage", "Decoy", "Attack" });
    public static readonly AchievementSpec MistakenIdentity = Define("Mistaken Identity",
        "Catch a player attacking a decoy that looks like them.", MiraUnleashedAssets.MirageRoleIcon,
        subIcon: MiraUnleashedAssets.MirageRoleIcon, subIconTooltip: "MiraUnleashed.Role.Mirage", rarity: 2, categoryOverride: "crewmate", tags: new[] { "Mirage", "Decoy", "Attack" });

    // Trapper
    public static readonly AchievementSpec PestControl = Define("Pest Control",
        "Snare 5 players in your vent traps.", MiraUnleashedAssets.TrapperRoleIcon,
        subIcon: MiraUnleashedAssets.TrapperRoleIcon, subIconTooltip: "MiraUnleashed.Role.Trapper", progressGoal: 5,
        persistence: AchievementPersistence.ThroughoutSessions, rarity: 1, categoryOverride: "crewmate", tags: new[] { "Trapper", "Vent", "Multi" });
    public static readonly AchievementSpec RevolvingDoor = Define("Revolving Door",
        "Snare the same player twice in one game.", MiraUnleashedAssets.TrapperRoleIcon,
        subIcon: MiraUnleashedAssets.TrapperRoleIcon, subIconTooltip: "MiraUnleashed.Role.Trapper", rarity: 1, categoryOverride: "crewmate", tags: new[] { "Trapper", "Vent" });
    public static readonly AchievementSpec HotPursuit = Define("Hot Pursuit",
        "Snare a player within 10 seconds of them killing someone.", MiraUnleashedAssets.TrapperRoleIcon,
        subIcon: MiraUnleashedAssets.TrapperRoleIcon, subIconTooltip: "MiraUnleashed.Role.Trapper", rarity: 2, toastOnGameEnd: true,
        categoryOverride: "crewmate", tags: new[] { "Trapper", "Vent", "Kill" });

    // Charlatan
    public static readonly AchievementSpec SmokeAndMirrors = Define("Smoke and Mirrors",
        "Deceive 3 bodies as Charlatan in a single game.", MiraUnleashedImpAssets.DeceiveButtonSprite,
        subIcon: MiraUnleashedImpAssets.CharlatanRole, subIconTooltip: "MiraUnleashed.Role.Charlatan", progressGoal: 3,
        persistence: AchievementPersistence.ThroughoutRounds, rarity: 1, categoryOverride: "impostor", tags: new[] { "Charlatan", "Deceive", "Body", "Multi" });
    public static readonly AchievementSpec Framed = Define("Framed",
        "Deceive-report a body and have a different player ejected in that meeting.",
        MiraUnleashedImpAssets.DeceiveButtonSprite, subIcon: MiraUnleashedImpAssets.CharlatanRole, subIconTooltip: "MiraUnleashed.Role.Charlatan",
        rarity: 2, categoryOverride: "impostor", tags: new[] { "Charlatan", "Deceive", "Report", "Meeting", "Eject" });
    public static readonly AchievementSpec NothingToSeeHere = Define("Nothing to See Here",
        "Have a body you concealed still lie unreported when a meeting is called by button.",
        MiraUnleashedImpAssets.ConcealButtonSprite, subIcon: MiraUnleashedImpAssets.CharlatanRole, subIconTooltip: "MiraUnleashed.Role.Charlatan",
        rarity: 2, categoryOverride: "impostor", tags: new[] { "Charlatan", "Conceal", "Body", "Meeting" });
    public static readonly AchievementSpec TheWayMojangIntended = Define("The Way Mojang Intended",
        "Play the role the way Mojang intended. (Deceive-report a body from a different room.)",
        MiraUnleashedImpAssets.DeceiveButtonSprite, subIcon: MiraUnleashedImpAssets.CharlatanRole, subIconTooltip: "MiraUnleashed.Role.Charlatan",
        categoryOverride: "impostor", tags: new[] { "Charlatan", "Deceive", "Report", "Meme" });

    // Hacker
    public static readonly AchievementSpec DdosAttack = Define("DDoS Attack",
        "Jam players 3 times as Hacker in a single game.", MiraUnleashedImpAssets.HackerJamButtonSprite,
        subIcon: MiraUnleashedImpAssets.HackerRole, subIconTooltip: "MiraUnleashed.Role.Hacker", progressGoal: 3,
        persistence: AchievementPersistence.ThroughoutRounds, rarity: 1, categoryOverride: "impostor", tags: new[] { "Hacker", "Jam", "Multi" });
    public static readonly AchievementSpec DeadAir = Define("Dead Air",
        "Kill a player while your Jam is active as Hacker.", MiraUnleashedImpAssets.HackerRole,
        subIcon: MiraUnleashedImpAssets.HackerRole, subIconTooltip: "MiraUnleashed.Role.Hacker", rarity: 1, categoryOverride: "impostor",
        idOverride: "MiraUnleashed_Low Ping, High Crime", tags: new[] { "Hacker", "Jam", "Kill" });
    public static readonly AchievementSpec Blackout = Define("Blackout",
        "Activate Jam while lights are sabotaged.", MiraUnleashedImpAssets.HackerJamButtonSprite,
        subIcon: MiraUnleashedImpAssets.HackerRole, subIconTooltip: "MiraUnleashed.Role.Hacker", rarity: 1, categoryOverride: "impostor", tags: new[] { "Hacker", "Jam", "Sabotage" });
    public static readonly AchievementSpec BackdoorAccess = Define("Backdoor Access",
        "Activate Jam while a Forestaller has locked out sabotages.", MiraUnleashedImpAssets.HackerRole,
        subIcon: MiraUnleashedImpAssets.HackerRole, subIconTooltip: "MiraUnleashed.Role.Hacker", rarity: 2, categoryOverride: "impostor", tags: new[] { "Hacker", "Jam", "Sabotage", "Forestaller" });

    // Injector
    public static readonly AchievementSpec Placebo = Define("Placebo",
        "Inject a player with a positive effect.", MiraUnleashedImpAssets.InjectorInjectButtonSprite,
        subIcon: MiraUnleashedImpAssets.InjectorRole, subIconTooltip: "MiraUnleashed.Role.Injector", categoryOverride: "impostor", tags: new[] { "Injector", "Inject" });
    public static readonly AchievementSpec DoubleDose = Define("Double Dose",
        "Inject the same player twice in one game.", MiraUnleashedImpAssets.InjectorRole,
        subIcon: MiraUnleashedImpAssets.InjectorRole, subIconTooltip: "MiraUnleashed.Role.Injector", rarity: 1, categoryOverride: "impostor", tags: new[] { "Injector", "Inject" });
    public static readonly AchievementSpec ClinicalTrial = Define("Clinical Trial",
        "Inject 5 players in a single game.", MiraUnleashedImpAssets.InjectorInjectButtonSprite,
        subIcon: MiraUnleashedImpAssets.InjectorRole, subIconTooltip: "MiraUnleashed.Role.Injector", progressGoal: 5,
        persistence: AchievementPersistence.ThroughoutRounds, rarity: 1, categoryOverride: "impostor", tags: new[] { "Injector", "Inject", "Multi" });
    public static readonly AchievementSpec TripleThreat = Define("Triple Threat",
        "Inject the same player three times in one game.", MiraUnleashedImpAssets.InjectorRole,
        subIcon: MiraUnleashedImpAssets.InjectorRole, subIconTooltip: "MiraUnleashed.Role.Injector", rarity: 2, categoryOverride: "impostor", tags: new[] { "Injector", "Inject" });
    public static readonly AchievementSpec HopelesslyLost = Define("Hopelessly Lost",
        "Inject a Clueless player with Confusion.", MiraUnleashedImpAssets.InjectorInjectButtonSprite,
        subIcon: MiraUnleashedImpAssets.InjectorRole, subIconTooltip: "MiraUnleashed.Role.Injector", rarity: 1, categoryOverride: "impostor", tags: new[] { "Injector", "Inject", "Clueless" });

    // Witch
    public static readonly AchievementSpec MarkedForDeath = Define("Marked for Death",
        "Place a spell on a player as Witch.", MiraUnleashedImpAssets.SpellButtonSprite,
        subIcon: MiraUnleashedImpAssets.SpellButtonSprite, subIconTooltip: "MiraUnleashed.Role.Witch",
        categoryOverride: "impostor", idOverride: "MiraUnleashed_Hex Marks the Spot", tags: new[] { "Witch", "Spell", "Hex" });
    public static readonly AchievementSpec PracticedHand = Define("Practiced Hand",
        "Place spells on 10 players as Witch.", MiraUnleashedImpAssets.SpellButtonSprite,
        subIcon: MiraUnleashedImpAssets.SpellButtonSprite, subIconTooltip: "MiraUnleashed.Role.Witch",
        progressGoal: 10, persistence: AchievementPersistence.ThroughoutRounds, rarity: 1,
        categoryOverride: "impostor", idOverride: "MiraUnleashed_Boil and Trouble", tags: new[] { "Witch", "Spell", "Multi" });
    public static readonly AchievementSpec TwinSacrifice = Define("Twin Sacrifice",
        "Kill 2 players with your hexes in a single meeting.", MiraUnleashedAssets.HexedSprite,
        subIcon: MiraUnleashedImpAssets.SpellButtonSprite, subIconTooltip: "MiraUnleashed.Role.Witch", rarity: 2,
        toastOnGameEnd: true, categoryOverride: "impostor", idOverride: "MiraUnleashed_Bubble, Bubble", tags: new[] { "Witch", "Hex", "Kill", "Multi", "Meeting" });
    public static readonly AchievementSpec FullCoven = Define("Full Coven",
        "Have 5 players under your spell at the same time.", MiraUnleashedAssets.HexedSprite,
        subIcon: MiraUnleashedImpAssets.SpellButtonSprite, subIconTooltip: "MiraUnleashed.Role.Witch", rarity: 2,
        categoryOverride: "impostor", tags: new[] { "Witch", "Spell", "Multi" });
    public static readonly AchievementSpec AndMakeItDouble = Define("And Make It Double",
        "Cast a spell on a player already hexed by a Spellslinger.", MiraUnleashedAssets.HexedSprite,
        subIcon: MiraUnleashedImpAssets.SpellButtonSprite, subIconTooltip: "MiraUnleashed.Role.Witch", rarity: 2, toastOnGameEnd: true,
        categoryOverride: "impostor", idOverride: "MiraUnleashed_Double Hex", tags: new[] { "Witch", "Spell", "Spellslinger" });

    // Wraith
    public static readonly AchievementSpec HitAndRun = Define("Hit and Run",
        "Kill a player, then teleport back to your lantern within 5 seconds.",
        MiraUnleashedImpAssets.LanternButtonSprite, subIcon: MiraUnleashedImpAssets.WraithRole, subIconTooltip: "MiraUnleashed.Role.Wraith",
        rarity: 1, categoryOverride: "impostor", tags: new[] { "Wraith", "Kill", "Teleport" });
    public static readonly AchievementSpec AirtightAlibi = Define("Airtight Alibi",
        "Return to your lantern after a kill and be in a different room when someone else reports the body.",
        MiraUnleashedImpAssets.LanternButtonSprite, subIcon: MiraUnleashedImpAssets.WraithRole, subIconTooltip: "MiraUnleashed.Role.Wraith",
        rarity: 2, toastOnGameEnd: true, categoryOverride: "impostor", tags: new[] { "Wraith", "Teleport", "Body" });
    public static readonly AchievementSpec LeaveNoTrace = Define("Leave No Trace",
        "Return to your lantern 5 times in one game without ever letting one break.",
        MiraUnleashedImpAssets.LanternButtonSprite, subIcon: MiraUnleashedImpAssets.WraithRole, subIconTooltip: "MiraUnleashed.Role.Wraith",
        rarity: 1, categoryOverride: "impostor", tags: new[] { "Wraith", "Teleport", "Multi" });
    public static readonly AchievementSpec SpeedDemon = Define("Speed Demon",
        "Kill 2 players during a single Dash.", TouImpAssets.SprintSprite,
        subIcon: MiraUnleashedImpAssets.WraithRole, subIconTooltip: "MiraUnleashed.Role.Wraith", rarity: 2, categoryOverride: "impostor", tags: new[] { "Wraith", "Kill", "Multi", "Dash" });

    // Lawyer
    public static readonly AchievementSpec ObjectionSustained = Define("Objection Sustained",
        "Object to the votes while your client has at least one vote on them.",
        MiraUnleashedAssets.ObjectionButtonSprite, subIcon: TouRoleIcons.Lawyer, subIconTooltip: "MiraUnleashed.Role.Lawyer",
        rarity: 1, toastOnGameEnd: true, categoryOverride: "neutral", tags: new[] { "Lawyer", "Object", "Vote", "Meeting" });
    public static readonly AchievementSpec DevilsAdvocate = Define("Devil's Advocate",
        "Win as Lawyer while your client is an Impostor or Neutral Killer.", MiraUnleashedAssets.DevilsAdvocateAchievementSprite,
        subIcon: TouRoleIcons.Lawyer, subIconTooltip: "MiraUnleashed.Role.Lawyer", rarity: 2, categoryOverride: "neutral", tags: new[] { "Lawyer", "Win", "Evil" });
    public static readonly AchievementSpec Overruled = Define("Overruled",
        "Object to the votes and have your client ejected anyway in the same meeting.",
        MiraUnleashedAssets.OverruledAchievementSprite, subIcon: TouRoleIcons.Lawyer, subIconTooltip: "MiraUnleashed.Role.Lawyer",
        rarity: 1, categoryOverride: "neutral", tags: new[] { "Lawyer", "Object", "Vote", "Eject", "Meeting" });
    public static readonly AchievementSpec Mistrial = Define("Mistrial",
        "Object to the votes and have the revote eject someone other than your client.",
        MiraUnleashedAssets.ObjectionButtonSprite, subIcon: TouRoleIcons.Lawyer, subIconTooltip: "MiraUnleashed.Role.Lawyer",
        rarity: 2, categoryOverride: "neutral", tags: new[] { "Lawyer", "Object", "Vote", "Eject", "Meeting" });

    // Scavenger
    public static readonly AchievementSpec CleanPlateClub = Define("Clean Plate Club",
        "Eat 3 bodies in a single game as Scavenger.", MiraUnleashedAssets.ScavengerEatButtonSprite,
        subIcon: MiraUnleashedAssets.ScavengerRoleIcon, subIconTooltip: "MiraUnleashed.Role.Scavenger", progressGoal: 3,
        persistence: AchievementPersistence.ThroughoutRounds, rarity: 1, categoryOverride: "neutral", tags: new[] { "Scavenger", "Eat", "Body", "Multi" });
    public static readonly AchievementSpec BuffetRegular = Define("Buffet Regular",
        "Eat 10 bodies as Scavenger.", MiraUnleashedAssets.ScavengerRoleIcon,
        subIcon: MiraUnleashedAssets.ScavengerRoleIcon, subIconTooltip: "MiraUnleashed.Role.Scavenger", progressGoal: 10,
        persistence: AchievementPersistence.ThroughoutSessions, rarity: 1, categoryOverride: "neutral", tags: new[] { "Scavenger", "Eat", "Body", "Multi" });
    public static readonly AchievementSpec BottomlessPit = Define("Bottomless Pit",
        "Eat 3 bodies in a single round as Scavenger.", MiraUnleashedAssets.ScavengerEatButtonSprite,
        subIcon: MiraUnleashedAssets.ScavengerRoleIcon, subIconTooltip: "MiraUnleashed.Role.Scavenger", rarity: 2, categoryOverride: "neutral", tags: new[] { "Scavenger", "Eat", "Body", "Multi" });
    public static readonly AchievementSpec FastFood = Define("Fast Food",
        "Eat a body within 10 seconds of the kill.", MiraUnleashedAssets.ScavengerEatButtonSprite,
        subIcon: MiraUnleashedAssets.ScavengerRoleIcon, subIconTooltip: "MiraUnleashed.Role.Scavenger", rarity: 1, categoryOverride: "neutral", tags: new[] { "Scavenger", "Eat", "Body" });
    public static readonly AchievementSpec AcquiredTaste = Define("Acquired Taste",
        "Eat a body that a Charlatan concealed.", MiraUnleashedAssets.ScavengerEatButtonSprite,
        subIcon: MiraUnleashedAssets.ScavengerRoleIcon, subIconTooltip: "MiraUnleashed.Role.Scavenger", rarity: 1, categoryOverride: "neutral", tags: new[] { "Scavenger", "Eat", "Body", "Charlatan" });

    // Serial Killer
    public static readonly AchievementSpec ColdOpen = Define("Cold Open",
        "Claim the first kill of the game as Serial Killer.", TouRoleIcons.SerialKiller,
        subIcon: TouRoleIcons.SerialKiller, subIconTooltip: "MiraUnleashed.Role.SerialKiller", toastOnGameEnd: true, categoryOverride: "neutral", tags: new[] { "SerialKiller", "Kill", "First" });
    public static readonly AchievementSpec SerialSeason = Define("Serial Season",
        "Kill 10 players as Serial Killer.", TouRoleIcons.SerialKiller,
        subIcon: TouRoleIcons.SerialKiller, subIconTooltip: "MiraUnleashed.Role.SerialKiller", progressGoal: 10,
        persistence: AchievementPersistence.ThroughoutSessions, rarity: 1, categoryOverride: "neutral", tags: new[] { "SerialKiller", "Kill", "Multi" });
    public static readonly AchievementSpec Massacre = Define("Massacre",
        "Kill 3 players as Serial Killer in a single round.", TouRoleIcons.SerialKiller,
        subIcon: TouRoleIcons.SerialKiller, subIconTooltip: "MiraUnleashed.Role.SerialKiller", rarity: 2, categoryOverride: "neutral", tags: new[] { "SerialKiller", "Kill", "Multi" });
    public static readonly AchievementSpec SnakeInThePipes = Define("Snake in the Pipes",
        "Kill a player inside a vent as Serial Killer.", TouRoleIcons.SerialKiller,
        subIcon: TouRoleIcons.SerialKiller, subIconTooltip: "MiraUnleashed.Role.SerialKiller", rarity: 1, categoryOverride: "neutral", tags: new[] { "SerialKiller", "Kill", "Vent" });
    public static readonly AchievementSpec BuzzerBeater = Define("Buzzer Beater",
        "Make a kill with less than 3 seconds left on your Maniac timer.", TouRoleIcons.SerialKiller,
        subIcon: TouRoleIcons.SerialKiller, subIconTooltip: "MiraUnleashed.Role.SerialKiller", rarity: 2, categoryOverride: "neutral", tags: new[] { "SerialKiller", "Kill", "Timer" });

    // Spiteful
    public static readonly AchievementSpec PyrrhicVote = Define("Pyrrhic Vote",
        "Get cursed after voting for a Spiteful player.", MiraUnleashedAssets.SpitefulModifierIcon,
        subIcon: MiraUnleashedAssets.SpitefulModifierIcon, subIconTooltip: "MiraUnleashed.Modifier.Spiteful", tags: new[] { "Spiteful", "Curse", "Vote" });
    public static readonly AchievementSpec MiseryLovesCompany = Define("Misery Loves Company",
        "Get ejected as a Spiteful player and curse 2 or more voters.",
        MiraUnleashedAssets.SpitefulModifierIcon, subIcon: MiraUnleashedAssets.SpitefulModifierIcon, subIconTooltip: "MiraUnleashed.Modifier.Spiteful",
        rarity: 1, tags: new[] { "Spiteful", "Curse", "Eject", "Multi" });
    public static readonly AchievementSpec FriendlyFire = Define("Friendly Fire",
        "Get cursed as an Impostor for voting out your own Spiteful teammate.",
        MiraUnleashedAssets.SpitefulModifierIcon, subIcon: MiraUnleashedAssets.SpitefulModifierIcon, subIconTooltip: "MiraUnleashed.Modifier.Spiteful",
        rarity: 1, toastOnGameEnd: true, tags: new[] { "Spiteful", "Curse", "Vote", "Teammate" });

    // Clueless
    public static readonly AchievementSpec WingIt = Define("Wing It",
        "Complete a task as a Clueless player.", MiraUnleashedAssets.CluelessModifierIcon,
        subIcon: MiraUnleashedAssets.CluelessModifierIcon, subIconTooltip: "MiraUnleashed.Modifier.Clueless", tags: new[] { "Clueless", "Task" });
    public static readonly AchievementSpec MuscleMemory = Define("Muscle Memory",
        "Complete 5 tasks as a Clueless player in a single game.", MiraUnleashedAssets.CluelessModifierIcon,
        subIcon: MiraUnleashedAssets.CluelessModifierIcon, subIconTooltip: "MiraUnleashed.Modifier.Clueless",
        progressGoal: 5, persistence: AchievementPersistence.ThroughoutRounds, tags: new[] { "Clueless", "Task", "Multi" });
    public static readonly AchievementSpec CluelessCompletionist = Define("Clueless Completionist",
        "Complete all tasks as a Clueless player in a single game.",
        MiraUnleashedAssets.CluelessModifierIcon, subIcon: MiraUnleashedAssets.CluelessModifierIcon, subIconTooltip: "MiraUnleashed.Modifier.Clueless",
        rarity: 2, tags: new[] { "Clueless", "Task", "All" });
    public static readonly AchievementSpec BurnTheWitch = Define("Burn the Witch",
        "While under a Witch's spell, vote out the Witch and live to tell about it.",
        MiraUnleashedAssets.HexedSprite, subIcon: MiraUnleashedAssets.CluelessModifierIcon, subIconTooltip: "MiraUnleashed.Modifier.Clueless",
        rarity: 1, toastOnGameEnd: true, tags: new[] { "Clueless", "Witch", "Vote", "Eject" });
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
