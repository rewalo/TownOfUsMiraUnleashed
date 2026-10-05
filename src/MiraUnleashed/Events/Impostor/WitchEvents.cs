using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Events.Vanilla.Meeting;
using MiraAPI.Events.Vanilla.Player;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using Reactor.Utilities;
using System.Collections;
using MiraUnleashed.Modifiers;
using MiraUnleashed.Options.Roles.Impostor;
using MiraUnleashed.Roles.Impostor;
using TownOfUs.Modifiers.Neutral;
using TownOfUs.Networking;
using TownOfUs.Utilities;
using UnityEngine;

namespace MiraUnleashed.Events.Impostor;

public static class WitchEvents
{
    private static readonly List<PlayerControl> PendingSpellDeaths = new();
    private static int _meetingCount;
    private static bool _processingDeaths;

    public static int GetCurrentMeetingCount() => _meetingCount;

    private static bool HasAnyWitch()
    {
        foreach (var player in PlayerControl.AllPlayerControls)
        {
            if (player != null && player.IsRole<WitchRole>())
            {
                return true;
            }
        }

        return false;
    }

    private static bool HasAnyHexedPlayers()
    {
        foreach (var player in PlayerControl.AllPlayerControls)
        {
            if (player != null && player.HasModifier<WitchSpellboundModifier>())
            {
                return true;
            }
        }

        return false;
    }

    [RegisterEvent]
    public static void StartMeetingEventHandler(StartMeetingEvent @event)
    {
        _meetingCount++;

        WitchRole.SendBatchedNotifications();

        if (MeetingHud.Instance == null)
        {
            return;
        }

        foreach (var player in PlayerControl.AllPlayerControls)
        {
            if (player == null || !player.HasModifier<WitchSpellboundModifier>())
            {
                continue;
            }

            var voteArea = MeetingHud.Instance.playerStates.FirstOrDefault(x => x.PlayerId == player.PlayerId);
            if (voteArea != null)
            {
                voteArea.NameText.color = MiraUnleashedColors.Witch;
            }
        }

        Coroutines.Start(CoMonitorMeetingEnd());
    }

    private static IEnumerator CoMonitorMeetingEnd()
    {
        while (MeetingHud.Instance != null)
        {
            yield return new WaitForSeconds(0.1f);
        }

        if (!HasAnyWitch() || !HasAnyHexedPlayers())
        {
            yield break;
        }

        if (!_processingDeaths)
        {
            _processingDeaths = true;
            Coroutines.Start(CoProcessSpellDeaths());
        }
    }

    [RegisterEvent]
    public static void EjectionEventHandler(EjectionEvent @event)
    {
        var exiled = @event.ExileController?.initData?.networkedPlayer?.Object;
        if (exiled == null)
        {
            return;
        }

        if (exiled.IsRole<WitchRole>())
        {
            if (AmongUsClient.Instance != null && AmongUsClient.Instance.AmHost)
            {
                WitchRole.RpcWitchClearSpellboundByWitch(PlayerControl.LocalPlayer, exiled.PlayerId);
            }
            return;
        }

        if (!HasAnyWitch() || !HasAnyHexedPlayers())
        {
            return;
        }

        _processingDeaths = true;
        Coroutines.Start(CoProcessSpellDeaths());
    }

    private static IEnumerator CoProcessSpellDeaths()
    {
        try
        {
            while (MeetingHud.Instance != null)
            {
                yield return new WaitForSeconds(0.05f);
            }

            yield return new WaitForSeconds(0.1f);

            if (AmongUsClient.Instance == null || !AmongUsClient.Instance.AmHost)
            {
                yield break;
            }

            if (!HasAnyWitch() || !HasAnyHexedPlayers())
            {
                yield break;
            }

            var witchAlive = false;

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player == null || !player.IsRole<WitchRole>())
                {
                    continue;
                }

                if (!player.HasDied())
                {
                    witchAlive = true;
                }
            }

            if (!witchAlive)
            {
                WitchRole.RpcWitchClearAllSpellbound(PlayerControl.LocalPlayer);
                PendingSpellDeaths.Clear();
                yield break;
            }

            var options = OptionGroupSingleton<WitchOptions>.Instance;
            var meetingsUntilDeath = options.MeetingsUntilDeath;

            var spellboundPlayers = new List<PlayerControl>();
            foreach (var pc in PlayerControl.AllPlayerControls)
            {
                if (pc != null && pc.HasModifier<WitchSpellboundModifier>())
                {
                    spellboundPlayers.Add(pc);
                }
            }

            foreach (var player in spellboundPlayers)
            {
                if (player == null || player.HasDied())
                {
                    continue;
                }

                var modifier = player.GetModifier<WitchSpellboundModifier>();
                if (modifier == null)
                {
                    continue;
                }

                var hexingWitch = MiscUtils.PlayerById(modifier.WitchId);
                if (hexingWitch == null || hexingWitch.HasDied() || !hexingWitch.IsRole<WitchRole>())
                {
                    WitchRole.RpcWitchClearSpellboundPlayer(PlayerControl.LocalPlayer, player.PlayerId);
                    continue;
                }

                var meetingsSinceSpell = _meetingCount - modifier.SpellCastMeeting;
                if (meetingsSinceSpell < meetingsUntilDeath)
                {
                    continue;
                }

                var shouldDie = true;

                if (player.HasModifier<GuardianAngelProtectModifier>())
                {
                    shouldDie = false;
                }

                if (shouldDie)
                {
                    hexingWitch.RpcSpecialMurder(
                        player,
                        isIndirect: true,
                        ignoreShield: false,
                        didSucceed: true,
                        resetKillTimer: true,
                        createDeadBody: false,
                        teleportMurderer: false,
                        showKillAnim: true,
                        playKillSound: false,
                        causeOfDeath: "Witch");
                }

                WitchRole.RpcWitchClearSpellboundPlayer(PlayerControl.LocalPlayer, player.PlayerId);
            }

            PendingSpellDeaths.Clear();
        }
        finally
        {
            _processingDeaths = false;
        }
    }

    [RegisterEvent]
    public static void PlayerDeathEventHandler(PlayerDeathEvent @event)
    {
        var victim = @event.Player;
        if (victim == null)
        {
            return;
        }

        if (victim.IsRole<WitchRole>())
        {
            // If a Witch dies during a meeting, wait for the meeting to end before processing
            if (MeetingHud.Instance != null)
            {
                if (!HasAnyHexedPlayers())
                {
                    return;
                }

                if (!_processingDeaths)
                {
                    _processingDeaths = true;
                    Coroutines.Start(CoProcessSpellDeaths());
                }

                return;
            }

            // If a Witch dies outside a meeting, clear only their spellbound modifiers immediately
            if (AmongUsClient.Instance != null && AmongUsClient.Instance.AmHost)
            {
                WitchRole.RpcWitchClearSpellboundByWitch(PlayerControl.LocalPlayer, victim.PlayerId);
            }
        }
    }

    [RegisterEvent]
    public static void RoundStartEventHandler(RoundStartEvent @event)
    {
        if (@event.TriggeredByIntro)
        {
            _meetingCount = 0;
        }
    }
}
