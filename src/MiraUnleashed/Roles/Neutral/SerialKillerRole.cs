using AmongUs.GameOptions;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Patches.Stubs;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using MiraUnleashed.Modifiers;
using MiraUnleashed.Options.Roles.Neutral;
using TownOfUs.Assets;
using TownOfUs.Extensions;
using TownOfUs.Modules.Wiki;
using TownOfUs.Roles;
using TownOfUs.Roles.Neutral;
using TownOfUs.Utilities;
using UnityEngine;

namespace MiraUnleashed.Roles.Neutral;

public sealed class SerialKillerRole(IntPtr cppPtr) : NeutralRole(cppPtr), IMiraUnleashedRole, IWikiDiscoverable, IDoomable
{
    public DoomableType DoomHintType => DoomableType.Fearmonger;

    public string GetAdvancedDescription()
    {
        return ((ICustomRole)this).RoleWikiDescription + MiscUtils.AppendOptionsText(GetType());
    }

    public Color RoleColor => MiraUnleashedColors.SerialKiller;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;
    public RoleAlignment RoleAlignment => RoleAlignment.NeutralKilling;

    public bool HasImpostorVision => true;

    public CustomRoleConfiguration Configuration => new(this)
    {
        CanUseVent = true,
        IntroSound = TouAudio.HexBombAlarmSound,
        Icon = TouRoleIcons.SerialKiller,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>()
    };

    public bool WinConditionMet()
    {
        if (Player.HasDied())
        {
            return false;
        }

        var aliveCount = Helpers.GetAlivePlayers().Count;
        var killersAlive = MiscUtils.KillersAliveCount;

        return aliveCount <= killersAlive && killersAlive == 1;
    }

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);

        if (!OptionGroupSingleton<SerialKillerOptions>.Instance.CanReportBodies && !Player.HasModifier<SerialKillerNoReportModifier>())
        {
            Player.AddModifier<SerialKillerNoReportModifier>();
        }

        if (Player.AmOwner && OptionGroupSingleton<SerialKillerOptions>.Instance.ManiacMode)
        {
            var options = OptionGroupSingleton<SerialKillerOptions>.Instance;
            Player.AddModifier<SerialKillerManiacModifier>(options.ManiacTimer.Value, options.ManiacCooldown.Value);
        }
    }

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);
    }

    public override bool CanUse(IUsable usable)
    {
        if (!GameManager.Instance.LogicUsables.CanUse(usable, Player))
        {
            return false;
        }

        var console = usable.TryCast<Console>()!;
        return console == null || console.AllowImpostor;
    }

    public override bool DidWin(GameOverReason gameOverReason)
    {
        return WinConditionMet();
    }
}
