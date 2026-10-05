using MiraAPI.GameOptions;
using MiraAPI.Keybinds;
using MiraAPI.Modifiers;
using MiraAPI.Utilities.Assets;
using MiraUnleashed.Assets;
using MiraUnleashed.Modifiers;
using MiraUnleashed.Options.Roles.Impostor;
using MiraUnleashed.Roles.Impostor;
using TownOfUs.Assets;
using TownOfUs.Buttons;
using TownOfUs.Utilities;
using UnityEngine;

namespace MiraUnleashed.Buttons.Impostor;

public sealed class WraithDashButton : TownOfUsRoleButton<WraithRole>
{
    public override string Name => MiraLocaleManager.Get("MiraUnleashed.Role.WraithDash", "Dash");
    public override BaseKeybind Keybind => Keybinds.ModifierAction;
    public override Color TextOutlineColor => MiraUnleashedColors.Wraith;
    public override float Cooldown => Math.Clamp(OptionGroupSingleton<WraithOptions>.Instance.DashCooldown + MapCooldown, 10f, 60f);
    public override float EffectDuration => OptionGroupSingleton<WraithOptions>.Instance.DashDuration;
    public override LoadableAsset<Sprite> Sprite => TouImpAssets.SprintSprite;

    public override bool ZeroIsInfinite { get; set; } = true;

    protected override void OnClick()
    {
        var player = PlayerControl.LocalPlayer;
        if (player == null || player.HasDied())
        {
            return;
        }

        TouAudio.PlaySound(MiraUnleashedAudio.WraithDashSound);
        player.AddModifier<WraithDashModifier>();
    }
}
