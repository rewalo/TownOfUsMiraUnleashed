using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Hud;
using MiraAPI.Patches.Stubs;
using MiraAPI.Roles;
using Reactor.Networking.Attributes;
using MiraUnleashed.Assets;
using MiraUnleashed.Buttons.Impostor;
using MiraUnleashed.Modules;
using MiraUnleashed.Networking;
using TownOfUs.Extensions;
using TownOfUs.Modules.Wiki;
using TownOfUs.Roles;
using TownOfUs.Utilities;
using UnityEngine;

namespace MiraUnleashed.Roles.Impostor;

public sealed class CharlatanRole(IntPtr cppPtr) : ImpostorRole(cppPtr), IMiraUnleashedRole, IWikiDiscoverable, IDoomable
{
    public DoomableType DoomHintType => DoomableType.Trickster;

    public string GetAdvancedDescription()
    {
        return ((ICustomRole)this).RoleWikiDescription + MiscUtils.AppendOptionsText(GetType());
    }

    public Color RoleColor => MiraUnleashedColors.Charlatan;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;
    public RoleAlignment RoleAlignment => RoleAlignment.ImpostorSupport;

    public CustomRoleConfiguration Configuration => new(this)
    {
        UseVanillaKillButton = true,
        Icon = MiraUnleashedImpAssets.CharlatanRole
    };

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities =>
        [
            new(
                MiraLocaleManager.Get("MiraUnleashed.Role.CharlatanDeceive", "Deceive"),
                MiraLocaleManager.Get("MiraUnleashed.Role.CharlatanDeceive.WikiDescription"),
                MiraUnleashedImpAssets.DeceiveButtonSprite),
            new(
                MiraLocaleManager.Get("MiraUnleashed.Role.CharlatanConceal", "Conceal"),
                MiraLocaleManager.Get("MiraUnleashed.Role.CharlatanConceal.WikiDescription"),
                MiraUnleashedImpAssets.ConcealButtonSprite)
        ];

    [HideFromIl2Cpp]
    public List<AdvancedWikiAbilityDescription> WikiAbilities =>
        [
            new(
                MiraLocaleManager.Get("MiraUnleashed.Role.CharlatanDeceive", "Deceive"),
                MiraLocaleManager.Get("MiraApi.AbilityType.Indirect"),
                MiraLocaleManager.Get("MiraUnleashed.Role.CharlatanDeceive.WikiDescription"),
                MiraUnleashedImpAssets.DeceiveButtonSprite),
            new(
                MiraLocaleManager.Get("MiraUnleashed.Role.CharlatanConceal", "Conceal"),
                MiraLocaleManager.Get("MiraApi.AbilityType.Interaction"),
                MiraLocaleManager.Get("MiraUnleashed.Role.CharlatanConceal.WikiDescription"),
                MiraUnleashedImpAssets.ConcealButtonSprite)
        ];

    public void LobbyStart()
    {
        CharlatanBodySystem.ClearAll();
    }

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);
    }

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);
        CharlatanBodySystem.ClearForPlayer(targetPlayer.PlayerId);
    }

    [MethodRpc((uint)MiraUnleashedRpc.CharlatanConcealStart)]
    public static void RpcConcealStart(PlayerControl charlatan, byte bodyId)
    {
        if (charlatan?.Data?.Role is not CharlatanRole)
        {
            return;
        }

        CharlatanBodySystem.StartChannel(charlatan.PlayerId, bodyId);
    }

    [MethodRpc((uint)MiraUnleashedRpc.CharlatanConcealComplete)]
    public static void RpcConcealComplete(PlayerControl charlatan, byte bodyId)
    {
        if (charlatan?.Data?.Role is not CharlatanRole)
        {
            return;
        }

        var bodyPlayer = MiscUtils.PlayerById(bodyId);
        if (bodyPlayer != null)
        {
            MiscUtils.RemovePet(bodyPlayer);
        }

        CharlatanBodySystem.CompleteChannel(charlatan.PlayerId, bodyId);

        if (charlatan.AmOwner)
        {
            CustomButtonSingleton<CharlatanConcealButton>.Instance.OnConcealCompleted();
        }
    }

    [MethodRpc((uint)MiraUnleashedRpc.CharlatanConcealCancel)]
    public static void RpcConcealCancel(PlayerControl charlatan, byte bodyId)
    {
        if (charlatan?.Data?.Role is not CharlatanRole)
        {
            return;
        }

        CharlatanBodySystem.ClearBody(bodyId);

        if (charlatan.AmOwner)
        {
            CustomButtonSingleton<CharlatanConcealButton>.Instance.OnConcealCancelled();
        }
    }

    [MethodRpc((uint)MiraUnleashedRpc.CharlatanDeceiveActivate)]
    public static void RpcDeceiveActivate(PlayerControl charlatan, byte bodyId, float duration)
    {
        if (charlatan?.Data?.Role is not CharlatanRole)
        {
            return;
        }

        CharlatanBodySystem.ActivateDeceive(charlatan.PlayerId, bodyId, duration);
    }
}
