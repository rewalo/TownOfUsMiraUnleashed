using HarmonyLib;
using MiraAPI.Modifiers;
using MiraUnleashed.Modifiers;
using MiraUnleashed.Modules;
using TownOfUs;
using TownOfUs.Patches;
using static TownOfUs.Patches.EndGamePatches;

namespace MiraUnleashed.Patches.Lawyer;

/// <summary>
/// Adds a Lawyer/Client duo marker to the end-game summary list, similar to how Lovers show a heart.
/// </summary>
[HarmonyPatch(typeof(EndGamePatches), nameof(EndGamePatches.BuildEndGameData))]
public static class LawyerEndGameSummaryIconPatch
{
    private const string Symbol = "§";

    [HarmonyPostfix]
    public static void Postfix()
    {
        var lawyerIds = new HashSet<byte>(LawyerDuoTracker.GetLawyers());
        var clientIds = new HashSet<byte>(LawyerDuoTracker.GetClients());

        if (lawyerIds.Count == 0 && clientIds.Count == 0)
        {
            foreach (var pc in PlayerControl.AllPlayerControls)
            {
                if (pc == null)
                {
                    continue;
                }

                var mods = pc.GetModifiers<LawyerTargetModifier>();
                foreach (var mod in mods)
                {
                    lawyerIds.Add(mod.OwnerId);
                    clientIds.Add(pc.PlayerId);
                }
            }

            if (lawyerIds.Count == 0 && clientIds.Count == 0)
            {
                return;
            }
        }

        var tag = $" <b>{TownOfUsColors.Lawyer.ToTextColor()}<size=60%>{Symbol}</size></color></b>";

        for (var i = 0; i < EndGameData.PlayerRecords.Count; i++)
        {
            var record = EndGameData.PlayerRecords[i];
            if (record == null)
            {
                continue;
            }

            if (!lawyerIds.Contains(record.PlayerId) && !clientIds.Contains(record.PlayerId))
            {
                continue;
            }

            if (!string.IsNullOrEmpty(record.PlayerName) && record.PlayerName.Contains(Symbol))
            {
                continue;
            }

            EndGameData.PlayerRecords[i] = record with { PlayerName = record.PlayerName + tag };
        }
    }
}
