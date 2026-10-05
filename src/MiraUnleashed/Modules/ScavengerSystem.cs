using System.Collections;
using MiraAPI.Modifiers;
using Reactor.Utilities;
using MiraUnleashed.Modifiers;
using TownOfUs.Utilities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MiraUnleashed.Modules;

public static class ScavengerSystem
{
    private static readonly HashSet<byte> EatenBodies = new();
    private static readonly Dictionary<byte, float> ActiveScavenges = new();

    public static bool IsBodyEaten(byte bodyId)
    {
        return EatenBodies.Contains(bodyId);
    }

    public static void MarkBodyEaten(byte bodyId)
    {
        EatenBodies.Add(bodyId);
    }

    public static void StartScavenge(byte scavengerId, float duration)
    {
        if (ActiveScavenges.ContainsKey(scavengerId))
        {
            return;
        }

        ActiveScavenges[scavengerId] = Time.time + duration;
        Coroutines.Start(CoScavenge(scavengerId, duration));
    }

    private static IEnumerator CoScavenge(byte scavengerId, float duration)
    {
        var scavenger = MiscUtils.PlayerById(scavengerId);
        if (scavenger == null || scavenger.HasDied())
        {
            ActiveScavenges.Remove(scavengerId);
            yield break;
        }

        var endTime = Time.time + duration;
        var updateInterval = 0.1f;
        var lastUpdate = Time.time;

        while (Time.time < endTime)
        {
            if (scavenger == null || scavenger.HasDied() || MeetingHud.Instance != null)
            {
                break;
            }

            if (Time.time - lastUpdate >= updateInterval)
            {
                UpdateScavengeArrows(scavenger);
                lastUpdate = Time.time;
            }

            yield return null;
        }

        if (scavenger != null && !scavenger.HasDied())
        {
            var modifiers = scavenger.GetModifiers<ScavengerBodyArrowModifier>().ToList();
            foreach (var modifier in modifiers)
            {
                scavenger.RpcRemoveModifier<ScavengerBodyArrowModifier>();
            }
        }

        ActiveScavenges.Remove(scavengerId);
    }

    private static void UpdateScavengeArrows(PlayerControl scavenger)
    {
        if (scavenger == null || scavenger.HasDied() || !scavenger.AmOwner)
        {
            return;
        }

        var allBodies = Object.FindObjectsOfType<DeadBody>();
        var existingArrows = scavenger.GetModifiers<ScavengerBodyArrowModifier>().ToList();
        var existingBodyIds = existingArrows.Select(m => m.BodyId).ToHashSet();

        foreach (var arrow in existingArrows)
        {
            var bodyExists = allBodies.Any(b => b.ParentId == arrow.BodyId);
            if (!bodyExists)
            {
                scavenger.RpcRemoveModifier<ScavengerBodyArrowModifier>();
            }
        }

        foreach (var body in allBodies)
        {
            if (IsBodyEaten(body.ParentId) || existingBodyIds.Contains(body.ParentId))
            {
                continue;
            }

            if (body != null && body.gameObject != null && body.gameObject.activeSelf)
            {
                scavenger.AddModifier<ScavengerBodyArrowModifier>(body, body.ParentId);
            }
        }
    }

    public static void ClearForPlayer(byte playerId)
    {
        ActiveScavenges.Remove(playerId);
    }

    public static void ClearAll()
    {
        EatenBodies.Clear();
        ActiveScavenges.Clear();
    }
}
