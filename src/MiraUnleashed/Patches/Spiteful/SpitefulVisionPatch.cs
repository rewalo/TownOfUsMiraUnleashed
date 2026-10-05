using HarmonyLib;
using MiraAPI.Modifiers;
using MiraUnleashed.Modifiers;
using MiraUnleashed.Options.Modifiers;
using UnityEngine;

namespace MiraUnleashed.Patches.Spiteful;

[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.CalculateLightRadius))]
[HarmonyPriority(Priority.Last)]
public static class SpitefulVisionPatch
{
    public static void Postfix(ShipStatus __instance, NetworkedPlayerInfo player, ref float __result)
    {
        if (player == null || player.IsDead || player.Object == null)
        {
            return;
        }

        if (player.Object.HasModifier<SpitefulEffectModifier>())
        {
            var mod = player.Object.GetModifier<SpitefulEffectModifier>();
            if (mod != null && mod.EffectType == SpitefulEffectType.LowerVision)
            {
                __result *= mod.VisionPerc;
            }
        }
    }
}
