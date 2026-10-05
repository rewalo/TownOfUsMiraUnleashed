using AmongUs.GameOptions;
using HarmonyLib;
using MiraUnleashed.Modules;
using MiraUnleashed.Roles.Crewmate;
using TownOfUs.Utilities;

namespace MiraUnleashed.Patches.MirageDecoy;

[HarmonyPatch]
public static class MirageDecoyInteractionPatches
{
    private static bool TryTriggerFromLocalPlayer(float maxDistance)
    {
        var local = PlayerControl.LocalPlayer;
        if (local == null || local.HasDied() || MeetingHud.Instance)
        {
            return false;
        }

        var from = local.GetTruePosition();
        if (!MirageDecoySystem.TryGetClosestDecoy(from, maxDistance, out var mirageId, out var decoyPos))
        {
            return false;
        }

        var mirage = MiscUtils.PlayerById(mirageId);
        if (mirage == null || mirage.HasDied() || !mirage.IsRole<MirageRole>())
        {
            return false;
        }

        MirageRole.RpcMirageTriggerDecoy(mirage, local, decoyPos);
        return true;
    }

    private static float GetKillDistance()
    {
        var opts = GameOptionsManager.Instance?.currentNormalGameOptions;
        if (opts == null)
        {
            return 1.0f;
        }

        var killDistances = opts.GetFloatArray(FloatArrayOptionNames.KillDistances);
        var idx = Math.Clamp(opts.KillDistance, 0, killDistances.Length - 1);
        return killDistances[idx];
    }

    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    [HarmonyPrefix]
    [HarmonyPriority(Priority.Last)]
    public static bool KillButtonDoClickPrefix()
    {
        if (!TryTriggerFromLocalPlayer(GetKillDistance()))
        {
            return true;
        }

        var local = PlayerControl.LocalPlayer;
        if (local != null)
        {
            local.SetKillTimer(local.GetKillCooldown());
        }

        return false;
    }
}
