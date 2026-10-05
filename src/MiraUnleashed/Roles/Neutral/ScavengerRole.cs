using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.GameModes;
using MiraAPI.GameOptions;
using MiraAPI.Patches.Stubs;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using MiraUnleashed.Assets;
using MiraUnleashed.Modules;
using MiraUnleashed.Networking;
using MiraUnleashed.Options.Roles.Neutral;
using TownOfUs.Assets;
using TownOfUs.Extensions;
using TownOfUs.Modules;
using TownOfUs.Modules.Components;
using TownOfUs.Modules.TimeLord;
using TownOfUs.Modules.Wiki;
using TownOfUs.Options;
using TownOfUs.Options.Roles.Crewmate;
using TownOfUs.Options.Roles.Neutral;
using TownOfUs.Roles;
using TownOfUs.Roles.Impostor;
using TownOfUs.Roles.Neutral;
using TownOfUs.Utilities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MiraUnleashed.Roles.Neutral;

public sealed class ScavengerRole(IntPtr cppPtr) : NeutralRole(cppPtr), IMiraUnleashedRole, IWikiDiscoverable, IDoomable
{
    public DoomableType DoomHintType => DoomableType.Fearmonger;

    public string GetAdvancedDescription()
    {
        return ((ICustomRole)this).RoleWikiDescription + MiscUtils.AppendOptionsText(GetType());
    }

    public Color RoleColor => MiraUnleashedColors.Scavenger;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;
    public RoleAlignment RoleAlignment => RoleAlignment.NeutralEvil;

    [HideFromIl2Cpp]
    public int BodiesEaten { get; set; }

    public CustomRoleConfiguration Configuration => new(this)
    {
        CanUseVent = OptionGroupSingleton<ScavengerOptions>.Instance.CanVent,
        Icon = MiraUnleashedAssets.ScavengerRoleIcon,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>()
    };

    public bool CanSpawnOnCurrentMode()
    {
        if (!Configuration.AssociatedGameMode.IsInstanceOfType(CustomGameModeManager.ActiveMode))
        {
            return false;
        }

        if (!OptionGroupSingleton<ScavengerOptions>.Instance.CannotSpawnWithJanitor)
        {
            return true;
        }

        var janitor = MiscUtils.AllInGameRoles.OfType<JanitorRole>().FirstOrDefault() as ICustomRole;
        if (janitor is { } custom && custom.GetCount() > 0 && custom.GetChance() > 0)
        {
            return false;
        }

        return true;
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities
    {
        get
        {
            var abilities = new List<CustomButtonWikiDescription>
            {
                new(
                    MiraLocaleManager.Get("MiraUnleashed.Role.ScavengerEat", "Eat"),
                    MiraLocaleManager.Get("MiraUnleashed.Role.ScavengerEat.WikiDescription"),
                    MiraUnleashedAssets.ScavengerEatButtonSprite)
            };

            var options = OptionGroupSingleton<ScavengerOptions>.Instance;
            if (options.ScavengeEnabled && options.ScavengeDuration.Value > 0f)
            {
                abilities.Add(new(
                    MiraLocaleManager.Get("MiraUnleashed.Role.ScavengerScavenge", "Scavenge"),
                    MiraLocaleManager.Get("MiraUnleashed.Role.ScavengerScavenge.WikiDescription"),
                    MiraUnleashedAssets.ScavengerScavengeButtonSprite));
            }

            return abilities;
        }
    }

    public bool WinConditionMet()
    {
        if (Player.HasDied())
        {
            return false;
        }

        var options = OptionGroupSingleton<ScavengerOptions>.Instance;
        return BodiesEaten >= (int)options.BodiesToWin;
    }

    public bool IsWinConditionImpossible()
    {
        if (Player.HasDied())
        {
            return true;
        }

        var options = OptionGroupSingleton<ScavengerOptions>.Instance;
        var bodiesNeeded = (int)options.BodiesToWin;
        var bodiesEaten = BodiesEaten;
        var bodiesRemaining = bodiesNeeded - bodiesEaten;

        var allBodies = Object.FindObjectsOfType<DeadBody>();
        var availableBodies = allBodies.Count(b => !ScavengerSystem.IsBodyEaten(b.ParentId));

        var alivePlayers = Helpers.GetAlivePlayers();
        var potentialBodies = alivePlayers.Count - 1;

        return availableBodies + potentialBodies < bodiesRemaining;
    }

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);
        BodiesEaten = 0;
    }

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);
        ScavengerSystem.ClearForPlayer(targetPlayer.PlayerId);
    }

    public override bool CanUse(IUsable usable)
    {
        return GameManager.Instance.LogicUsables.CanUse(usable, Player);
    }

    public override bool DidWin(GameOverReason gameOverReason)
    {
        return WinConditionMet();
    }

    [MethodRpc((uint)MiraUnleashedRpc.ScavengerEat)]
    public static void RpcScavengerEat(PlayerControl scavenger, byte bodyId)
    {
        if (scavenger?.Data?.Role is not ScavengerRole role)
        {
            return;
        }

        var body = TimeLordBodyManager.FindDeadBodyIncludingInactive(bodyId);
        if (body == null)
        {
            body = Object.FindObjectsOfType<DeadBody>().FirstOrDefault(x => x.ParentId == bodyId);
        }

        if (body == null)
        {
            return;
        }

        role.BodiesEaten++;
        ScavengerSystem.MarkBodyEaten(bodyId);

        if (scavenger.AmOwner)
        {
            TouAudio.PlaySound(MiraUnleashedAudio.ScavengerEatSound);
        }

        var isHost = AmongUsClient.Instance != null && AmongUsClient.Instance.AmHost;
        var optionEnabled = OptionGroupSingleton<TimeLordOptions>.Instance.UncleanBodiesOnRewind;
        var shouldRecord = isHost ? optionEnabled : (optionEnabled || TimeLordRewindSystem.MatchHasTimeLord());

        var destroyBody = (BodyVitalsMode)OptionGroupSingleton<GameMechanicOptions>.Instance.CleanedBodiesAppearance.Value;

        if (shouldRecord)
        {
            var bodyPlayer = MiscUtils.PlayerById(bodyId);
            if (bodyPlayer != null)
            {
                TownOfUs.Events.Crewmate.TimeLordEventHandlers.RecordBodyCleaned(scavenger, body, body.transform.position,
                    TimeLordBodyManager.CleanedBodySource.Janitor);
            }
            Coroutines.Start(TimeLordBodyManager.CoHideBodyForTimeLord(body, destroyBody));
        }
        else
        {
            Coroutines.Start(body.CoCleanCustom(destroyBody));
        }
        Coroutines.Start(CrimeSceneComponent.CoClean(body));

        if (role.IsWinConditionImpossible() && !scavenger.HasDied())
        {
            var options = OptionGroupSingleton<ScavengerOptions>.Instance;
            var roleType = ((BecomeOptions)options.OnLoseBecomes.Value) switch
            {
                BecomeOptions.Crew => (ushort)RoleTypes.Crewmate,
                BecomeOptions.Jester => RoleId.Get<JesterRole>(),
                BecomeOptions.Survivor => RoleId.Get<SurvivorRole>(),
                BecomeOptions.Amnesiac => RoleId.Get<AmnesiacRole>(),
                BecomeOptions.Mercenary => RoleId.Get<MercenaryRole>(),
                _ => (ushort)RoleTypes.Crewmate
            };

            scavenger.ChangeRole(roleType);
        }
    }

    [MethodRpc((uint)MiraUnleashedRpc.ScavengerScavenge)]
    public static void RpcScavengerScavenge(PlayerControl scavenger)
    {
        if (scavenger?.Data?.Role is not ScavengerRole)
        {
            return;
        }

        var options = OptionGroupSingleton<ScavengerOptions>.Instance;
        ScavengerSystem.StartScavenge(scavenger.PlayerId, options.ScavengeDuration.Value);
    }
}
