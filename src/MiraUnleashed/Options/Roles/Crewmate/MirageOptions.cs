using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using MiraUnleashed.Roles.Crewmate;

namespace MiraUnleashed.Options.Roles.Crewmate;

public sealed class MirageOptions : AbstractOptionGroup<MirageRole>
{
    public override string GroupName => MiraLocaleManager.Get("MiraUnleashed.Role.Mirage", "Mirage");

    [ModdedNumberOption("MiraUnleashed.Options.Mirage.InitialUses", 1f, 15f, 1f, MiraNumberSuffixes.None, "0")]
    public float InitialUses { get; set; } = 4f;

    public ModdedNumberOption UsesPerTasks { get; } =
        new("MiraUnleashed.Options.Mirage.UsesPerTasks", 0f, 0f, 15f, 1f, "Off", "#", MiraNumberSuffixes.None, "0");

    [ModdedNumberOption("MiraUnleashed.Options.Mirage.DecoyCooldown", 1f, 60f, 1f, MiraNumberSuffixes.Seconds)]
    public float DecoyCooldown { get; set; } = 25f;

    public ModdedNumberOption DecoyDuration { get; } =
        new("MiraUnleashed.Options.Mirage.DecoyDuration", 0f, 0f, 60f, 1f, "Off", "#", MiraNumberSuffixes.Seconds, "0");

    [ModdedEnumOption("MiraUnleashed.Options.Mirage.DecoyType", typeof(MirageDecoyType),
        ["MiraUnleashed.Options.Mirage.DecoyType.Mirage", "MiraUnleashed.Options.Mirage.DecoyType.RandomPlayer"])]
    public MirageDecoyType DecoyType { get; set; } = MirageDecoyType.RandomPlayer;

    [ModdedNumberOption("MiraUnleashed.Options.Mirage.ArrowTime", 0f, 15f, 0.5f, MiraNumberSuffixes.Seconds, "0", true)]
    public float ArrowTime { get; set; } = 6f;

    [ModdedEnumOption("MiraUnleashed.Options.Mirage.ArrowTarget", typeof(MirageArrowTarget),
        ["MiraUnleashed.Options.Mirage.ArrowTarget.Mirage", "MiraUnleashed.Options.Mirage.ArrowTarget.Interactor"])]
    public MirageArrowTarget ArrowTarget { get; set; } = MirageArrowTarget.Interactor;
}

public enum MirageDecoyType
{
    Mirage,
    RandomPlayer,
}

public enum MirageArrowTarget
{
    Mirage,
    Interactor,
}
