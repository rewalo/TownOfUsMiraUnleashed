using AchievementsAPI.API;
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
/// Central hub for Mira Unleashed achievement triggers. Awards go through the
/// Achievements API's tab singletons, which are populated when the API scans
/// this assembly's <see cref="AchievementsTab"/> subclasses at load.
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
            AchievementsTabSingleton<MuGeneralAchievementsTab>.Instance.Unleashed.Unlock();
        }

        if (local.HasModifier<SpitefulModifier>() && local.HasModifier<CluelessModifier>())
        {
            AchievementsTabSingleton<MuGeneralAchievementsTab>.Instance.DoubleJeopardy.Unlock();
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
                AchievementsTabSingleton<MuNeutralAchievementsTab>.Instance.CaseClosed.Unlock();
                break;
            case ScavengerRole:
                AchievementsTabSingleton<MuNeutralAchievementsTab>.Instance.FullBelly.Unlock();
                break;
            case SerialKillerRole:
                AchievementsTabSingleton<MuNeutralAchievementsTab>.Instance.TheLastScene.Unlock();
                break;
        }

        if (local.HasModifier<CluelessModifier>())
        {
            AchievementsTabSingleton<MuModifierAchievementsTab>.Instance.PureInstinct.Unlock();
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
                AchievementsTabSingleton<MuNeutralAchievementsTab>.Instance.SerialSeason.Increment(1);
                if (GameHistory.KilledPlayers.Count <= 1)
                {
                    AchievementsTabSingleton<MuNeutralAchievementsTab>.Instance.ColdOpen.Unlock();
                }

                break;
            case WraithRole:
                AchievementsTabSingleton<MuImpostorAchievementsTab>.Instance.FadeToBlack.Increment(1);
                if (source.HasModifier<WraithDashModifier>())
                {
                    AchievementsTabSingleton<MuImpostorAchievementsTab>.Instance.HitTheGroundScreaming.Unlock();
                }

                if (source.HasModifier<WraithLanternInvisibilityModifier>())
                {
                    AchievementsTabSingleton<MuImpostorAchievementsTab>.Instance.LamplightLethality.Unlock();
                }

                break;
            case HackerRole when HackerSystem.IsJammed:
                AchievementsTabSingleton<MuImpostorAchievementsTab>.Instance.LowPingHighCrime.Unlock();
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

        AchievementsTabSingleton<MuModifierAchievementsTab>.Instance.WingIt.Unlock();
        AchievementsTabSingleton<MuModifierAchievementsTab>.Instance.MuscleMemory.Increment(1);
    }

    // --- Role/system hook points (called directly from role RPCs and systems) ---

    public static void OnMirageDecoyTriggered(PlayerControl mirage, PlayerControl? interactor)
    {
        if (!mirage.AmOwner || interactor == null || interactor == mirage)
        {
            return;
        }

        AchievementsTabSingleton<MuCrewmateAchievementsTab>.Instance.Gottem.Unlock();
        AchievementsTabSingleton<MuCrewmateAchievementsTab>.Instance.CarnivalOfIllusions.Increment(1);
    }

    public static void OnTrapperTrapTriggered(PlayerControl trapper, PlayerControl? victim)
    {
        if (!trapper.AmOwner || victim == null || victim == trapper)
        {
            return;
        }

        AchievementsTabSingleton<MuCrewmateAchievementsTab>.Instance.WelcomingCommittee.Unlock();
        AchievementsTabSingleton<MuCrewmateAchievementsTab>.Instance.PestControl.Increment(1);
        AchievementsTabSingleton<MuCrewmateAchievementsTab>.Instance.RedHanded.Increment(1);
    }

    public static void OnCharlatanConcealComplete(PlayerControl charlatan)
    {
        if (charlatan.AmOwner)
        {
            AchievementsTabSingleton<MuImpostorAchievementsTab>.Instance.BodyDouble.Unlock();
        }
    }

    public static void OnCharlatanDeceive(PlayerControl charlatan)
    {
        if (charlatan.AmOwner)
        {
            AchievementsTabSingleton<MuImpostorAchievementsTab>.Instance.SmokeAndMirrors.Increment(1);
        }
    }

    public static void OnHackerJamStarted(PlayerControl hacker)
    {
        if (hacker.AmOwner)
        {
            AchievementsTabSingleton<MuImpostorAchievementsTab>.Instance.DdosAttack.Increment(1);
        }
    }

    public static void OnInjectorInject(PlayerControl injector, PlayerControl target)
    {
        if (!injector.AmOwner || target == null)
        {
            return;
        }

        AchievementsTabSingleton<MuImpostorAchievementsTab>.Instance.Malpractice.Unlock();
        AchievementsTabSingleton<MuImpostorAchievementsTab>.Instance.ClinicalTrial.Increment(1);

        InjectionsPerTarget[target.PlayerId] = InjectionsPerTarget.GetValueOrDefault(target.PlayerId) + 1;
        if (InjectionsPerTarget[target.PlayerId] == 2)
        {
            AchievementsTabSingleton<MuImpostorAchievementsTab>.Instance.DoubleDose.Unlock();
        }
    }

    public static void OnWitchSpell(PlayerControl witch)
    {
        if (!witch.AmOwner)
        {
            return;
        }

        AchievementsTabSingleton<MuImpostorAchievementsTab>.Instance.HexMarksTheSpot.Unlock();
        AchievementsTabSingleton<MuImpostorAchievementsTab>.Instance.BoilAndTrouble.Increment(1);
    }

    public static void OnWitchHexKill(PlayerControl witch)
    {
        if (witch.AmOwner && ++_witchHexKillsThisMeeting >= 2)
        {
            AchievementsTabSingleton<MuImpostorAchievementsTab>.Instance.BubbleBubble.Unlock();
        }
    }

    public static void OnForestallerLockActivated(PlayerControl forestaller)
    {
        if (!forestaller.AmOwner)
        {
            return;
        }

        AchievementsTabSingleton<MuCrewmateAchievementsTab>.Instance.NothingLeftToSabotage.Unlock();
        if (_meetingsSeen == 0)
        {
            AchievementsTabSingleton<MuCrewmateAchievementsTab>.Instance.SpeedrunLockdown.Unlock();
        }
    }

    public static void OnScavengerEat(PlayerControl scavenger)
    {
        if (!scavenger.AmOwner)
        {
            return;
        }

        AchievementsTabSingleton<MuNeutralAchievementsTab>.Instance.CleanPlateClub.Increment(1);
        AchievementsTabSingleton<MuNeutralAchievementsTab>.Instance.BuffetRegular.Increment(1);
    }

    public static void OnLawyerClientAssigned(PlayerControl lawyer)
    {
        if (lawyer.AmOwner)
        {
            AchievementsTabSingleton<MuNeutralAchievementsTab>.Instance.CourtAppointed.Unlock();
        }
    }

    public static void OnLawyerObjectVotes(PlayerControl lawyer)
    {
        if (lawyer.AmOwner && ++_lawyerObjectionsThisMeeting >= 2)
        {
            AchievementsTabSingleton<MuNeutralAchievementsTab>.Instance.ObjectionOverruled.Unlock();
        }
    }

    public static void OnSpitefulCurseApplied(PlayerControl voter)
    {
        if (voter.AmOwner)
        {
            AchievementsTabSingleton<MuModifierAchievementsTab>.Instance.PyrrhicVote.Unlock();
        }
    }

    public static void OnSpitefulEjected(PlayerControl exiled, int cursedVoterCount)
    {
        if (exiled.AmOwner && cursedVoterCount > 1)
        {
            AchievementsTabSingleton<MuModifierAchievementsTab>.Instance.MiseryLovesCompany.Unlock();
        }
    }
}
