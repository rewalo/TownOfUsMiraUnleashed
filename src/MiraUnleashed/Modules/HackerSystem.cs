using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Attributes;
using TownOfUs.Utilities;
using UnityEngine;
using Object = UnityEngine.Object;
using Resources = UnityEngine.Resources;

namespace MiraUnleashed.Modules;

public enum HackerInfoSource : byte
{
    None = 0,
    Admin = 1,
    Cameras = 2,
    Vitals = 3,
    DoorLog = 4
}

public static class HackerSystem
{
    private static readonly Dictionary<byte, HackerInfoSource> LockedSourceByPlayer = new();
    private static readonly Dictionary<byte, float> BatterySecondsByPlayer = new();
    private static readonly Dictionary<byte, byte> JamChargesByPlayer = new();
    private static SystemConsole[] _cachedSystemConsoles = null!;
    private static MapConsole[] _cachedMapConsoles = null!;
    private static SystemConsole _cachedCameraConsole = null!;
    private static SystemConsole _cachedDoorLogConsole = null!;
    private static int _cachedConsoleFrame = -1;

    public static float JamActiveUntil { get; private set; }

    public static bool IsJammed => Time.time < JamActiveUntil;

    public static void ResetAll()
    {
        LockedSourceByPlayer.Clear();
        BatterySecondsByPlayer.Clear();
        JamChargesByPlayer.Clear();
        JamActiveUntil = 0f;
        InvalidateConsoleCache();
    }

    public static void ResetRoundState()
    {
        LockedSourceByPlayer.Clear();
        BatterySecondsByPlayer.Clear();
        JamActiveUntil = 0f;
        InvalidateConsoleCache();
    }

    private static void InvalidateConsoleCache()
    {
        _cachedSystemConsoles = null!;
        _cachedMapConsoles = null!;
        _cachedCameraConsole = null!;
        _cachedDoorLogConsole = null!;
        _cachedConsoleFrame = -1;
    }

    public static HackerInfoSource GetLockedSource(byte playerId)
    {
        return LockedSourceByPlayer.TryGetValue(playerId, out var src) ? src : HackerInfoSource.None;
    }

    public static void SetLockedSource(byte playerId, HackerInfoSource source)
    {
        if (source == HackerInfoSource.None)
        {
            LockedSourceByPlayer.Remove(playerId);
            return;
        }

        LockedSourceByPlayer[playerId] = source;
    }

    public static float GetBatterySeconds(byte playerId)
    {
        return BatterySecondsByPlayer.TryGetValue(playerId, out var v) ? v : 0f;
    }

    public static void SetBatterySeconds(byte playerId, float seconds)
    {
        if (seconds <= 0f)
        {
            BatterySecondsByPlayer.Remove(playerId);
            return;
        }

        BatterySecondsByPlayer[playerId] = seconds;
    }

    public static byte GetJamCharges(byte playerId)
    {
        return JamChargesByPlayer.TryGetValue(playerId, out var c) ? c : (byte)0;
    }

    public static void SetJamCharges(byte playerId, byte charges)
    {
        if (charges == 0)
        {
            JamChargesByPlayer.Remove(playerId);
            return;
        }

        JamChargesByPlayer[playerId] = charges;
    }

    public static void AddJamCharge(byte playerId, int delta, int maxCharges)
    {
        if (delta <= 0 || maxCharges <= 0)
        {
            return;
        }

        var current = GetJamCharges(playerId);
        var next = Mathf.Clamp(current + delta, 0, maxCharges);
        SetJamCharges(playerId, (byte)next);
    }

    public static bool TryConsumeJamCharge(byte playerId)
    {
        var current = GetJamCharges(playerId);
        if (current <= 0)
        {
            return false;
        }

        SetJamCharges(playerId, (byte)(current - 1));
        return true;
    }

    public static void ActivateJam(float durationSeconds)
    {
        var end = Time.time + Mathf.Max(0.1f, durationSeconds);
        JamActiveUntil = Mathf.Max(JamActiveUntil, end);
    }

    public static bool TryFindNearbyDownloadSource(PlayerControl player, float range, out HackerInfoSource source)
    {
        source = HackerInfoSource.None;
        if (player == null)
        {
            return false;
        }

        var pos = player.GetTruePosition();
        var bestDist = float.MaxValue;
        HackerInfoSource best = HackerInfoSource.None;

        if (TryGetAdminDistance(pos, range, out var dAdmin) && dAdmin < bestDist)
        {
            bestDist = dAdmin;
            best = HackerInfoSource.Admin;
        }

        if (TryGetCameraDistance(pos, range, out var dCam) && dCam < bestDist)
        {
            bestDist = dCam;
            best = HackerInfoSource.Cameras;
        }

        if (TryGetVitalsDistance(pos, range, out var dVit) && dVit < bestDist)
        {
            bestDist = dVit;
            best = HackerInfoSource.Vitals;
        }

        if (TryGetDoorLogDistance(pos, range, out var dDoor) && dDoor < bestDist)
        {
            best = HackerInfoSource.DoorLog;
        }

        source = best;
        return best != HackerInfoSource.None;
    }

    public static bool IsPlayerNearSource(PlayerControl player, HackerInfoSource source, float range)
    {
        if (player == null)
        {
            return false;
        }

        var pos = player.GetTruePosition();
        return source switch
        {
            HackerInfoSource.Admin => TryGetAdminDistance(pos, range, out _),
            HackerInfoSource.Cameras => TryGetCameraDistance(pos, range, out _),
            HackerInfoSource.Vitals => TryGetVitalsDistance(pos, range, out _),
            HackerInfoSource.DoorLog => TryGetDoorLogDistance(pos, range, out _),
            _ => false
        };
    }

    private static bool TryGetAdminDistance(Vector2 from, float range, out float dist)
    {
        dist = float.MaxValue;

        if (_cachedMapConsoles == null || Time.frameCount != _cachedConsoleFrame)
        {
            _cachedMapConsoles = Object.FindObjectsOfType<MapConsole>();
            _cachedConsoleFrame = Time.frameCount;
        }

        var consoles = _cachedMapConsoles;
        if (consoles == null || consoles.Length == 0)
        {
            return false;
        }

        foreach (var mc in consoles)
        {
            if (mc == null)
            {
                continue;
            }

            var p = (Vector2)mc.transform.position;
            var d = Vector2.Distance(from, p);
            if (d <= range && d < dist)
            {
                dist = d;
            }
        }

        return dist < float.MaxValue;
    }

    private static bool TryGetVitalsDistance(Vector2 from, float range, out float dist)
    {
        dist = float.MaxValue;

        var consoles = GetCachedSystemConsoles();
        if (consoles.Length == 0)
        {
            return false;
        }

        foreach (var sc in consoles)
        {
            if (sc == null || sc.MinigamePrefab == null)
            {
                continue;
            }

            if (sc.MinigamePrefab.TryCast<VitalsMinigame>() == null && !sc.name.Contains("vitals", System.StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var p = (Vector2)sc.transform.position;
            var d = Vector2.Distance(from, p);
            if (d <= range && d < dist)
            {
                dist = d;
            }
        }

        return dist < float.MaxValue;
    }

    private static bool TryGetCameraDistance(Vector2 from, float range, out float dist)
    {
        dist = float.MaxValue;
        if (!TryFindCameraConsole(out var sc) || sc == null)
        {
            return false;
        }

        var p = (Vector2)sc.transform.position;
        var d = Vector2.Distance(from, p);
        if (d <= range)
        {
            dist = d;
            return true;
        }

        return false;
    }

    private static bool TryGetDoorLogDistance(Vector2 from, float range, out float dist)
    {
        dist = float.MaxValue;

        var mapId = (ExpandedMapNames)GameOptionsManager.Instance.currentNormalGameOptions.MapId;
        if (TutorialManager.InstanceExists)
        {
            mapId = (ExpandedMapNames)AmongUsClient.Instance.TutorialMapId;
        }

        if (mapId is ExpandedMapNames.MiraHq)
        {
            var miraDoorLogPos = new Vector2(15.9f, 4.8f);
            var d = Vector2.Distance(from, miraDoorLogPos);
            if (d <= range)
            {
                dist = d;
                return true;
            }
            return false;
        }

        if (!TryFindDoorLogConsole(out var sc) || sc == null)
        {
            return false;
        }

        var p = (Vector2)sc.transform.position;
        var d2 = Vector2.Distance(from, p);
        if (d2 <= range)
        {
            dist = d2;
            return true;
        }

        return false;
    }

    [HideFromIl2Cpp]
    public static bool TryFindCameraConsole(out SystemConsole console)
    {
        console = null!;

        if (_cachedCameraConsole != null && _cachedCameraConsole.gameObject != null && _cachedCameraConsole.gameObject.activeInHierarchy)
        {
            console = _cachedCameraConsole;
            return true;
        }

        _cachedCameraConsole = null!;

        var mapId = (ExpandedMapNames)GameOptionsManager.Instance.currentNormalGameOptions.MapId;
        if (TutorialManager.InstanceExists)
        {
            mapId = (ExpandedMapNames)AmongUsClient.Instance.TutorialMapId;
        }

        var consoles = GetCachedSystemConsoles();
        if (consoles.Length == 0)
        {
            return false;
        }

        SystemConsole? result = default;
        if (mapId is ExpandedMapNames.Airship)
        {
            var found = consoles.FirstOrDefault(x => x != null && x.gameObject.name.Contains("task_cams"));
            if (found != null) result = found;
        }
        else if (mapId is ExpandedMapNames.Skeld or ExpandedMapNames.Dleks)
        {
            var found = consoles.FirstOrDefault(x => x != null && x.gameObject.name.Contains("SurvConsole"));
            if (found != null) result = found;
        }
        else if (mapId is ExpandedMapNames.MiraHq)
        {
            var found = consoles.FirstOrDefault(IsDoorLogConsole);
            if (found == null) found = consoles.FirstOrDefault(x => x != null && x.gameObject.name.Contains("SurvLogConsole"));
            if (found != null) result = found;
        }
        else if (mapId is ExpandedMapNames.Submerged)
        {
            var found = consoles.FirstOrDefault(x => x != null && x.gameObject.name.Contains("SecurityConsole"));
            if (found != null) result = found;
        }
        else
        {
            var found = consoles.FirstOrDefault(x =>
                x != null && (x.gameObject.name.Contains("Surv_Panel") || x.name.Contains("Cam") ||
                              x.name.Contains("BinocularsSecurityConsole")));
            if (found != null) result = found;
        }

        if (result != null)
        {
            _cachedCameraConsole = result;
            console = result;
            return true;
        }

        return false;
    }

    [HideFromIl2Cpp]
    public static bool TryFindDoorLogConsole(out SystemConsole? console)
    {
        console = default;

        if (_cachedDoorLogConsole != null && _cachedDoorLogConsole.gameObject != null && _cachedDoorLogConsole.gameObject.activeInHierarchy)
        {
            console = _cachedDoorLogConsole;
            return true;
        }

        _cachedDoorLogConsole = null!;

        var consoles = GetCachedSystemConsoles();
        if (consoles.Length == 0)
        {
            return false;
        }

        var found1 = consoles.FirstOrDefault(IsDoorLogConsole);
        SystemConsole result = null!;
        if (found1 != null)
        {
            result = found1;
        }
        else
        {
            var found2 = consoles.FirstOrDefault(x =>
                x != null &&
                (x.gameObject.name.Contains("DoorLog", System.StringComparison.OrdinalIgnoreCase) ||
                 x.gameObject.name.Contains("SurvLogConsole", System.StringComparison.OrdinalIgnoreCase) ||
                 x.gameObject.name.Contains("SurvLog", System.StringComparison.OrdinalIgnoreCase) ||
                 x.name.Contains("DoorLog", System.StringComparison.OrdinalIgnoreCase)));
            if (found2 != null) result = found2;
        }

        if (result != null)
        {
            _cachedDoorLogConsole = result;
            console = result;
            return true;
        }

        return false;
    }

    [HideFromIl2Cpp]
    private static bool IsDoorLogConsole(SystemConsole console)
    {
        if (console == null || console.MinigamePrefab == null)
        {
            return false;
        }

        if (console.MinigamePrefab.TryCast<SecurityLogGame>() != null)
        {
            return true;
        }

        return console.gameObject.name.Contains("SurvLogConsole", System.StringComparison.OrdinalIgnoreCase) ||
               console.gameObject.name.Contains("DoorLog", System.StringComparison.OrdinalIgnoreCase);
    }

    private static SystemConsole[] GetCachedSystemConsoles()
    {
        if (_cachedSystemConsoles == null || Time.frameCount != _cachedConsoleFrame)
        {
            _cachedSystemConsoles = FindAllSystemConsoles();
            _cachedConsoleFrame = Time.frameCount;
        }

        return _cachedSystemConsoles ?? System.Array.Empty<SystemConsole>();
    }

    private static SystemConsole[] FindAllSystemConsoles()
    {
        var consoles = Object.FindObjectsOfType<SystemConsole>();
        if (consoles != null && consoles.Length > 0)
        {
            return consoles;
        }

        try
        {
            var allObjects = Resources.FindObjectsOfTypeAll(Il2CppType.From(typeof(SystemConsole)));
            if (allObjects == null)
            {
                return System.Array.Empty<SystemConsole>();
            }

            var result = new List<SystemConsole>();
            foreach (var obj in allObjects)
            {
                if (obj == null)
                {
                    continue;
                }

                var sc = obj.TryCast<SystemConsole>();
                if (sc == null || sc.gameObject == null)
                {
                    continue;
                }

                if (!sc.gameObject.scene.isLoaded)
                {
                    continue;
                }

                result.Add(sc);
            }

            return result.ToArray();
        }
        catch (System.Exception e)
        {
            Warning($"HackerSystem: FindObjectsOfTypeAll(SystemConsole) failed: {e.Message}");
            return System.Array.Empty<SystemConsole>();
        }
    }
}
