namespace MiraUnleashed.Modifiers;

public interface IInjectedModifier
{
    Guid InjectionId { get; set; }
    string GetEffectDescription();
}
