using MiraUnleashed.Options.Roles.Impostor;

namespace MiraUnleashed.Modifiers;

public sealed class InjectedNoVentModifier : InjectedModifier
{
    public InjectedNoVentModifier(float duration, InjectorEffectDurationType durationType)
        : base(InjectorEffectType.NoVent, duration, durationType)
    {
    }

    public override bool? CanVent()
    {
        return false;
    }
}
