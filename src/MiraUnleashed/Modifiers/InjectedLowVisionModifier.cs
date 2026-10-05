using MiraAPI.Modifiers;
using MiraAPI.Modifiers.Types;
using MiraAPI.Utilities.Assets;
using MiraUnleashed.Events.Impostor;
using MiraUnleashed.Options.Roles.Impostor;
using TownOfUs.Events.Impostor;
using UnityEngine;

namespace MiraUnleashed.Modifiers;

public sealed class InjectedLowVisionModifier : TimedModifier, IInjectedModifier
{
    public override string ModifierName => "Injected (Low Vision)";
    public override bool HideOnUi => true;
    public override LoadableAsset<Sprite>? ModifierIcon => null;

    private readonly float _duration;
    private readonly InjectorEffectDurationType _durationType;

    public InjectedLowVisionModifier(float duration, InjectorEffectDurationType durationType)
    {
        _duration = duration;
        _durationType = durationType;
    }

    public Guid InjectionId { get; set; }
    public float VisionPerc { get; set; } = 0.5f;

    public override float Duration => _durationType switch
            {
                InjectorEffectDurationType.AllRound => -1f,
                InjectorEffectDurationType.AllGame => -1f,
                InjectorEffectDurationType.SetTime => _duration,
                _ => _duration
            };

    public override bool AutoStart => true;

    public override void OnMeetingStart()
    {
        if (_durationType == InjectorEffectDurationType.AllRound)
        {
            Player.RemoveModifier(this);
        }
    }

    public override void OnDeactivate()
    {
        VisionPerc = 1f;
        if (Player != null && Player.AmOwner)
        {
            InjectorEvents.ShowEffectWoreOffNotification(Player, "MiraUnleashed.Injector.Notification.WoreOffLowVision");
        }
    }

    public string GetEffectDescription()
    {
        return MiraLocaleManager.Get("MiraUnleashed.Injector.EffectDescription.LowVision", "0.5x vision");
    }
}
