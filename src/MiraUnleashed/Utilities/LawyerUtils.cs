using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Modifiers;
using MiraUnleashed.Modifiers;
using MiraUnleashed.Roles.Neutral;
using TownOfUs.Utilities;

namespace MiraUnleashed.Utilities;

public static class LawyerUtils
{
    [HideFromIl2Cpp]
    public static PlayerControl? FindClientForLawyer(byte lawyerId)
    {
        foreach (var pc in PlayerControl.AllPlayerControls)
        {
            if (pc == null)
            {
                continue;
            }

            if (pc.HasModifier<LawyerTargetModifier>(m => m.OwnerId == lawyerId))
            {
                return pc;
            }
        }

        return null;
    }

    [HideFromIl2Cpp]
    public static LawyerRole? GetLawyerForClient(PlayerControl client, byte lawyerId)
    {
        if (client == null || !client.HasModifier<LawyerTargetModifier>(x => x.OwnerId == lawyerId))
        {
            return null;
        }

        foreach (var pc in PlayerControl.AllPlayerControls)
        {
            if (pc != null && pc.PlayerId == lawyerId && pc.IsRole<LawyerRole>())
            {
                return pc.GetRole<LawyerRole>();
            }
        }

        return null;
    }

    public static List<LawyerRole> GetAllLawyersForClient(PlayerControl client)
    {
        if (client == null)
        {
            return [];
        }

        var lawyerModifiers = client.GetModifiers<LawyerTargetModifier>();
        var lawyers = new List<LawyerRole>();

        foreach (var modifier in lawyerModifiers)
        {
            foreach (var pc in PlayerControl.AllPlayerControls)
            {
                if (pc != null && pc.PlayerId == modifier.OwnerId && pc.IsRole<LawyerRole>())
                {
                    var lawyerRole = pc.GetRole<LawyerRole>();
                    if (lawyerRole != null)
                    {
                        lawyers.Add(lawyerRole);
                    }

                    break;
                }
            }
        }

        return lawyers;
    }

    [HideFromIl2Cpp]
    public static PlayerControl? GetClientForLawyer(PlayerControl lawyer)
    {
        if (lawyer == null)
        {
            return null;
        }

        var lawyerRole = lawyer.GetRole<LawyerRole>();
        return lawyerRole?.Client ?? FindClientForLawyer(lawyer.PlayerId);
    }

    public static bool IsClientOfLawyer(PlayerControl player, byte lawyerId)
    {
        return player != null && player.HasModifier<LawyerTargetModifier>(x => x.OwnerId == lawyerId);
    }

    public static bool IsClientOfAnyLawyer(PlayerControl player)
    {
        return player != null && player.HasModifier<LawyerTargetModifier>();
    }

    public static bool HasLawyerClientRelationship(PlayerControl lawyer, PlayerControl client)
    {
        if (lawyer == null || client == null)
        {
            return false;
        }

        var lawyerRole = lawyer.GetRole<LawyerRole>();
        return lawyerRole != null &&
               lawyerRole.Client != null &&
               lawyerRole.Client.PlayerId == client.PlayerId;
    }
}
