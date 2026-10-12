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
    private static MiraUnleashedAchievementsTab Tab => AchievementsTabSingleton<MiraUnleashedAchievementsTab>.Instance;
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
            Tab.Unleashed.Unlock();
        }

        if (local.HasModifier<SpitefulModifier>() && local.HasModifier<CluelessModifier>())
        {
            Tab.DoubleJeopardy.Unlock();
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
                Tab.CaseClosed.Unlock();
                break;
            case ScavengerRole:
                Tab.FullBelly.Unlock();
                break;
            case SerialKillerRole:
                Tab.TheLastScene.Unlock();
                break;
        }

        if (local.HasModifier<CluelessModifier>())
        {
            Tab.PureInstinct.Unlock();
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
                Tab.SerialSeason.Increment(1);
                if (GameHistory.KilledPlayers.Count <= 1)
                {
                    Tab.ColdOpen.Unlock();
                }

                break;
            case WraithRole:
                Tab.FadeToBlack.Increment(1);
                if (source.HasModifier<WraithDashModifier>())
                {
                    Tab.HitTheGroundScreaming.Unlock();
                }

                if (source.HasModifier<WraithLanternInvisibilityModifier>())
                {
                    Tab.LamplightLethality.Unlock();
                }

                break;
            case HackerRole when HackerSystem.IsJammed:
                Tab.LowPingHighCrime.Unlock();
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

        Tab.WingIt.Unlock();
        Tab.MuscleMemory.Increment(1);
    }

    // --- Role/system hook points (called directly from role RPCs and systems) ---

    public static void OnMirageDecoyTriggered(PlayerControl mirage, PlayerControl? interactor)
    {
        if (!mirage.AmOwner || interactor == null || interactor == mirage)
        {
            return;
        }

        Tab.Gottem.Unlock();
        Tab.CarnivalOfIllusions.Increment(1);
    }

    public static void OnTrapperTrapTriggered(PlayerControl trapper, PlayerControl? victim)
    {
        if (!trapper.AmOwner || victim == null || victim == trapper)
        {
            return;
        }

        Tab.WelcomingCommittee.Unlock();
        Tab.PestControl.Increment(1);
        Tab.RedHanded.Increment(1);
    }

    public static void OnCharlatanConcealComplete(PlayerControl charlatan)
    {
        if (charlatan.AmOwner)
        {
            Tab.BodyDouble.Unlock();
        }
    }

    public static void OnCharlatanDeceive(PlayerControl charlatan)
    {
        if (charlatan.AmOwner)
        {
            Tab.SmokeAndMirrors.Increment(1);
        }
    }

    public static void OnHackerJamStarted(PlayerControl hacker)
    {
        if (hacker.AmOwner)
        {
            Tab.DdosAttack.Increment(1);
        }
    }

    public static void OnInjectorInject(PlayerControl injector, PlayerControl target)
    {
        if (!injector.AmOwner || target == null)
        {
            return;
        }

        Tab.Malpractice.Unlock();
        Tab.ClinicalTrial.Increment(1);

        InjectionsPerTarget[target.PlayerId] = InjectionsPerTarget.GetValueOrDefault(target.PlayerId) + 1;
        if (InjectionsPerTarget[target.PlayerId] == 2)
        {
            Tab.DoubleDose.Unlock();
        }
    }

    public static void OnWitchSpell(PlayerControl witch)
    {
        if (!witch.AmOwner)
        {
            return;
        }

        Tab.HexMarksTheSpot.Unlock();
        Tab.BoilAndTrouble.Increment(1);
    }

    public static void OnWitchHexKill(PlayerControl witch)
    {
        if (witch.AmOwner && ++_witchHexKillsThisMeeting >= 2)
        {
            Tab.BubbleBubble.Unlock();
        }
    }

    public static void OnForestallerLockActivated(PlayerControl forestaller)
    {
        if (!forestaller.AmOwner)
        {
            return;
        }

        Tab.NothingLeftToSabotage.Unlock();
        if (_meetingsSeen == 0)
        {
            Tab.SpeedrunLockdown.Unlock();
        }
    }

    public static void OnScavengerEat(PlayerControl scavenger)
    {
        if (!scavenger.AmOwner)
        {
            return;
        }

        Tab.CleanPlateClub.Increment(1);
        Tab.BuffetRegular.Increment(1);
    }

    public static void OnLawyerClientAssigned(PlayerControl lawyer)
    {
        if (lawyer.AmOwner)
        {
            Tab.CourtAppointed.Unlock();
        }
    }

    public static void OnLawyerObjectVotes(PlayerControl lawyer)
    {
        if (lawyer.AmOwner && ++_lawyerObjectionsThisMeeting >= 2)
        {
            Tab.ObjectionOverruled.Unlock();
        }
    }

    public static void OnSpitefulCurseApplied(PlayerControl voter)
    {
        if (voter.AmOwner)
        {
            Tab.PyrrhicVote.Unlock();
        }
    }

    public static void OnSpitefulEjected(PlayerControl exiled, int cursedVoterCount)
    {
        if (exiled.AmOwner && cursedVoterCount > 1)
        {
            Tab.MiseryLovesCompany.Unlock();
        }
    }
}
