using HarmonyLib;
using MiraAPI.GameOptions;
using MiraUnleashed.Buttons.Impostor;
using MiraUnleashed.Options.Roles.Impostor;
using MiraUnleashed.Roles.Impostor;
using TownOfUs.Utilities;

namespace MiraUnleashed.Patches.Hacker;

[HarmonyPatch]
public static class HackerMoveWithDevicePatch
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CanMove), MethodType.Getter)]
    [HarmonyPostfix]
    public static void PlayerControlCanMovePostfix(PlayerControl __instance, ref bool __result)
    {
        var lp = PlayerControl.LocalPlayer;
        if (lp == null || __instance == null)
        {
            return;
        }

        if (MeetingHud.Instance)
        {
            return;
        }

        if (lp.IsRole<HackerRole>() &&
            OptionGroupSingleton<HackerOptions>.Instance.MoveWithDevice &&
            HackerDeviceButton.IsPortableDeviceOpen)
        {
            __result = __instance.moveable;
        }
    }
}
