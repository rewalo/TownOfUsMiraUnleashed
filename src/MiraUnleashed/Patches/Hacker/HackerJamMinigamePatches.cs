using HarmonyLib;
using MiraUnleashed.Modules;
using UnityEngine;
using UnityEngine.UI;

namespace MiraUnleashed.Patches.Hacker;

[HarmonyPatch]
public static class HackerJamMinigamePatches
{
    private static void ApplyAdminJammed(MapCountOverlay overlay)
    {
        if (overlay == null)
        {
            return;
        }

        overlay.isSab = true;
        overlay.BackgroundColor?.SetColor(Palette.DisabledGrey);
        overlay.SabotageText?.gameObject.SetActive(true);

        if (overlay.CountAreas != null)
        {
            foreach (var area in overlay.CountAreas)
            {
                area?.UpdateCount(0);
            }
        }
    }

    private static void ApplyCamsJammed(SurveillanceMinigame cams)
    {
        if (cams == null)
        {
            return;
        }

        cams.isStatic = true;
        if (cams.ViewPorts != null)
        {
            foreach (var vp in cams.ViewPorts.Where(v => v != null))
            {
                vp.sharedMaterial = cams.StaticMaterial;
            }
        }

        if (cams.SabText != null)
        {
            foreach (var t in cams.SabText)
            {
                t?.gameObject.SetActive(true);
            }
        }
    }

    private static void ApplyPlanetCamsJammed(PlanetSurveillanceMinigame cams)
    {
        if (cams == null)
        {
            return;
        }

        if (cams.ViewPort != null)
        {
            cams.ViewPort.sharedMaterial = cams.StaticMaterial;
        }

        cams.SabText?.gameObject.SetActive(true);
    }

    private static void TrySetActive(object? obj, bool active)
    {
        if (obj == null)
        {
            return;
        }

        switch (obj)
        {
            case GameObject go:
                go.SetActive(active);
                return;
            case Component comp:
                comp.gameObject.SetActive(active);
                return;
            case System.Collections.IEnumerable enumerable:
                {
                    foreach (var item in enumerable)
                    {
                        TrySetActive(item, active);
                    }

                    return;
                }
        }
    }

    private static void SetVitalsCommsUi(VitalsMinigame vitals, bool active)
    {
        var type = vitals.GetType();
        var candidateFields = new[]
        {
            "SabText", "SabotageText", "NoCommsText", "CommsDownText", "CommsText",
            "CommsDisabledText", "DisabledText", "sabotageText", "sabText"
        };

        var candidateContainers = new[]
        {
            "SabotagePanel", "NoCommsPanel", "CommsDownPanel", "CommsDisabledPanel",
            "sabotagePanel", "noCommsPanel"
        };

        foreach (var fName in candidateFields.Concat(candidateContainers))
        {
            try
            {
                var field = AccessTools.Field(type, fName);
                if (field == null)
                {
                    continue;
                }

                var value = field.GetValue(vitals);
                TrySetActive(value, active);
            }
            catch (System.Exception e)
            {
                Warning($"HackerJam: failed to toggle VitalsMinigame field '{fName}': {e.Message}");
            }
        }
    }

    [HarmonyPatch(typeof(MapCountOverlay), nameof(MapCountOverlay.OnEnable))]
    [HarmonyPrefix]
    public static void MapCountOverlayOnEnablePrefix(MapCountOverlay __instance)
    {
        if (__instance == null)
        {
            return;
        }

        __instance.SetOptions(false, true);

        if (HackerSystem.IsJammed)
        {
            ApplyAdminJammed(__instance);
        }
    }

    [HarmonyPatch(typeof(MapCountOverlay), nameof(MapCountOverlay.Update))]
    [HarmonyPrefix]
    public static bool MapCountOverlayUpdatePrefix(MapCountOverlay __instance)
    {
        if (__instance == null)
        {
            return true;
        }

        if (!HackerSystem.IsJammed)
        {
            return true;
        }

        ApplyAdminJammed(__instance);

        return false;
    }

    [HarmonyPatch(typeof(SurveillanceMinigame), nameof(SurveillanceMinigame.Begin))]
    [HarmonyPostfix]
    public static void SurveillanceMinigameBeginPostfix(SurveillanceMinigame __instance)
    {
        if (__instance != null && HackerSystem.IsJammed)
        {
            ApplyCamsJammed(__instance);
        }
    }

    [HarmonyPatch(typeof(SurveillanceMinigame), nameof(SurveillanceMinigame.Update))]
    [HarmonyPrefix]
    public static bool SurveillanceMinigameUpdatePrefix(SurveillanceMinigame __instance)
    {
        if (__instance == null)
        {
            return true;
        }

        if (!HackerSystem.IsJammed)
        {
            return true;
        }

        ApplyCamsJammed(__instance);

        return false;
    }

    [HarmonyPatch(typeof(PlanetSurveillanceMinigame), nameof(PlanetSurveillanceMinigame.Begin))]
    [HarmonyPostfix]
    public static void PlanetSurveillanceMinigameBeginPostfix(PlanetSurveillanceMinigame __instance)
    {
        if (__instance != null && HackerSystem.IsJammed)
        {
            ApplyPlanetCamsJammed(__instance);
        }
    }

    [HarmonyPatch(typeof(PlanetSurveillanceMinigame), nameof(PlanetSurveillanceMinigame.NextCamera))]
    private static class PlanetSurveillanceMinigameNextCameraPatch
    {
        [HarmonyPrefix]
        private static bool Prefix(PlanetSurveillanceMinigame __instance, int direction)
        {
            if (__instance == null)
            {
                return true;
            }

            if (HackerSystem.IsJammed)
            {
                ApplyPlanetCamsJammed(__instance);

                if (direction != 0 && Constants.ShouldPlaySfx())
                {
                    SoundManager.Instance.PlaySound(__instance.ChangeSound, false, 1f);
                }

                __instance.Dots[__instance.currentCamera].sprite = __instance.DotDisabled;

                var len = __instance.survCameras != null ? __instance.survCameras.Length : 0;
                if (len > 0)
                {
                    var next = (__instance.currentCamera + direction) % len;
                    if (next < 0)
                    {
                        next += len;
                    }

                    __instance.currentCamera = next;
                }

                __instance.Dots[__instance.currentCamera].sprite = __instance.DotEnabled;

                var survCamera = __instance.survCameras?[__instance.currentCamera];
                if (survCamera != null)
                {
                    __instance.Camera.transform.position = survCamera.transform.position + survCamera.Offset;
                    __instance.LocationName.text = survCamera.CamName;
                }

                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(PlanetSurveillanceMinigame), nameof(PlanetSurveillanceMinigame.Update))]
    [HarmonyPrefix]
    public static bool PlanetSurveillanceMinigameUpdatePrefix(PlanetSurveillanceMinigame __instance)
    {
        if (__instance == null)
        {
            return true;
        }

        if (!HackerSystem.IsJammed)
        {
            return true;
        }

        ApplyPlanetCamsJammed(__instance);

        return false;
    }

    [HarmonyPatch(typeof(FungleSurveillanceMinigame), nameof(FungleSurveillanceMinigame.Update))]
    [HarmonyPrefix]
    public static bool FungleSurveillanceMinigameUpdatePrefix(FungleSurveillanceMinigame __instance)
    {
        if (__instance == null)
        {
            return true;
        }

        if (!HackerSystem.IsJammed)
        {
            return true;
        }

        __instance.Close();
        return false;
    }

    private static void ApplyDoorLogJammed(SecurityLogGame doorLog)
    {
        if (doorLog == null)
        {
            return;
        }

        if (doorLog.SabText != null)
        {
            doorLog.SabText.gameObject.SetActive(true);
        }

        var allChildren = doorLog.transform.GetComponentsInChildren<Transform>(true);
        if (allChildren != null)
        {
            Transform? sabTextTransform = doorLog.SabText != null ? doorLog.SabText.transform : null;

            Transform? logContainer = null;
            foreach (var child in allChildren)
            {
                if (child == null || child == doorLog.transform)
                {
                    continue;
                }

                var scrollRect = child.GetComponent<ScrollRect>();
                if (scrollRect != null && scrollRect.content != null)
                {
                    logContainer = scrollRect.content;
                    break;
                }

                var name = child.name.ToLowerInvariant();
                if (name.Contains("content") || name.Contains("viewport") ||
                    (name.Contains("scroll") && name.Contains("view")))
                {
                    logContainer = child;
                    break;
                }
            }

            if (logContainer != null)
            {
                foreach (Il2CppSystem.Object entryObj in logContainer)
                {
                    if (entryObj.TryCast<Transform>() is { } entry && entry != logContainer && entry != sabTextTransform)
                    {
                        entry.gameObject.SetActive(false);
                    }
                }
            }

            foreach (var child in allChildren)
            {
                if (child == null || child == doorLog.transform || child == sabTextTransform)
                {
                    continue;
                }

                var name = child.name.ToLowerInvariant();

                if (name.Contains("log") && (name.Contains("entry") || name.Contains("item") || name.Contains("row")))
                {
                    child.gameObject.SetActive(false);
                }
            }
        }
    }

    [HarmonyPatch(typeof(SecurityLogGame), nameof(SecurityLogGame.Update))]
    [HarmonyPrefix]
    public static bool SecurityLogGameUpdatePrefix(SecurityLogGame __instance)
    {
        if (__instance == null)
        {
            return true;
        }

        if (HackerSystem.IsJammed)
        {
            ApplyDoorLogJammed(__instance);
            return false;
        }

        return true;
    }

    [HarmonyPatch(typeof(VitalsMinigame), nameof(VitalsMinigame.Update))]
    [HarmonyPrefix]
    public static bool VitalsMinigameUpdatePrefix(VitalsMinigame __instance)
    {
        if (__instance == null)
        {
            return true;
        }

        if (HackerSystem.IsJammed)
        {
            if (__instance.vitals != null)
            {
                foreach (var panel in __instance.vitals)
                {
                    panel?.gameObject.SetActive(false);
                }
            }

            if (__instance.SabText != null)
            {
                __instance.SabText.gameObject.SetActive(true);
            }
            else
            {
                SetVitalsCommsUi(__instance, true);
            }

            return false;
        }

        var localPlayer = PlayerControl.LocalPlayer;
        if (localPlayer != null && !PlayerTask.PlayerHasTaskOfType<IHudOverrideTask>(localPlayer))
        {
            if (__instance.vitals != null)
            {
                foreach (var panel in __instance.vitals)
                {
                    panel?.gameObject.SetActive(true);
                }
            }

            __instance.SabText?.gameObject.SetActive(false);

            SetVitalsCommsUi(__instance, false);
        }

        return true;
    }
}
