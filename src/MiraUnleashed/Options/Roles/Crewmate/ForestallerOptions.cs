using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using MiraUnleashed.Roles.Crewmate;

namespace MiraUnleashed.Options.Roles.Crewmate;

public sealed class ForestallerOptions : AbstractOptionGroup<ForestallerRole>
{
    public override string GroupName => MiraLocaleManager.Get("MiraUnleashed.Role.Forestaller", "Forestaller");

    [ModdedNumberOption("MiraUnleashed.Options.Forestaller.ExtraShortTasks", 0f, 5f, 1f, MiraNumberSuffixes.None, "0")]
    public float ExtraShortTasks { get; set; } = 1f;

    [ModdedNumberOption("MiraUnleashed.Options.Forestaller.ExtraLongTasks", 0f, 5f, 1f, MiraNumberSuffixes.None, "0")]
    public float ExtraLongTasks { get; set; } = 1f;

    [ModdedEnumOption("MiraUnleashed.Options.Forestaller.RevealTiming", typeof(ForestallerRevealTiming),
        ["MiraUnleashed.Options.Forestaller.RevealTiming.NextMeeting", "MiraUnleashed.Options.Forestaller.RevealTiming.Instant"])]
    public ForestallerRevealTiming RevealTiming { get; set; } = ForestallerRevealTiming.Instant;
}

public enum ForestallerRevealTiming
{
    NextMeeting,
    Instant,
}
