using TownOfUs.Utilities;

namespace MiraUnleashed.Modules;

/// <summary>
/// Tracks when a Serial Killer can kill someone in a vent.
/// </summary>
public static class SerialKillerVentKillSystem
{
    private static readonly Dictionary<byte, byte> VentKillTargets = new();

    public static void SetVentKillTarget(byte serialKillerId, PlayerControl target)
    {
        if (target == null)
        {
            VentKillTargets.Remove(serialKillerId);
        }
        else
        {
            VentKillTargets[serialKillerId] = target.PlayerId;
        }
    }

    public static bool TryGetVentKillTarget(byte serialKillerId, out PlayerControl target)
    {
        if (VentKillTargets.TryGetValue(serialKillerId, out var targetId))
        {
            var player = MiscUtils.PlayerById(targetId);
            if (player != null)
            {
                target = player;
                return true;
            }
        }

        target = null!;
        return false;
    }

    public static void ClearAll()
    {
        VentKillTargets.Clear();
    }

    public static void ClearForPlayer(byte playerId)
    {
        VentKillTargets.Remove(playerId);
    }
}
