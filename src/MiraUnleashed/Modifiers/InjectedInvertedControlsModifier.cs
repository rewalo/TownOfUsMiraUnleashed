using MiraAPI.Modifiers.Types;
using MiraUnleashed.Options.Roles.Impostor;
using TownOfUs.Events.Impostor;
using TownOfUs.Utilities;
using TownOfUs.Utilities.Appearances;

namespace MiraUnleashed.Modifiers;

public sealed class InjectedInvertedControlsModifier : InjectedModifier, IVisualAppearance
{
    public InjectedInvertedControlsModifier(float duration, InjectorEffectDurationType durationType)
        : base(InjectorEffectType.InvertedControls, duration, durationType)
    {
    }

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
        appearance.Speed = -1;
        return appearance;
    }
}
