using MiraUnleashed.Options.Roles.Impostor;

namespace MiraUnleashed.Modifiers;

public sealed class InjectedRegenerationModifier : InjectedModifier
{
    public InjectedRegenerationModifier(float duration, InjectorEffectDurationType durationType)
        : base(InjectorEffectType.Regeneration, duration, durationType)
    {
    }
}
