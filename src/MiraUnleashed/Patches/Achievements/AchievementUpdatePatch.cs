using HarmonyLib;
using MiraUnleashed.Events.Achievements;

namespace MiraUnleashed.Patches.Achievements;

[HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
public static class AchievementUpdatePatch
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        AchievementEvents.OnHudUpdate();
    }
}
