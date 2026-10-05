using System.Collections;
using MiraAPI.LocalSettings;
using MiraAPI.Modifiers;
using MiraAPI.Modifiers.Types;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities;
using MiraUnleashed.Events.Impostor;
using MiraUnleashed.Options.Roles.Impostor;
using TownOfUs.Events.Impostor;
using TownOfUs.Utilities;
using TownOfUs.Utilities.Appearances;
using UnityEngine;

namespace MiraUnleashed.Modifiers;

public sealed class InjectedNauseaModifier : TimedModifier, IVisualAppearance, IInjectedModifier
{
    public override string ModifierName => "Injected (Nausea)";
    public override bool HideOnUi => true;
    public override LoadableAsset<Sprite>? ModifierIcon => null;

    private readonly float _duration;
    private readonly InjectorEffectDurationType _durationType;
    private IEnumerator? _cameraShakeCoroutine;
    private Quaternion _originalCameraRotation;

    public InjectedNauseaModifier(float duration, InjectorEffectDurationType durationType)
    {
        _duration = duration;
        _durationType = durationType;
    }

    public Guid InjectionId { get; set; }
    public float SpeedFactor { get; set; } = 0.7f;
    public float VisionPerc { get; set; } = 0.7f;

    // Camera shake parameters
    private const float ShakeIntensity = 3f; // Maximum rotation angle in degrees
    private const float ShakeSpeed = 2.5f; // Speed of the shake animation

    public override float Duration => _durationType switch
            {
                InjectorEffectDurationType.AllRound => -1f,
                InjectorEffectDurationType.AllGame => -1f,
                InjectorEffectDurationType.SetTime => _duration,
                _ => _duration
            };

    public override bool AutoStart => true;

    public override void OnActivate()
    {
        Player.RawSetAppearance(this);

        if (Player != null && Player.AmOwner && Camera.main != null)
        {
            var localSettings = LocalSettingsTabSingleton<MiraUnleashedLocalSettings>.Instance;
            if (localSettings != null && localSettings.EnableNauseaCameraShake.Value)
            {
                _originalCameraRotation = Camera.main.transform.rotation;
                _cameraShakeCoroutine = Coroutines.Start(CoCameraShake());
            }
        }
    }

    public override void OnDeactivate()
    {
        StopCameraShake();
        Player?.ResetAppearance(fullReset: true);
        if (Player != null && Player.AmOwner)
        {
            InjectorEvents.ShowEffectWoreOffNotification(Player, "MiraUnleashed.Injector.Notification.WoreOffNausea");
        }
    }

    public override void OnMeetingStart()
    {
        StopCameraShake();
        if (_durationType == InjectorEffectDurationType.AllRound)
        {
            Player.RemoveModifier(this);
        }
    }

    private void StopCameraShake()
    {
        if (_cameraShakeCoroutine != null)
        {
            Coroutines.Stop(_cameraShakeCoroutine);
            _cameraShakeCoroutine = null;
        }

        if (Player != null && Player.AmOwner && Camera.main != null)
        {
            Camera.main.transform.rotation = _originalCameraRotation;
        }
    }

    private IEnumerator CoCameraShake()
    {
        if (Camera.main == null)
        {
            yield break;
        }

        var time = 0f;
        var randomOffset = UnityEngine.Random.Range(0f, 1000f); // Random offset for Perlin noise to make each instance unique

        while (Player != null && !Player.HasDied() && Camera.main != null)
        {
            time += Time.deltaTime * ShakeSpeed;

            // Use Perlin noise for smooth, organic camera movement
            var noiseX = (Mathf.PerlinNoise(time + randomOffset, 0f) * 2f) - 1f;
            var noiseY = (Mathf.PerlinNoise(0f, time + randomOffset) * 2f) - 1f;
            var noiseZ = (Mathf.PerlinNoise(time + randomOffset, time + randomOffset) * 2f) - 1f;

            // Apply rotation based on Perlin noise
            var rotationX = noiseY * ShakeIntensity;
            var rotationY = noiseX * ShakeIntensity;
            var rotationZ = noiseZ * ShakeIntensity * 0.5f; // Less Z rotation for more subtle effect

            Camera.main.transform.rotation = _originalCameraRotation * Quaternion.Euler(rotationX, rotationY, rotationZ);

            yield return null;
        }

        // Reset camera rotation when done
        if (Camera.main != null)
        {
            Camera.main.transform.rotation = _originalCameraRotation;
        }
    }

    public VisualAppearance GetVisualAppearance()
    {
        var appearance = Player.GetDefaultAppearance();
        appearance.Speed = SpeedFactor;
        return appearance;
    }

    public string GetEffectDescription()
    {
        return MiraLocaleManager.Get("MiraUnleashed.Injector.EffectDescription.Nausea", "0.7x speed, 0.7x vision, camera shake");
    }
}
