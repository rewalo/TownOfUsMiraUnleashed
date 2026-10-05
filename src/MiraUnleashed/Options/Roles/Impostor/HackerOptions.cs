using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using MiraUnleashed.Roles.Impostor;

namespace MiraUnleashed.Options.Roles.Impostor;

public sealed class HackerOptions : AbstractOptionGroup<HackerRole>
{
    public override string GroupName => MiraLocaleManager.Get("MiraUnleashed.Role.Hacker", "Hacker");

    [ModdedToggleOption("MiraUnleashed.Options.Hacker.SimpleModeJamOnly")]
    public bool SimpleModeJamOnly { get; set; } = false;

    [ModdedNumberOption("MiraUnleashed.Options.Hacker.MaxBatterySeconds", 3f, 15f, 1f, MiraNumberSuffixes.Seconds)]
    public float MaxBatterySeconds { get; set; } = 10f;

    [ModdedNumberOption("MiraUnleashed.Options.Hacker.BatteryPerDownloadSecond", 1f, 4f, 1f, MiraNumberSuffixes.Seconds)]
    public float BatteryPerDownloadSecond { get; set; } = 2f;

    [ModdedNumberOption("MiraUnleashed.Options.Hacker.DownloadRange", 0.5f, 2.5f, 0.25f, MiraNumberSuffixes.None)]
    public float DownloadRange { get; set; } = 1f;

    [ModdedToggleOption("MiraUnleashed.Options.Hacker.MoveWithDevice")]
    public bool MoveWithDevice { get; set; } = true;

    [ModdedNumberOption("MiraUnleashed.Options.Hacker.InitialJamCharges", 0f, 10f, 1f, MiraNumberSuffixes.None)]
    public float InitialJamCharges { get; set; } = 3f;

    [ModdedNumberOption("MiraUnleashed.Options.Hacker.JamChargesPerKill", 0f, 5f, 1f, MiraNumberSuffixes.None)]
    public float JamChargesPerKill { get; set; } = 1f;

    [ModdedNumberOption("MiraUnleashed.Options.Hacker.JamMaxCharges", 1f, 10f, 1f, MiraNumberSuffixes.None)]
    public float JamMaxCharges { get; set; } = 6f;

    [ModdedNumberOption("MiraUnleashed.Options.Hacker.JamCooldown", 10f, 35f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float JamCooldownSeconds { get; set; } = 25f;

    [ModdedNumberOption("MiraUnleashed.Options.Hacker.JamDuration", 5f, 20f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float JamDurationSeconds { get; set; } = 15f;

    public bool JamEnabled =>
        JamMaxCharges > 0f && (SimpleModeJamOnly || JamChargesPerKill > 0f || InitialJamCharges > 0f);
}
