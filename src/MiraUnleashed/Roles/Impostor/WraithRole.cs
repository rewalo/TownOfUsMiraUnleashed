using Il2CppInterop.Runtime.Attributes;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Patches.Stubs;
using MiraAPI.Roles;
using MiraAPI.Utilities.Assets;
using Reactor.Networking.Attributes;
using MiraUnleashed.Assets;
using MiraUnleashed.Modifiers;
using MiraUnleashed.Modules;
using MiraUnleashed.Networking;
using MiraUnleashed.Options.Roles.Impostor;
using TownOfUs.Assets;
using TownOfUs.Extensions;
using TownOfUs.Modules.Wiki;
using TownOfUs.Roles;
using TownOfUs.Utilities;
using UnityEngine;

namespace MiraUnleashed.Roles.Impostor;

public sealed class WraithRole(IntPtr cppPtr) : ImpostorRole(cppPtr), IMiraUnleashedRole, IWikiDiscoverable, IDoomable
{
    public DoomableType DoomHintType => DoomableType.Hunter;

    public string GetAdvancedDescription()
    {
        return ((ICustomRole)this).RoleWikiDescription + MiscUtils.AppendOptionsText(GetType());
    }

    public Color RoleColor => MiraUnleashedColors.Wraith;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;
    public RoleAlignment RoleAlignment => RoleAlignment.ImpostorPower;

    public CustomRoleConfiguration Configuration => new(this)
    {
        UseVanillaKillButton = true,
        Icon = TouRoleIcons.Wraith,
        IconTmp = TmpSpriteUtils.CreateSpriteAsset(TouRoleIcons.Wraith.LoadAsset(), "MiraUnleashed.Role.Impostor.Wraith", 1.45f)    };

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities =>
        [
            new(
                MiraLocaleManager.Get("MiraUnleashed.Role.WraithDash", "Dash"),
                MiraLocaleManager.Get("MiraUnleashed.Role.WraithDash.WikiDescription"),
                TouImpAssets.SprintSprite),
            new(
                MiraLocaleManager.Get("MiraUnleashed.Role.WraithLantern", "Lantern"),
                MiraLocaleManager.Get("MiraUnleashed.Role.WraithLantern.WikiDescription"),
                MiraUnleashedImpAssets.LanternButtonSprite)
        ];

    [HideFromIl2Cpp]
    public List<AdvancedWikiAbilityDescription> WikiAbilities =>
        [
            new(
                MiraLocaleManager.Get("MiraUnleashed.Role.WraithDash", "Dash"),
                MiraLocaleManager.Get("MiraApi.AbilityType.Basic"),
                MiraLocaleManager.Get("MiraUnleashed.Role.WraithDash.WikiDescription"),
                TouImpAssets.SprintSprite),
            new(
                MiraLocaleManager.Get("MiraUnleashed.Role.WraithLantern", "Lantern"),
                MiraLocaleManager.Get("MiraApi.AbilityType.Radius"),
                MiraLocaleManager.Get("MiraUnleashed.Role.WraithLantern.WikiDescription"),
                MiraUnleashedImpAssets.LanternButtonSprite)
        ];

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);
    }

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);
        WraithLanternSystem.ClearForPlayer(targetPlayer.PlayerId);
    }

    [MethodRpc((uint)MiraUnleashedRpc.WraithPlaceLantern)]
    public static void RpcWraithPlaceLantern(PlayerControl wraith, Vector2 pos)
    {
        if (wraith?.Data?.Role is not WraithRole)
        {
            return;
        }

        var opts = OptionGroupSingleton<WraithOptions>.Instance;
        if (!opts.LanternEnabled)
        {
            return;
        }

        WraithLanternSystem.PlaceLantern(wraith.PlayerId, pos, opts.LanternDuration.Value);
    }

    [MethodRpc((uint)MiraUnleashedRpc.WraithReturnLantern)]
    public static void RpcWraithReturnLantern(PlayerControl wraith, Vector2 pos)
    {
        if (wraith?.Data?.Role is not WraithRole)
        {
            return;
        }

        if (!WraithLanternSystem.TryReturnLantern(wraith.PlayerId, out var markedPos))
        {
            return;
        }

        if (Vector2.Distance(markedPos, pos) > 0.25f)
        {
            pos = markedPos;
        }

        wraith.transform.position = pos;
        wraith.MyPhysics.ResetMoveState();
        wraith.NetTransform.SnapTo(pos);
        if (wraith.AmOwner)
        {
            PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(pos);
        }

        var invisDuration = OptionGroupSingleton<WraithOptions>.Instance.InvisibleDuration.Value;
        if (invisDuration > 0f && !wraith.HasDied())
        {
            wraith.AddModifier<WraithLanternInvisibilityModifier>();
        }
    }

    [MethodRpc((uint)MiraUnleashedRpc.WraithBreakLantern)]
    public static void RpcWraithBreakLantern(PlayerControl wraith, Vector2 pos)
    {
        if (wraith == null)
        {
            return;
        }

        if (PlayerControl.LocalPlayer != null)
        {
            TouAudio.PlaySound(MiraUnleashedAudio.LanternBreakSound);
        }

        WraithLanternSystem.BreakLantern(wraith.PlayerId, pos);
    }
}
