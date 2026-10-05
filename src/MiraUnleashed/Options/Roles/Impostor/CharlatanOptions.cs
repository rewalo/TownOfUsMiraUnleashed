using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using MiraUnleashed.Roles.Impostor;

namespace MiraUnleashed.Options.Roles.Impostor;

public sealed class CharlatanOptions : AbstractOptionGroup<CharlatanRole>
{
    public override string GroupName => MiraLocaleManager.Get("MiraUnleashed.Role.Charlatan", "Charlatan");

    [ModdedNumberOption("MiraUnleashed.Options.Charlatan.DeceiveBaseDuration", 0f, 60f, 1f, MiraNumberSuffixes.Seconds)]
    public float DeceiveBaseDuration { get; set; } = 15f;

    [ModdedNumberOption("MiraUnleashed.Options.Charlatan.DeceiveDurationIncreasePerKill", 0f, 15f, 0.5f, MiraNumberSuffixes.Seconds)]
    public float DeceiveDurationIncreasePerKill { get; set; } = 2.5f;

    [ModdedNumberOption("MiraUnleashed.Options.Charlatan.ConcealUses", 0f, 10f, 1f, MiraNumberSuffixes.None)]
    public float ConcealUses { get; set; } = 2f;

    [ModdedNumberOption("MiraUnleashed.Options.Charlatan.ConcealChargesPerKill", 1f, 10f, 1f, MiraNumberSuffixes.None)]
    public float ConcealChargesPerKill { get; set; } = 1f;

    [ModdedEnumOption("MiraUnleashed.Options.Charlatan.ConcealReportRange", typeof(ReportRangeType), ["MiraUnleashed.Options.Charlatan.ConcealReportRangeEnum.ExtremelyShort", "MiraUnleashed.Options.Charlatan.ConcealReportRangeEnum.VeryShort", "MiraUnleashed.Options.Charlatan.ConcealReportRangeEnum.Short"])]
    public ReportRangeType ConcealReportRange { get; set; } = ReportRangeType.VeryShort;

    [ModdedNumberOption("MiraUnleashed.Options.Charlatan.ConcealChannelDuration", 1f, 15f, 0.5f, MiraNumberSuffixes.Seconds)]
    public float ConcealChannelDuration { get; set; } = 2.5f;

    [ModdedNumberOption("MiraUnleashed.Options.Charlatan.ConcealCooldown", 5f, 300f, 0.5f, MiraNumberSuffixes.Seconds)]
    public float ConcealCooldown { get; set; } = 30f;
}

public enum ReportRangeType
{
    ExtremelyShort = 0,
    VeryShort = 1,
    Short = 2
}
