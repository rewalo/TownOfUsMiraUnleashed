using HarmonyLib;
using MiraAPI.Roles;
using ScavengerRole = TownOfUs.Roles.Impostor.ScavengerRole;

namespace MiraUnleashed.Patches.Scavenger;

/// <summary>
/// Renames TOU-Mira's Impostor Scavenger role to Bloodhound so it doesn't collide with
/// this mod's Neutral Scavenger. The TownOfUsMira.Role.Bloodhound* strings are provided
/// by this mod's locale file.
/// </summary>
[HarmonyPatch(typeof(ScavengerRole), nameof(ICustomRole.IdPart), MethodType.Getter)]
public static class RenameScavengerToBloodhoundPatch
{
    [HarmonyPostfix]
    public static void Postfix(ref string __result)
    {
        __result = "Bloodhound";
    }
}
