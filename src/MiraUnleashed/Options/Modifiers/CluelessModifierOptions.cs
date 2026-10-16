using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;

namespace MiraUnleashed.Options.Modifiers;

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
}
