using MiraAPI.GameOptions;
using MiraAPI.Utilities.Assets;
using MiraUnleashed.Assets;
using MiraUnleashed.Options.Modifiers;
using MiraUnleashed.Roles.Crewmate;
using TownOfUs.Modifiers;
using TownOfUs.Modifiers.Game;
using TownOfUs.Modules.Wiki;
using TownOfUs.Roles.Crewmate;
using TownOfUs.Roles.Neutral;
using TownOfUs.Utilities;
using UnityEngine;

namespace MiraUnleashed.Modifiers;

/// <summary>
/// Clueless: removes all task guidance (task list, task arrows/markers, and map task locations).
/// Tasks still function normally and contribute to the task bar.
/// </summary>
public sealed class CluelessModifier : UniversalGameModifier, IWikiDiscoverable
{
    public override string IdPrefix => "MiraUnleashed.Modifier";
    public override string IdPart => "Clueless";
    public override string IntroInfo => MiraLocaleManager.Get("MiraUnleashed.Modifier.Clueless.IntroBlurb");
    public override LoadableAsset<Sprite> ModifierIcon => MiraUnleashedAssets.CluelessModifierIcon;

    public override string GetDescription()
    {
        return MiraLocaleManager.Get("MiraUnleashed.Modifier.Clueless.TabDescription");
    }

    public string GetAdvancedDescription()
    {
        return MiraLocaleManager.Get("MiraUnleashed.Modifier.Clueless.WikiDescription")
               + MiscUtils.AppendOptionsText(GetType());
    }

    public override Color FreeplayFileColor => new Color32(180, 180, 180, 255);
    public override Color GeneralColor => FreeplayFileColor;
    public override ModifierUiConfiguration Configuration => new(FreeplayFileColor, TmpSpriteUtils.CreateSpriteAsset(MiraUnleashedAssets.CluelessModifierIcon.LoadAsset(), "MiraUnleashed.Modifier.Universal.Clueless", 1.45f));

    public override ModifierFaction FactionType => ModifierFaction.UniversalPassive;

    public List<CustomButtonWikiDescription> Abilities { get; } = [];

    public override int GetAssignmentChance()
    {
        return (int)OptionGroupSingleton<CluelessModifierOptions>.Instance.CluelessChance;
    }

    public override int GetAmountPerGame()
    {
        return (int)OptionGroupSingleton<CluelessModifierOptions>.Instance.CluelessAmount;
    }

    public override bool IsModifierValidOn(RoleBehaviour role)
    {
        if (!base.IsModifierValidOn(role) || role is SnitchRole || role is ForestallerRole)
        {
            return false;
        }

        // Prevent ghosts from being clueless
        var player = role.Player;
        if (player == null || player.Data == null)
        {
            return false;
        }

        if (player.Data.IsDead)
        {
            return false;
        }

        if (role is HaunterRole || role is SpectreRole)
        {
            return false;
        }

        return true;
    }

    public override void OnActivate()
    {
        base.OnActivate();

        if (!Player.AmOwner)
        {
            return;
        }

        if (HudManager.Instance != null && HudManager.Instance.TaskPanel != null &&
            HudManager.Instance.TaskPanel.taskText != null)
        {
            HudManager.Instance.TaskPanel.taskText.text = string.Empty;
        }

        if (MapBehaviour.Instance != null)
        {
            MapBehaviour.Instance.taskOverlay?.Hide();
        }
    }
}
