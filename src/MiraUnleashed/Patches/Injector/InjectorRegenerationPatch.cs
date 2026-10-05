using HarmonyLib;
using MiraAPI.Modifiers;
using MiraUnleashed.Modifiers;
using TownOfUs.Buttons;

namespace MiraUnleashed.Patches.Injector;

[HarmonyPatch(typeof(TownOfUsButton))]
public static class InjectorRegenerationPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(TownOfUsButton.FixedUpdateHandler))]
    public static void FixedUpdateHandlerPostfix(TownOfUsButton __instance, PlayerControl playerControl)
    {
        if (playerControl == null || !playerControl.AmOwner)
        {
            return;
        }

        if (!playerControl.HasModifier<InjectedRegenerationModifier>())
        {
            return;
        }

        // Regeneration makes cooldowns tick 1.5x faster
        if (__instance.Timer > 0 && !__instance.TimerPaused)
        {
            __instance.Timer -= UnityEngine.Time.deltaTime * 0.5f; // Additional 0.5x = 1.5x total
        }
    }
}
