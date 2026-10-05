using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraUnleashed.Buttons.Impostor;
using MiraUnleashed.Modules;
using MiraUnleashed.Options.Roles.Impostor;
using MiraUnleashed.Roles.Impostor;
using TownOfUs.Buttons;
using TownOfUs.Modules;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MiraUnleashed.Events.Impostor;

public static class CharlatanEvents
{
    [RegisterEvent]
    public static void RoundStartEventHandler(RoundStartEvent @event)
    {
        if (!@event.TriggeredByIntro)
        {
            return;
        }

        CharlatanBodySystem.ClearAll();
    }

    [RegisterEvent]
    public static void GameEndEventHandler(GameEndEvent @event)
    {
        CharlatanBodySystem.ClearAll();
    }

    [RegisterEvent]
    public static void AfterMurderEventHandler(AfterMurderEvent @event)
    {
        var source = @event.Source;
        if (source == null || source.Data.Role is not CharlatanRole)
        {
            return;
        }

        var target = @event.Target;
        if (target == null)
        {
            return;
        }

        if (source.AmOwner)
        {
            var options = OptionGroupSingleton<CharlatanOptions>.Instance;
            var killCount = GameHistory.KilledPlayers.Count(k => k.KillerId == source.PlayerId);
            var duration = options.DeceiveBaseDuration + (killCount * options.DeceiveDurationIncreasePerKill);

            var body = Object.FindObjectsOfType<DeadBody>().FirstOrDefault(x => x.ParentId == target.PlayerId);
            if (body != null)
            {
                CharlatanRole.RpcDeceiveActivate(source, target.PlayerId, duration);
            }
        }

        var concealButton = CustomButtonSingleton<CharlatanConcealButton>.Instance;
        if (concealButton != null && concealButton.LimitedUses)
        {
            var chargesPerKill = (int)OptionGroupSingleton<CharlatanOptions>.Instance.ConcealChargesPerKill;
            concealButton.UsesLeft += chargesPerKill;
            concealButton.SetUses(concealButton.UsesLeft);
        }
    }
}
