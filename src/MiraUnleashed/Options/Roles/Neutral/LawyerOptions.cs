using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using MiraUnleashed.Roles.Neutral;
using TownOfUs.Options.Roles.Neutral;

namespace MiraUnleashed.Options.Roles.Neutral;

public sealed class LawyerOptions : AbstractOptionGroup<LawyerRole>
{
    public override string GroupName => MiraLocaleManager.Get("MiraUnleashed.Role.Lawyer", "Lawyer");

    [ModdedEnumOption("MiraUnleashed.Options.Lawyer.WinMode", typeof(LawyerWinMode),
        ["MiraUnleashed.Options.Lawyer.WinMode.WithClient", "MiraUnleashed.Options.Lawyer.WinMode.StealWin"])]
    public LawyerWinMode WinMode { get; set; } = LawyerWinMode.WinWithClient;

    [ModdedNumberOption("MiraUnleashed.Options.Lawyer.KillerClientChance", 0f, 100f, 10f, MiraNumberSuffixes.Percent)]
    public float KillerClientChance { get; set; } = 70f;

    public ModdedEnumOption OnClientDeath { get; } =
        new("MiraUnleashed.Options.Lawyer.BecomesOnClientDeath", (int)BecomeOptions.Crew, typeof(BecomeOptions),
            ["CrewmateKeyword", "TouRoleAmnesiac", "TouRoleSurvivor", "TouRoleMercenary", "TouRoleJester"])
        {
            Visible = () => !OptionGroupSingleton<LawyerOptions>.Instance.DieOnClientDeath,
        };

    [ModdedToggleOption("MiraUnleashed.Options.Lawyer.DieOnClientDeath")]
    public bool DieOnClientDeath { get; set; }

    [ModdedToggleOption("MiraUnleashed.Options.Lawyer.GetVotedOutWithClient")]
    public bool GetVotedOutWithClient { get; set; } = true;

    [ModdedToggleOption("MiraUnleashed.Options.Lawyer.CanSeeClientRole")]
    public bool CanSeeClientRole { get; set; } = true;

    [ModdedNumberOption("MiraUnleashed.Options.Lawyer.MaxObjections", 0f, 10f, 1f, MiraNumberSuffixes.None, "0")]
    public float MaxObjections { get; set; } = 1f;

    [ModdedNumberOption("MiraUnleashed.Options.Lawyer.MaxObjectionsPerMeeting", 0f, 10f, 1f, MiraNumberSuffixes.None, "0")]
    public float MaxObjectionsPerMeeting { get; set; } = 1f;

    [ModdedToggleOption("MiraUnleashed.Options.Lawyer.ObjectionPreventsSameVote")]
    public bool ObjectionPreventsSameVote { get; set; } = true;

    [ModdedToggleOption("MiraUnleashed.Options.Lawyer.PrivateChat")]
    public bool LawyerChat { get; set; } = true;
}

public enum LawyerWinMode
{
    WinWithClient,
    StealWin
}
