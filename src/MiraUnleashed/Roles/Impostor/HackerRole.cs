using Il2CppInterop.Runtime.Attributes;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Patches.Stubs;
using MiraAPI.Roles;
using Reactor.Networking.Attributes;
using Reactor.Networking.Rpc;
using MiraUnleashed.Assets;
using MiraUnleashed.Buttons.Impostor;
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

    [MethodRpc((uint)MiraUnleashedRpc.HackerActivateJam, LocalHandling = RpcLocalHandling.Before)]
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

        var newCharges = HackerSystem.GetJamCharges(hacker.PlayerId);
        RpcHackerStartJam(PlayerControl.LocalPlayer, hacker.PlayerId, newCharges, opts.JamDurationSeconds);
    }

    [MethodRpc((uint)MiraUnleashedRpc.HackerStartJam, LocalHandling = RpcLocalHandling.Before)]
    public static void RpcHackerStartJam(PlayerControl sender, byte hackerId, byte remainingCharges, float durationSeconds)
    {
        HackerSystem.SetJamCharges(hackerId, remainingCharges);
        HackerSystem.ActivateJam(durationSeconds);

        var localPlayer = PlayerControl.LocalPlayer;
        var isHacker = localPlayer != null && localPlayer.PlayerId == hackerId && localPlayer.Data?.Role is HackerRole;
        if (isHacker)
        {
            CustomButtonSingleton<HackerJamButton>.Instance.OnJamStarted(durationSeconds);
        }

        var opts = OptionGroupSingleton<HackerOptions>.Instance;
        if (opts.JamSoundCue.Value == HackerJamSoundCue.Everyone || isHacker)
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
