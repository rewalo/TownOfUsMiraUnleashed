using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using MiraUnleashed.Roles.Neutral;

namespace MiraUnleashed.Options.Roles.Neutral;

public sealed class SerialKillerOptions : AbstractOptionGroup<SerialKillerRole>
{
    public override string GroupName => MiraLocaleManager.Get("MiraUnleashed.Role.SerialKiller", "Serial Killer");

    [ModdedToggleOption("MiraUnleashed.Options.SerialKiller.CanReportBodies")]
    public bool CanReportBodies { get; set; } = false;

    [ModdedEnumOption("MiraUnleashed.Options.SerialKiller.VentKillTargets", typeof(VentKillTargets),
        ["MiraUnleashed.Options.SerialKiller.VentKillTargetsEnum.Impostors",
         "MiraUnleashed.Options.SerialKiller.VentKillTargetsEnum.ImpNK",
         "MiraUnleashed.Options.SerialKiller.VentKillTargetsEnum.ImpNeutrals",
         "MiraUnleashed.Options.SerialKiller.VentKillTargetsEnum.Any"])]
    public VentKillTargets VentKillTargets { get; set; } = VentKillTargets.Any;

    [ModdedToggleOption("MiraUnleashed.Options.SerialKiller.ManiacMode")]
    public bool ManiacMode { get; set; } = true;

    public ModdedNumberOption ManiacTimer { get; } = new("MiraUnleashed.Options.SerialKiller.ManiacTimer", 40f, 5f, 60f, 5f, MiraNumberSuffixes.Seconds, "0.0")
    {
        Visible = () => OptionGroupSingleton<SerialKillerOptions>.Instance.ManiacMode
    };

    public ModdedNumberOption ManiacCooldown { get; } = new("MiraUnleashed.Options.SerialKiller.ManiacCooldown", 19f, 0f, 30f, 0.5f, MiraNumberSuffixes.Seconds, "0.0")
    {
        Visible = () => OptionGroupSingleton<SerialKillerOptions>.Instance.ManiacMode
    };
}

public enum VentKillTargets
{
    Impostors,
    ImpNK,
    ImpNeutrals,
    Any
}
