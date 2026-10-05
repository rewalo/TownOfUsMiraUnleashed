using System.Collections;
using System.Globalization;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Events.Vanilla.Meeting;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using Reactor.Utilities;
using MiraUnleashed.Assets;
using MiraUnleashed.Buttons.Impostor;
using MiraUnleashed.Modifiers;
using MiraUnleashed.Options.Roles.Impostor;
using MiraUnleashed.Roles.Impostor;
using TownOfUs.Buttons;
using TownOfUs.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MiraUnleashed.Events.Impostor;

public static class InjectorEvents
{
    private static readonly Dictionary<byte, List<PendingInjection>> PendingInjections = new();

    public static void ScheduleInjection(PlayerControl injector, PlayerControl target, InjectorEffectType effectType)
    {
        if (target == null || target.HasDied() || injector == null)
        {
            return;
        }

        var options = OptionGroupSingleton<InjectorOptions>.Instance;
        var delay = options.EffectDelay;

        var pending = new PendingInjection
        {
            Injector = injector,
            Target = target,
            Effect = effectType,
            Delay = delay,
            ScheduledTime = Time.time,
            InjectionId = Guid.NewGuid()
        };

        if (!PendingInjections.TryGetValue(target.PlayerId, out var pendingInjection))
        {
            pendingInjection = new List<PendingInjection>();
        }
        pendingInjection.Add(pending);
        Coroutines.Start(CoApplyInjection(pending));
    }

    private static IEnumerator CoApplyInjection(PendingInjection pending)
    {
        yield return new WaitForSeconds(pending.Delay);

        if (pending.Target == null || pending.Target.HasDied() || pending.Injector == null || pending.Injector.HasDied())
        {
            if (pending.Target != null && PendingInjections.TryGetValue(pending.Target.PlayerId, out var pendingInjection))
            {
                pendingInjection.RemoveAll(p => p.InjectionId == pending.InjectionId);
                if (pendingInjection.Count == 0)
                {
                    PendingInjections.Remove(pending.Target.PlayerId);
                }
            }
            yield break;
        }

        ApplyInjectionEffect(pending.Injector, pending.Target, pending.InjectionId, pending.Effect);

        if (PendingInjections.TryGetValue(pending.Target.PlayerId, out var pendingInjection2))
        {
            pendingInjection2.RemoveAll(p => p.InjectionId == pending.InjectionId);
            if (pendingInjection2.Count == 0)
            {
                PendingInjections.Remove(pending.Target.PlayerId);
            }
        }
    }

    public static InjectorEffectType RollEffect(PlayerControl target)
    {
        var options = OptionGroupSingleton<InjectorOptions>.Instance;

        var effects = new List<(float Weight, InjectorEffectType Type)>
        {
            // Negative effects
            (options.ChanceInvertedControls, InjectorEffectType.InvertedControls),
            (options.ChanceLowVision, InjectorEffectType.LowVision),
            (options.ChanceSlowness, InjectorEffectType.Slowness),
            (options.ChanceVeryLowVision, InjectorEffectType.VeryLowVision),
            (options.ChanceConfused, InjectorEffectType.Confused)
        };

        // Only add NoVent if the player can actually vent
        var canVent = target?.Data?.Role != null && (
            target.IsImpostor() ||
            target.Data.Role.CanVent ||
            (target.Data.Role is ICustomRole customRole && customRole.Configuration.CanUseVent)
        );
        if (canVent)
        {
            effects.Add((options.ChanceNoVent, InjectorEffectType.NoVent));
        }

        effects.Add((options.ChanceNoUse, InjectorEffectType.NoUse));
        effects.Add((options.ChanceNoReport, InjectorEffectType.NoReport));
        effects.Add((options.ChanceNausea, InjectorEffectType.Nausea));
        effects.Add((options.ChanceWeakness, InjectorEffectType.Weakness));

        // Positive effects (only if enabled)
        if (options.PositiveEffectsEnabled)
        {
            effects.Add((options.ChanceSpeedBoost, InjectorEffectType.SpeedBoost));
            effects.Add((options.ChanceVisionBoost, InjectorEffectType.VisionBoost));
            effects.Add((options.ChanceRegeneration, InjectorEffectType.Regeneration));
        }

        // If total weight is 0, default to InvertedControls to ensure an effect is always applied
        var totalWeight = effects.Sum(e => e.Weight);
        if (totalWeight <= 0f)
        {
            return InjectorEffectType.InvertedControls;
        }

        var randomValue = Random.RandomRange(0f, totalWeight);
        var cumulativeWeight = 0f;

        foreach (var (weight, type) in effects)
        {
            cumulativeWeight += weight;
            if (randomValue <= cumulativeWeight)
            {
                return type;
            }
        }

        return InjectorEffectType.InvertedControls;
    }

    private static IInjectedModifier CreateModifier(InjectorEffectType effect, float duration, InjectorEffectDurationType durationType)
    {
        return effect switch
        {
            InjectorEffectType.LowVision => new InjectedLowVisionModifier(duration, durationType),
            InjectorEffectType.Slowness => new InjectedSlownessModifier(duration, durationType),
            InjectorEffectType.VeryLowVision => new InjectedVeryLowVisionModifier(duration, durationType),
            InjectorEffectType.Confused => new InjectedConfusedModifier(duration, durationType),
            InjectorEffectType.NoVent => new InjectedNoVentModifier(duration, durationType),
            InjectorEffectType.NoUse => new InjectedNoUseModifier(duration, durationType),
            InjectorEffectType.NoReport => new InjectedNoReportModifier(duration, durationType),
            InjectorEffectType.Nausea => new InjectedNauseaModifier(duration, durationType),
            InjectorEffectType.Weakness => new InjectedWeaknessModifier(duration, durationType),
            InjectorEffectType.SpeedBoost => new InjectedSpeedBoostModifier(duration, durationType),
            InjectorEffectType.VisionBoost => new InjectedVisionBoostModifier(duration, durationType),
            InjectorEffectType.Regeneration => new InjectedRegenerationModifier(duration, durationType),
            _ => new InjectedInvertedControlsModifier(duration, durationType)
        };
    }

    private static void ApplyInjectionEffect(PlayerControl injector, PlayerControl target, Guid injectionId, InjectorEffectType effect)
    {
        if (target == null || target.HasDied())
        {
            return;
        }

        var options = OptionGroupSingleton<InjectorOptions>.Instance;
        var duration = options.EffectDuration;
        var durationType = options.EffectDurationType.Value;

        var modifier = CreateModifier(effect, duration, durationType);
        modifier.InjectionId = injectionId;
        target.AddModifier((BaseModifier)modifier);

        ShowInjectionNotification(target, effect, duration, durationType);
    }

    private static void ShowInjectionNotification(PlayerControl target, InjectorEffectType effect, float duration, InjectorEffectDurationType durationType)
    {
        if (target == null || !target.AmOwner)
        {
            return;
        }

        var flavour = MiraLocaleManager.Get($"MiraUnleashed.Injector.Notification.{effect}");
        var effectName = MiraLocaleManager.Get($"MiraUnleashed.Options.Injector.EffectType.{effect}");
        var effectDescription = MiraLocaleManager.Get($"MiraUnleashed.Injector.EffectDescription.{effect}");
        var durationText = durationType switch
        {
            InjectorEffectDurationType.SetTime => MiraLocaleManager.Get("MiraUnleashed.Injector.Duration.Lasts")
                .Replace("<time>", Mathf.CeilToInt(duration).ToString(CultureInfo.InvariantCulture)),
            InjectorEffectDurationType.AllRound => MiraLocaleManager.Get("MiraUnleashed.Injector.Duration.AllRound"),
            _ => MiraLocaleManager.Get("MiraUnleashed.Injector.Duration.AllGame")
        };

        var explicitLine = MiraLocaleManager.Get("MiraUnleashed.Injector.Notification.Injected")
            .Replace("<effect>", effectName)
            .Replace("<description>", effectDescription)
            .Replace("<duration>", durationText);

        var injectorColor = ColorUtility.ToHtmlStringRGBA(MiraUnleashedColors.Injector);
        var notif = Helpers.CreateAndShowNotification(
            $"<b><color=#{injectorColor}>{flavour}\n{explicitLine}</color></b>",
            Color.white,
            new Vector3(0f, 1f, -20f),
            spr: MiraUnleashedImpAssets.InjectorRole.LoadAsset());

        notif.AdjustNotification();
    }

    public static void ShowEffectWoreOffNotification(PlayerControl target, string notificationKey)
    {
        if (target == null || !target.AmOwner)
        {
            return;
        }

        var message = MiraLocaleManager.Get(notificationKey, notificationKey);
        var injectorColor = ColorUtility.ToHtmlStringRGBA(MiraUnleashedColors.Injector);
        var notif = Helpers.CreateAndShowNotification(
            $"<b><color=#{injectorColor}>{message}</color></b>",
            Color.white,
            new Vector3(0f, 1f, -20f),
            spr: MiraUnleashedImpAssets.InjectorRole.LoadAsset());

        notif.AdjustNotification();
    }

    [RegisterEvent]
    public static void EjectionEventHandler(EjectionEvent @event)
    {
        var exiled = @event.ExileController?.initData?.networkedPlayer?.Object;
        if (exiled == null)
        {
            return;
        }

        var keysToCheck = PendingInjections.Keys.ToList();
        foreach (var key in keysToCheck)
        {
            if (PendingInjections[key].Any(p => p.Injector?.PlayerId == exiled.PlayerId))
            {
                PendingInjections[key].RemoveAll(p => p.Injector?.PlayerId == exiled.PlayerId);
                if (PendingInjections[key].Count == 0)
                {
                    PendingInjections.Remove(key);
                }
            }
        }
    }

    [RegisterEvent]
    public static void RoundStartEventHandler(RoundStartEvent @event)
    {
        if (!@event.TriggeredByIntro)
        {
            return;
        }

        if (PlayerControl.LocalPlayer?.Data?.Role is not InjectorRole)
        {
            return;
        }

        var btn = CustomButtonSingleton<InjectorInjectButton>.Instance;
        var options = OptionGroupSingleton<InjectorOptions>.Instance;
        btn.SetUses((int)options.InitialUses);
    }

    [RegisterEvent]
    public static void AfterMurderEventHandler(AfterMurderEvent @event)
    {
        var source = @event.Source;
        if (source == null || source.Data.Role is not InjectorRole)
        {
            return;
        }

        var options = OptionGroupSingleton<InjectorOptions>.Instance;
        if (options.UsesPerKill <= 0)
        {
            return;
        }

        var injectButton = CustomButtonSingleton<InjectorInjectButton>.Instance;
        if (injectButton != null && injectButton.LimitedUses)
        {
            injectButton.UsesLeft += (int)options.UsesPerKill;
            injectButton.SetUses(injectButton.UsesLeft);
        }
    }

    private sealed class PendingInjection
    {
        public PlayerControl? Injector { get; set; }
        public PlayerControl? Target { get; set; }
        public InjectorEffectType Effect { get; set; }
        public float Delay { get; set; }
        public float ScheduledTime { get; set; }
        public Guid InjectionId { get; set; }
    }
}
