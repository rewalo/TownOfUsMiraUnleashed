using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Patches.Stubs;
using MiraAPI.Roles;
using MiraAPI.Utilities.Assets;
using Reactor.Networking.Attributes;
using MiraUnleashed.Assets;
using MiraUnleashed.Events.Impostor;
using MiraUnleashed.Networking;
using MiraUnleashed.Options.Roles.Impostor;
using TownOfUs.Extensions;
using TownOfUs.Modules.Wiki;
using TownOfUs.Roles;
using TownOfUs.Utilities;
using UnityEngine;

namespace MiraUnleashed.Roles.Impostor;

public sealed class InjectorRole(IntPtr cppPtr) : ImpostorRole(cppPtr), IMiraUnleashedRole, IWikiDiscoverable, IDoomable
{
    public DoomableType DoomHintType => DoomableType.Insight;

    public string GetAdvancedDescription()
    {
        return ((ICustomRole)this).RoleWikiDescription + MiscUtils.AppendOptionsText(GetType());
    }

    public Color RoleColor => MiraUnleashedColors.Injector;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;
    public RoleAlignment RoleAlignment => RoleAlignment.ImpostorSupport;

    public CustomRoleConfiguration Configuration => new(this)
    {
        UseVanillaKillButton = true,
        Icon = MiraUnleashedImpAssets.InjectorRole,
        IconTmp = TmpSpriteUtils.CreateSpriteAsset(MiraUnleashedImpAssets.InjectorRole.LoadAsset(), "MiraUnleashed.Role.Impostor.Injector", 1.45f),
    };

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities =>
        [
            new(
                MiraLocaleManager.Get("MiraUnleashed.Role.InjectorInject", "Inject"),
                MiraLocaleManager.Get("MiraUnleashed.Role.InjectorInject.WikiDescription"),
                MiraUnleashedImpAssets.InjectorInjectButtonSprite)
        ];

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);
    }

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);
    }

    [MethodRpc((uint)MiraUnleashedRpc.InjectorInject)]
    public static void RpcInjectorInject(PlayerControl injector, PlayerControl target, byte effectType)
    {
        if (injector?.Data?.Role is not InjectorRole)
        {
            Error("RpcInjectorInject - Invalid injector");
            return;
        }

        if (target == null || target.HasDied())
        {
            return;
        }

        InjectorEvents.ScheduleInjection(injector, target, (InjectorEffectType)effectType);
    }
}
