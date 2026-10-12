using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Events.Vanilla.Meeting;
using MiraAPI.Events.Vanilla.Player;
using MiraAPI.Modifiers;
using MiraUnleashed.Achievements;
using MiraUnleashed.Modifiers;
using MiraUnleashed.Modules;
using MiraUnleashed.Roles;
using MiraUnleashed.Roles.Impostor;
using MiraUnleashed.Roles.Neutral;
using TownOfUs.Modules;
using TownOfUs.Utilities;

namespace MiraUnleashed.Events.Achievements;

/// <summary>
/// Central hub for Mira Unleashed achievement triggers. All logic here only references
/// types that are always present; actual awarding goes through the gated
/// <see cref="AchievementsIntegration"/> bridge, so nothing here requires the
/// Achievements API mod to be installed.
/// </summary>
public static class AchievementEvents
{
    private static readonly Dictionary<byte, int> InjectionsPerTarget = new();
    private static int _meetingsSeen;
    private static int _witchHexKillsThisMeeting;
    private static int _lawyerObjectionsThisMeeting;

    [RegisterEvent]
    public static void RoundStartEventHandler(RoundStartEvent @event)
    {
        if (!@event.TriggeredByIntro)
        {
            return;
        }

        InjectionsPerTarget.Clear();
        _meetingsSeen = 0;
        _witchHexKillsThisMeeting = 0;
        _lawyerObjectionsThisMeeting = 0;

        var local = PlayerControl.LocalPlayer;
        if (local == null)
        {
            return;
        }

        if (local.Data?.Role is IMiraUnleashedRole)
        {
            AchievementsIntegration.Unlock(MuAchievements.Unleashed);
        }

        if (local.HasModifier<SpitefulModifier>() && local.HasModifier<CluelessModifier>())
        {
            AchievementsIntegration.Unlock(MuAchievements.DoubleJeopardy);
        }
    }

    [RegisterEvent]
    public static void StartMeetingEventHandler(StartMeetingEvent @event)
    {
        _meetingsSeen++;
        _witchHexKillsThisMeeting = 0;
        _lawyerObjectionsThisMeeting = 0;
    }

    [RegisterEvent]
    public static void GameEndEventHandler(GameEndEvent @event)
    {
        var local = PlayerControl.LocalPlayer;
        if (local == null || !LocalPlayerWon())
        {
            return;
        }

        switch (local.Data?.Role)
        {
            case LawyerRole:
                AchievementsIntegration.Unlock(MuAchievements.CaseClosed);
                break;
            case ScavengerRole:
                AchievementsIntegration.Unlock(MuAchievements.FullBelly);
                break;
            case SerialKillerRole:
                AchievementsIntegration.Unlock(MuAchievements.TheLastScene);
                break;
        }

        if (local.HasModifier<CluelessModifier>())
        {
            AchievementsIntegration.Unlock(MuAchievements.PureInstinct);
        }
    }

    private static bool LocalPlayerWon()
    {
        var local = PlayerControl.LocalPlayer;
        return local != null
               && EndGameResult.CachedWinners.ToArray()
                   .Any(w => w.PlayerName == local.Data?.PlayerName);
    }

    [RegisterEvent]
    public static void AfterMurderEventHandler(AfterMurderEvent @event)
    {
        var source = @event.Source;
        if (source == null || !source.AmOwner || source == @event.Target)
        {
            return;
        }

        switch (source.Data?.Role)
        {
            case SerialKillerRole:
                AchievementsIntegration.Increment(MuAchievements.SerialSeason);
                if (GameHistory.KilledPlayers.Count <= 1)
                {
                    AchievementsIntegration.Unlock(MuAchievements.ColdOpen);
                }

                break;
            case WraithRole:
                AchievementsIntegration.Increment(MuAchievements.FadeToBlack);
                if (source.HasModifier<WraithDashModifier>())
                {
                    AchievementsIntegration.Unlock(MuAchievements.HitTheGroundScreaming);
                }

                if (source.HasModifier<WraithLanternInvisibilityModifier>())
                {
                    AchievementsIntegration.Unlock(MuAchievements.LamplightLethality);
                }

                break;
            case HackerRole when HackerSystem.IsJammed:
                AchievementsIntegration.Unlock(MuAchievements.LowPingHighCrime);
                break;
        }
    }

    [RegisterEvent]
    public static void CompleteTaskEvent(CompleteTaskEvent @event)
    {
        var player = @event.Player;
        if (player == null || !player.AmOwner || !player.HasModifier<CluelessModifier>())
        {
            return;
        }

        AchievementsIntegration.Unlock(MuAchievements.WingIt);
        AchievementsIntegration.Increment(MuAchievements.MuscleMemory);
    }

    // --- Role/system hook points (called directly from role RPCs and systems) ---

    public static void OnMirageDecoyTriggered(PlayerControl mirage, PlayerControl? interactor)
    {
        if (!mirage.AmOwner || interactor == null || interactor == mirage)
        {
            return;
        }

        AchievementsIntegration.Increment(MuAchievements.CarnivalOfIllusions);
        if (GameHistory.KilledPlayers.Any(k => k.KillerId == interactor.PlayerId))
        {
            AchievementsIntegration.Unlock(MuAchievements.Gottem);
        }
    }

    public static void OnTrapperTrapTriggered(PlayerControl trapper, PlayerControl? victim)
    {
        if (!trapper.AmOwner || victim == null || victim == trapper)
        {
            return;
        }

        AchievementsIntegration.Unlock(MuAchievements.WelcomingCommittee);
        AchievementsIntegration.Increment(MuAchievements.PestControl);
        if (GameHistory.KilledPlayers.Any(k => k.KillerId == victim.PlayerId))
        {
            AchievementsIntegration.Unlock(MuAchievements.RedHanded);
        }
    }

    public static void OnCharlatanConcealComplete(PlayerControl charlatan)
    {
        if (charlatan.AmOwner)
        {
            AchievementsIntegration.Unlock(MuAchievements.BodyDouble);
        }
    }

    public static void OnCharlatanDeceive(PlayerControl charlatan)
    {
        if (charlatan.AmOwner)
        {
            AchievementsIntegration.Increment(MuAchievements.SmokeAndMirrors);
        }
    }

    public static void OnHackerJamStarted(PlayerControl hacker)
    {
        if (hacker.AmOwner)
        {
            AchievementsIntegration.Increment(MuAchievements.DdosAttack);
        }
    }

    public static void OnInjectorInject(PlayerControl injector, PlayerControl target)
    {
        if (!injector.AmOwner || target == null)
        {
            return;
        }

        AchievementsIntegration.Unlock(MuAchievements.Malpractice);
        AchievementsIntegration.Increment(MuAchievements.ClinicalTrial);

        InjectionsPerTarget[target.PlayerId] = InjectionsPerTarget.GetValueOrDefault(target.PlayerId) + 1;
        if (InjectionsPerTarget[target.PlayerId] == 2)
        {
            AchievementsIntegration.Unlock(MuAchievements.DoubleDose);
        }
    }

    public static void OnWitchSpell(PlayerControl witch)
    {
        if (!witch.AmOwner)
        {
            return;
        }

        AchievementsIntegration.Unlock(MuAchievements.HexMarksTheSpot);
        AchievementsIntegration.Increment(MuAchievements.BoilAndTrouble);
    }

    public static void OnWitchHexKill(PlayerControl witch)
    {
        if (witch.AmOwner && ++_witchHexKillsThisMeeting >= 2)
        {
            AchievementsIntegration.Unlock(MuAchievements.BubbleBubble);
        }
    }

    public static void OnForestallerLockActivated(PlayerControl forestaller)
    {
        if (!forestaller.AmOwner)
        {
            return;
        }

        AchievementsIntegration.Unlock(MuAchievements.NothingLeftToSabotage);
        if (_meetingsSeen == 0)
        {
            AchievementsIntegration.Unlock(MuAchievements.SpeedrunLockdown);
        }
    }

    public static void OnScavengerEat(PlayerControl scavenger)
    {
        if (!scavenger.AmOwner)
        {
            return;
        }

        AchievementsIntegration.Increment(MuAchievements.CleanPlateClub);
        AchievementsIntegration.Increment(MuAchievements.BuffetRegular);
    }

    public static void OnLawyerClientAssigned(PlayerControl lawyer)
    {
        if (lawyer.AmOwner)
        {
            AchievementsIntegration.Unlock(MuAchievements.CourtAppointed);
        }
    }

    public static void OnLawyerObjectVotes(PlayerControl lawyer)
    {
        if (lawyer.AmOwner && ++_lawyerObjectionsThisMeeting >= 2)
        {
            AchievementsIntegration.Unlock(MuAchievements.ObjectionOverruled);
        }
    }

    public static void OnSpitefulCurseApplied(PlayerControl voter)
    {
        if (voter.AmOwner)
        {
            AchievementsIntegration.Unlock(MuAchievements.PyrrhicVote);
        }
    }

    public static void OnSpitefulEjected(PlayerControl exiled, int cursedVoterCount)
    {
        if (exiled.AmOwner && cursedVoterCount > 1)
        {
            AchievementsIntegration.Unlock(MuAchievements.MiseryLovesCompany);
        }
    }
}
