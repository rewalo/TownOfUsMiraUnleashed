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

    [LocalToggleSetting]
    public ConfigEntry<bool> EnableNauseaCameraShake { get; private set; } =
        config.Bind("Accessibility", "EnableNauseaCameraShake", true);
}
