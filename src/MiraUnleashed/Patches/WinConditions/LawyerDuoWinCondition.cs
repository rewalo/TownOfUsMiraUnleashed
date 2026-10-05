using MiraAPI.GameEnd;
using MiraAPI.GameOptions;
using MiraAPI.Utilities;
using MiraUnleashed.GameOver;
using MiraUnleashed.Modules;
using MiraUnleashed.Options.Roles.Neutral;
using MiraUnleashed.Roles.Neutral;
using MiraUnleashed.Utilities;
using TownOfUs.Interfaces;
using TownOfUs.Modules;
using TownOfUs.Roles;
using TownOfUs.Utilities;

namespace MiraUnleashed.Patches.WinConditions;

/// <summary>
/// "Final 2" protection:
/// If the only two living players are an alive Lawyer and their alive Client, end the game.
/// This matches the intended "survivor-like" behavior where two specific players must be alive together,
/// and also bypasses any "continues game" stalling logic.
/// </summary>
public sealed class LawyerDuoWinCondition : IWinCondition, IWinConditionWithBlocking
{
    public int Priority => 12;

    public bool BlocksOthers => true;

    private static bool ClientHasWonAlone(PlayerControl client)
    {
        if (client == null || client.HasDied())
        {
            return false;
        }

        var clientRole = client.GetRoleWhenAlive();
        if (clientRole is ITownOfUsRole townOfUsRole)
        {
            return townOfUsRole.WinConditionMet();
        }

        return false;
    }

    public bool IsMet(LogicGameFlowNormal gameFlow)
    {
        if (AmongUsClient.Instance == null || !AmongUsClient.Instance.AmHost)
        {
            return false;
        }

        if (LawyerWinConditionState.Triggered)
        {
            return false;
        }

        var winMode = OptionGroupSingleton<LawyerOptions>.Instance.WinMode;
        if (winMode == LawyerWinMode.WinWithClient)
        {
            return false;
        }

        var alivePlayers = Helpers.GetAlivePlayers();
        if (alivePlayers.Count != 2)
        {
            return false;
        }

        foreach (var lawyerPc in PlayerControl.AllPlayerControls)
        {
            if (lawyerPc == null || lawyerPc.HasDied() || !lawyerPc.IsRole<LawyerRole>())
            {
                continue;
            }

            var client = LawyerUtils.FindClientForLawyer(lawyerPc.PlayerId);
            if (client == null || client.HasDied())
            {
                continue;
            }

            var alivePlayerIds = alivePlayers.Select(ap => ap.PlayerId).ToHashSet();
            if (!alivePlayerIds.Contains(lawyerPc.PlayerId) || !alivePlayerIds.Contains(client.PlayerId))
            {
                continue;
            }

            if (ClientHasWonAlone(client))
            {
                continue;
            }

            return true;
        }

        return false;
    }

    public void TriggerGameOver(LogicGameFlowNormal gameFlow)
    {
        if (AmongUsClient.Instance == null || !AmongUsClient.Instance.AmHost)
        {
            return;
        }

        if (LawyerWinConditionState.Triggered)
        {
            return;
        }

        var alivePlayers = Helpers.GetAlivePlayers();
        var winMode = OptionGroupSingleton<LawyerOptions>.Instance.WinMode;

        var winners = new HashSet<NetworkedPlayerInfo>();

        foreach (var lawyerPc in PlayerControl.AllPlayerControls)
        {
            if (lawyerPc == null || lawyerPc.HasDied() || !lawyerPc.IsRole<LawyerRole>())
            {
                continue;
            }

            var client = LawyerUtils.FindClientForLawyer(lawyerPc.PlayerId);
            if (client == null || client.HasDied())
            {
                continue;
            }

            if (lawyerPc.Data == null || client.Data == null)
            {
                continue;
            }

            var alivePlayerIds = alivePlayers.Select(ap => ap.PlayerId).ToHashSet();
            if (!alivePlayerIds.Contains(lawyerPc.PlayerId) || !alivePlayerIds.Contains(client.PlayerId))
            {
                continue;
            }

            var lawyerRole = lawyerPc.GetRole<LawyerRole>();
            if (lawyerRole != null)
            {
                lawyerRole.AboutToWin = true;
            }

            if (winMode == LawyerWinMode.WinWithClient)
            {
                return;
            }

            winners.Add(lawyerPc.Data);
            winners.Add(client.Data);
        }

        if (winners.Count < 2)
        {
            return;
        }

        LawyerWinConditionState.MarkTriggered();
        CustomGameOver.Trigger<LawyerGameOver>(winners.ToArray());
    }
}
