using HarmonyLib;
using MiraUnleashed.Events.Achievements;

namespace MiraUnleashed.Patches.Achievements;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.StartMeeting))]
public static class AchievementMeetingPatch
{
    [HarmonyPrefix]
    public static void Prefix(PlayerControl __instance, NetworkedPlayerInfo? target)
    {
        AchievementEvents.OnMeetingCalled(__instance, target);
    }
}
