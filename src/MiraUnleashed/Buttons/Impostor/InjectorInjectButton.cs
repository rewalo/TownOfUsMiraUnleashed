using MiraAPI.GameOptions;
using MiraAPI.Keybinds;
using MiraAPI.Modifiers;
using MiraAPI.Utilities.Assets;
using MiraUnleashed.Assets;
using MiraUnleashed.Events.Impostor;
using MiraUnleashed.Options.Roles.Impostor;
using MiraUnleashed.Roles.Impostor;
using TownOfUs.Buttons;
using TownOfUs.Modifiers;
using TownOfUs.Utilities;
using UnityEngine;

namespace MiraUnleashed.Buttons.Impostor;

public sealed class InjectorInjectButton : TownOfUsKillRoleButton<InjectorRole, PlayerControl>
{
    public override string Name => MiraLocaleManager.Get("MiraUnleashed.Role.InjectorInject", "Inject");
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => MiraUnleashedColors.Injector;
    public override float Cooldown => Math.Clamp(OptionGroupSingleton<InjectorOptions>.Instance.InjectCooldown + MapCooldown, 5f, 120f);
    public override LoadableAsset<Sprite> Sprite => MiraUnleashedImpAssets.InjectorInjectButtonSprite;
    public override int MaxUses => (int)OptionGroupSingleton<InjectorOptions>.Instance.InitialUses;

    public override bool ZeroIsInfinite { get; set; } = true;

    public override bool IsTargetValid(PlayerControl? target)
    {
        if (!base.IsTargetValid(target) || target == null)
        {
            return false;
        }

        return CanInjectTarget(target);
    }

    private static bool CanInjectTarget(PlayerControl? target)
    {
        if (target == null)
        {
            return false;
        }

        if (target.IsImpostor())
        {
            return false;
        }

        // Cannot inject shielded players
        if (target.HasModifier<BaseShieldModifier>() || target.HasModifier<FirstDeadShield>())
        {
            return false;
        }

        return true;
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            Error("Injector Inject: Target is null");
            return;
        }

        var player = PlayerControl.LocalPlayer;
        if (player == null)
        {
            Error("Injector Inject: LocalPlayer is null");
            return;
        }

        // Roll the effect on the injector's client so every client applies the same one.
        var effect = InjectorEvents.RollEffect(Target);
        InjectorRole.RpcInjectorInject(player, Target, (byte)effect);
        player.SetKillTimer(player.GetKillCooldown());
    }
}
