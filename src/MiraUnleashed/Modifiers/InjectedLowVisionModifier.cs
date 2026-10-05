using MiraUnleashed.Options.Roles.Impostor;

namespace MiraUnleashed.Modifiers;

public sealed class InjectedLowVisionModifier : InjectedModifier
{
    public InjectedLowVisionModifier(float duration, InjectorEffectDurationType durationType)
        : base(InjectorEffectType.LowVision, duration, durationType)
    {
    }

    public float VisionPerc { get; set; } = 0.5f;

    protected override void OnEffectRemoved()
    {
        VisionPerc = 1f;
    }
}
