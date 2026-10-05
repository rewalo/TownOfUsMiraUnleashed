using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using MiraUnleashed.Roles.Impostor;

namespace MiraUnleashed.Options.Roles.Impostor;

public sealed class WitchOptions : AbstractOptionGroup<WitchRole>
{
    public override string GroupName => MiraLocaleManager.Get("MiraUnleashed.Role.Witch", "Witch");

    [ModdedNumberOption("MiraUnleashed.Options.Witch.SpellCooldown", 5f, 120f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float SpellCooldown { get; set; } = 37.5f;

    [ModdedNumberOption("MiraUnleashed.Options.Witch.AdditionalCooldown", 0f, 30f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float AdditionalCooldown { get; set; } = 2.5f;

    [ModdedNumberOption("MiraUnleashed.Options.Witch.SpellCastingDuration", 0.5f, 5f, 0.5f, MiraNumberSuffixes.Seconds)]
    public float SpellCastingDuration { get; set; } = 2f;

    [ModdedNumberOption("MiraUnleashed.Options.Witch.MeetingsUntilDeath", 1f, 5f, 1f, MiraNumberSuffixes.None)]
    public float MeetingsUntilDeath { get; set; } = 1f;

    [ModdedNumberOption("MiraUnleashed.Options.Witch.SpellRange", 1f, 3f, 0.25f, MiraNumberSuffixes.Multiplier)]
    public float SpellRange { get; set; } = 1.5f;

    [ModdedToggleOption("MiraUnleashed.Options.Witch.SpellResetsKillCooldown")]
    public bool SpellResetsKillCooldown { get; set; } = false;
}
