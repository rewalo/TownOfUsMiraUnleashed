using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using MiraUnleashed.Roles.Crewmate;

namespace MiraUnleashed.Options.Roles.Crewmate;

public sealed class TrapperOptions : AbstractOptionGroup<TrapperRole>
{
    public override string GroupName => MiraLocaleManager.Get("MiraUnleashed.Role.Trapper", "Trapper");

    [ModdedNumberOption("MiraUnleashed.Options.Trapper.TrapCooldown", 1f, 60f, 1f, MiraNumberSuffixes.Seconds)]
    public float TrapCooldown { get; set; } = 25f;

    [ModdedNumberOption("MiraUnleashed.Options.Trapper.Trappeduration", 0.5f, 15f, 0.5f, MiraNumberSuffixes.Seconds)]
    public float Trappeduration { get; set; } = 4f;

    [ModdedNumberOption("MiraUnleashed.Options.Trapper.ArrowDuration", 0.5f, 15f, 0.5f, MiraNumberSuffixes.Seconds)]
    public float ArrowDuration { get; set; } = 4.5f;

    [ModdedNumberOption("MiraUnleashed.Options.Trapper.MaxTraps", 1f, 15f, 1f, MiraNumberSuffixes.None, "0")]
    public float MaxTraps { get; set; } = 3f;

    [ModdedNumberOption("MiraUnleashed.Options.Trapper.TrapRoundsLast", 0f, 15f, 1f, MiraNumberSuffixes.None, "0", true)]
    public float TrapRoundsLast { get; set; } = 3f;

    [ModdedToggleOption("MiraUnleashed.Options.Trapper.GetMoreFromTasks")]
    public bool GetMoreFromTasks { get; set; } = true;

    [ModdedNumberOption("MiraUnleashed.Options.Trapper.TasksUntilMoreTraps", 1f, 10f, 1f, MiraNumberSuffixes.None, "0")]
    public float TasksUntilMoreTraps { get; set; } = 2f;

    [ModdedEnumOption("MiraUnleashed.Options.Trapper.TrapTargets", typeof(VentTrapTargets),
        ["MiraUnleashed.Options.Trapper.TrapTargets.Impostors", "MiraUnleashed.Options.Trapper.TrapTargets.ImpostorsAndNeutrals", "MiraUnleashed.Options.Trapper.TrapTargets.All"])]
    public VentTrapTargets TrapTargets { get; set; } = VentTrapTargets.ImpostorsAndNeutrals;

    [ModdedEnumOption("MiraUnleashed.Options.Trapper.ArrowTarget", typeof(TrapperArrowTarget),
        ["MiraUnleashed.Options.Trapper.ArrowTarget.Vent", "MiraUnleashed.Options.Trapper.ArrowTarget.Person"])]
    public TrapperArrowTarget ArrowTarget { get; set; } = TrapperArrowTarget.Vent;
}

public enum VentTrapTargets
{
    Impostors,
    ImpostorsAndNeutrals,
    All,
}

public enum TrapperArrowTarget
{
    Vent,
    Person,
}
