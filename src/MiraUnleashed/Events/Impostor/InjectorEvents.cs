using System.Collections;
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

    public static void ScheduleInjection(PlayerControl injector, PlayerControl target)
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

        ApplyInjectionEffect(pending.Injector, pending.Target, pending.InjectionId);

        if (PendingInjections.TryGetValue(pending.Target.PlayerId, out var pendingInjection2))
        {
            pendingInjection2.RemoveAll(p => p.InjectionId == pending.InjectionId);
            if (pendingInjection2.Count == 0)
            {
                PendingInjections.Remove(pending.Target.PlayerId);
            }
        }
    }

    private static void ApplyInjectionEffect(PlayerControl injector, PlayerControl target, Guid injectionId)
    {
        if (target == null || target.HasDied())
        {
            return;
        }

        var options = OptionGroupSingleton<InjectorOptions>.Instance;
        var duration = options.EffectDuration;
        var durationType = options.EffectDurationType.Value;

        var effects = new List<(float Weight, Func<BaseModifier> CreateModifier, string NotificationKey)>();

        // Negative effects
        effects.Add((options.ChanceInvertedControls, () => new InjectedInvertedControlsModifier(duration, durationType), "MiraUnleashed.Injector.Notification.InvertedControls"));
        effects.Add((options.ChanceLowVision, () => new InjectedLowVisionModifier(duration, durationType), "MiraUnleashed.Injector.Notification.LowVision"));
        effects.Add((options.ChanceSlowness, () => new InjectedSlownessModifier(duration, durationType), "MiraUnleashed.Injector.Notification.Slowness"));
        effects.Add((options.ChanceVeryLowVision, () => new InjectedVeryLowVisionModifier(duration, durationType), "MiraUnleashed.Injector.Notification.VeryLowVision"));
        effects.Add((options.ChanceConfused, () => new InjectedConfusedModifier(duration, durationType), "MiraUnleashed.Injector.Notification.Confused"));

        // Only add NoVent if the player can actually vent
        var canVent = target.Data?.Role != null && (
            target.IsImpostor() ||
            target.Data.Role.CanVent ||
            (target.Data.Role is ICustomRole customRole && customRole.Configuration.CanUseVent)
        );
        if (canVent)
        {
            effects.Add((options.ChanceNoVent, () => new InjectedNoVentModifier(duration, durationType), "MiraUnleashed.Injector.Notification.NoVent"));
        }

        effects.Add((options.ChanceNoUse, () => new InjectedNoUseModifier(duration, durationType), "MiraUnleashed.Injector.Notification.NoUse"));
        effects.Add((options.ChanceNoReport, () => new InjectedNoReportModifier(duration, durationType), "MiraUnleashed.Injector.Notification.NoReport"));
        effects.Add((options.ChanceNausea, () => new InjectedNauseaModifier(duration, durationType), "MiraUnleashed.Injector.Notification.Nausea"));
        effects.Add((options.ChanceWeakness, () => new InjectedWeaknessModifier(duration, durationType), "MiraUnleashed.Injector.Notification.Weakness"));

        // Positive effects (only if enabled)
        if (options.PositiveEffectsEnabled)
        {
            effects.Add((options.ChanceSpeedBoost, () => new InjectedSpeedBoostModifier(duration, durationType), "MiraUnleashed.Injector.Notification.SpeedBoost"));
            effects.Add((options.ChanceVisionBoost, () => new InjectedVisionBoostModifier(duration, durationType), "MiraUnleashed.Injector.Notification.VisionBoost"));
            effects.Add((options.ChanceRegeneration, () => new InjectedRegenerationModifier(duration, durationType), "MiraUnleashed.Injector.Notification.Regeneration"));
        }

        // Calculate total weight
        var totalWeight = effects.Sum(e => e.Weight);

        // If total weight is 0, default to InvertedControls to ensure an effect is always applied
        if (totalWeight <= 0f)
        {
            var defaultModifier = new InjectedInvertedControlsModifier(duration, durationType);
            if (defaultModifier is IInjectedModifier defaultInjectedMod)
            {
                defaultInjectedMod.InjectionId = injectionId;
            }
            target.AddModifier(defaultModifier);
            ShowNotification(target, "MiraUnleashed.Injector.Notification.InvertedControls",
                defaultModifier is IInjectedModifier defaultInjected ? defaultInjected.GetEffectDescription() : string.Empty);
            return;
        }

        var randomValue = Random.RandomRange(0f, totalWeight);
        var cumulativeWeight = 0f;
        BaseModifier? selectedModifier = null;
        string selectedNotificationKey = string.Empty;

        foreach (var (weight, createModifier, notificationKey) in effects)
        {
            cumulativeWeight += weight;
            if (randomValue <= cumulativeWeight)
            {
                selectedModifier = createModifier();
                selectedNotificationKey = notificationKey;
                break;
            }
        }

        if (selectedModifier == null)
        {
            selectedModifier = new InjectedInvertedControlsModifier(duration, durationType);
            selectedNotificationKey = "MiraUnleashed.Injector.Notification.InvertedControls";
        }

        if (selectedModifier is IInjectedModifier injectedMod)
        {
            injectedMod.InjectionId = injectionId;
        }
        target.AddModifier(selectedModifier);
        var effectDesc = selectedModifier is IInjectedModifier injected ? injected.GetEffectDescription() : string.Empty;
        ShowNotification(target, selectedNotificationKey, effectDesc);
    }

    private static void ShowNotification(PlayerControl target, string notificationKey, string effectDescription = "")
    {
        if (target == null || !target.AmOwner)
        {
            return;
        }

        var baseMessage = MiraLocaleManager.Get(notificationKey, notificationKey);
        var message = string.IsNullOrEmpty(effectDescription) ? baseMessage : $"{baseMessage} ({effectDescription})";
        var injectorColor = ColorUtility.ToHtmlStringRGBA(MiraUnleashedColors.Injector);
        var notif = Helpers.CreateAndShowNotification(
            $"<b><color=#{injectorColor}>{message}</color></b>",
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
        public float Delay { get; set; }
        public float ScheduledTime { get; set; }
        public Guid InjectionId { get; set; }
    }
}
