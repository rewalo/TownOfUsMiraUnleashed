using MiraAPI.Modifiers.Types;
using MiraUnleashed.Options.Roles.Impostor;
using TownOfUs.Events.Impostor;
using TownOfUs.Utilities;
using TownOfUs.Utilities.Appearances;

namespace MiraUnleashed.Modifiers;

public sealed class InjectedWeaknessModifier : InjectedModifier, IVisualAppearance
{
    public InjectedWeaknessModifier(float duration, InjectorEffectDurationType durationType)
        : base(InjectorEffectType.Weakness, duration, durationType)
    {
    }

    public float SpeedFactor { get; set; } = 0.6f;

    public override void OnActivate()
    {
        Player.RawSetAppearance(this);
    }

    protected override void OnEffectRemoved()
    {
        Player?.ResetAppearance(fullReset: true);
    }

    public VisualAppearance GetVisualAppearance()
    {
        var appearance = Player.GetDefaultAppearance();
        appearance.Speed = SpeedFactor;
        return appearance;
    }
}
