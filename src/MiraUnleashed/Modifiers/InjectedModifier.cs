using System.Globalization;
using MiraAPI.Modifiers;
using MiraAPI.Modifiers.Types;
using MiraAPI.PluginLoading;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using MiraUnleashed.Assets;
using MiraUnleashed.Events.Impostor;
using MiraUnleashed.Options.Roles.Impostor;
using UnityEngine;

namespace MiraUnleashed.Modifiers;

[MiraIgnore]
public abstract class InjectedModifier : TimedModifier, IInjectedModifier
{
    private readonly float _duration;

    protected InjectedModifier(InjectorEffectType effect, float duration, InjectorEffectDurationType durationType)
    {
        Effect = effect;
        _duration = duration;
        DurationType = durationType;
    }

    public InjectorEffectType Effect { get; }

    protected InjectorEffectDurationType DurationType { get; }

    public Guid InjectionId { get; set; }

    public string EffectName => MiraLocaleManager.Get($"MiraUnleashed.Options.Injector.EffectType.{Effect}");

    public override string ModifierName =>
        MiraLocaleManager.Get("MiraUnleashed.Injector.Modifier.Name").Replace("<effect>", EffectName);

    public override float Duration => DurationType == InjectorEffectDurationType.SetTime ? _duration : 0f;

    public override bool AutoStart => DurationType == InjectorEffectDurationType.SetTime;

    public override bool RemoveOnComplete => true;

    public override bool HideOnUi => false;

    public override LoadableAsset<Sprite>? ModifierIcon => MiraUnleashedImpAssets.InjectorInjectButtonSprite;

    public string RemainingText => DurationType switch
    {
        InjectorEffectDurationType.SetTime => MiraLocaleManager.Get("MiraUnleashed.Injector.Duration.SetTime")
            .Replace("<time>", Mathf.CeilToInt(TimeRemaining).ToString(CultureInfo.InvariantCulture)),
        InjectorEffectDurationType.AllRound => MiraLocaleManager.Get("MiraUnleashed.Injector.Duration.AllRound"),
        _ => MiraLocaleManager.Get("MiraUnleashed.Injector.Duration.AllGame")
    };

    public string GetEffectDescription() => MiraLocaleManager.Get($"MiraUnleashed.Injector.EffectDescription.{Effect}");

    public override string GetDescription() => $"{GetEffectDescription()}\n{RemainingText}";

    public override void OnMeetingStart()
    {
        if (DurationType == InjectorEffectDurationType.AllRound)
        {
            Player?.RemoveModifier(this);
        }
    }

    public override void OnDeactivate()
    {
        if (Player != null && Player.AmOwner)
        {
            InjectorEvents.ShowEffectWoreOffNotification(Player, $"MiraUnleashed.Injector.Notification.WoreOff{Effect}");
        }

        OnEffectRemoved();
        base.OnDeactivate();
    }

    protected virtual void OnEffectRemoved()
    {
    }
}
