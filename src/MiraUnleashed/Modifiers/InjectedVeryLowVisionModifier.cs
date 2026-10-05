using MiraUnleashed.Options.Roles.Impostor;

namespace MiraUnleashed.Modifiers;

public sealed class InjectedVeryLowVisionModifier : InjectedModifier
{
    public InjectedVeryLowVisionModifier(float duration, InjectorEffectDurationType durationType)
        : base(InjectorEffectType.VeryLowVision, duration, durationType)
    {
    }

    public float VisionPerc { get; set; } = 0.1f;

    protected override void OnEffectRemoved()
    {
        VisionPerc = 1f;
    }
}
