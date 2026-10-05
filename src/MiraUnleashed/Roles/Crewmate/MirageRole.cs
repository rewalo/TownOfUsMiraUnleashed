using Il2CppInterop.Runtime.Attributes;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Patches.Stubs;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using MiraUnleashed.Assets;
using MiraUnleashed.Buttons.Crewmate;
using MiraUnleashed.Modifiers;
using MiraUnleashed.Modules;
using MiraUnleashed.Networking;
using MiraUnleashed.Options.Roles.Crewmate;
using TownOfUs.Assets;
using TownOfUs.Extensions;
using TownOfUs.Modules.Wiki;
using TownOfUs.Roles;
using TownOfUs.Utilities;
using UnityEngine;

namespace MiraUnleashed.Roles.Crewmate;

public sealed class MirageRole(IntPtr cppPtr) : CrewmateRole(cppPtr), IMiraUnleashedRole, IWikiDiscoverable, IDoomable, IUnguessable
{
    public DoomableType DoomHintType => DoomableType.Insight;

    public string GetAdvancedDescription()
    {
        return ((ICustomRole)this).RoleWikiDescription + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities =>
        [
            new(
                MiraLocaleManager.Get("MiraUnleashed.Role.MirageDecoy", "Decoy"),
                MiraLocaleManager.Get("MiraUnleashed.Role.MirageDecoy.WikiDescription"),
                MiraUnleashedCrewAssets.DecoyButtonSprite)
        ];

    [HideFromIl2Cpp]
    public List<AdvancedWikiAbilityDescription> WikiAbilities =>
        [
            new(
                MiraLocaleManager.Get("MiraUnleashed.Role.MirageDecoy", "Decoy"),
                MiraLocaleManager.Get("MiraApi.AbilityType.Basic"),
                MiraLocaleManager.Get("MiraUnleashed.Role.MirageDecoy.WikiDescription"),
                MiraUnleashedCrewAssets.DecoyButtonSprite)
        ];

    public Color RoleColor => MiraUnleashedColors.Mirage;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmateSupport;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = MiraUnleashedAssets.MirageRoleIcon,
    };
    public bool IsGuessable => OptionGroupSingleton<MirageOptions>.Instance.DecoyType != MirageDecoyType.Mirage;
    public RoleBehaviour AppearAs => this;

    public void LobbyStart()
    {
        MirageDecoySystem.ClearAll();
    }

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);
    }

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);
        MirageDecoySystem.ClearForPlayer(targetPlayer.PlayerId);
    }

    [MethodRpc((uint)MiraUnleashedRpc.MiragePlaceDecoy)]
    public static void RpcMiragePlaceDecoy(
        PlayerControl mirage,
        PlayerControl appearanceSource,
        Vector2 pos,
        float z,
        float durationSeconds,
        float zRot,
        bool flipX)
    {
        if (mirage?.Data?.Role is not MirageRole)
        {
            return;
        }

        if (mirage.AmOwner)
        {
            TouAudio.PlaySound(MiraUnleashedAudio.DecoyPlaceSound);
        }

        var worldPos = new Vector3(pos.x, pos.y, z);
        MirageDecoySystem.RevealOrSpawnDecoy(mirage.PlayerId, appearanceSource, worldPos, zRot, flipX, durationSeconds);
    }

    [MethodRpc((uint)MiraUnleashedRpc.MiragePrimeDecoy)]
    public static void RpcMiragePrimeDecoy(
        PlayerControl mirage,
        PlayerControl appearanceSource,
        Vector2 pos,
        float z,
        float zRot,
        bool flipX)
    {
        if (mirage?.Data?.Role is not MirageRole)
        {
            return;
        }

        var worldPos = new Vector3(pos.x, pos.y, z);
        MirageDecoySystem.PrimeDecoy(mirage.PlayerId, appearanceSource, worldPos, zRot, flipX);
    }

    [MethodRpc((uint)MiraUnleashedRpc.MirageDestroyDecoy)]
    public static void RpcMirageDestroyDecoy(PlayerControl mirage)
    {
        if (mirage?.Data?.Role is not MirageRole)
        {
            return;
        }

        if (mirage.AmOwner)
        {
            TouAudio.PlaySound(MiraUnleashedAudio.DecoyDestroySound);
        }

        if (MirageDecoySystem.TryRemoveDecoy(mirage.PlayerId, out _) && mirage.AmOwner)
        {
            CustomButtonSingleton<MirageDecoyButton>.Instance.StartCooldownAndReset();
        }
    }

    [MethodRpc((uint)MiraUnleashedRpc.MirageTriggerDecoy)]
    public static void RpcMirageTriggerDecoy(PlayerControl mirage, PlayerControl interactor, Vector2 pos)
    {
        if (mirage?.Data?.Role is not MirageRole)
        {
            return;
        }

        if (mirage.AmOwner)
        {
            TouAudio.PlaySound(MiraUnleashedAudio.DecoyDestroySound);
        }

        MirageDecoySystem.TryRemoveDecoy(mirage.PlayerId, out _);

        if (interactor != null && interactor.AmOwner)
        {
            Coroutines.Start(MiscUtils.CoFlash(MiraUnleashedColors.Mirage));
            TouAudio.PlaySound(TouAudio.DiscoveredSound);
            var msg = MiraLocaleManager.Get("MiraUnleashed.Mirage.InteractorTriggered", "You interacted with a decoy!");
            var notif = Helpers.CreateAndShowNotification(
                msg,
                Color.white,
                new Vector3(0f, 1f, -20f),
                spr: MiraUnleashedAssets.MirageRoleIcon.LoadAsset());
            notif.AdjustNotification();
        }

        if (mirage.AmOwner)
        {
            CustomButtonSingleton<MirageDecoyButton>.Instance.StartCooldownAndReset();

            Coroutines.Start(MiscUtils.CoFlash(MiraUnleashedColors.Mirage));
            TouAudio.PlaySound(TouAudio.DiscoveredSound);

            var msg = MiraLocaleManager.Get("MiraUnleashed.Mirage.OwnerTriggered", "Your decoy was triggered!");

            var notif = Helpers.CreateAndShowNotification(
                msg,
                Color.white,
                new Vector3(0f, 1f, -20f),
                spr: MiraUnleashedAssets.MirageRoleIcon.LoadAsset());
            notif.AdjustNotification();

            var arrowDur = OptionGroupSingleton<MirageOptions>.Instance.ArrowTime;
            var arrowTarget = OptionGroupSingleton<MirageOptions>.Instance.ArrowTarget;
            if (arrowDur > 0f && mirage.TryGetComponent<ModifierComponent>(out var modComp))
            {
                if (arrowTarget == MirageArrowTarget.Interactor && interactor != null)
                {
                    modComp.AddModifier(new PlayerArrowModifier(interactor, MiraUnleashedColors.Mirage, arrowDur));
                }
                else
                {
                    modComp.AddModifier(new VentArrowModifier(pos, MiraUnleashedColors.Mirage, arrowDur));
                }
            }
        }
    }
}
