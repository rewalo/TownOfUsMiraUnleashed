using HarmonyLib;
using MiraAPI.GameOptions;
using MiraAPI.Utilities;
using MiraUnleashed.Events.Neutral;
using MiraUnleashed.Options.Roles.Neutral;
using TownOfUs.Utilities;
using UnityEngine;

namespace MiraUnleashed.Patches.Lawyer;

[HarmonyPatch(typeof(PlayerVoteArea))]
public static class LawyerVoteBlockPatch
{
    private static readonly Dictionary<byte, byte> CurrentVotes = new();

    public static byte? GetCurrentVote(byte playerId)
    {
        return CurrentVotes.TryGetValue(playerId, out var vote) ? vote : null;
    }

    public static void ClearVotes()
    {
        CurrentVotes.Clear();
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(PlayerVoteArea.Select))]
    public static bool PlayerVoteAreaSelectPrefix(PlayerVoteArea __instance)
    {
        if (!PlayerControl.LocalPlayer.AmOwner)
        {
            return true;
        }

        var options = OptionGroupSingleton<LawyerOptions>.Instance;
        if (options == null || !options.ObjectionPreventsSameVote)
        {
            return true;
        }

        if (__instance == MeetingHud.Instance.SkipVoteButton)
        {
            return true;
        }

        var localPlayerId = PlayerControl.LocalPlayer.PlayerId;
        var targetPlayerId = __instance.PlayerId;

        var isObjected = LawyerEvents.IsObjectedVoter(localPlayerId);

        if (isObjected)
        {
            if (LawyerEvents.TryGetOriginalVote(localPlayerId, out var originalVote) &&
                originalVote == targetPlayerId)
            {
                var msg = MiraLocaleManager.Get("MiraUnleashed.Lawyer.CannotVoteSamePerson");

                var notif = Helpers.CreateAndShowNotification(
                    $"<b>{Color.white.ToTextColor()}{msg}</color></b>",
                    Color.white,
                    new Vector3(0f, 1f, -20f),
                    spr: TownOfUs.Assets.TouRoleIcons.Lawyer.LoadAsset());
                notif.AdjustNotification();

                return false;
            }
        }

        CurrentVotes[localPlayerId] = targetPlayerId;

        return true;
    }
}
