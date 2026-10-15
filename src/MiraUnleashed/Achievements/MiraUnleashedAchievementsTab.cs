using AchievementsAPI.API;
using MiraUnleashed.Assets;
using TownOfUs;
using TownOfUs.Assets;
using UnityEngine;

namespace MiraUnleashed.Achievements;

public sealed class MiraUnleashedAchievementsTab : AchievementsTab
{
    private const string Crew = AchievementCategories.Crewmate;
    private const string Imp = AchievementCategories.Impostor;
    private const string Neut = AchievementCategories.Neutral;

    /// <inheritdoc/>
    public override StringNames Name => MiraLocaleManager.GetOrCreateLocaleString("MiraUnleashed.Achievements.Tab");

    /// <inheritdoc/>
    public override string CategoryId => AchievementCategories.General;

    /// <inheritdoc/>
    public override Color GetTabColor() => MiraUnleashedColors.Trapper;

    /// <inheritdoc/>
    public override Sprite GetIcon() => MiraUnleashedAssets.MirageRoleIcon.LoadAsset();

    // General
    public MuBaseAchievement Unleashed { get; set; } = new("Unleashed",
        "Get assigned a Mira Unleashed role.", MiraUnleashedImpAssets.InjectorRole);

    public MuBaseAchievement DoubleJeopardy { get; set; } = new("Double Jeopardy",
        "Start a game with both the Spiteful and Clueless modifiers.", MiraUnleashedAssets.SpitefulModifierIcon, 1,
        true);

    // Forestaller
    public MuBaseAchievement NothingLeftToSabotage { get; set; } = new("Nothing Left to Sabotage",
        "Complete all tasks as Forestaller and lock evildoers out of the sabotage menu.",
        MiraUnleashedAssets.ForestallerRoleIcon)
    {
        Category = Crew
    };

    public MuBaseAchievement SpeedrunLockdown { get; set; } = new("Speedrun Lockdown",
        "Activate the Forestaller lock before the first meeting of the game.",
        MiraUnleashedAssets.ForestallerRoleIcon, 1)
    {
        Category = Crew
    };

    public MuBaseAchievement UnderPressure { get; set; } = new("Under Pressure",
        "Complete all tasks as Forestaller while a Hex Bomb countdown is active.",
        MiraUnleashedAssets.ForestallerRoleIcon, 2)
    {
        Category = Crew
    };

    public MuBaseAchievement EleventhHour { get; set; } = new("Eleventh Hour",
        "Activate the Forestaller lock while a critical sabotage is counting down.",
        MiraUnleashedAssets.ForestallerRoleIcon, 2)
    {
        Category = Crew
    };

    // Mirage
    public MuBaseAchievement Gottem { get; set; } = new("Gottem!",
        "Catch someone interacting with your decoy as Mirage.", MiraUnleashedCrewAssets.DecoyButtonSprite)
    {
        Category = Crew
    };

    public CountAchievement CarnivalOfIllusions { get; set; } = new MuCountAchievement("Carnival of Illusions",
        "Have your decoys triggered 3 times in a single game.", MiraUnleashedAssets.MirageRoleIcon, 3,
        AchPersistence.ThroughoutRounds, 1)
    {
        Category = Crew
    };

    public MuBaseAchievement MirrorMatch { get; set; } = new("Mirror Match",
        "Trigger a decoy on a player who is currently morphed.",
        MiraUnleashedAssets.MirageRoleIcon, 2)
    {
        Category = Crew,
        ToastTiming = AchToastTiming.OnGameEnd
    };

    // Trapper
    public MuBaseAchievement WelcomingCommittee { get; set; } = new("The Welcoming Committee",
        "Snare someone in one of your vent traps as Trapper.", MiraUnleashedAssets.TrapperRoleIcon)
    {
        Category = Crew
    };

    public CountAchievement PestControl { get; set; } = new MuCountAchievement("Pest Control",
        "Snare 5 players in your vent traps.", MiraUnleashedAssets.TrapperRoleIcon, 5,
        AchPersistence.ThroughoutSessions, 1)
    {
        Category = Crew
    };

    public CountAchievement RedHanded { get; set; } = new MuCountAchievement("Red Handed",
        "Snare 2 players in your vent traps in a single game.", MiraUnleashedAssets.TrapperRoleIcon, 2,
        AchPersistence.ThroughoutRounds, 1)
    {
        Category = Crew
    };

    public MuBaseAchievement CaughtInTheAct { get; set; } = new("Caught in the Act",
        "Snare a player who was invisible when they entered your trapped vent.",
        MiraUnleashedAssets.TrapperRoleIcon, 2)
    {
        Category = Crew,
        ToastTiming = AchToastTiming.OnGameEnd
    };

    public MuBaseAchievement CaughtAndConvicted { get; set; } = new("Caught and Convicted",
        "Have a player you snared get ejected in the very next meeting.", MiraUnleashedAssets.TrapperRoleIcon, 1)
    {
        Category = Crew
    };

    // Charlatan
    public MuBaseAchievement BodyDouble { get; set; } = new("Body Double",
        "Fully conceal a body as Charlatan.", MiraUnleashedImpAssets.ConcealButtonSprite)
    {
        Category = Imp
    };

    public CountAchievement SmokeAndMirrors { get; set; } = new MuCountAchievement("Smoke and Mirrors",
        "Deceive 3 bodies as Charlatan in a single game.", MiraUnleashedImpAssets.DeceiveButtonSprite, 3,
        AchPersistence.ThroughoutRounds, 1)
    {
        Category = Imp
    };

    public MuBaseAchievement Framed { get; set; } = new("Framed",
        "Deceive-report a body and have a different player ejected in that meeting.",
        MiraUnleashedImpAssets.DeceiveButtonSprite, 2)
    {
        Category = Imp
    };

    // Hacker
    public CountAchievement DdosAttack { get; set; } = new MuCountAchievement("DDoS Attack",
        "Jam players 3 times as Hacker in a single game.", MiraUnleashedImpAssets.HackerJamButtonSprite, 3,
        AchPersistence.ThroughoutRounds, 1)
    {
        Category = Imp
    };

    public MuBaseAchievement LowPingHighCrime { get; set; } = new("Low Ping, High Crime",
        "Kill a player while your jam is active.", MiraUnleashedImpAssets.HackerRole, 1)
    {
        Category = Imp
    };

    public MuBaseAchievement Blackout { get; set; } = new("Blackout",
        "Activate Jam while lights are sabotaged.", MiraUnleashedImpAssets.HackerJamButtonSprite, 1)
    {
        Category = Imp
    };

    public MuBaseAchievement BackdoorAccess { get; set; } = new("Backdoor Access",
        "Activate Jam while a Forestaller has locked out sabotages.", MiraUnleashedImpAssets.HackerRole, 2)
    {
        Category = Imp
    };

    // Injector
    public MuBaseAchievement Malpractice { get; set; } = new("Malpractice",
        "Inject a player as Injector.", MiraUnleashedImpAssets.InjectorInjectButtonSprite)
    {
        Category = Imp
    };

    public MuBaseAchievement DoubleDose { get; set; } = new("Double Dose",
        "Inject the same player twice in one game.", MiraUnleashedImpAssets.InjectorRole, 1)
    {
        Category = Imp
    };

    public CountAchievement ClinicalTrial { get; set; } = new MuCountAchievement("Clinical Trial",
        "Inject 5 players in a single game.", MiraUnleashedImpAssets.InjectorInjectButtonSprite, 5,
        AchPersistence.ThroughoutRounds, 1)
    {
        Category = Imp
    };

    public MuBaseAchievement TripleThreat { get; set; } = new("Triple Threat",
        "Inject the same player three times in one game.", MiraUnleashedImpAssets.InjectorRole, 2)
    {
        Category = Imp
    };

    public MuBaseAchievement BadMedicine { get; set; } = new("Bad Medicine",
        "Inject a player who is under a Witch's spell.", MiraUnleashedImpAssets.InjectorInjectButtonSprite, 1)
    {
        Category = Imp
    };

    public MuBaseAchievement LabRat { get; set; } = new("Lab Rat",
        "Have a player you injected get killed by a teammate before the next meeting.",
        MiraUnleashedImpAssets.InjectorRole, 1)
    {
        Category = Imp
    };

    // Witch
    public MuBaseAchievement MarkedForDeath { get; set; } = new("Marked for Death",
        "Place a spell on a player as Witch.", MiraUnleashedImpAssets.SpellButtonSprite)
    {
        Category = Imp,
        Id = "MiraUnleashed_Hex Marks the Spot"
    };

    public CountAchievement PracticedHand { get; set; } = new MuCountAchievement("Practiced Hand",
        "Place spells on 10 players as Witch.", MiraUnleashedImpAssets.SpellButtonSprite, 10,
        AchPersistence.ThroughoutRounds, 1)
    {
        Category = Imp,
        Id = "MiraUnleashed_Boil and Trouble"
    };

    public MuBaseAchievement TwinSacrifice { get; set; } = new("Twin Sacrifice",
        "Kill 2 players with your hexes in a single meeting.", MiraUnleashedAssets.HexedSprite, 2)
    {
        Category = Imp,
        Id = "MiraUnleashed_Bubble, Bubble",
        ToastTiming = AchToastTiming.OnMeetingEnd
    };

    public MuBaseAchievement FullCoven { get; set; } = new("Full Coven",
        "Have 5 players under your spell at the same time.", MiraUnleashedAssets.HexedSprite, 2)
    {
        Category = Imp
    };

    public MuBaseAchievement DoubleHex { get; set; } = new("Double Hex",
        "Cast a spell on a player already hexed by a Spellslinger.", MiraUnleashedAssets.HexedSprite, 2)
    {
        Category = Imp,
        ToastTiming = AchToastTiming.OnGameEnd
    };

    // Wraith
    public MuBaseAchievement HitTheGroundScreaming { get; set; } = new("Hit the Ground Screaming",
        "Kill a player while dashing as Wraith.", TouImpAssets.SprintSprite, 1)
    {
        Category = Imp
    };

    public MuBaseAchievement LamplightLethality { get; set; } = new("Lamplight Lethality",
        "Kill a player while invisible next to your lantern.", MiraUnleashedImpAssets.LanternButtonSprite, 1)
    {
        Category = Imp
    };

    public CountAchievement FadeToBlack { get; set; } = new MuCountAchievement("Fade to Black",
        "Kill 5 players as Wraith.", MiraUnleashedImpAssets.WraithRole, 5,
        AchPersistence.ThroughoutSessions, 1)
    {
        Category = Imp
    };

    public MuBaseAchievement LanternGuided { get; set; } = new("Lantern Guided",
        "Teleport back to your lantern and kill a player within 3 seconds.",
        MiraUnleashedImpAssets.LanternButtonSprite, 2)
    {
        Category = Imp
    };

    // Lawyer
    public MuBaseAchievement CourtAppointed { get; set; } = new("Court Appointed",
        "Get assigned a client as Lawyer.", TouRoleIcons.Lawyer)
    {
        Category = Neut
    };

    public MuBaseAchievement ObjectionSustained { get; set; } = new("Objection Sustained",
        "Object to the votes while your client has at least one vote on them.",
        MiraUnleashedAssets.ObjectionButtonSprite, 1)
    {
        Category = Neut,
        ToastTiming = AchToastTiming.OnMeetingEnd
    };

    public MuBaseAchievement DevilsAdvocate { get; set; } = new("Devil's Advocate",
        "Win as Lawyer while your client is an Impostor or Neutral Killer.", TouRoleIcons.Lawyer, 2)
    {
        Category = Neut
    };

    // Scavenger
    public CountAchievement CleanPlateClub { get; set; } = new MuCountAchievement("Clean Plate Club",
        "Eat 3 bodies in a single game as Scavenger.", MiraUnleashedAssets.ScavengerEatButtonSprite, 3,
        AchPersistence.ThroughoutRounds, 1)
    {
        Category = Neut
    };

    public CountAchievement BuffetRegular { get; set; } = new MuCountAchievement("Buffet Regular",
        "Eat 10 bodies as Scavenger.", MiraUnleashedAssets.ScavengerRoleIcon, 10,
        AchPersistence.ThroughoutSessions, 1)
    {
        Category = Neut
    };

    public MuBaseAchievement BottomlessPit { get; set; } = new("Bottomless Pit",
        "Eat 3 bodies in a single round as Scavenger.", MiraUnleashedAssets.ScavengerEatButtonSprite, 2)
    {
        Category = Neut
    };

    public MuBaseAchievement FastFood { get; set; } = new("Fast Food",
        "Eat a body within 10 seconds of the kill.", MiraUnleashedAssets.ScavengerEatButtonSprite, 1)
    {
        Category = Neut
    };

    // Serial Killer
    public MuBaseAchievement ColdOpen { get; set; } = new("Cold Open",
        "Claim the first kill of the game as Serial Killer.", TouRoleIcons.SerialKiller)
    {
        Category = Neut,
        ToastTiming = AchToastTiming.OnMeetingStart
    };

    public CountAchievement SerialSeason { get; set; } = new MuCountAchievement("Serial Season",
        "Kill 10 players as Serial Killer.", TouRoleIcons.SerialKiller, 10,
        AchPersistence.ThroughoutSessions, 1)
    {
        Category = Neut
    };

    public MuBaseAchievement Massacre { get; set; } = new("Massacre",
        "Kill 3 players as Serial Killer in a single round.", TouRoleIcons.SerialKiller, 2)
    {
        Category = Neut
    };

    public MuBaseAchievement SnakeInThePipes { get; set; } = new("Snake in the Pipes",
        "Kill a player inside a vent as Serial Killer.", TouRoleIcons.SerialKiller, 1)
    {
        Category = Neut
    };

    // Spiteful
    public MuBaseAchievement PyrrhicVote { get; set; } = new("Pyrrhic Vote",
        "Get cursed after voting for a Spiteful player.", MiraUnleashedAssets.SpitefulModifierIcon);

    public MuBaseAchievement MiseryLovesCompany { get; set; } = new("Misery Loves Company",
        "Get ejected as a Spiteful player and curse 2 or more voters.", MiraUnleashedAssets.SpitefulModifierIcon, 1);

    public MuBaseAchievement PartingGift { get; set; } = new("Parting Gift",
        "Get ejected as a Spiteful player and curse everyone who voted for you.",
        MiraUnleashedAssets.SpitefulModifierIcon, 2);

    // Clueless
    public MuBaseAchievement WingIt { get; set; } = new("Wing It",
        "Complete a task as a Clueless player.", MiraUnleashedAssets.CluelessModifierIcon);

    public CountAchievement MuscleMemory { get; set; } = new MuCountAchievement("Muscle Memory",
        "Complete 5 tasks as a Clueless player in a single game.", MiraUnleashedAssets.CluelessModifierIcon, 5,
        AchPersistence.ThroughoutRounds, 1);

    public MuBaseAchievement CluelessCompletionist { get; set; } = new("Clueless Completionist",
        "Complete all tasks as a Clueless player in a single game.",
        MiraUnleashedAssets.CluelessModifierIcon, 2);

    public MuBaseAchievement BurnTheWitch { get; set; } = new("Burn the Witch",
        "While under a Witch's spell, vote out the Witch and live to tell about it.",
        MiraUnleashedAssets.HexedSprite, 1)
    {
        ToastTiming = AchToastTiming.OnMeetingEnd
    };

    public MuBaseAchievement BlindJustice { get; set; } = new("Blind Justice",
        "Vote out an Impostor or Neutral Killer while Clueless.", MiraUnleashedAssets.CluelessModifierIcon, 1)
    {
        ToastTiming = AchToastTiming.OnGameEnd
    };
}
