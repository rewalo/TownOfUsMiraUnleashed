using HarmonyLib;
using MiraAPI.Roles;
using TownOfUs.Roles.Crewmate;

namespace MiraUnleashed.Patches.Trapper;

/// <summary>
/// Patches to rename the existing Trapper role (ground traps) to Revealer.
/// TOU-Mira 1.7.3 resolves role name/descriptions through ICustomRole.IdPart locale IDs,
/// so swapping IdPart to "Revealer" renames name/intro/tab/wiki/ability strings in one place.
/// The TownOfUsMira.Role.Revealer* strings are provided by this mod's locale file.
/// </summary>
[HarmonyPatch(typeof(TrapperRole), nameof(ICustomRole.IdPart), MethodType.Getter)]
public static class RenameTrapperToRevealerPatches
{
    [HarmonyPostfix]
    public static void Postfix(ref string __result)
    {
        __result = "Revealer";
    }
}
