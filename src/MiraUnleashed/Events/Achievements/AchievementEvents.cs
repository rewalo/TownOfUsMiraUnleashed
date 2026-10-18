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
using MiraUnleashed.Options.Roles.Impostor;
using MiraUnleashed.Roles;
using MiraUnleashed.Roles.Impostor;
using MiraUnleashed.Roles.Neutral;
using TownOfUs;
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
using Object = UnityEngine.Object;

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
    private static readonly Dictionary<byte, float> _killTimes = new();
    private static bool _charlatanDeceivedThisMeeting;

    private static bool _forestallerLockArmed;
    private static bool _localLockActive;
    private static float _forestallerDeathTime;
    private static bool _sabotageWasActive;
    private static readonly Dictionary<byte, int> _snaresPerVictim = new();
    private static readonly Dictionary<byte, float> _lastKillTimeByKiller = new();
    private static readonly HashSet<byte> _concealedBodies = new();
    private static bool _lawyerObjectedThisMeeting;
    private static float _wraithLastKillTime;
    private static byte? _wraithLastVictimId;
    private static bool _wraithReturnedAfterKill;
    private static int _wraithLanternReturns;
    private static bool _wraithLanternBroke;
    private static WraithDashModifier? _currentDash;
    private static int _killsThisDash;

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
            // A round start after an ejection means gameplay resumed; restart the Floodgates
            // window so the exile cutscene doesn't burn it.
            if (_forestallerDeathTime > 0f)
            {
                _forestallerDeathTime = Time.time;
            }

            return;
        }

        InjectionsPerTarget.Clear();
        _meetingsSeen = 0;
        _witchHexKillsThisMeeting = 0;
        _scavengerEatsThisRound = 0;
        _serialKillerKillsThisRound = 0;
        _killTimes.Clear();
        _charlatanDeceivedThisMeeting = false;
        _forestallerLockArmed = false;
        _localLockActive = false;
        _forestallerDeathTime = 0f;
        _sabotageWasActive = false;
        _snaresPerVictim.Clear();
        _lastKillTimeByKiller.Clear();
        _concealedBodies.Clear();
        _lawyerObjectedThisMeeting = false;
        _wraithLastKillTime = -1000f;
        _wraithLastVictimId = null;
        _wraithReturnedAfterKill = false;
        _wraithLanternReturns = 0;
        _wraithLanternBroke = false;
        _currentDash = null;
        _killsThisDash = 0;

        var local = PlayerControl.LocalPlayer;
        if (local == null)
        {
            return;
        }

        if (local.Data?.Role is IMiraUnleashedRole miraRole)
        {
            AchievementsAPIHelper.Unlock(MuAchievements.Unleashed);

            var playedCount = MuAchievementStorage.AddPlayedRole(miraRole.GetType().FullName!);
            if (playedCount == MuAchievements.JackOfAllTrades.ProgressGoal
                && !AchievementsAPIHelper.IsUnlocked(MuAchievements.JackOfAllTrades))
            {
                AchievementsAPIHelper.UnlockWithToast(MuAchievements.JackOfAllTrades, AchToastTiming.OnGameEnd);
            }

            // Store progress silently: SetValue's UI path would toast progress for a hidden achievement.
            AchievementsAPIHelper.SetValue(MuAchievements.JackOfAllTrades, playedCount, showOnUI: false);
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
    }

    [RegisterEvent]
    public static void EndMeetingEventHandler(EndMeetingEvent @event)
    {
        if (!AchievementsAPIHelper.IsLoaded)
        {
            return;
        }

        _charlatanDeceivedThisMeeting = false;
        _concealedBodies.Clear();
        _lawyerObjectedThisMeeting = false;
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

        if (exiled == local && _localLockActive)
        {
            _forestallerDeathTime = Time.time;
            _localLockActive = false;
        }

        if (_charlatanDeceivedThisMeeting && exiled != local)
        {
            AchievementsAPIHelper.Unlock(MuAchievements.Framed);
        }

        if (_lawyerObjectedThisMeeting && local.Data?.Role is LawyerRole { Client: not null } lawyerRole)
        {
            if (exiled == lawyerRole.Client)
            {
                AchievementsAPIHelper.Unlock(MuAchievements.Overruled);
            }
            else if (exiled != local)
            {
                AchievementsAPIHelper.Unlock(MuAchievements.Mistrial);
            }
        }

        if (!local.HasDied()
            && local.GetModifiers<WitchSpellboundModifier>().Any(m => m.WitchId == exiled.PlayerId)
            && local.GetVoteData().VotedFor(exiled.PlayerId))
        {
            AchievementsAPIHelper.Unlock(MuAchievements.BurnTheWitch);
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
        _lastKillTimeByKiller[source.PlayerId] = Time.time;

        // The first death after a Forestaller lock decides Targeted.
        if (_forestallerLockArmed)
        {
            if (target.AmOwner)
            {
                AchievementsAPIHelper.Unlock(MuAchievements.Targeted);
            }

            _forestallerLockArmed = false;
        }

        // Dying with the lock active arms the Floodgates sabotage window.
        if (target.AmOwner && _localLockActive)
        {
            _forestallerDeathTime = Time.time;
            _localLockActive = false;
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
                _wraithLastKillTime = Time.time;
                _wraithLastVictimId = target.PlayerId;
                _wraithReturnedAfterKill = false;

                if (source.TryGetModifier<WraithDashModifier>(out var dash))
                {
                    if (!ReferenceEquals(dash, _currentDash))
                    {
                        _currentDash = dash;
                        _killsThisDash = 0;
                    }

                    if (++_killsThisDash >= 2)
                    {
                        AchievementsAPIHelper.Unlock(MuAchievements.SpeedDemon);
                    }
                }

                break;
            case HackerRole when HackerSystem.IsJammed:
                AchievementsAPIHelper.Unlock(MuAchievements.DeadAir);
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

    /// <summary>
    /// Called every frame from <c>HudManager.Update</c>. Watches for a sabotage starting inside
    /// the Floodgates window after the local Forestaller died with the lock active.
    /// </summary>
    public static void OnHudUpdate()
    {
        if (!AchievementsAPIHelper.IsLoaded)
        {
            return;
        }

        var sabotageActive = ShipStatus.Instance != null
            && ShipStatus.Instance.Systems.TryGetValue(SystemTypes.Sabotage, out var sabotageSystem)
            && sabotageSystem.TryCast<SabotageSystemType>() is { AnyActive: true };

        if (_forestallerDeathTime > 0f)
        {
            if (Time.time - _forestallerDeathTime > 15f)
            {
                _forestallerDeathTime = 0f;
            }
            else if (sabotageActive && !_sabotageWasActive)
            {
                AchievementsAPIHelper.Unlock(MuAchievements.Floodgates);
                _forestallerDeathTime = 0f;
            }
        }

        _sabotageWasActive = sabotageActive;
    }

    /// <summary>
    /// Called from a <c>PlayerControl.StartMeeting</c> prefix so it fires on every client for
    /// both button meetings (target is null) and body reports.
    /// </summary>
    public static void OnMeetingCalled(PlayerControl caller, NetworkedPlayerInfo? target)
    {
        if (!AchievementsAPIHelper.IsLoaded)
        {
            return;
        }

        var local = PlayerControl.LocalPlayer;
        if (local == null)
        {
            return;
        }

        if (target == null
            && local.Data?.Role is CharlatanRole
            && _concealedBodies.Count > 0
            && Object.FindObjectsOfType<DeadBody>().Any(b => _concealedBodies.Contains(b.ParentId)))
        {
            AchievementsAPIHelper.Unlock(MuAchievements.NothingToSeeHere);
        }

        if (target != null
            && local.Data?.Role is WraithRole
            && caller != local
            && _wraithReturnedAfterKill
            && _wraithLastVictimId == target.PlayerId)
        {
            var body = Helpers.GetBodyById(target.PlayerId);
            if (body != null)
            {
                var localRoom = RoomHelpers.GetRoomName(local.GetTruePosition());
                var bodyRoom = RoomHelpers.GetRoomName(body.TruePosition);
                if (localRoom != null && bodyRoom != null && localRoom != bodyRoom)
                {
                    UnlockTentative(MuAchievements.AirtightAlibi);
                }
            }
        }

        _wraithReturnedAfterKill = false;
        _wraithLastVictimId = null;
        _wraithLastKillTime = -1000f;
        _currentDash = null;
        _killsThisDash = 0;
    }

    public static void OnMirageDecoyTriggered(PlayerControl mirage, PlayerControl? interactor, byte? appearanceId)
    {
        if (!AchievementsAPIHelper.IsLoaded || !mirage.AmOwner || interactor == null || interactor == mirage)
        {
            return;
        }

        AchievementsAPIHelper.Unlock(MuAchievements.Gottem);
        AchievementsAPIHelper.Increment(MuAchievements.CarnivalOfIllusions, 1);

        if (appearanceId == mirage.PlayerId)
        {
            AchievementsAPIHelper.Unlock(MuAchievements.StuntDouble);
        }

        if (appearanceId == interactor.PlayerId)
        {
            AchievementsAPIHelper.Unlock(MuAchievements.MistakenIdentity);
        }
    }

    public static void OnTrapperTrapTriggered(PlayerControl trapper, PlayerControl? victim)
    {
        if (!AchievementsAPIHelper.IsLoaded || !trapper.AmOwner || victim == null || victim == trapper)
        {
            return;
        }

        AchievementsAPIHelper.Increment(MuAchievements.PestControl, 1);

        var snares = _snaresPerVictim.GetValueOrDefault(victim.PlayerId) + 1;
        _snaresPerVictim[victim.PlayerId] = snares;
        if (snares >= 2)
        {
            AchievementsAPIHelper.Unlock(MuAchievements.RevolvingDoor);
        }

        if (_lastKillTimeByKiller.TryGetValue(victim.PlayerId, out var killTime)
            && Time.time - killTime <= 10f)
        {
            UnlockTentative(MuAchievements.HotPursuit);
        }
    }

    public static void OnCharlatanConcealComplete(PlayerControl charlatan, byte bodyId)
    {
        if (!AchievementsAPIHelper.IsLoaded || !charlatan.AmOwner)
        {
            return;
        }

        _concealedBodies.Add(bodyId);
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

    public static void OnCharlatanDeceiveReport(PlayerControl charlatan, DeadBody body)
    {
        if (!AchievementsAPIHelper.IsLoaded || !charlatan.AmOwner || body == null)
        {
            return;
        }

        var charlatanRoom = RoomHelpers.GetRoomName(charlatan.GetTruePosition());
        var bodyRoom = RoomHelpers.GetRoomName(body.TruePosition);
        if (charlatanRoom != null && bodyRoom != null && charlatanRoom != bodyRoom)
        {
            AchievementsAPIHelper.Unlock(MuAchievements.TheWayMojangIntended);
        }
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

    public static void OnInjectorInject(PlayerControl injector, PlayerControl target, InjectorEffectType effect)
    {
        if (!AchievementsAPIHelper.IsLoaded || !injector.AmOwner || target == null)
        {
            return;
        }

        AchievementsAPIHelper.Increment(MuAchievements.ClinicalTrial, 1);

        if (effect is InjectorEffectType.SpeedBoost or InjectorEffectType.VisionBoost
            or InjectorEffectType.Regeneration)
        {
            AchievementsAPIHelper.Unlock(MuAchievements.Placebo);
        }

        if (effect == InjectorEffectType.Confused && target.HasModifier<CluelessModifier>())
        {
            AchievementsAPIHelper.Unlock(MuAchievements.HopelesslyLost);
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
            UnlockTentative(MuAchievements.AndMakeItDouble);
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

        _forestallerLockArmed = true;
        _localLockActive = true;

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

        if (CharlatanBodySystem.IsBodyConcealed(bodyId))
        {
            AchievementsAPIHelper.Unlock(MuAchievements.AcquiredTaste);
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

    public static void OnLawyerObjectVotes(PlayerControl lawyer, bool clientHadVotes)
    {
        if (!AchievementsAPIHelper.IsLoaded || !lawyer.AmOwner)
        {
            return;
        }

        _lawyerObjectedThisMeeting = true;

        if (!clientHadVotes)
        {
            return;
        }

        AchievementsAPIHelper.Unlock(MuAchievements.ObjectionSustained);
    }

    public static void OnWraithLanternReturned(PlayerControl wraith)
    {
        if (!AchievementsAPIHelper.IsLoaded || !wraith.AmOwner)
        {
            return;
        }

        if (Time.time - _wraithLastKillTime <= 5f)
        {
            AchievementsAPIHelper.Unlock(MuAchievements.HitAndRun);
        }

        if (_wraithLastVictimId.HasValue)
        {
            _wraithReturnedAfterKill = true;
        }

        _wraithLanternReturns++;
        if (_wraithLanternReturns >= 5 && !_wraithLanternBroke)
        {
            AchievementsAPIHelper.Unlock(MuAchievements.LeaveNoTrace);
        }
    }

    public static void OnWraithLanternBroken(PlayerControl wraith)
    {
        if (!AchievementsAPIHelper.IsLoaded || !wraith.AmOwner)
        {
            return;
        }

        _wraithLanternBroke = true;
    }

    public static void OnSerialKillerManiacKill(PlayerControl killer, float timeRemaining)
    {
        if (!AchievementsAPIHelper.IsLoaded || !killer.AmOwner)
        {
            return;
        }

        if (timeRemaining > 0f && timeRemaining < 3f)
        {
            AchievementsAPIHelper.Unlock(MuAchievements.BuzzerBeater);
        }
    }

    public static void OnSpitefulCurseApplied(PlayerControl voter, PlayerControl exiled)
    {
        if (!AchievementsAPIHelper.IsLoaded || !voter.AmOwner)
        {
            return;
        }

        AchievementsAPIHelper.Unlock(MuAchievements.PyrrhicVote);

        if (voter.IsImpostor() && exiled.IsImpostor())
        {
            UnlockTentative(MuAchievements.FriendlyFire);
        }
    }

    public static void OnSpitefulEjected(PlayerControl exiled, int cursedVoterCount)
    {
        if (!AchievementsAPIHelper.IsLoaded || !exiled.AmOwner)
        {
            return;
        }

        if (cursedVoterCount >= 2)
        {
            AchievementsAPIHelper.Unlock(MuAchievements.MiseryLovesCompany);
        }
    }
}
