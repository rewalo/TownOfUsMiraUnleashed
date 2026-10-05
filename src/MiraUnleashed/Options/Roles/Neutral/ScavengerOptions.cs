using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using MiraUnleashed.Roles.Neutral;
using TownOfUs.Options.Roles.Neutral;

namespace MiraUnleashed.Options.Roles.Neutral;

public sealed class ScavengerOptions : AbstractOptionGroup<ScavengerRole>
{
    public override string GroupName => MiraLocaleManager.Get("MiraUnleashed.Role.Scavenger", "Scavenger");

    [ModdedNumberOption("MiraUnleashed.Options.Scavenger.EatCooldown", 5f, 120f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float EatCooldown { get; set; } = 17.5f;

    [ModdedNumberOption("MiraUnleashed.Options.Scavenger.EatDuration", 0.5f, 10f, 0.1f, MiraNumberSuffixes.Seconds)]
    public float EatDuration { get; set; } = 1.5f;

    [ModdedNumberOption("MiraUnleashed.Options.Scavenger.BodiesToWin", 1f, 15f, 1f, MiraNumberSuffixes.None)]
    public float BodiesToWin { get; set; } = 3f;

    [ModdedToggleOption("MiraUnleashed.Options.Scavenger.CanVent")]
    public bool CanVent { get; set; } = false;

    [ModdedToggleOption("MiraUnleashed.Options.Scavenger.CannotSpawnWithJanitor")]
    public bool CannotSpawnWithJanitor { get; set; } = false;

    [ModdedToggleOption("MiraUnleashed.Options.Scavenger.ScavengeEnabled")]
    public bool ScavengeEnabled { get; set; } = false;

    public ModdedNumberOption ScavengeCooldown { get; } =
        new("MiraUnleashed.Options.Scavenger.ScavengeCooldown", 30f, 5f, 120f, 2.5f, MiraNumberSuffixes.Seconds)
        {
            Visible = () => OptionGroupSingleton<ScavengerOptions>.Instance.ScavengeEnabled,
        };

    public ModdedNumberOption ScavengeDuration { get; } =
        new("MiraUnleashed.Options.Scavenger.ScavengeDuration", 5f, 5f, 60f, 1f, MiraNumberSuffixes.Seconds)
        {
            Visible = () => OptionGroupSingleton<ScavengerOptions>.Instance.ScavengeEnabled,
        };

    public ModdedEnumOption OnLoseBecomes { get; } =
        new("MiraUnleashed.Options.Scavenger.OnLoseBecomes", (int)BecomeOptions.Crew, typeof(BecomeOptions),
            ["MiraApi.RoleTeam.Crewmate", "TownOfUsMira.Role.Amnesiac", "TownOfUsMira.Role.Survivor", "TownOfUsMira.Role.Mercenary", "TownOfUsMira.Role.Jester"]);
}
