using MiraAPI.Modifiers;
using MiraUnleashed.Options.Modifiers;
using TownOfUs.Utilities;
using TownOfUs.Utilities.Appearances;
using UnityEngine;

namespace MiraUnleashed.Modifiers;

public sealed class SpitefulEffectModifier : BaseModifier, IVisualAppearance
{
    public override string ModifierName => "Spiteful (Effect)";
    public override bool HideOnUi => false;
    public override bool Unique => true; // Prevent duplicate modifiers

    private readonly SpitefulEffectType _effectType;
    private readonly SpitefulDurationType _durationType;
    private int _roundsRemaining;
    private readonly float _impact;

    public SpitefulEffectModifier(SpitefulEffectType effectType, SpitefulDurationType durationType, int rounds, float impact)
    {
        _effectType = effectType;
        _durationType = durationType;
        _roundsRemaining = rounds;
        _impact = impact;
    }

    public SpitefulEffectType EffectType => _effectType;
    public SpitefulDurationType DurationType => _durationType;
    public int RoundsRemaining => _roundsRemaining;
    public float ImpactPercent => _impact; // Impact as percentage (15-75%)

    // Vision multiplier: Honest scaling - 25% impact = 0.75x vision (25% reduction), 75% impact = 0.25x vision (75% reduction)
    // Formula: 1 - (impact/100), clamped to minimum 0.1x
    public float VisionPerc => Math.Max(0.1f, 1f - (_impact / 100f));

    // Speed multiplier: Same honest scaling as vision
    public float SpeedMultiplier => Math.Max(0.1f, 1f - (_impact / 100f));

    // Cooldown multiplier: Honest scaling - 25% impact = 1.25x cooldown (25% increase), 75% impact = 1.75x cooldown (75% increase)
    // Formula: 1 + (impact/100)
    public float CooldownMultiplier => 1f + (_impact / 100f);

    public void DecrementRounds()
    {
        _roundsRemaining--;
    }

    public override void OnActivate()
    {
        base.OnActivate();

        // Set appearance for slowness effect
        if (_effectType == SpitefulEffectType.Slowness)
        {
            Player.RawSetAppearance(this);
        }
    }

    public override void OnDeactivate()
    {
        if (_effectType == SpitefulEffectType.Slowness)
        {
            Player?.ResetAppearance(fullReset: true);
        }

        base.OnDeactivate();
    }

    public VisualAppearance GetVisualAppearance()
    {
        var appearance = Player.GetDefaultAppearance();
        if (_effectType == SpitefulEffectType.Slowness)
        {
            appearance.Speed = SpeedMultiplier;
        }
        return appearance;
    }

    public string GetEffectDescription()
    {
        return _effectType switch
        {
            SpitefulEffectType.LowerVision => MiraLocaleManager.Get("MiraUnleashed.Modifier.Spiteful.Effect.LowerVision"),
            SpitefulEffectType.Slowness => MiraLocaleManager.Get("MiraUnleashed.Modifier.Spiteful.Effect.Slowness"),
            SpitefulEffectType.IncreasedCooldowns => MiraLocaleManager.Get("MiraUnleashed.Modifier.Spiteful.Effect.IncreasedCooldowns"),
            _ => string.Empty
        };
    }
}
