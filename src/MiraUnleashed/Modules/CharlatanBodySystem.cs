using MiraAPI.GameOptions;
using MiraUnleashed.Options.Roles.Impostor;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MiraUnleashed.Modules;

public enum CharlatanBodyState : byte
{
    Channeling,
    Concealed
}

public static class CharlatanBodySystem
{
    private sealed class BodyEntry(byte charlatanId)
    {
        public byte CharlatanId { get; } = charlatanId;
        public CharlatanBodyState State { get; set; }
        public DeadBody? Body { get; set; }
        public float LastAppliedAlpha { get; set; } = 1f;
    }

    private sealed record DeceiveEntry(byte BodyId, float ExpiresAt);

    private static readonly Dictionary<byte, BodyEntry> Bodies = new();
    private static readonly Dictionary<byte, DeceiveEntry> Deceives = new();

    public static void ClearAll()
    {
        foreach (var entry in Bodies.Values)
        {
            RestoreBodyAlpha(entry);
            entry.Body = null;
        }
        Bodies.Clear();
        Deceives.Clear();
    }

    public static void ClearForPlayer(byte charlatanId)
    {
        var toRemove = Bodies.Where(kvp => kvp.Value.CharlatanId == charlatanId).Select(kvp => kvp.Key).ToList();
        foreach (var bodyId in toRemove)
        {
            ClearBody(bodyId);
        }
        Deceives.Remove(charlatanId);
    }

    public static void StartChannel(byte charlatanId, byte bodyId)
    {
        Bodies[bodyId] = new BodyEntry(charlatanId) { State = CharlatanBodyState.Channeling };
    }

    public static void CompleteChannel(byte charlatanId, byte bodyId)
    {
        if (Bodies.TryGetValue(bodyId, out var entry))
        {
            entry.State = CharlatanBodyState.Concealed;
        }
        else
        {
            Bodies[bodyId] = new BodyEntry(charlatanId) { State = CharlatanBodyState.Concealed };
        }
    }

    public static void ClearBody(byte bodyId)
    {
        if (Bodies.TryGetValue(bodyId, out var entry))
        {
            RestoreBodyAlpha(entry);
            Bodies.Remove(bodyId);
        }
    }

    public static bool IsBodyTracked(byte bodyId) => Bodies.ContainsKey(bodyId);

    public static bool IsBodyConcealed(byte bodyId)
    {
        return Bodies.TryGetValue(bodyId, out var entry) && entry.State == CharlatanBodyState.Concealed;
    }

    public static float GetConcealedReportRange(byte bodyId)
    {
        if (!IsBodyConcealed(bodyId))
        {
            return -1f;
        }

        var options = OptionGroupSingleton<CharlatanOptions>.Instance;
        return options.ConcealReportRange switch
        {
            ReportRangeType.ExtremelyShort => 0.3f,
            ReportRangeType.VeryShort => 0.5f,
            ReportRangeType.Short => 0.75f,
            _ => 0.5f
        };
    }

    public static void ActivateDeceive(byte charlatanId, byte bodyId, float duration)
    {
        Deceives[charlatanId] = new DeceiveEntry(bodyId, Time.time + duration);
    }

    public static bool CanDeceiveReport(byte charlatanId, byte bodyId)
    {
        if (!Deceives.TryGetValue(charlatanId, out var state))
        {
            return false;
        }

        if (state.BodyId != bodyId)
        {
            return false;
        }

        if (Time.time >= state.ExpiresAt)
        {
            Deceives.Remove(charlatanId);
            return false;
        }

        return true;
    }

    public static float GetRemainingTime(byte charlatanId)
    {
        if (!Deceives.TryGetValue(charlatanId, out var state))
        {
            return 0f;
        }

        return Mathf.Max(0f, state.ExpiresAt - Time.time);
    }

    public static void UpdateBodyTransparency()
    {
        if (Bodies.Count == 0)
        {
            return;
        }

        var options = OptionGroupSingleton<CharlatanOptions>.Instance;
        var concealedAlpha = options.ConcealReportRange switch
        {
            ReportRangeType.ExtremelyShort => 0.08f,
            ReportRangeType.VeryShort => 0.15f,
            ReportRangeType.Short => 0.4f,
            _ => 0.1f
        };

        foreach (var (bodyId, entry) in Bodies)
        {
            var alpha = entry.State == CharlatanBodyState.Concealed ? concealedAlpha : 1f;

            if (entry.Body == null)
            {
                entry.Body = FindBody(bodyId);
            }

            var body = entry.Body;
            if (body == null || Mathf.Approximately(entry.LastAppliedAlpha, alpha))
            {
                continue;
            }

            SetBodyAlpha(body, alpha);
            entry.LastAppliedAlpha = alpha;
        }
    }

    private static DeadBody? FindBody(byte bodyId)
    {
        return Object.FindObjectsOfType<DeadBody>().FirstOrDefault(x => x.ParentId == bodyId);
    }

    private static void RestoreBodyAlpha(BodyEntry entry)
    {
        if (entry.Body != null && !Mathf.Approximately(entry.LastAppliedAlpha, 1f))
        {
            SetBodyAlpha(entry.Body, 1f);
        }
        entry.LastAppliedAlpha = 1f;
    }

    private static void SetBodyAlpha(DeadBody body, float alpha)
    {
        if (body == null)
        {
            return;
        }

        foreach (var sr in body.GetComponentsInChildren<SpriteRenderer>(true))
        {
            if (sr == null)
            {
                continue;
            }

            var c = sr.color;
            c.a = Mathf.Clamp01(alpha);
            sr.color = c;
        }

        foreach (var tmp in body.GetComponentsInChildren<TMP_Text>(true))
        {
            if (tmp == null)
            {
                continue;
            }

            var c = tmp.color;
            c.a = Mathf.Clamp01(alpha);
            tmp.color = c;
        }
    }
}
