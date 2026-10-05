using System.Reflection;
using HarmonyLib;
using TownOfUs.Roles.Crewmate;

namespace MiraUnleashed.Patches.Trapper;

/// <summary>
/// Patches to rename the existing Trapper role (ground traps) to Revealer.
/// TOU-Mira 1.7.3 resolves role name/descriptions through ICustomRole.IdPart locale IDs,
/// so swapping IdPart to "Revealer" renames name/intro/tab/wiki/ability strings in one place.
/// The TownOfUsMira.Role.Revealer* strings are provided by this mod's locale file.
/// </summary>
[HarmonyPatch]
public static class RenameTrapperToRevealerPatches
{
    private static readonly string[] FallbackGetters =
    [
        "get_RoleName",
        "get_RoleDescription",
        "get_RoleMedDescription",
        "get_RoleLongDescription",
        "get_RoleWikiDescription",
    ];

    private static IEnumerable<MethodBase> TargetMethods()
    {
        var idPartGetter = AccessTools.PropertyGetter(typeof(TrapperRole), "IdPart");
        if (idPartGetter != null)
        {
            yield return idPartGetter;
            yield break;
        }

        foreach (var name in FallbackGetters)
        {
            var method = AccessTools.Method(typeof(TrapperRole), name);
            if (method != null)
            {
                yield return method;
            }
        }
    }

    private static void Postfix(MethodBase __originalMethod, ref string __result)
    {
        __result = __originalMethod.Name switch
        {
            "get_IdPart" => "Revealer",
            "get_RoleName" => MiraLocaleManager.Get("TownOfUsMira.Role.Revealer", "Revealer"),
            "get_RoleDescription" => MiraLocaleManager.GetParsed("TownOfUsMira.Role.Revealer.IntroBlurb", []),
            "get_RoleMedDescription" or "get_RoleLongDescription" or "get_RoleWikiDescription" =>
                MiraLocaleManager.GetParsed("TownOfUsMira.Role.Revealer.TabDescription", []),
            _ => __result,
        };
    }
}
