using AmongUs.GameOptions;
using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using MiraUnleashed.Roles.Impostor;
using TownOfUs.Interfaces;

namespace MiraUnleashed.Options.Roles.Impostor;

public enum InjectorEffectDurationType
{
    AllRound,
    AllGame,
    SetTime
}

public enum InjectorEffectType
{
    InvertedControls,
    LowVision,
    Slowness,
    VeryLowVision,
    Confused,
    NoVent,
    NoUse,
    NoReport,
    Nausea,
    Weakness,
    SpeedBoost,
    VisionBoost,
    Regeneration
}

public sealed class InjectorOptions : AbstractOptionGroup<InjectorRole>, IWikiOptionsSummaryProvider
{
    public override string GroupName => MiraLocaleManager.Get("MiraUnleashed.Role.Injector", "Injector");

    [ModdedNumberOption("MiraUnleashed.Options.Injector.InjectCooldown", 5f, 120f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float InjectCooldown { get; set; } = 25f;

    [ModdedNumberOption("MiraUnleashed.Options.Injector.EffectDelay", 0f, 30f, 0.5f, MiraNumberSuffixes.Seconds)]
    public float EffectDelay { get; set; } = 5f;

    private static readonly string[] EffectDurationTypeValues =
    [
        "MiraUnleashed.Options.Injector.EffectDurationType.AllRound",
        "MiraUnleashed.Options.Injector.EffectDurationType.AllGame",
        "MiraUnleashed.Options.Injector.EffectDurationType.SetTime"
    ];

    public ModdedEnumOption<InjectorEffectDurationType> EffectDurationType { get; } =
        new("MiraUnleashed.Options.Injector.EffectDurationType", InjectorEffectDurationType.SetTime, EffectDurationTypeValues);

    [ModdedNumberOption("MiraUnleashed.Options.Injector.EffectDuration", 5f, 200f, 5f, MiraNumberSuffixes.Seconds)]
    public float EffectDuration { get; set; } = 45f;

    [ModdedNumberOption("MiraUnleashed.Options.Injector.InitialUses", 0, 15)]
    public float InitialUses { get; set; } = 4f;

    [ModdedNumberOption("MiraUnleashed.Options.Injector.UsesPerKill", 0, 5)]
    public float UsesPerKill { get; set; } = 1f;

    [ModdedToggleOption("MiraUnleashed.Options.Injector.PositiveEffectsEnabled")]
    public bool PositiveEffectsEnabled { get; set; } = true;

    // Effect selection and chance configuration
    private static readonly string[] EffectTypeValues =
    [
        "MiraUnleashed.Options.Injector.EffectType.InvertedControls",
        "MiraUnleashed.Options.Injector.EffectType.LowVision",
        "MiraUnleashed.Options.Injector.EffectType.Slowness",
        "MiraUnleashed.Options.Injector.EffectType.VeryLowVision",
        "MiraUnleashed.Options.Injector.EffectType.Confused",
        "MiraUnleashed.Options.Injector.EffectType.NoVent",
        "MiraUnleashed.Options.Injector.EffectType.NoUse",
        "MiraUnleashed.Options.Injector.EffectType.NoReport",
        "MiraUnleashed.Options.Injector.EffectType.Nausea",
        "MiraUnleashed.Options.Injector.EffectType.Weakness",
        "MiraUnleashed.Options.Injector.EffectType.SpeedBoost",
        "MiraUnleashed.Options.Injector.EffectType.VisionBoost",
        "MiraUnleashed.Options.Injector.EffectType.Regeneration"
    ];

    public ModdedEnumOption<InjectorEffectType> SelectedEffectType { get; } =
        new("MiraUnleashed.Options.Injector.SelectedEffectType", InjectorEffectType.InvertedControls, EffectTypeValues);

    // Individual chance options for each effect type - only the selected one is visible
    public ModdedNumberOption ChanceInvertedControlsOption { get; } =
        new("MiraUnleashed.Options.Injector.ChanceInvertedControls", 30f, 0f, 100f, 10f, MiraNumberSuffixes.Percent)
        {
            Visible = () => OptionGroupSingleton<InjectorOptions>.Instance.SelectedEffectType.Value == InjectorEffectType.InvertedControls
        };

    public ModdedNumberOption ChanceLowVisionOption { get; } =
        new("MiraUnleashed.Options.Injector.ChanceLowVision", 30f, 0f, 100f, 10f, MiraNumberSuffixes.Percent)
        {
            Visible = () => OptionGroupSingleton<InjectorOptions>.Instance.SelectedEffectType.Value == InjectorEffectType.LowVision
        };

    public ModdedNumberOption ChanceSlownessOption { get; } =
        new("MiraUnleashed.Options.Injector.ChanceSlowness", 30f, 0f, 100f, 10f, MiraNumberSuffixes.Percent)
        {
            Visible = () => OptionGroupSingleton<InjectorOptions>.Instance.SelectedEffectType.Value == InjectorEffectType.Slowness
        };

    public ModdedNumberOption ChanceVeryLowVisionOption { get; } =
        new("MiraUnleashed.Options.Injector.ChanceVeryLowVision", 50f, 0f, 100f, 10f, MiraNumberSuffixes.Percent)
        {
            Visible = () => OptionGroupSingleton<InjectorOptions>.Instance.SelectedEffectType.Value == InjectorEffectType.VeryLowVision
        };

    public ModdedNumberOption ChanceConfusedOption { get; } =
        new("MiraUnleashed.Options.Injector.ChanceConfused", 40f, 0f, 100f, 10f, MiraNumberSuffixes.Percent)
        {
            Visible = () => OptionGroupSingleton<InjectorOptions>.Instance.SelectedEffectType.Value == InjectorEffectType.Confused
        };

    public ModdedNumberOption ChanceNoVentOption { get; } =
        new("MiraUnleashed.Options.Injector.ChanceNoVent", 60f, 0f, 100f, 10f, MiraNumberSuffixes.Percent)
        {
            Visible = () => OptionGroupSingleton<InjectorOptions>.Instance.SelectedEffectType.Value == InjectorEffectType.NoVent
        };

    public ModdedNumberOption ChanceNoUseOption { get; } =
        new("MiraUnleashed.Options.Injector.ChanceNoUse", 30f, 0f, 100f, 10f, MiraNumberSuffixes.Percent)
        {
            Visible = () => OptionGroupSingleton<InjectorOptions>.Instance.SelectedEffectType.Value == InjectorEffectType.NoUse
        };

    public ModdedNumberOption ChanceNoReportOption { get; } =
        new("MiraUnleashed.Options.Injector.ChanceNoReport", 30f, 0f, 100f, 10f, MiraNumberSuffixes.Percent)
        {
            Visible = () => OptionGroupSingleton<InjectorOptions>.Instance.SelectedEffectType.Value == InjectorEffectType.NoReport
        };

    public ModdedNumberOption ChanceNauseaOption { get; } =
        new("MiraUnleashed.Options.Injector.ChanceNausea", 50f, 0f, 100f, 10f, MiraNumberSuffixes.Percent)
        {
            Visible = () => OptionGroupSingleton<InjectorOptions>.Instance.SelectedEffectType.Value == InjectorEffectType.Nausea
        };

    public ModdedNumberOption ChanceWeaknessOption { get; } =
        new("MiraUnleashed.Options.Injector.ChanceWeakness", 20f, 0f, 100f, 10f, MiraNumberSuffixes.Percent)
        {
            Visible = () => OptionGroupSingleton<InjectorOptions>.Instance.SelectedEffectType.Value == InjectorEffectType.Weakness
        };

    public ModdedNumberOption ChanceSpeedBoostOption { get; } =
        new("MiraUnleashed.Options.Injector.ChanceSpeedBoost", 10f, 0f, 100f, 10f, MiraNumberSuffixes.Percent)
        {
            Visible = () => OptionGroupSingleton<InjectorOptions>.Instance.SelectedEffectType.Value == InjectorEffectType.SpeedBoost &&
                            OptionGroupSingleton<InjectorOptions>.Instance.PositiveEffectsEnabled
        };

    public ModdedNumberOption ChanceVisionBoostOption { get; } =
        new("MiraUnleashed.Options.Injector.ChanceVisionBoost", 10f, 0f, 100f, 10f, MiraNumberSuffixes.Percent)
        {
            Visible = () => OptionGroupSingleton<InjectorOptions>.Instance.SelectedEffectType.Value == InjectorEffectType.VisionBoost &&
                            OptionGroupSingleton<InjectorOptions>.Instance.PositiveEffectsEnabled
        };

    public ModdedNumberOption ChanceRegenerationOption { get; } =
        new("MiraUnleashed.Options.Injector.ChanceRegeneration", 10f, 0f, 100f, 10f, MiraNumberSuffixes.Percent)
        {
            Visible = () => OptionGroupSingleton<InjectorOptions>.Instance.SelectedEffectType.Value == InjectorEffectType.Regeneration &&
                            OptionGroupSingleton<InjectorOptions>.Instance.PositiveEffectsEnabled
        };

    public float GetEffectChance(InjectorEffectType effectType)
    {
        return effectType switch
        {
            InjectorEffectType.InvertedControls => ChanceInvertedControlsOption.Value,
            InjectorEffectType.LowVision => ChanceLowVisionOption.Value,
            InjectorEffectType.Slowness => ChanceSlownessOption.Value,
            InjectorEffectType.VeryLowVision => ChanceVeryLowVisionOption.Value,
            InjectorEffectType.Confused => ChanceConfusedOption.Value,
            InjectorEffectType.NoVent => ChanceNoVentOption.Value,
            InjectorEffectType.NoUse => ChanceNoUseOption.Value,
            InjectorEffectType.NoReport => ChanceNoReportOption.Value,
            InjectorEffectType.Nausea => ChanceNauseaOption.Value,
            InjectorEffectType.Weakness => ChanceWeaknessOption.Value,
            InjectorEffectType.SpeedBoost => ChanceSpeedBoostOption.Value,
            InjectorEffectType.VisionBoost => ChanceVisionBoostOption.Value,
            InjectorEffectType.Regeneration => ChanceRegenerationOption.Value,
            _ => 0f
        };
    }

    public float ChanceInvertedControls => ChanceInvertedControlsOption.Value;
    public float ChanceLowVision => ChanceLowVisionOption.Value;
    public float ChanceSlowness => ChanceSlownessOption.Value;
    public float ChanceVeryLowVision => ChanceVeryLowVisionOption.Value;
    public float ChanceConfused => ChanceConfusedOption.Value;
    public float ChanceNoVent => ChanceNoVentOption.Value;
    public float ChanceNoUse => ChanceNoUseOption.Value;
    public float ChanceNoReport => ChanceNoReportOption.Value;
    public float ChanceNausea => ChanceNauseaOption.Value;
    public float ChanceWeakness => ChanceWeaknessOption.Value;
    public float ChanceSpeedBoost => ChanceSpeedBoostOption.Value;
    public float ChanceVisionBoost => ChanceVisionBoostOption.Value;
    public float ChanceRegeneration => ChanceRegenerationOption.Value;

    public IReadOnlySet<StringNames> WikiHiddenOptionKeys =>
        new HashSet<StringNames>
        {
            SelectedEffectType.StringName,
            ChanceInvertedControlsOption.StringName,
            ChanceLowVisionOption.StringName,
            ChanceSlownessOption.StringName,
            ChanceVeryLowVisionOption.StringName,
            ChanceConfusedOption.StringName,
            ChanceNoVentOption.StringName,
            ChanceNoUseOption.StringName,
            ChanceNoReportOption.StringName,
            ChanceNauseaOption.StringName,
            ChanceWeaknessOption.StringName,
            ChanceSpeedBoostOption.StringName,
            ChanceVisionBoostOption.StringName,
            ChanceRegenerationOption.StringName
        };

    public IEnumerable<string> GetWikiOptionSummaryLines()
    {
        var enabledEffects = new List<string>();

        // Negative effects
        if (GetEffectChance(InjectorEffectType.InvertedControls) > 0)
            enabledEffects.Add($"Inverted Controls: {GetEffectChance(InjectorEffectType.InvertedControls)}%");
        if (GetEffectChance(InjectorEffectType.LowVision) > 0)
            enabledEffects.Add($"Low Vision: {GetEffectChance(InjectorEffectType.LowVision)}%");
        if (GetEffectChance(InjectorEffectType.Slowness) > 0)
            enabledEffects.Add($"Slowness: {GetEffectChance(InjectorEffectType.Slowness)}%");
        if (GetEffectChance(InjectorEffectType.VeryLowVision) > 0)
            enabledEffects.Add($"Very Low Vision: {GetEffectChance(InjectorEffectType.VeryLowVision)}%");
        if (GetEffectChance(InjectorEffectType.Confused) > 0)
            enabledEffects.Add($"Confused: {GetEffectChance(InjectorEffectType.Confused)}%");
        if (GetEffectChance(InjectorEffectType.NoVent) > 0)
            enabledEffects.Add($"No Vent: {GetEffectChance(InjectorEffectType.NoVent)}%");
        if (GetEffectChance(InjectorEffectType.NoUse) > 0)
            enabledEffects.Add($"No Use: {GetEffectChance(InjectorEffectType.NoUse)}%");
        if (GetEffectChance(InjectorEffectType.NoReport) > 0)
            enabledEffects.Add($"No Report: {GetEffectChance(InjectorEffectType.NoReport)}%");
        if (GetEffectChance(InjectorEffectType.Nausea) > 0)
            enabledEffects.Add($"Nausea: {GetEffectChance(InjectorEffectType.Nausea)}%");
        if (GetEffectChance(InjectorEffectType.Weakness) > 0)
            enabledEffects.Add($"Weakness: {GetEffectChance(InjectorEffectType.Weakness)}%");

        // Positive effects (only if enabled)
        if (PositiveEffectsEnabled)
        {
            if (GetEffectChance(InjectorEffectType.SpeedBoost) > 0)
                enabledEffects.Add($"Speed Boost: {GetEffectChance(InjectorEffectType.SpeedBoost)}%");
            if (GetEffectChance(InjectorEffectType.VisionBoost) > 0)
                enabledEffects.Add($"Vision Boost: {GetEffectChance(InjectorEffectType.VisionBoost)}%");
            if (GetEffectChance(InjectorEffectType.Regeneration) > 0)
                enabledEffects.Add($"Regeneration: {GetEffectChance(InjectorEffectType.Regeneration)}%");
        }

        if (enabledEffects.Count == 0)
        {
            return new[] { "Effect Chances: None configured" };
        }

        return new[] { $"Effect Chances: {string.Join(", ", enabledEffects)}" };
    }
}
