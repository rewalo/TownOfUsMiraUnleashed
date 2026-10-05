using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Modifiers.Types;
using Reactor.Utilities.Extensions;
using TownOfUs.Modules;
using TownOfUs.Utilities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MiraUnleashed.Modifiers;

/// <summary>
/// Arrow pointing to a dead body for the Scavenger's Scavenge ability.
/// Handles Submerged map special case where arrows point to elevator if body is on different floor.
/// </summary>
public sealed class ScavengerBodyArrowModifier(DeadBody deadBody, byte bodyId) : TimedModifier
{
    public override string ModifierName => "Scavenger Body Arrow";
    public override bool HideOnUi => true;
    public override bool AutoStart => true;
    public override bool RemoveOnComplete => false;
    public override float Duration => float.MaxValue;

    [HideFromIl2Cpp] public bool IsHiddenFromList => true;

    public DeadBody DeadBody { get; } = deadBody;
    public byte BodyId { get; } = bodyId;

    private ArrowBehaviour? _arrow;
    private Vector3? _elevatorTarget;

    public override void OnActivate()
    {
        base.OnActivate();

        if (!Player.AmOwner)
        {
            return;
        }

        _arrow = MiscUtils.CreateArrow(Player.transform, MiraUnleashedColors.Scavenger);
        UpdateArrowTarget();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (!Player.AmOwner || _arrow == null)
        {
            return;
        }

        var body = Object.FindObjectsOfType<DeadBody>().FirstOrDefault(b => b.ParentId == BodyId);
        if (body == null || body.gameObject == null || !body.gameObject.activeSelf)
        {
            ModifierComponent?.RemoveModifier(this);
            return;
        }

        UpdateArrowTarget();

        if (_arrow != null)
        {
            var target = _elevatorTarget ?? body.transform.position;
            _arrow.target = target;
            _arrow.Update();
        }
    }

    private void UpdateArrowTarget()
    {
        var body = Object.FindObjectsOfType<DeadBody>().FirstOrDefault(b => b.ParentId == BodyId);
        if (body == null || Player == null)
        {
            return;
        }

        if (ModCompatibility.IsSubmerged())
        {
            var playerFloor = Player.transform.position.y > -7f;
            var bodyFloor = body.transform.position.y > -7f;

            if (playerFloor != bodyFloor)
            {
                var elevatorTarget = FindNearestElevator();
                if (elevatorTarget.HasValue)
                {
                    _elevatorTarget = elevatorTarget.Value;
                    return;
                }
            }
        }

        _elevatorTarget = null;
    }

    private Vector3? FindNearestElevator()
    {
        if (!ModCompatibility.IsSubmerged())
        {
            return null;
        }

        var allConsoles = Object.FindObjectsOfType<Console>();
        foreach (var console in allConsoles)
        {
            if (console.name.Contains("Elevator", StringComparison.OrdinalIgnoreCase))
            {
                return console.transform.position;
            }
        }

        return null;
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
