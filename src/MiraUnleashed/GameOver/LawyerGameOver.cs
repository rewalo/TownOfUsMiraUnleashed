using MiraAPI.GameEnd;
using MiraAPI.GameOptions;
using MiraAPI.Utilities;
using MiraUnleashed.Options.Roles.Neutral;
using MiraUnleashed.Roles.Neutral;
using MiraUnleashed.Utilities;
using Reactor.Utilities.Extensions;
using TownOfUs;
using TownOfUs.Modules;
using TownOfUs.Utilities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MiraUnleashed.GameOver;

public sealed class LawyerGameOver : CustomGameOver
{
    public override bool VerifyCondition(PlayerControl playerControl, NetworkedPlayerInfo[] winners)
    {
        if (winners == null || winners.Length < 2 || playerControl == null)
        {
            return false;
        }

        var winMode = OptionGroupSingleton<LawyerOptions>.Instance.WinMode;

        if (winMode == LawyerWinMode.WinWithClient)
        {
            // When "Win with client", check if the player is in the winners list
            // The winners list should include all players from the client's team (including the lawyer)
            return winners.Any(w => w?.Object != null && w.Object.PlayerId == playerControl.PlayerId);
        }

        var winningLawyers = ExtractWinningLawyers(winners);

        if (winningLawyers.Count == 0)
        {
            return false;
        }

        if (!ValidateStealWinWinners(winners, winningLawyers))
        {
            return false;
        }

        return IsPlayerWinner(playerControl, winners, winningLawyers);
    }

    public override void AfterEndGameSetup(EndGameManager endGameManager)
    {
        var (winColor, winText) = DetermineWinCondition();
        SetWinningFaction(winColor, winText);
        SetupWinScreen(endGameManager, winColor, winText);
    }

    private static List<PlayerControl> ExtractWinningLawyers(NetworkedPlayerInfo[] winners)
    {
        return winners
            .Where(w => w?.Object != null)
            .Select(w => w.Object)
            .Where(pc => pc.IsRole<LawyerRole>())
            .ToList();
    }

    private static bool ValidateStealWinWinners(NetworkedPlayerInfo[] winners, List<PlayerControl> winningLawyers)
    {
        foreach (var w in winners)
        {
            var pc = w?.Object;
            if (pc == null)
            {
                return false;
            }

            if (pc.IsRole<LawyerRole>())
            {
                continue;
            }

            var isClient = winningLawyers.Any(lawyerPc => LawyerUtils.IsClientOfLawyer(pc, lawyerPc.PlayerId));
            if (!isClient)
            {
                return false;
            }
        }

        foreach (var lawyerPc in winningLawyers)
        {
            var hasClientWinner = winners.Any(w =>
            {
                var obj = w?.Object;
                return obj != null &&
                       !obj.IsRole<LawyerRole>() &&
                       LawyerUtils.IsClientOfLawyer(obj, lawyerPc.PlayerId);
            });
            if (!hasClientWinner)
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsPlayerWinner(PlayerControl playerControl, NetworkedPlayerInfo[] winners,
        List<PlayerControl> winningLawyers)
    {
        var playerInWinners = winners.Any(w =>
        {
            var pc = w?.Object;
            return pc != null && pc.PlayerId == playerControl.PlayerId;
        });

        if (!playerInWinners)
        {
            return false;
        }

        if (playerControl.IsRole<LawyerRole>())
        {
            return true;
        }

        return winningLawyers.Any(lawyerPc => LawyerUtils.IsClientOfLawyer(playerControl, lawyerPc.PlayerId));
    }

    private static (Color winColor, string winText) DetermineWinCondition()
    {
        return (TownOfUsColors.Lawyer,
            $"{MiraLocaleManager.Get("MiraUnleashed.Role.Lawyer", "Lawyer")} {MiraLocaleManager.Get("MiraUnleashed.Lawyer.Win", "Wins")}");
    }

    private static void SetWinningFaction(Color winColor, string winText)
    {
        GameHistory.WinningFaction = $"<color=#{winColor.ToHtmlStringRGBA()}>{winText}</color>";
    }

    private static void SetupWinScreen(EndGameManager endGameManager, Color winColor, string winText)
    {
        endGameManager.BackgroundBar.material.SetColor(ShaderID.Color, winColor);

        var text = Object.Instantiate(endGameManager.WinText);
        text.text = $"<size=4>{winText}!</size>";
        text.color = winColor;

        var pos = endGameManager.WinText.transform.localPosition;
        pos.y = 1.5f;
        pos += Vector3.down * 0.15f;
        text.transform.localScale = new Vector3(1f, 1f, 1f);
        text.transform.position = pos;
    }
}
