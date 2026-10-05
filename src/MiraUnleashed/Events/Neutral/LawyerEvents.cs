using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Meeting;
using MiraAPI.Events.Vanilla.Meeting.Voting;
using MiraAPI.Events.Vanilla.Player;
using MiraAPI.GameOptions;
using MiraAPI.Utilities;
using MiraUnleashed.Options.Roles.Neutral;
using MiraUnleashed.Patches.Lawyer;
using MiraUnleashed.Roles.Neutral;
using TownOfUs.Events;
using TownOfUs.Modifiers;
using TownOfUs.Modules;
using TownOfUs.Modules.Components;
using TownOfUs.Utilities;
using UnityEngine;

namespace MiraUnleashed.Events.Neutral;

public static class LawyerEvents
{
    public static readonly Dictionary<byte, byte> ObjectedVoterOriginalVotes = [];

    public static void ClearObjectedVoters()
    {
        ObjectedVoterOriginalVotes.Clear();
    }

    public static void AddObjectedVoter(byte voterId, byte originalVote)
    {
        ObjectedVoterOriginalVotes[voterId] = originalVote;
    }

    public static bool IsObjectedVoter(byte voterId)
    {
        return ObjectedVoterOriginalVotes.ContainsKey(voterId);
    }

    public static bool TryGetOriginalVote(byte voterId, out byte originalVote)
    {
        return ObjectedVoterOriginalVotes.TryGetValue(voterId, out originalVote);
    }

    [RegisterEvent]
    public static void StartMeetingEventHandler(StartMeetingEvent @event)
    {
        ClearObjectedVoters();
        LawyerVoteBlockPatch.ClearVotes();
    }

    [RegisterEvent]
    public static void EjectionEventHandler(EjectionEvent @event)
    {
        var exiled = @event.ExileController?.initData?.networkedPlayer?.Object;
        if (exiled == null)
        {
            return;
        }

        foreach (var player in PlayerControl.AllPlayerControls)
        {
            if (player == null || !player.IsRole<LawyerRole>())
            {
                continue;
            }

            var lawyer = player.GetRole<LawyerRole>();
            if (lawyer == null || lawyer.Client == null)
            {
                continue;
            }

            if (lawyer.Client.PlayerId == exiled.PlayerId)
            {
                lawyer.ClientVoted = true;

                if (OptionGroupSingleton<LawyerOptions>.Instance.GetVotedOutWithClient)
                {
                    GameHistory.UpdatePlayerDeathData(lawyer.Player,
                        MiraLocaleManager.Get("MiraUnleashed.Lawyer.DiedWithClient"),
                        roundOfDeath: HudManagerHelper.Instance.CurrentRound,
                        diedThisRound: DeathHandlerOverride.SetFalse,
                        lockInfo: DeathHandlerOverride.SetTrue);
                    lawyer.Player.Exiled();
                }

                lawyer.CheckClientDeath(exiled);
            }
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

        foreach (var player in PlayerControl.AllPlayerControls)
        {
            if (player == null || !player.IsRole<LawyerRole>())
            {
                continue;
            }

            var lawyer = player.GetRole<LawyerRole>();
            if (lawyer == null || lawyer.Client == null)
            {
                continue;
            }

            if (lawyer.Client.PlayerId == victim.PlayerId)
            {
                lawyer.CheckClientDeath(victim);
            }
        }
    }
}
