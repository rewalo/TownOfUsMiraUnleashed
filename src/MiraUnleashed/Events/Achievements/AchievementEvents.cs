using System.Linq;
using AchievementsAPI.API;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Events.Vanilla.Meeting;
using MiraAPI.Events.Vanilla.Player;
using MiraAPI.GameEnd;
using MiraAPI.Modifiers;
using MiraUnleashed.Achievements;
using MiraUnleashed.Modifiers;
using MiraUnleashed.Modules;
using MiraUnleashed.Roles;
using MiraUnleashed.Roles.Impostor;
using MiraUnleashed.Roles.Neutral;
using TownOfUs.Events.TouEvents;
using TownOfUs.GameOver;
using TownOfUs.Modifiers.Impostor;
using TownOfUs.Modules;
using TownOfUs.Modules.Components;
using UnityEngine;

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
    private static int _scavengerEatsThisRound;
    private static int _serialKillerKillsThisRound;
    private static float _wraithLastLanternReturn;

    // Hidden-information achievements unlock immediately (with a deferred toast) but are
    // tracked here so a Time Lord rewind or host abort can reset them before the toast shows.
    private static readonly HashSet<BaseAchievement> _tentativeThisRound = new();

    [RegisterEvent]
    public static void RoundStartEventHandler(RoundStartEvent @event)
    {
        _tentativeThisRound.Clear();

        if (!@event.TriggeredByIntro)
        {
            return;
        }

        InjectionsPerTarget.Clear();
        _meetingsSeen = 0;
        _witchHexKillsThisMeeting = 0;
        _scavengerEatsThisRound = 0;
        _serialKillerKillsThisRound = 0;
        _wraithLastLanternReturn = -1000f;

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

    private static void UnlockTentative(BaseAchievement achievement)
    {
        if (achievement.Unlocked)
        {
            return;
        }

        achievement.Unlock();
        _tentativeThisRound.Add(achievement);
    }

    private static void ResetTentative()
    {
        foreach (var achievement in _tentativeThisRound)
        {
            achievement.Reset();
        }

        _tentativeThisRound.Clear();
    }

    [RegisterEvent]
    public static void StartMeetingEventHandler(StartMeetingEvent @event)
    {
        _tentativeThisRound.Clear();
        _meetingsSeen++;
        _witchHexKillsThisMeeting = 0;
        _scavengerEatsThisRound = 0;
        _serialKillerKillsThisRound = 0;
    }

    /// <summary>
    /// Resets achievements that would be mooted by a Time Lord rewind.
    /// </summary>
    [RegisterEvent]
    public static void TouAbilityEventHandler(TouAbilityEvent @event)
    {
        if (@event.AbilityType != AbilityType.TimeLordRewind)
        {
            return;
        }

        // Rewinding can undo the deaths/tasks/votes that earned these, so drop them.
        ResetTentative();

        // Round-local kill/eat counts are also invalidated by a rewind.
        _scavengerEatsThisRound = 0;
        _serialKillerKillsThisRound = 0;
        _witchHexKillsThisMeeting = 0;
    }

    /// <summary>
    /// A host abort means the game never really concluded, so tentative unlocks are rolled back.
    /// </summary>
    [RegisterEvent]
    public static void BeforeGameEndEventHandler(BeforeGameEndEvent @event)
    {
        if (CustomGameOver.Instance is HostGameOver)
        {
            ResetTentative();
        }
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

                _serialKillerKillsThisRound++;
                if (_serialKillerKillsThisRound >= 3)
                {
                    Tab.Massacre.Unlock();
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

                if (Time.time - _wraithLastLanternReturn <= 3f)
                {
                    Tab.LanternGuided.Unlock();
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
        if (player == null || !player.AmOwner)
        {
            return;
        }

        if (player.HasModifier<CluelessModifier>())
        {
            Tab.WingIt.Unlock();
            Tab.MuscleMemory.Increment(1);

            if (HasCompletedAllTasks(player))
            {
                Tab.CluelessCompletionist.Unlock();
            }
        }
    }

    private static bool HasCompletedAllTasks(PlayerControl player)
    {
        if (player?.Data?.Tasks == null)
        {
            return false;
        }

        var tasks = player.Data.Tasks.ToArray();
        return tasks.Length > 0 && tasks.All(t => t.Complete);
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

        if (interactor.HasModifier<MorphlingMorphModifier>())
        {
            UnlockTentative(Tab.MirrorMatch);
        }
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

        if (InjectionsPerTarget[target.PlayerId] == 3)
        {
            Tab.TripleThreat.Unlock();
        }
    }

    public static void OnWitchSpell(PlayerControl witch)
    {
        if (!witch.AmOwner)
        {
            return;
        }

        Tab.MarkedForDeath.Unlock();
        Tab.PracticedHand.Increment(1);

        var currentCurses = PlayerControl.AllPlayerControls.ToArray()
            .Count(p => p != null && p.HasModifier<WitchSpellboundModifier>());
        if (currentCurses >= 5)
        {
            Tab.FullCoven.Unlock();
        }
    }

    public static void OnWitchHexKill(PlayerControl witch)
    {
        if (witch.AmOwner && ++_witchHexKillsThisMeeting >= 2)
        {
            Tab.TwinSacrifice.Unlock();
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

        if (HexBombSabotageSystem.Instance is { Stage: HexBombStage.Countdown })
        {
            Tab.UnderPressure.Unlock();
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

        _scavengerEatsThisRound++;
        if (_scavengerEatsThisRound >= 3)
        {
            Tab.BottomlessPit.Unlock();
        }
    }

    public static void OnLawyerClientAssigned(PlayerControl lawyer)
    {
        if (lawyer.AmOwner)
        {
            Tab.CourtAppointed.Unlock();
        }
    }

    public static void OnLawyerObjectVotes(PlayerControl lawyer, bool clientHadVotes)
    {
        if (lawyer.AmOwner && clientHadVotes)
        {
            Tab.ObjectionSustained.Unlock();
        }
    }

    public static void OnWraithLanternReturned(PlayerControl wraith)
    {
        if (wraith.AmOwner)
        {
            _wraithLastLanternReturn = Time.time;
        }
    }

    public static void OnSpitefulCurseApplied(PlayerControl voter)
    {
        if (voter.AmOwner)
        {
            Tab.PyrrhicVote.Unlock();
        }
    }

    public static void OnSpitefulEjected(PlayerControl exiled, int cursedVoterCount, int totalVoters)
    {
        if (!exiled.AmOwner)
        {
            return;
        }

        if (cursedVoterCount >= 2)
        {
            Tab.MiseryLovesCompany.Unlock();
        }

        // The exiled player does not curse themselves, so a "full house" means everyone
        // else who voted for them was cursed.
        var nonSelfVoters = Mathf.Max(0, totalVoters - 1);
        if (cursedVoterCount > 0 && cursedVoterCount >= nonSelfVoters)
        {
            Tab.PartingGift.Unlock();
        }
    }
}
