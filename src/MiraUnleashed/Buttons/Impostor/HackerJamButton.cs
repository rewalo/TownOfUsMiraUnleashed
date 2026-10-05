using MiraAPI.GameOptions;
using MiraAPI.Keybinds;
using MiraAPI.Utilities.Assets;
using System.Globalization;
using MiraUnleashed.Assets;
using MiraUnleashed.Modules;
using MiraUnleashed.Options.Roles.Impostor;
using MiraUnleashed.Roles.Impostor;
using TownOfUs.Buttons;
using UnityEngine;

namespace MiraUnleashed.Buttons.Impostor;

public sealed class HackerJamButton : TownOfUsRoleButton<HackerRole>
{
    public override string Name => MiraLocaleManager.Get("MiraUnleashed.Role.HackerJam", "Jam");
    public override BaseKeybind Keybind => OptionGroupSingleton<HackerOptions>.Instance.SimpleModeJamOnly
        ? Keybinds.SecondaryAction
        : Keybinds.ModifierAction;
    public override Color TextOutlineColor => MiraUnleashedColors.Hacker;
    public override float Cooldown => Math.Clamp(OptionGroupSingleton<HackerOptions>.Instance.JamCooldownSeconds + MapCooldown, 5f, 120f);
    public override float EffectDuration => OptionGroupSingleton<HackerOptions>.Instance.JamDurationSeconds;
    public override LoadableAsset<Sprite> Sprite => MiraUnleashedImpAssets.HackerJamButtonSprite;
    public override bool ZeroIsInfinite { get; set; } = true;

    public override void CreateButton(Transform parent)
    {
        base.CreateButton(parent);
        EnsureChargesInitialized();
    }

    private static void EnsureChargesInitialized()
    {
        var player = PlayerControl.LocalPlayer;
        if (player == null || player.Data?.Role == null)
        {
            return;
        }

        if (player.Data.Role is not HackerRole)
        {
            return;
        }

        var opts = OptionGroupSingleton<HackerOptions>.Instance;
        if (opts.JamMaxCharges <= 0f)
        {
            return;
        }

        if (HackerSystem.GetJamCharges(player.PlayerId) == 0)
        {
            var max = (int)opts.JamMaxCharges;
            var initial = (byte)Mathf.Clamp((int)opts.InitialJamCharges, 0, Math.Max(0, max));
            HackerSystem.SetJamCharges(player.PlayerId, initial);
        }
    }

    public override bool Enabled(RoleBehaviour? role)
    {
        return base.Enabled(role) && OptionGroupSingleton<HackerOptions>.Instance.JamEnabled;
    }

    public override bool CanUse()
    {
        if (!base.CanUse())
        {
            return false;
        }

        if (HackerSystem.IsJammed)
        {
            return false;
        }

        var player = PlayerControl.LocalPlayer;
        if (player == null)
        {
            return false;
        }

        return Timer <= 0f && HackerSystem.GetJamCharges(player.PlayerId) > 0;
    }

    protected override void FixedUpdate(PlayerControl playerControl)
    {
        base.FixedUpdate(playerControl);

        if (Button == null || PlayerControl.LocalPlayer == null)
        {
            return;
        }

        EnsureChargesInitialized();

        var charges = HackerSystem.GetJamCharges(PlayerControl.LocalPlayer.PlayerId);
        Button.usesRemainingText.gameObject.SetActive(true);
        Button.usesRemainingSprite.gameObject.SetActive(true);
        Button.usesRemainingText.text = charges.ToString(CultureInfo.InvariantCulture);

        if (EffectActive && !HackerSystem.IsJammed)
        {
            EffectActive = false;
            Timer = Cooldown;
        }
    }

    protected override void OnClick()
    {
        var player = PlayerControl.LocalPlayer;
        if (player == null)
        {
            return;
        }

        HackerRole.RpcHackerActivateJam(player);
    }
}
