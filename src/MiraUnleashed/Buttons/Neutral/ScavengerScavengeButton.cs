using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Keybinds;
using MiraAPI.Utilities.Assets;
using MiraUnleashed.Assets;
using MiraUnleashed.Options.Roles.Neutral;
using MiraUnleashed.Roles.Neutral;
using TownOfUs.Buttons;
using UnityEngine;

namespace MiraUnleashed.Buttons.Neutral;

public sealed class ScavengerScavengeButton : TownOfUsRoleButton<ScavengerRole>
{
    public override string Name => MiraLocaleManager.Get("MiraUnleashed.Role.ScavengerScavenge", "Scavenge");
    public override BaseKeybind Keybind => Keybinds.ModifierAction;
    public override Color TextOutlineColor => MiraUnleashedColors.Scavenger;
    public override float Cooldown => Math.Clamp(OptionGroupSingleton<ScavengerOptions>.Instance.ScavengeCooldown.Value + MapCooldown, 5f, 120f);
    public override float EffectDuration => OptionGroupSingleton<ScavengerOptions>.Instance.ScavengeDuration.Value;
    public override LoadableAsset<Sprite> Sprite => MiraUnleashedAssets.ScavengerScavengeButtonSprite;

    public override bool ZeroIsInfinite { get; set; } = true;

    public override bool Enabled(RoleBehaviour? role)
    {
        return base.Enabled(role) && OptionGroupSingleton<ScavengerOptions>.Instance.ScavengeEnabled;
    }

    protected override void OnClick()
    {
        if (PlayerControl.LocalPlayer == null)
        {
            return;
        }

        ScavengerRole.RpcScavengerScavenge(PlayerControl.LocalPlayer);
        ResetCooldownAndOrEffect();
    }
}
