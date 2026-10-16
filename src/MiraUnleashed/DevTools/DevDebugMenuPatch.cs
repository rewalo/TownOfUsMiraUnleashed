using HarmonyLib;

namespace MiraUnleashed.DebugTools;

[HarmonyPatch]
internal static class DevDebugMenuPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
    public static void MainMenuManagerStartPostfix()
    {
        DevDebugMenu.Install();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
    public static void HudManagerStartPostfix()
    {
        DevDebugMenu.Install();
    }
}
