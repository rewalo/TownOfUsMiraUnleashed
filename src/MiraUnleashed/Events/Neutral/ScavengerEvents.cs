using AmongUs.GameOptions;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Events.Vanilla.Player;
using MiraAPI.GameOptions;
using MiraAPI.Roles;
using MiraUnleashed.Options.Roles.Neutral;
using MiraUnleashed.Roles.Neutral;
using TownOfUs.Options.Roles.Neutral;
using TownOfUs.Roles;
using TownOfUs.Roles.Neutral;
using TownOfUs.Utilities;

namespace MiraUnleashed.Events.Neutral;

public static class ScavengerEvents
{
    [RegisterEvent]
    public static void PlayerDeathEvent(PlayerDeathEvent @event)
    {
        foreach (var player in PlayerControl.AllPlayerControls)
        {
            if (player == null || player.HasDied() || player.Data?.Role is not ScavengerRole role)
            {
                continue;
            }

            if (role.IsWinConditionImpossible())
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

                player.ChangeRole(roleType);
            }
        }
    }

    [RegisterEvent]
    public static void RoundStartEventHandler(RoundStartEvent @event)
    {
        if (@event.TriggeredByIntro)
        {
            Modules.ScavengerSystem.ClearAll();
        }
    }
}
