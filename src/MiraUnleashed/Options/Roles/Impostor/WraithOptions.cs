using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using MiraUnleashed.Roles.Impostor;

namespace MiraUnleashed.Options.Roles.Impostor;

public sealed class WraithOptions : AbstractOptionGroup<WraithRole>
{
    public override string GroupName => MiraLocaleManager.Get("MiraUnleashed.Role.Wraith", "Wraith");

    [ModdedNumberOption("MiraUnleashed.Options.Wraith.DashCooldown", 10f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float DashCooldown { get; set; } = 32.5f;

    [ModdedNumberOption("MiraUnleashed.Options.Wraith.DashDuration", 3f, 15f, 0.5f, MiraNumberSuffixes.Seconds)]
    public float DashDuration { get; set; } = 6f;

    [ModdedToggleOption("MiraUnleashed.Options.Wraith.Lantern")]
    public bool LanternEnabled { get; set; } = true;

    public ModdedNumberOption LanternCooldown { get; } = new("MiraUnleashed.Options.Wraith.LanternCooldown", 37.5f, 5f, 60f, 2.5f, MiraNumberSuffixes.Seconds)
    {
        Visible = () => OptionGroupSingleton<WraithOptions>.Instance.LanternEnabled
    };

    public ModdedNumberOption LanternDuration { get; } = new("MiraUnleashed.Options.Wraith.LanternDuration", 10f, 1f, 20f, 0.5f, MiraNumberSuffixes.Seconds)
    {
        Visible = () => OptionGroupSingleton<WraithOptions>.Instance.LanternEnabled
    };

    public ModdedNumberOption InvisibleDuration { get; } = new("MiraUnleashed.Options.Wraith.InvisibleDuration", 2.5f, 0f, 5f, 0.25f, MiraNumberSuffixes.Seconds)
    {
        Visible = () => OptionGroupSingleton<WraithOptions>.Instance.LanternEnabled
    };
}
