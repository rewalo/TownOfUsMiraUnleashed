using HarmonyLib;
using MiraUnleashed.Modules;

namespace MiraUnleashed.Patches.MirageDecoy;

[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.FixedUpdate))]
public static class MirageDecoyHostUpdatePatch
{
    [HarmonyPostfix]
    public static void FixedUpdatePostfix()
    {
        MirageDecoySystem.UpdateHost();
    }
}
