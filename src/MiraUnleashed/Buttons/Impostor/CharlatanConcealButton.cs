using System.Collections;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Keybinds;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities;
using MiraUnleashed.Assets;
using MiraUnleashed.Modules;
using MiraUnleashed.Options.Roles.Impostor;
using MiraUnleashed.Roles.Impostor;
using TownOfUs.Buttons;
using TownOfUs.Utilities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MiraUnleashed.Buttons.Impostor;

public sealed class CharlatanConcealButton : TownOfUsRoleButton<CharlatanRole, DeadBody>
{
    private bool _isChanneling;

    public override string Name => MiraLocaleManager.Get("MiraUnleashed.Role.CharlatanConceal", "Conceal");
    public override BaseKeybind Keybind => Keybinds.TertiaryAction;
    public override Color TextOutlineColor => MiraUnleashedColors.Charlatan;
    public override float Cooldown => Math.Clamp(OptionGroupSingleton<CharlatanOptions>.Instance.ConcealCooldown + MapCooldown, 5f, 120f);
    public override float EffectDuration => OptionGroupSingleton<CharlatanOptions>.Instance.ConcealChannelDuration;
    public override LoadableAsset<Sprite> Sprite => MiraUnleashedImpAssets.ConcealButtonSprite;
    public override float Distance => 2f;

    public override bool ZeroIsInfinite { get; set; } = true;

    public override int MaxUses => (int)OptionGroupSingleton<CharlatanOptions>.Instance.ConcealUses;

    public override DeadBody? GetTarget()
    {
        if (PlayerControl.LocalPlayer == null)
        {
            return null;
        }

        var player = PlayerControl.LocalPlayer;
        var allBodies = Object.FindObjectsOfType<DeadBody>();
        DeadBody? closest = null;
        var closestDistance = float.MaxValue;

        foreach (var body in allBodies)
        {
            var distance = Vector2.Distance(player.GetTruePosition(), body.TruePosition);
            if (distance <= Distance && distance < closestDistance)
            {
                closest = body;
                closestDistance = distance;
            }
        }

        return closest;
    }

    public override bool IsTargetValid(DeadBody? target)
    {
        if (target == null || PlayerControl.LocalPlayer == null)
        {
            return false;
        }

        if (UsesLeft <= 0 && LimitedUses)
        {
            return false;
        }

        if (CharlatanBodySystem.IsBodyTracked(target.ParentId))
        {
            return false;
        }

        var player = PlayerControl.LocalPlayer;
        var distance = Vector2.Distance(player.GetTruePosition(), target.TruePosition);
        return distance <= Distance;
    }

    public override bool CanUse()
    {
        if (!base.CanUse())
        {
            return false;
        }

        if (_isChanneling)
        {
            return true;
        }

        if (UsesLeft <= 0 && LimitedUses)
        {
            return false;
        }

        return true;
    }

    public override void ClickHandler()
    {
        if (!CanClick())
        {
            return;
        }

        if (Target == null)
        {
            return;
        }

        if (_isChanneling)
        {
            return;
        }

        OnClick();
    }

    protected override void OnClick()
    {
        var player = PlayerControl.LocalPlayer;
        if (player == null || Target == null)
        {
            return;
        }

        _isChanneling = true;
        EffectActive = true;
        Timer = EffectDuration;
        Button?.SetDisabled();

        CharlatanRole.RpcConcealStart(player, Target.ParentId);
        Coroutines.Start(CoChannelConceal(Target.ParentId));
    }

    private IEnumerator CoChannelConceal(byte bodyId)
    {
        var player = PlayerControl.LocalPlayer;
        if (player == null)
        {
            yield break;
        }

        var options = OptionGroupSingleton<CharlatanOptions>.Instance;
        var channelDuration = options.ConcealChannelDuration;
        var elapsed = 0f;

        while (elapsed < channelDuration)
        {
            var cancelled = player.HasDied() || MeetingHud.Instance != null;

            var body = Object.FindObjectsOfType<DeadBody>().FirstOrDefault(x => x.ParentId == bodyId);
            if (body == null)
            {
                cancelled = true;
            }
            else
            {
                var distance = Vector2.Distance(player.GetTruePosition(), body.TruePosition);
                if (distance > Distance)
                {
                    cancelled = true;
                }
            }

            if (cancelled)
            {
                CharlatanRole.RpcConcealCancel(player, bodyId);
                yield break;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        var finalBody = Object.FindObjectsOfType<DeadBody>().FirstOrDefault(x => x.ParentId == bodyId);
        if (finalBody != null && !player.HasDied() &&
            Vector2.Distance(player.GetTruePosition(), finalBody.TruePosition) <= Distance)
        {
            CharlatanRole.RpcConcealComplete(player, bodyId);
        }
        else
        {
            CharlatanRole.RpcConcealCancel(player, bodyId);
        }
    }

    public void OnConcealCompleted()
    {
        _isChanneling = false;
        EffectActive = false;

        if (UsesLeft > 0 && LimitedUses)
        {
            UsesLeft--;
            SetUses(UsesLeft);
        }

        Timer = Cooldown;
    }

    public void OnConcealCancelled()
    {
        _isChanneling = false;
        EffectActive = false;
        Timer = 0f;
    }

    public override void OnEffectEnd()
    {
        base.OnEffectEnd();
        _isChanneling = false;
    }
}
