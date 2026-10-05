using System.Collections;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Keybinds;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities;
using MiraUnleashed.Assets;
using MiraUnleashed.Modules;
using MiraUnleashed.Options.Roles.Neutral;
using MiraUnleashed.Roles.Neutral;
using TownOfUs.Buttons;
using TownOfUs.Utilities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MiraUnleashed.Buttons.Neutral;

public sealed class ScavengerEatButton : TownOfUsRoleButton<ScavengerRole, DeadBody>
{
    private bool _isChanneling;

    public override string Name => MiraLocaleManager.Get("MiraUnleashed.Role.ScavengerEat", "Eat");
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => MiraUnleashedColors.Scavenger;
    public override float Cooldown => Math.Clamp(OptionGroupSingleton<ScavengerOptions>.Instance.EatCooldown + MapCooldown, 5f, 120f);
    public override float EffectDuration => OptionGroupSingleton<ScavengerOptions>.Instance.EatDuration;
    public override LoadableAsset<Sprite> Sprite => MiraUnleashedAssets.ScavengerEatButtonSprite;
    public override float Distance => 1.5f;

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
            if (ScavengerSystem.IsBodyEaten(body.ParentId))
            {
                continue;
            }

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

        if (ScavengerSystem.IsBodyEaten(target.ParentId))
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

        Coroutines.Start(CoChannelEat(Target.ParentId));
    }

    private IEnumerator CoChannelEat(byte bodyId)
    {
        var player = PlayerControl.LocalPlayer;
        if (player == null)
        {
            _isChanneling = false;
            EffectActive = false;
            yield break;
        }

        var options = OptionGroupSingleton<ScavengerOptions>.Instance;
        var channelDuration = options.EatDuration;
        var elapsed = 0f;

        while (elapsed < channelDuration)
        {
            if (player.HasDied() || MeetingHud.Instance != null)
            {
                // Channel cancelled - don't eat
                _isChanneling = false;
                EffectActive = false;
                yield break;
            }

            var body = Object.FindObjectsOfType<DeadBody>().FirstOrDefault(x => x.ParentId == bodyId);
            if (body == null)
            {
                // Body disappeared - don't eat
                _isChanneling = false;
                EffectActive = false;
                yield break;
            }

            if (ScavengerSystem.IsBodyEaten(bodyId))
            {
                // Body already eaten - cancel channel
                _isChanneling = false;
                EffectActive = false;
                yield break;
            }

            var distance = Vector2.Distance(player.GetTruePosition(), body.TruePosition);
            if (distance > Distance)
            {
                // Moved too far - cancel channel and don't eat
                _isChanneling = false;
                EffectActive = false;
                yield break;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        var finalBody = Object.FindObjectsOfType<DeadBody>().FirstOrDefault(x => x.ParentId == bodyId);
        if (finalBody != null && player != null && !player.HasDied())
        {
            var finalDistance = Vector2.Distance(player.GetTruePosition(), finalBody.TruePosition);
            if (finalDistance <= Distance && !ScavengerSystem.IsBodyEaten(bodyId))
            {
                ScavengerRole.RpcScavengerEat(player, bodyId);
            }
        }

        _isChanneling = false;
        EffectActive = false;
        ResetCooldownAndOrEffect();
    }

    public override void OnEffectEnd()
    {
        base.OnEffectEnd();
        _isChanneling = false;
    }
}
