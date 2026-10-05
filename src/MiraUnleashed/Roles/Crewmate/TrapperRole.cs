using System.Collections;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Patches.Stubs;
using MiraAPI.Roles;
using MiraAPI.Utilities.Assets;
using MiraAPI.Utilities;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using MiraUnleashed.Assets;
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

/// <summary>
/// Trapper role: Places traps on vents that immobilize players who use them.
/// </summary>
public sealed class TrapperRole(IntPtr cppPtr) : CrewmateRole(cppPtr), IMiraUnleashedRole, IWikiDiscoverable, IDoomable
{
    public override bool IsAffectedByComms => false;

    public DoomableType DoomHintType => DoomableType.Trickster;

    public string GetAdvancedDescription()
    {
        return ((ICustomRole)this).RoleWikiDescription + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities =>
        [
            new(
                MiraLocaleManager.Get("MiraUnleashed.Role.TrapperTrap", "Trap"),
                MiraLocaleManager.Get("MiraUnleashed.Role.TrapperTrap.WikiDescription"),
                TouCrewAssets.TrapSprite)
        ];

    public Color RoleColor => MiraUnleashedColors.Trapper;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmateSupport;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = MiraUnleashedAssets.TrapperRoleIcon,
        IconTmp = TmpSpriteUtils.CreateSpriteAsset(MiraUnleashedAssets.TrapperRoleIcon.LoadAsset(), "MiraUnleashed.Role.Crewmate.Trapper", 1.45f),
        IntroSound = TouAudio.EngineerIntroSound,
    };

    public void LobbyStart()
    {
        VentTrapSystem.ClearAll();
    }

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);

        VentTrapSystem.ClearOwnedBy(targetPlayer.PlayerId);
    }

    [MethodRpc((uint)MiraUnleashedRpc.TrapperPlaceTrap)]
    public static void RpcTrapperPlaceTrap(PlayerControl trapper, int ventId)
    {
        if (trapper == null || trapper.Data?.Role is not TrapperRole)
        {
            return;
        }

        VentTrapSystem.Place(ventId, trapper.PlayerId);

        if (trapper.AmOwner)
        {
            var vent = Helpers.GetVentById(ventId);
            var room = vent != null ? MiscUtils.GetRoomName(vent.transform.position) : MiraLocaleManager.Get("MiraUnleashed.Common.Unknown", "Unknown");
            var msg = MiraLocaleManager.GetParsed("MiraUnleashed.Trapper.Placed", new()
            {
                ["<room>"] = room,
            }, "Trapped a vent in <room>!");

            var notif = Helpers.CreateAndShowNotification(
                msg,
                Color.white,
                new Vector3(0f, 1f, -20f),
                spr: TouRoleIcons.Trapper.LoadAsset());
            notif.AdjustNotification();
        }
    }

    [MethodRpc((uint)MiraUnleashedRpc.TrapperTriggerTrap)]
    public static IEnumerator RpcTrapperTriggerTrap(PlayerControl trapper, int ventId, byte victimId)
    {
        if (trapper == null)
        {
            yield break;
        }

        VentTrapSystem.Remove(ventId);

        var victim = MiscUtils.PlayerById(victimId);
        if (victim == null)
        {
            yield break;
        }

        if (!VentTrapSystem.IsEligibleToBeTrapped(victim))
        {
            yield break;
        }

        var vent = Helpers.GetVentById(ventId);
        var ventTopPos = vent != null ? VentTrapSystem.GetVentTopPosition(vent) : (Vector2)victim.transform.position;

        yield return new WaitForSeconds(0.3f);

        if (victim.AmOwner)
        {
            CoApplyTrapToVictimAfterVentAnim(victim, ventId, ventTopPos, vent);
        }
        else if (trapper.AmOwner)
        {
            Coroutines.Start(MiscUtils.CoFlash(MiraUnleashedColors.Trapper));

            var arrowDur = OptionGroupSingleton<TrapperOptions>.Instance.ArrowDuration;
            var arrowTarget = OptionGroupSingleton<TrapperOptions>.Instance.ArrowTarget;
            if (trapper.TryGetComponent<ModifierComponent>(out var modifierComp) && arrowDur > 0f)
            {
                if (arrowTarget == TrapperArrowTarget.Person && victim != null)
                {
                    modifierComp.AddModifier(new PlayerArrowModifier(victim, MiraUnleashedColors.Trapper, arrowDur));
                }
                else
                {
                    modifierComp.AddModifier(new VentArrowModifier(ventTopPos, MiraUnleashedColors.Trapper, arrowDur));
                }
            }

            var room = vent != null ? MiscUtils.GetRoomName(vent.transform.position) : MiraLocaleManager.Get("MiraUnleashed.Common.Unknown", "Unknown");
            var msg = MiraLocaleManager.GetParsed("MiraUnleashed.Trapper.Triggered", new()
            {
                ["<room>"] = room,
            }, "Your trap was triggered in <room>!");

            var notif = Helpers.CreateAndShowNotification(
                msg,
                Color.white,
                new Vector3(0f, 1f, -20f),
                spr: TouRoleIcons.Trapper.LoadAsset());
            notif.AdjustNotification();
        }
    }

    private static void CoApplyTrapToVictimAfterVentAnim(PlayerControl victim, int ventId, Vector2 ventTopPos, Vent? vent)
    {
        if (victim == null || victim.HasDied() || !victim.AmOwner)
        {
            return;
        }

        var dur = OptionGroupSingleton<TrapperOptions>.Instance.TrapDuration;
        if (victim.TryGetComponent<ModifierComponent>(out var modifierComp))
        {
            modifierComp.AddModifier(new TrappedOnVentModifier(ventTopPos, dur, ventId));
        }

        Coroutines.Start(MiscUtils.CoFlash(MiraUnleashedColors.Trapper));
        TouAudio.PlaySound(TouAudio.DiscoveredSound);

        var room = vent != null ? MiscUtils.GetRoomName(vent.transform.position) : MiraLocaleManager.Get("MiraUnleashed.Common.Unknown", "Unknown");
        var msg = MiraLocaleManager.GetParsed("MiraUnleashed.Trapper.Caught", new()
        {
            ["<room>"] = room,
        }, "You were caught in a trap in <room>!");

        var notif = Helpers.CreateAndShowNotification(
            msg,
            Color.white,
            new Vector3(0f, 1f, -20f),
            spr: TouRoleIcons.Trapper.LoadAsset());
        notif.AdjustNotification();
    }
}
