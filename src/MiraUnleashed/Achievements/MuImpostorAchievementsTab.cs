using AchievementsAPI.API;
using MiraUnleashed.Assets;
using TownOfUs.Assets;
using UnityEngine;

namespace MiraUnleashed.Achievements;

public sealed class MuImpostorAchievementsTab : AchievementsTab
{
    /// <inheritdoc/>
    public override string Name => "Mira Unleashed: Impostor";

    /// <inheritdoc/>
    public override Color GetTabColor() => Palette.ImpostorRoleHeaderRed;

    /// <inheritdoc/>
    public override Sprite GetIcon() => MiraUnleashedImpAssets.CharlatanRole.LoadAsset();

    public MuBaseAchievement BodyDouble { get; set; } = new("Body Double",
        "Fully conceal a body as Charlatan.", MiraUnleashedImpAssets.ConcealButtonSprite);

    public MuCountAchievement SmokeAndMirrors { get; set; } = new("Smoke and Mirrors",
        "Deceive 3 bodies as Charlatan in a single game.", MiraUnleashedImpAssets.DeceiveButtonSprite, 3,
        AchPersistence.ThroughoutRounds);

    public MuCountAchievement DdosAttack { get; set; } = new("DDoS Attack",
        "Jam players 3 times as Hacker in a single game.", MiraUnleashedImpAssets.HackerJamButtonSprite, 3,
        AchPersistence.ThroughoutRounds);

    public MuBaseAchievement LowPingHighCrime { get; set; } = new("Low Ping, High Crime",
        "Kill a player while your jam is active.", MiraUnleashedImpAssets.HackerRole, 1);

    public MuBaseAchievement Malpractice { get; set; } = new("Malpractice",
        "Inject a player as Injector.", MiraUnleashedImpAssets.InjectorInjectButtonSprite);

    public MuBaseAchievement DoubleDose { get; set; } = new("Double Dose",
        "Inject the same player twice in one game.", MiraUnleashedImpAssets.InjectorRole, 1);

    public MuCountAchievement ClinicalTrial { get; set; } = new("Clinical Trial",
        "Inject 5 players in a single game.", MiraUnleashedImpAssets.InjectorInjectButtonSprite, 5,
        AchPersistence.ThroughoutRounds);

    public MuBaseAchievement HexMarksTheSpot { get; set; } = new("Hex Marks the Spot",
        "Place a spell on a player as Witch.", MiraUnleashedImpAssets.SpellButtonSprite);

    public MuCountAchievement BoilAndTrouble { get; set; } = new("Boil and Trouble",
        "Spell 10 players as Witch.", MiraUnleashedImpAssets.SpellButtonSprite, 10);

    public MuBaseAchievement BubbleBubble { get; set; } = new("Bubble, Bubble",
        "Kill 2 players with your hexes in a single meeting.", MiraUnleashedAssets.HexedSprite, 2, true);

    public MuBaseAchievement HitTheGroundScreaming { get; set; } = new("Hit the Ground Screaming",
        "Kill a player while dashing as Wraith.", TouImpAssets.SprintSprite, 1);

    public MuBaseAchievement LamplightLethality { get; set; } = new("Lamplight Lethality",
        "Kill a player while invisible next to your lantern.", MiraUnleashedImpAssets.LanternButtonSprite, 2, true);

    public MuCountAchievement FadeToBlack { get; set; } = new("Fade to Black",
        "Kill 5 players as Wraith.", MiraUnleashedImpAssets.WraithRole, 5);
}
