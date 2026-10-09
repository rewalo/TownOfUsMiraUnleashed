using BepInEx.Configuration;
using MiraAPI.LocalSettings;
using MiraAPI.LocalSettings.Attributes;
using MiraUnleashed.Assets;

namespace MiraUnleashed;

public class MiraUnleashedLocalSettings(ConfigFile config) : LocalSettingsTab(config)
{
    public override string TabName => "Mira Unleashed";

    protected override bool ShouldCreateLabels => true;

    public override LocalSettingTabAppearance TabAppearance => new()
    {
        TabIcon = MiraUnleashedImpAssets.InjectorRole,
    };

    [LocalToggleSetting("MiraUnleashed.LocalSetting.EnableNauseaCameraShake")]
    public ConfigEntry<bool> EnableNauseaCameraShake { get; private set; } =
        config.Bind("Accessibility", "EnableNauseaCameraShake", true);

    [LocalEnumSetting("MiraUnleashed.LocalSetting.CluelessCensorType", names:
    [
        "MiraUnleashed.LocalSetting.CluelessCensorType.WhiteBars",
        "MiraUnleashed.LocalSetting.CluelessCensorType.Asterisks",
        "MiraUnleashed.LocalSetting.CluelessCensorType.QuestionMarks",
        "MiraUnleashed.LocalSetting.CluelessCensorType.Remove",
    ])]
    public ConfigEntry<CluelessCensorType> CluelessCensorType { get; private set; } =
        config.Bind("Modifier Visuals", "CluelessCensorType", MiraUnleashed.CluelessCensorType.Asterisks);
}

public enum CluelessCensorType
{
    WhiteBars,
    Asterisks,
    QuestionMarks,
    Remove,
}
