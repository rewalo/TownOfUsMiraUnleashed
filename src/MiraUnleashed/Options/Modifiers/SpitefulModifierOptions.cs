using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using MiraUnleashed.Modifiers;
using SpitefulEffectTypeEnum = MiraUnleashed.Options.Modifiers.SpitefulEffectType;
using SpitefulDurationTypeEnum = MiraUnleashed.Options.Modifiers.SpitefulDurationType;

namespace MiraUnleashed.Options.Modifiers;

public enum SpitefulEffectType
{
    LowerVision,
    Slowness,
    IncreasedCooldowns
}

public enum SpitefulDurationType
{
    NextRounds,
    RestOfGame
}

public sealed class SpitefulModifierOptions : AbstractOptionGroup<SpitefulModifier>
{
    public override string GroupName => MiraLocaleManager.Get("MiraUnleashed.Modifier.Spiteful", "Spiteful");
    public override MenuCategory ParentMenu => MenuCategory.Modifiers;
    public override uint GroupPriority => 1;

    [ModdedNumberOption("MiraUnleashed.Options.Spiteful.Amount", 0, 15)]
    public float SpitefulAmount { get; set; } = 0;

    public ModdedNumberOption SpitefulChance { get; } =
        new("MiraUnleashed.Options.Spiteful.Chance", 50f, 0, 100f, 10f, MiraNumberSuffixes.Percent)
        {
            Visible = () => OptionGroupSingleton<SpitefulModifierOptions>.Instance.SpitefulAmount > 0
        };

    public ModdedNumberOption SpitefulImpact { get; } =
        new("MiraUnleashed.Options.Spiteful.Impact", 25f, 15f, 75f, 5f, MiraNumberSuffixes.Percent)
        {
            Visible = () => OptionGroupSingleton<SpitefulModifierOptions>.Instance.SpitefulAmount > 0
        };

    private static readonly string[] SpitefulEffectTypeValues =
    [
        "MiraUnleashed.Options.Spiteful.EffectType.LowerVision",
        "MiraUnleashed.Options.Spiteful.EffectType.Slowness",
        "MiraUnleashed.Options.Spiteful.EffectType.IncreasedCooldowns"
    ];

    public ModdedEnumOption<SpitefulEffectType> SpitefulEffectType { get; } =
        new("MiraUnleashed.Options.Spiteful.EffectType", SpitefulEffectTypeEnum.IncreasedCooldowns, SpitefulEffectTypeValues)
        {
            Visible = () => OptionGroupSingleton<SpitefulModifierOptions>.Instance.SpitefulAmount > 0
        };

    private static readonly string[] SpitefulDurationTypeValues =
    [
        "MiraUnleashed.Options.Spiteful.DurationType.NextRounds",
        "MiraUnleashed.Options.Spiteful.DurationType.RestOfGame"
    ];

    public ModdedEnumOption<SpitefulDurationType> SpitefulDurationType { get; } =
        new("MiraUnleashed.Options.Spiteful.DurationType", SpitefulDurationTypeEnum.RestOfGame, SpitefulDurationTypeValues)
        {
            Visible = () => OptionGroupSingleton<SpitefulModifierOptions>.Instance.SpitefulAmount > 0
        };

    public ModdedNumberOption SpitefulRoundCount { get; } =
        new("MiraUnleashed.Options.Spiteful.RoundCount", 1f, 1f, 5f, 1f, MiraNumberSuffixes.None)
        {
            Visible = () => OptionGroupSingleton<SpitefulModifierOptions>.Instance.SpitefulAmount > 0 &&
                             OptionGroupSingleton<SpitefulModifierOptions>.Instance.SpitefulDurationType.Value == SpitefulDurationTypeEnum.NextRounds
        };
}
