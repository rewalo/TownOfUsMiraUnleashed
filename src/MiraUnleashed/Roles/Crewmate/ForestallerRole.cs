using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Patches.Stubs;
using MiraAPI.Roles;
using MiraUnleashed.Assets;
using TownOfUs.Extensions;
using TownOfUs.Modules.Wiki;
using TownOfUs.Roles;
using TownOfUs.Utilities;
using UnityEngine;

namespace MiraUnleashed.Roles.Crewmate;

/// <summary>
/// Forestaller role: when they complete all tasks, sabotages are disabled (while they are alive).
/// They are revealed in meetings after completing all tasks.
/// </summary>
public sealed class ForestallerRole(IntPtr cppPtr) : CrewmateRole(cppPtr), IMiraUnleashedRole, IWikiDiscoverable, IDoomable, IUnguessable
{
    public DoomableType DoomHintType => DoomableType.Insight;

    public string GetAdvancedDescription()
    {
        return ((ICustomRole)this).RoleWikiDescription + MiscUtils.AppendOptionsText(GetType());
    }

    public Color RoleColor => MiraUnleashedColors.Forestaller;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmateSupport;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = MiraUnleashedAssets.ForestallerRoleIcon,
        IntroSound = TownOfUs.Assets.TouAudio.EngineerIntroSound,
    };

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);
        Modules.ForestallerSystem.ClearForPlayer(targetPlayer.PlayerId);
    }

    [HideFromIl2Cpp]
    public void CheckTaskRequirements()
    {
        Modules.ForestallerSystem.TryActivateIfCompletedAllTasks(Player);
    }

    public bool IsGuessable => Player != null && !Modules.ForestallerSystem.IsForestallerRevealed(Player.PlayerId);
    public RoleBehaviour AppearAs => this;
}
