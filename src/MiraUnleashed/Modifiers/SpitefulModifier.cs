using MiraAPI.GameOptions;
using MiraAPI.Utilities.Assets;
using MiraUnleashed.Assets;
using MiraUnleashed.Options.Modifiers;
using TownOfUs;
using TownOfUs.Modifiers;
using TownOfUs.Modifiers.Game;
using TownOfUs.Modules.Wiki;
using TownOfUs.Utilities;
using UnityEngine;

namespace MiraUnleashed.Modifiers;

public sealed class SpitefulModifier : UniversalGameModifier, IWikiDiscoverable
{
    public override string IdPrefix => "MiraUnleashed.Modifier";
    public override string IdPart => "Spiteful";
    public override string IntroInfo => MiraLocaleManager.Get("MiraUnleashed.Modifier.Spiteful.IntroBlurb");
    public override LoadableAsset<Sprite>? ModifierIcon => MiraUnleashedAssets.SpitefulModifierIcon;

    public override string GetDescription()
    {
        return MiraLocaleManager.Get("MiraUnleashed.Modifier.Spiteful.TabDescription");
    }

    public string GetAdvancedDescription()
    {
        var options = OptionGroupSingleton<SpitefulModifierOptions>.Instance;
        var description = MiraLocaleManager.Get("MiraUnleashed.Modifier.Spiteful.WikiDescription");

        var effectTypeName = MiraLocaleManager.Get("MiraUnleashed.Modifier.Spiteful.EffectType");
        var effectTypeValue = options.SpitefulEffectType.Value switch
        {
            SpitefulEffectType.LowerVision => MiraLocaleManager.Get("MiraUnleashed.Options.Spiteful.EffectType.LowerVision"),
            SpitefulEffectType.Slowness => MiraLocaleManager.Get("MiraUnleashed.Options.Spiteful.EffectType.Slowness"),
            SpitefulEffectType.IncreasedCooldowns => MiraLocaleManager.Get("MiraUnleashed.Options.Spiteful.EffectType.IncreasedCooldowns"),
            _ => options.SpitefulEffectType.Value.ToString()
        };

        var optionsText = MiscUtils.AppendOptionsText(GetType());
        var punishmentEffectLine = $"{effectTypeName}: {effectTypeValue}";
        var alreadyIncluded = !string.IsNullOrWhiteSpace(optionsText) &&
                             optionsText.Contains(punishmentEffectLine, StringComparison.Ordinal);

        if (string.IsNullOrWhiteSpace(optionsText))
        {
            description += $"\n\n<size=50%> \n</size><b>{TownOfUsColors.Vigilante.ToTextColor()}{MiraLocaleManager.Get("Options")}</color></b>";
            description += $"\n{punishmentEffectLine}";
        }
        else if (!alreadyIncluded)
        {
            var headerText = $"\n<size=50%> \n</size><b>{TownOfUsColors.Vigilante.ToTextColor()}{MiraLocaleManager.Get("Options")}</color></b>";
            var headerIndex = optionsText.IndexOf(headerText, StringComparison.Ordinal);

            if (headerIndex >= 0)
            {
                var afterHeader = headerIndex + headerText.Length;
                description += optionsText.Substring(0, afterHeader);
                description += $"\n{punishmentEffectLine}";
                description += optionsText.Substring(afterHeader);
            }
            else
            {
                description += $"\n\n<size=50%> \n</size><b>{TownOfUsColors.Vigilante.ToTextColor()}{MiraLocaleManager.Get("Options")}</color></b>";
                description += $"\n{punishmentEffectLine}";
                description += optionsText;
            }
        }
        else
        {
            description += optionsText;
        }

        return description;
    }

    public override Color FreeplayFileColor => new Color32(255, 100, 0, 255);
    public override Color GeneralColor => FreeplayFileColor;
    public override ModifierUiConfiguration Configuration => new(FreeplayFileColor);

    public override ModifierFaction FactionType => ModifierFaction.UniversalPassive;

    public List<CustomButtonWikiDescription> Abilities { get; } = [];

    public override int GetAssignmentChance()
    {
        return (int)OptionGroupSingleton<SpitefulModifierOptions>.Instance.SpitefulChance;
    }

    public override int GetAmountPerGame()
    {
        return (int)OptionGroupSingleton<SpitefulModifierOptions>.Instance.SpitefulAmount;
    }

    public override bool IsModifierValidOn(RoleBehaviour role)
    {
        if (!base.IsModifierValidOn(role))
        {
            return false;
        }

        var player = role.Player;
        if (player == null || player.Data == null || player.Data.IsDead)
        {
            return false;
        }

        return true;
    }
}
