using System.Linq;
using AchievementsAPI.API;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Events.Vanilla.Meeting;
using MiraAPI.Events.Vanilla.Player;
using MiraAPI.GameEnd;
using MiraAPI.Modifiers;
using MiraAPI.Utilities;
using MiraUnleashed.Achievements;
using MiraUnleashed.Modifiers;
using MiraUnleashed.Modules;
using MiraUnleashed.Roles;
using MiraUnleashed.Roles.Impostor;
using MiraUnleashed.Roles.Neutral;
using TownOfUs.Events.TouEvents;
using TownOfUs.GameOver;
using TownOfUs.Modifiers;
using TownOfUs.Modifiers.Impostor;
using TownOfUs.Modules;
using TownOfUs.Modules.Components;
using TownOfUs.Roles;
using TownOfUs.Utilities;
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
    private static readonly HashSet<byte> _snaredThisRound = new();
    private static readonly HashSet<byte> _injectedThisRound = new();
    private static readonly Dictionary<byte, float> _killTimes = new();
    private static bool _charlatanDeceivedThisMeeting;

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
        _snaredThisRound.Clear();
        _injectedThisRound.Clear();
        _killTimes.Clear();
        _charlatanDeceivedThisMeeting = false;

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
        _injectedThisRound.Clear();
    }

    [RegisterEvent]
    public static void EndMeetingEventHandler(EndMeetingEvent @event)
    {
        _snaredThisRound.Clear();
        _charlatanDeceivedThisMeeting = false;
    }

    [RegisterEvent]
    public static void EjectionEventHandler(EjectionEvent @event)
    {
        var exiled = @event.ExileController?.initData?.networkedPlayer?.Object;
        var local = PlayerControl.LocalPlayer;
        if (exiled == null || local == null)
        {
            return;
        }

        if (_snaredThisRound.Contains(exiled.PlayerId))
        {
            Tab.CaughtAndConvicted.Unlock();
        }

        if (_charlatanDeceivedThisMeeting && exiled != local)
        {
            Tab.Framed.Unlock();
        }

        if (!local.HasDied()
            && local.GetModifiers<WitchSpellboundModifier>().Any(m => m.WitchId == exiled.PlayerId)
            && local.GetVoteData().VotedFor(exiled.PlayerId))
        {
            Tab.BurnTheWitch.Unlock();
        }

        if (local.HasModifier<CluelessModifier>()
            && local.GetVoteData().VotedFor(exiled.PlayerId)
            && (exiled.IsImpostor() || exiled.Is(RoleAlignment.NeutralKilling)))
        {
            Tab.BlindJustice.Unlock();
        }
    }

    [RegisterEvent]
    public static void GameEndEventHandler(GameEndEvent @event)
    {
        var local = PlayerControl.LocalPlayer;
        if (local?.Data?.Role is not LawyerRole lawyerRole || !LocalPlayerWon(local))
        {
            return;
        }

        var client = lawyerRole.Client;
        if (client != null && (client.IsImpostor() || client.Is(RoleAlignment.NeutralKilling)))
        {
            Tab.DevilsAdvocate.Unlock();
        }
    }

    private static bool LocalPlayerWon(PlayerControl local)
    {
        return EndGameResult.CachedWinners.ToArray()
            .Any(w => w.PlayerName == local.Data?.PlayerName);
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
    public static void BeforeMurderEventHandler(BeforeMurderEvent @event)
    {
        var killer = @event.Source;
        var victim = @event.Target;
        if (killer == null || victim == null || !killer.AmOwner || !killer.IsRole<SerialKillerRole>())
        {
            return;
        }

        if (SerialKillerVentKillSystem.TryGetVentKillTarget(killer.PlayerId, out var ventTarget)
            && ventTarget != null && ventTarget.PlayerId == victim.PlayerId)
        {
            Tab.SnakeInThePipes.Unlock();
        }
    }

    [RegisterEvent]
    public static void AfterMurderEventHandler(AfterMurderEvent @event)
    {
        var source = @event.Source;
        var target = @event.Target;
        if (source == null || target == null)
        {
            return;
        }

        _killTimes[target.PlayerId] = Time.time;

        var local = PlayerControl.LocalPlayer;
        if (_injectedThisRound.Contains(target.PlayerId)
            && source != local
            && source.IsImpostor()
            && local?.Data?.Role is InjectorRole)
        {
            Tab.LabRat.Unlock();
        }

        if (!source.AmOwner || source == target)
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

        _snaredThisRound.Add(victim.PlayerId);

        if (victim.GetModifiers<ConcealedModifier>().Any())
        {
            UnlockTentative(Tab.CaughtInTheAct);
        }
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
        if (!charlatan.AmOwner)
        {
            return;
        }

        Tab.SmokeAndMirrors.Increment(1);
        _charlatanDeceivedThisMeeting = true;
    }

    public static void OnHackerJamStarted(PlayerControl hacker)
    {
        if (!hacker.AmOwner)
        {
            return;
        }

        Tab.DdosAttack.Increment(1);

        if (ShipStatus.Instance != null
            && ShipStatus.Instance.Systems.TryGetValue(SystemTypes.Electrical, out var lightsSystem)
            && lightsSystem.TryCast<SwitchSystem>() is { IsActive: true })
        {
            Tab.Blackout.Unlock();
        }

        if (ForestallerSystem.AnyActiveForestallerAlive())
        {
            Tab.BackdoorAccess.Unlock();
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

        _injectedThisRound.Add(target.PlayerId);
        if (target.HasModifier<WitchSpellboundModifier>())
        {
            Tab.BadMedicine.Unlock();
        }

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

    public static void OnWitchSpell(PlayerControl witch, PlayerControl? target)
    {
        if (!witch.AmOwner)
        {
            return;
        }

        Tab.MarkedForDeath.Unlock();
        Tab.PracticedHand.Increment(1);

        if (target != null && target.HasModifier<SpellslingerHexedModifier>())
        {
            UnlockTentative(Tab.DoubleHex);
        }

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

        if (ShipStatus.Instance != null)
        {
            foreach (var system in ShipStatus.Instance.Systems.Values)
            {
                if (system.TryCast<ICriticalSabotage>() is { IsActive: true })
                {
                    Tab.EleventhHour.Unlock();
                    break;
                }
            }
        }
    }

    public static void OnScavengerEat(PlayerControl scavenger, byte bodyId)
    {
        if (!scavenger.AmOwner)
        {
            return;
        }

        Tab.CleanPlateClub.Increment(1);
        Tab.BuffetRegular.Increment(1);

        if (_killTimes.TryGetValue(bodyId, out var killTime) && Time.time - killTime <= 10f)
        {
            Tab.FastFood.Unlock();
        }

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
