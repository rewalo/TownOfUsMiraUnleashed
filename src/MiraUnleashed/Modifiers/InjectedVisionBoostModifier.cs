using MiraUnleashed.Options.Roles.Impostor;

namespace MiraUnleashed.Modifiers;

public sealed class InjectedVisionBoostModifier : InjectedModifier
{
    public InjectedVisionBoostModifier(float duration, InjectorEffectDurationType durationType)
        : base(InjectorEffectType.VisionBoost, duration, durationType)
    {
    }

    public float VisionPerc { get; set; } = 1.5f;

    protected override void OnEffectRemoved()
    {
        VisionPerc = 1f;
    }
}
