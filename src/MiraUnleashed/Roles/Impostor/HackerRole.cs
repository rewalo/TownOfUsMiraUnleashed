using Il2CppInterop.Runtime.Attributes;
using MiraAPI.GameOptions;
using MiraAPI.Patches.Stubs;
using MiraAPI.Roles;
using Reactor.Networking.Attributes;
using Reactor.Networking.Rpc;
using Reactor.Utilities;
using System.Collections;
using MiraUnleashed.Assets;
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

public sealed class HackerRole(IntPtr cppPtr) : ImpostorRole(cppPtr), IMiraUnleashedRole, IWikiDiscoverable, IDoomable
{
    public DoomableType DoomHintType => DoomableType.Insight;

    public string GetAdvancedDescription()
    {
        return ((ICustomRole)this).RoleWikiDescription + MiscUtils.AppendOptionsText(GetType());
    }

    public Color RoleColor => MiraUnleashedColors.Hacker;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;
    public RoleAlignment RoleAlignment => RoleAlignment.ImpostorSupport;

    public CustomRoleConfiguration Configuration => new(this)
    {
        UseVanillaKillButton = true,
        Icon = MiraUnleashedImpAssets.HackerRole,
    };

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities =>
        [
            new(
                MiraLocaleManager.Get("MiraUnleashed.Role.HackerDownload", "Download"),
                MiraLocaleManager.Get("MiraUnleashed.Role.HackerDownload.WikiDescription"),
                MiraUnleashedImpAssets.HackerDownloadButtonSprite),
            new(
                MiraLocaleManager.Get("MiraUnleashed.Role.HackerDevice", "Device"),
                MiraLocaleManager.Get("MiraUnleashed.Role.HackerDevice.WikiDescription"),
                MiraUnleashedImpAssets.HackerDeviceGenericSprite),
            new(
                MiraLocaleManager.Get("MiraUnleashed.Role.HackerJam", "Jam"),
                MiraLocaleManager.Get("MiraUnleashed.Role.HackerJam.WikiDescription"),
                MiraUnleashedImpAssets.HackerJamButtonSprite)
        ];

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);
    }

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);
    }

    [MethodRpc((uint)MiraUnleashedRpc.HackerActivateJam)]
    public static void RpcHackerActivateJam(PlayerControl hacker)
    {
        if (AmongUsClient.Instance == null || !AmongUsClient.Instance.AmHost)
        {
            return;
        }

        if (hacker?.Data?.Role is not HackerRole)
        {
            return;
        }

        var opts = OptionGroupSingleton<HackerOptions>.Instance;
        if (!opts.JamEnabled)
        {
            return;
        }

        if (!HackerSystem.TryConsumeJamCharge(hacker.PlayerId))
        {
            return;
        }

        var host = PlayerControl.LocalPlayer;
        if (host == null)
        {
            return;
        }

        var newCharges = HackerSystem.GetJamCharges(hacker.PlayerId);
        HackerSystem.SetJamCharges(hacker.PlayerId, newCharges);
        HackerSystem.ActivateJam(opts.JamDurationSeconds);

        Coroutines.Start(CoBroadcastJamNextFrame(host, hacker.PlayerId, newCharges, opts.JamDurationSeconds));
    }

    private static IEnumerator CoBroadcastJamNextFrame(PlayerControl host, byte hackerId, byte newCharges, float durationSeconds)
    {
        yield return null;

        if (AmongUsClient.Instance == null || !AmongUsClient.Instance.AmHost)
        {
            yield break;
        }

        if (host == null || PlayerControl.LocalPlayer == null)
        {
            yield break;
        }

        if (!HackerSystem.IsJammed)
        {
            yield break;
        }

        RpcHackerSetJamCharges(host, hackerId, newCharges);
        RpcHackerStartJam(host, hackerId, durationSeconds);
    }

    [MethodRpc((uint)MiraUnleashedRpc.HackerStartJam, LocalHandling = RpcLocalHandling.Before)]
    public static void RpcHackerStartJam(PlayerControl sender, byte hackerId, float durationSeconds)
    {
        HackerSystem.ActivateJam(durationSeconds);

        var localPlayer = PlayerControl.LocalPlayer;
        if (localPlayer != null && localPlayer.PlayerId == hackerId && localPlayer.Data?.Role is HackerRole)
        {
            TouAudio.PlaySound(MiraUnleashedAudio.HackerJamSound);
        }
    }

    [MethodRpc((uint)MiraUnleashedRpc.HackerSetJamCharges, LocalHandling = RpcLocalHandling.Before)]
    public static void RpcHackerSetJamCharges(PlayerControl sender, byte targetPlayerId, byte charges)
    {
        HackerSystem.SetJamCharges(targetPlayerId, charges);
    }

    [MethodRpc((uint)MiraUnleashedRpc.HackerResetRound, LocalHandling = RpcLocalHandling.Before)]
    public static void RpcHackerResetRound(PlayerControl sender)
    {
        HackerSystem.ResetRoundState();
    }
}
