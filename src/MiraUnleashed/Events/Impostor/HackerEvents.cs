using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Events.Vanilla.Meeting;
using MiraAPI.GameOptions;
using MiraUnleashed.Modules;
using MiraUnleashed.Options.Roles.Impostor;
using MiraUnleashed.Roles.Impostor;

namespace MiraUnleashed.Events.Impostor;

public static class HackerEvents
{
    [RegisterEvent]
    public static void RoundStartEventHandler(RoundStartEvent @event)
    {
        if (!@event.TriggeredByIntro)
        {
            return;
        }

        if (AmongUsClient.Instance == null || !AmongUsClient.Instance.AmHost || PlayerControl.LocalPlayer == null)
        {
            return;
        }

        HackerSystem.ResetAll();
    }

    [RegisterEvent]
    public static void GameEndEventHandler(GameEndEvent @event)
    {
        HackerSystem.ResetAll();
    }

    [RegisterEvent]
    public static void StartMeetingEventHandler(StartMeetingEvent @event)
    {
        if (AmongUsClient.Instance != null && AmongUsClient.Instance.AmHost && PlayerControl.LocalPlayer != null)
        {
            HackerRole.RpcHackerResetRound(PlayerControl.LocalPlayer);
        }
    }

    [RegisterEvent]
    public static void AfterMurderEventHandler(AfterMurderEvent @event)
    {
        var killer = @event.Source;
        if (killer?.Data?.Role is not HackerRole)
        {
            return;
        }

        if (AmongUsClient.Instance == null || !AmongUsClient.Instance.AmHost || PlayerControl.LocalPlayer == null)
        {
            return;
        }

        var opts = OptionGroupSingleton<HackerOptions>.Instance;
        if (!opts.JamEnabled)
        {
            return;
        }

        var perKill = (int)opts.JamChargesPerKill;
        var max = (int)opts.JamMaxCharges;
        HackerSystem.AddJamCharge(killer.PlayerId, perKill, max);

        var newCharges = HackerSystem.GetJamCharges(killer.PlayerId);
        HackerRole.RpcHackerSetJamCharges(PlayerControl.LocalPlayer, killer.PlayerId, newCharges);
    }
}
