using MiraUnleashed.Options.Roles.Impostor;

namespace MiraUnleashed.Modifiers;

public sealed class InjectedNoUseModifier : InjectedDisabledModifier
{
    public InjectedNoUseModifier(float duration, InjectorEffectDurationType durationType)
        : base(InjectorEffectType.NoUse, duration, durationType)
    {
    }

    public override bool CanUseAbilities => false;
}
