using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Modifiers.Types;
using Reactor.Utilities.Extensions;
using TownOfUs.Utilities;
using UnityEngine;

namespace MiraUnleashed.Modifiers;

/// <summary>
/// Short-lived arrow pointing at a player.
/// Only shows on the owning client.
/// </summary>
public sealed class PlayerArrowModifier(PlayerControl targetPlayer, Color color, float durationSeconds) : TimedModifier
{
    public override string ModifierName => "Player Arrow";
    public override bool HideOnUi => true;
    public override bool AutoStart => true;
    public override bool RemoveOnComplete => true;
    public override float Duration => Mathf.Max(0.05f, durationSeconds);

    [HideFromIl2Cpp] public bool IsHiddenFromList => true;

    public PlayerControl TargetPlayer { get; } = targetPlayer;
    public Color Color { get; } = color;

    private ArrowBehaviour? _arrow;

    public override void OnActivate()
    {
        base.OnActivate();

        if (!Player.AmOwner)
        {
            return;
        }

        _arrow = MiscUtils.CreateArrow(Player.transform, Color);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (!Player.AmOwner)
        {
            return;
        }

        if (TargetPlayer == null || TargetPlayer.Data == null || TargetPlayer.transform == null)
        {
            ModifierComponent?.RemoveModifier(this);
            return;
        }

        if (_arrow != null)
        {
            _arrow.target = TargetPlayer.transform.position;
            _arrow.Update();
        }
    }

    public override void OnDeactivate()
    {
        base.OnDeactivate();

        if (_arrow != null && !_arrow.IsDestroyedOrNull())
        {
            _arrow.gameObject.Destroy();
            _arrow.Destroy();
        }

        _arrow = null;
    }
}
