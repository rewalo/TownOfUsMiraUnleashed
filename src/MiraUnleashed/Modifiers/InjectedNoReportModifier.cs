using MiraUnleashed.Options.Roles.Impostor;

namespace MiraUnleashed.Modifiers;

public sealed class InjectedNoReportModifier : InjectedDisabledModifier
{
    public InjectedNoReportModifier(float duration, InjectorEffectDurationType durationType)
        : base(InjectorEffectType.NoReport, duration, durationType)
    {
    }

    public override bool CanReport => false;

    public override bool CanUseAbilities => true;
}
