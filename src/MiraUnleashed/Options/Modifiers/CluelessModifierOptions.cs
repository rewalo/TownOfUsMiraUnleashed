using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using CluelessCensorTypeEnum = MiraUnleashed.Options.Modifiers.CluelessCensorType;

namespace MiraUnleashed.Options.Modifiers;

public enum CluelessCensorType
{
    WhiteBars,
    Asterisks,
    QuestionMarks,
    Remove,
}

public sealed class CluelessModifierOptions : AbstractOptionGroup
{
    public override string GroupName => MiraLocaleManager.Get("MiraUnleashed.Modifier.Clueless", "Clueless");
    public override MenuCategory ParentMenu => MenuCategory.Modifiers;
    public override uint GroupPriority => 1;

    [ModdedNumberOption("MiraUnleashed.Options.Clueless.Amount", 0, 15)]
    public float CluelessAmount { get; set; } = 0;

    public ModdedNumberOption CluelessChance { get; } =
        new("MiraUnleashed.Options.Clueless.Chance", 50f, 0, 100f, 10f, MiraNumberSuffixes.Percent)
        {
            Visible = () => OptionGroupSingleton<CluelessModifierOptions>.Instance.CluelessAmount > 0,
        };

    private static readonly string[] CluelessCensorTypeValues =
    [
        "MiraUnleashed.Options.Clueless.CensorType.WhiteBars",
        "MiraUnleashed.Options.Clueless.CensorType.Asterisks",
        "MiraUnleashed.Options.Clueless.CensorType.QuestionMarks",
        "MiraUnleashed.Options.Clueless.CensorType.Remove",
    ];

    public ModdedEnumOption<CluelessCensorType> CluelessCensorType { get; } =
        new("MiraUnleashed.Options.Clueless.CensorType", CluelessCensorTypeEnum.Asterisks, CluelessCensorTypeValues)
        {
            Visible = () => OptionGroupSingleton<CluelessModifierOptions>.Instance.CluelessAmount > 0,
        };
}
