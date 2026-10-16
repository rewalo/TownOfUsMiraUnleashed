using System.Linq;
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
using TownOfUs.Achievements;
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
/// Central hub for Mira Unleashed achievement triggers. Awards go through the shared
/// <see cref="AchievementsAPIHelper"/>, which is a no-op when the Achievements API plugin
/// is not installed.
/// </summary>
public static class AchievementEvents
{
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
    private static readonly HashSet<AchievementSpec> _tentativeThisRound = new();

    [RegisterEvent]
    public static void RoundStartEventHandler(RoundStartEvent @event)
    {
        if (!AchievementsAPIHelper.IsLoaded)
        {
            return;
        }

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
            AchievementsAPIHelper.Unlock(MuAchievements.Unleashed);
        }

        if (local.HasModifier<SpitefulModifier>() && local.HasModifier<CluelessModifier>())
        {
            AchievementsAPIHelper.Unlock(MuAchievements.DoubleJeopardy);
        }
    }

    private static void UnlockTentative(AchievementSpec achievement)
    {
        if (!AchievementsAPIHelper.IsLoaded || AchievementsAPIHelper.IsUnlocked(achievement))
        {
            return;
        }

        AchievementsAPIHelper.Unlock(achievement);
        _tentativeThisRound.Add(achievement);
    }

    private static void ResetTentative()
    {
        if (!AchievementsAPIHelper.IsLoaded)
        {
            return;
        }

        foreach (var achievement in _tentativeThisRound)
        {
            AchievementsAPIHelper.Reset(achievement);
        }

        _tentativeThisRound.Clear();
    }

    [RegisterEvent]
    public static void StartMeetingEventHandler(StartMeetingEvent @event)
    {
        if (!AchievementsAPIHelper.IsLoaded)
        {
            return;
        }

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
        if (!AchievementsAPIHelper.IsLoaded)
        {
            return;
        }

        _snaredThisRound.Clear();
        _charlatanDeceivedThisMeeting = false;
    }

    [RegisterEvent]
    public static void EjectionEventHandler(EjectionEvent @event)
    {
        if (!AchievementsAPIHelper.IsLoaded)
        {
            return;
        }

        var exiled = @event.ExileController?.initData?.networkedPlayer?.Object;
        var local = PlayerControl.LocalPlayer;
        if (exiled == null || local == null)
        {
            return;
        }

        if (_snaredThisRound.Contains(exiled.PlayerId))
        {
            AchievementsAPIHelper.Unlock(MuAchievements.CaughtAndConvicted);
        }

        if (_charlatanDeceivedThisMeeting && exiled != local)
        {
            AchievementsAPIHelper.Unlock(MuAchievements.Framed);
        }

        if (!local.HasDied()
            && local.GetModifiers<WitchSpellboundModifier>().Any(m => m.WitchId == exiled.PlayerId)
            && local.GetVoteData().VotedFor(exiled.PlayerId))
        {
            AchievementsAPIHelper.Unlock(MuAchievements.BurnTheWitch);
        }

        if (local.HasModifier<CluelessModifier>()
            && local.GetVoteData().VotedFor(exiled.PlayerId)
            && (exiled.IsImpostor() || exiled.Is(RoleAlignment.NeutralKilling)))
        {
            AchievementsAPIHelper.Unlock(MuAchievements.BlindJustice);
        }
    }

    [RegisterEvent]
    public static void GameEndEventHandler(GameEndEvent @event)
    {
        if (!AchievementsAPIHelper.IsLoaded)
        {
            return;
        }

        var local = PlayerControl.LocalPlayer;
        if (local?.Data?.Role is not LawyerRole lawyerRole || !LocalPlayerWon(local))
        {
            return;
        }

        var client = lawyerRole.Client;
        if (client != null && (client.IsImpostor() || client.Is(RoleAlignment.NeutralKilling)))
        {
            AchievementsAPIHelper.Unlock(MuAchievements.DevilsAdvocate);
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
        if (!AchievementsAPIHelper.IsLoaded || @event.AbilityType != AbilityType.TimeLordRewind)
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
        if (!AchievementsAPIHelper.IsLoaded)
        {
            return;
        }

        if (CustomGameOver.Instance is HostGameOver)
        {
            ResetTentative();
        }
    }

    [RegisterEvent]
    public static void BeforeMurderEventHandler(BeforeMurderEvent @event)
    {
        if (!AchievementsAPIHelper.IsLoaded)
        {
            return;
        }

        var killer = @event.Source;
        var victim = @event.Target;
        if (killer == null || victim == null || !killer.AmOwner || !killer.IsRole<SerialKillerRole>())
        {
            return;
        }

        if (SerialKillerVentKillSystem.TryGetVentKillTarget(killer.PlayerId, out var ventTarget)
            && ventTarget != null && ventTarget.PlayerId == victim.PlayerId)
        {
            AchievementsAPIHelper.Unlock(MuAchievements.SnakeInThePipes);
        }
    }

    [RegisterEvent]
    public static void AfterMurderEventHandler(AfterMurderEvent @event)
    {
        if (!AchievementsAPIHelper.IsLoaded)
        {
            return;
        }

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
            AchievementsAPIHelper.Unlock(MuAchievements.LabRat);
        }

        if (!source.AmOwner || source == target)
        {
            return;
        }

        switch (source.Data?.Role)
        {
            case SerialKillerRole:
                AchievementsAPIHelper.Increment(MuAchievements.SerialSeason, 1);
                if (GameHistory.KilledPlayers.Count <= 1)
                {
                    AchievementsAPIHelper.Unlock(MuAchievements.ColdOpen);
                }

                _serialKillerKillsThisRound++;
                if (_serialKillerKillsThisRound >= 3)
                {
                    AchievementsAPIHelper.Unlock(MuAchievements.Massacre);
                }

                break;
            case WraithRole:
                AchievementsAPIHelper.Increment(MuAchievements.FadeToBlack, 1);
                if (source.HasModifier<WraithDashModifier>())
                {
                    AchievementsAPIHelper.Unlock(MuAchievements.HitTheGroundScreaming);
                }

                if (source.HasModifier<WraithLanternInvisibilityModifier>())
                {
                    AchievementsAPIHelper.Unlock(MuAchievements.LamplightLethality);
                }

                if (Time.time - _wraithLastLanternReturn <= 3f)
                {
                    AchievementsAPIHelper.Unlock(MuAchievements.LanternGuided);
                }

                break;
            case HackerRole when HackerSystem.IsJammed:
                AchievementsAPIHelper.Unlock(MuAchievements.LowPingHighCrime);
                break;
        }
    }

    [RegisterEvent]
    public static void CompleteTaskEvent(CompleteTaskEvent @event)
    {
        if (!AchievementsAPIHelper.IsLoaded)
        {
            return;
        }

        var player = @event.Player;
        if (player == null || !player.AmOwner)
        {
            return;
        }

        if (player.HasModifier<CluelessModifier>())
        {
            AchievementsAPIHelper.Unlock(MuAchievements.WingIt);
            AchievementsAPIHelper.Increment(MuAchievements.MuscleMemory, 1);

            if (HasCompletedAllTasks(player))
            {
                AchievementsAPIHelper.Unlock(MuAchievements.CluelessCompletionist);
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
        if (!AchievementsAPIHelper.IsLoaded || !mirage.AmOwner || interactor == null || interactor == mirage)
        {
            return;
        }

        AchievementsAPIHelper.Unlock(MuAchievements.Gottem);
        AchievementsAPIHelper.Increment(MuAchievements.CarnivalOfIllusions, 1);

        if (interactor.HasModifier<MorphlingMorphModifier>())
        {
            UnlockTentative(MuAchievements.MirrorMatch);
        }
    }

    public static void OnTrapperTrapTriggered(PlayerControl trapper, PlayerControl? victim)
    {
        if (!AchievementsAPIHelper.IsLoaded || !trapper.AmOwner || victim == null || victim == trapper)
        {
            return;
        }

        AchievementsAPIHelper.Unlock(MuAchievements.WelcomingCommittee);
        AchievementsAPIHelper.Increment(MuAchievements.PestControl, 1);
        AchievementsAPIHelper.Increment(MuAchievements.RedHanded, 1);

        _snaredThisRound.Add(victim.PlayerId);

        if (victim.GetModifiers<ConcealedModifier>().Any())
        {
            UnlockTentative(MuAchievements.CaughtInTheAct);
        }
    }

    public static void OnCharlatanConcealComplete(PlayerControl charlatan)
    {
        if (!AchievementsAPIHelper.IsLoaded || !charlatan.AmOwner)
        {
            return;
        }

        AchievementsAPIHelper.Unlock(MuAchievements.BodyDouble);
    }

    public static void OnCharlatanDeceive(PlayerControl charlatan)
    {
        if (!AchievementsAPIHelper.IsLoaded || !charlatan.AmOwner)
        {
            return;
        }

        AchievementsAPIHelper.Increment(MuAchievements.SmokeAndMirrors, 1);
        _charlatanDeceivedThisMeeting = true;
    }

    public static void OnHackerJamStarted(PlayerControl hacker)
    {
        if (!AchievementsAPIHelper.IsLoaded || !hacker.AmOwner)
        {
            return;
        }

        AchievementsAPIHelper.Increment(MuAchievements.DdosAttack, 1);

        if (ShipStatus.Instance != null
            && ShipStatus.Instance.Systems.TryGetValue(SystemTypes.Electrical, out var lightsSystem)
            && lightsSystem.TryCast<SwitchSystem>() is { IsActive: true })
        {
            AchievementsAPIHelper.Unlock(MuAchievements.Blackout);
        }

        if (ForestallerSystem.AnyActiveForestallerAlive())
        {
            AchievementsAPIHelper.Unlock(MuAchievements.BackdoorAccess);
        }
    }

    public static void OnInjectorInject(PlayerControl injector, PlayerControl target)
    {
        if (!AchievementsAPIHelper.IsLoaded || !injector.AmOwner || target == null)
        {
            return;
        }

        AchievementsAPIHelper.Unlock(MuAchievements.Malpractice);
        AchievementsAPIHelper.Increment(MuAchievements.ClinicalTrial, 1);

        _injectedThisRound.Add(target.PlayerId);
        if (target.HasModifier<WitchSpellboundModifier>())
        {
            AchievementsAPIHelper.Unlock(MuAchievements.BadMedicine);
        }

        InjectionsPerTarget[target.PlayerId] = InjectionsPerTarget.GetValueOrDefault(target.PlayerId) + 1;
        if (InjectionsPerTarget[target.PlayerId] == 2)
        {
            AchievementsAPIHelper.Unlock(MuAchievements.DoubleDose);
        }

        if (InjectionsPerTarget[target.PlayerId] == 3)
        {
            AchievementsAPIHelper.Unlock(MuAchievements.TripleThreat);
        }
    }

    public static void OnWitchSpell(PlayerControl witch, PlayerControl? target)
    {
        if (!AchievementsAPIHelper.IsLoaded || !witch.AmOwner)
        {
            return;
        }

        AchievementsAPIHelper.Unlock(MuAchievements.MarkedForDeath);
        AchievementsAPIHelper.Increment(MuAchievements.PracticedHand, 1);

        if (target != null && target.HasModifier<SpellslingerHexedModifier>())
        {
            UnlockTentative(MuAchievements.DoubleHex);
        }

        var currentCurses = PlayerControl.AllPlayerControls.ToArray()
            .Count(p => p != null && p.HasModifier<WitchSpellboundModifier>());
        if (currentCurses >= 5)
        {
            AchievementsAPIHelper.Unlock(MuAchievements.FullCoven);
        }
    }

    public static void OnWitchHexKill(PlayerControl witch)
    {
        if (!AchievementsAPIHelper.IsLoaded || !witch.AmOwner || ++_witchHexKillsThisMeeting < 2)
        {
            return;
        }

        AchievementsAPIHelper.Unlock(MuAchievements.TwinSacrifice);
    }

    public static void OnForestallerLockActivated(PlayerControl forestaller)
    {
        if (!AchievementsAPIHelper.IsLoaded || !forestaller.AmOwner)
        {
            return;
        }

        AchievementsAPIHelper.Unlock(MuAchievements.NothingLeftToSabotage);
        if (_meetingsSeen == 0)
        {
            AchievementsAPIHelper.Unlock(MuAchievements.SpeedrunLockdown);
        }

        if (HexBombSabotageSystem.Instance is { Stage: HexBombStage.Countdown })
        {
            AchievementsAPIHelper.Unlock(MuAchievements.UnderPressure);
        }

        if (ShipStatus.Instance != null)
        {
            foreach (var system in ShipStatus.Instance.Systems.Values)
            {
                if (system.TryCast<ICriticalSabotage>() is { IsActive: true })
                {
                    AchievementsAPIHelper.Unlock(MuAchievements.EleventhHour);
                    break;
                }
            }
        }
    }

    public static void OnScavengerEat(PlayerControl scavenger, byte bodyId)
    {
        if (!AchievementsAPIHelper.IsLoaded || !scavenger.AmOwner)
        {
            return;
        }

        AchievementsAPIHelper.Increment(MuAchievements.CleanPlateClub, 1);
        AchievementsAPIHelper.Increment(MuAchievements.BuffetRegular, 1);

        if (_killTimes.TryGetValue(bodyId, out var killTime) && Time.time - killTime <= 10f)
        {
            AchievementsAPIHelper.Unlock(MuAchievements.FastFood);
        }

        _scavengerEatsThisRound++;
        if (_scavengerEatsThisRound >= 3)
        {
            AchievementsAPIHelper.Unlock(MuAchievements.BottomlessPit);
        }
    }

    public static void OnLawyerClientAssigned(PlayerControl lawyer)
    {
        if (!AchievementsAPIHelper.IsLoaded || !lawyer.AmOwner)
        {
            return;
        }

        AchievementsAPIHelper.Unlock(MuAchievements.CourtAppointed);
    }

    public static void OnLawyerObjectVotes(PlayerControl lawyer, bool clientHadVotes)
    {
        if (!AchievementsAPIHelper.IsLoaded || !lawyer.AmOwner || !clientHadVotes)
        {
            return;
        }

        AchievementsAPIHelper.Unlock(MuAchievements.ObjectionSustained);
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
        if (!AchievementsAPIHelper.IsLoaded || !voter.AmOwner)
        {
            return;
        }

        AchievementsAPIHelper.Unlock(MuAchievements.PyrrhicVote);
    }

    public static void OnSpitefulEjected(PlayerControl exiled, int cursedVoterCount, int totalVoters)
    {
        if (!AchievementsAPIHelper.IsLoaded || !exiled.AmOwner)
        {
            return;
        }

        if (cursedVoterCount >= 2)
        {
            AchievementsAPIHelper.Unlock(MuAchievements.MiseryLovesCompany);
        }

        // The exiled player does not curse themselves, so a "full house" means everyone
        // else who voted for them was cursed.
        var nonSelfVoters = Mathf.Max(0, totalVoters - 1);
        if (cursedVoterCount > 0 && cursedVoterCount >= nonSelfVoters)
        {
            AchievementsAPIHelper.Unlock(MuAchievements.PartingGift);
        }
    }
}
