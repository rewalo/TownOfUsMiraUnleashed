using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Events.Vanilla.Meeting;
using MiraAPI.Events.Vanilla.Meeting.Voting;
using MiraAPI.Events.Vanilla.Player;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Utilities;
using MiraUnleashed.Modifiers;
using MiraUnleashed.Options.Roles.Impostor;
using MiraUnleashed.Roles.Impostor;
using TownOfUs.Modifiers;
using TownOfUs.Modifiers.Neutral;
using TownOfUs.Networking;
using TownOfUs.Utilities;

namespace MiraUnleashed.Events.Impostor;

public static class WitchEvents
{
    private static int _meetingCount;

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
    }

    [RegisterEvent]
    public static void ProcessVotesEventHandler(ProcessVotesEvent @event)
    {
        if (!PlayerControl.LocalPlayer.IsHost())
        {
            return;
        }

        if (!HasAnyWitch() || !HasAnyHexedPlayers())
        {
            return;
        }

        var exiledId = @event.ExiledPlayer?.PlayerId;
        var meetingsUntilDeath = OptionGroupSingleton<WitchOptions>.Instance.MeetingsUntilDeath;

        foreach (var player in PlayerControl.AllPlayerControls)
        {
            if (player == null || player.HasDied() || !player.HasModifier<WitchSpellboundModifier>())
            {
                continue;
            }

            if (player.PlayerId == exiledId)
            {
                continue;
            }

            var modifier = player.GetModifier<WitchSpellboundModifier>();
            if (modifier == null)
            {
                continue;
            }

            var hexingWitch = MiscUtils.PlayerById(modifier.WitchId);
            if (hexingWitch == null || hexingWitch.HasDied() || !hexingWitch.IsRole<WitchRole>() || hexingWitch.PlayerId == exiledId)
            {
                WitchRole.RpcWitchClearSpellboundPlayer(PlayerControl.LocalPlayer, player.PlayerId);
                continue;
            }

            var meetingsSinceSpell = _meetingCount - modifier.SpellCastMeeting;
            if (meetingsSinceSpell < meetingsUntilDeath)
            {
                continue;
            }

            hexingWitch.RpcMeetingMurder(
                player,
                MeetingAnimation.PlayerNameplateAnimation,
                CustomTouMurderRpcs.GetRandomMeetingAnim(DeathAnimType.Nameplate),
                didSucceed: !player.HasModifier<InvulnerabilityModifier>() && !player.HasModifier<GuardianAngelProtectModifier>(),
                causeOfDeath: "Witch");

            WitchRole.RpcWitchClearSpellboundPlayer(PlayerControl.LocalPlayer, player.PlayerId);
        }
    }

    [RegisterEvent]
    public static void PlayerDeathEventHandler(PlayerDeathEvent @event)
    {
        var victim = @event.Player;
        if (victim == null || !victim.IsRole<WitchRole>())
        {
            return;
        }

        if (AmongUsClient.Instance != null && AmongUsClient.Instance.AmHost)
        {
            WitchRole.RpcWitchClearSpellboundByWitch(PlayerControl.LocalPlayer, victim.PlayerId);
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
