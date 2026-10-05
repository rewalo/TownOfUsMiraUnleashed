using MiraAPI.Modifiers;
using MiraUnleashed.Modifiers;
using MiraUnleashed.Modules;
using MiraUnleashed.Roles.Neutral;
using MiraUnleashed.Utilities;
using PerfectComms.Api;
using TownOfUs.Utilities;

namespace MiraUnleashed.Integrations;

/// <summary>
/// Registers Mira Unleashed's voice rules, host options and managed radio channels with Perfect
/// Comms. All PerfectComms.Api types live inside this type only.
/// </summary>
internal static class PerfectCommsRuntime
{
    private const string MuteWraithWhileInvisible = nameof(MuteWraithWhileInvisible);
    private const string JamDisruptsVoice = nameof(JamDisruptsVoice);
    private const string MuffleInjectedHearing = nameof(MuffleInjectedHearing);
    private const string TeamRadioLawyer = nameof(TeamRadioLawyer);

    // Object-typed caches keep Perfect Comms types out of this type's field signatures. This matters
    // when Harmony enumerates mod types while the optional runtime assembly is absent.
    private static readonly object WraithInvisibleMuted = VoiceRuleResult.Mute("Invisible");
    private static readonly object ListenerMuffled = new VoiceListenerFilterResult(true);
    private static readonly object ListenerSightObscured =
        new VoiceListenerFilterResult(false) { SightObscured = true };
    private static readonly object ListenerMuffledAndSightObscured =
        new VoiceListenerFilterResult(true) { SightObscured = true };
    private static readonly object ListenerNormal = new VoiceListenerFilterResult(false);
    private static readonly Dictionary<ushort, object> LawyerRadios = [];

    private static bool _registered;

    internal static void Register()
    {
        if (_registered)
        {
            return;
        }

        var required =
            VoiceApiCapability.PerSpeakerMuffle |
            VoiceApiCapability.GlobalReceiveGate |
            VoiceApiCapability.ContextualListeners |
            VoiceApiCapability.ListenerSightObscuration |
            VoiceApiCapability.ManagedTeamRadio |
            VoiceApiCapability.PersistentHostOptions |
            VoiceApiCapability.ConditionalHostOptions |
            VoiceApiCapability.OverlayPrivacy;
        if (!PerfectCommsApi.Supports(required))
        {
            Warning($"Perfect Comms {PerfectCommsApi.RuntimeApiVersion} does not expose every Mira Unleashed integration capability; voice integration is disabled.");
            return;
        }

        var id = MiraUnleashedPlugin.Id;
        try
        {
            RegisterOptions(id);
            PerfectCommsApi.RegisterVoiceRule(id, ResolveSpeakerRule);
            PerfectCommsApi.RegisterContextualGlobalGate(id, VoicePhaseKind.Tasks,
                context => context.GetOption(JamDisruptsVoice) && HackerSystem.IsJammed, "Jammed");
            PerfectCommsApi.RegisterContextualListenerFilter(id, ResolveListenerFilter);
            PerfectCommsApi.RegisterManagedRadioChannel(id, ResolveLawyerRadio);
            PerfectCommsApi.RegisterOverlaySpeakerRule(id, ResolveOverlaySpeaker);
            _registered = true;
            Info($"Registered Mira Unleashed voice integration with Perfect Comms API {PerfectCommsApi.RuntimeApiVersion}.");
        }
        catch (Exception ex)
        {
            PerfectCommsApi.Unregister(id);
            Error($"Perfect Comms integration failed and was disabled: {ex}");
        }
    }

    private static void RegisterOptions(string id)
    {
        PerfectCommsApi.RegisterModTab(id, "Mira Unleashed");

        RegisterToggle(id, MuteWraithWhileInvisible,
            "<color=#FF0000><b>Wraith</b></color>: Mute While Invisible", true,
            "Prevents an invisible Wraith from transmitting voice until the Lantern invisibility ends.");
        RegisterToggle(id, JamDisruptsVoice,
            "<color=#FF0000><b>Hacker</b></color>: Jam Disrupts Voice", true,
            "Mutes everyone's voice during tasks while a Hacker's Jam is active, the same way a communications sabotage does.");
        RegisterToggle(id, MuffleInjectedHearing,
            "<color=#FF0000><b>Injector</b></color>: Muffle Injected Hearing", true,
            "Muffles incoming voice during tasks for players injected with Low Vision, Very Low Vision, Confusion or Nausea.");
        RegisterToggle(id, TeamRadioLawyer,
            "Team Radio - <color=#EDB38C><b>Lawyer</b></color>", true,
            "Enables a private managed Team Radio channel between a Lawyer and their client when Team Radio is on. Like the private chat, this lets the client know who their Lawyer is.",
            context => context.TeamRadioEnabled);
    }

    private static void RegisterToggle(
        string id,
        string key,
        string label,
        bool defaultValue,
        string description,
        Func<VoiceHostOptionContext, bool>? visible = null)
    {
        PerfectCommsApi.RegisterHostOption(
            id,
            new VoiceHostOption(key, label, defaultValue)
            {
                Description = description,
                Visible = visible,
            });
    }

    private static VoiceRuleResult ResolveSpeakerRule(VoiceRuleContext context)
    {
        if (!context.IsDead &&
            context.Phase is VoicePhaseKind.Tasks or VoicePhaseKind.Meeting or VoicePhaseKind.Exile &&
            context.GetOption(MuteWraithWhileInvisible) &&
            context.Player.HasModifier<WraithLanternInvisibilityModifier>())
        {
            return (VoiceRuleResult)WraithInvisibleMuted;
        }

        return VoiceRuleResult.Pass;
    }

    private static VoiceListenerFilterResult ResolveListenerFilter(VoiceListenerContext context)
    {
        if (context.Phase != VoicePhaseKind.Tasks)
        {
            return (VoiceListenerFilterResult)ListenerNormal;
        }

        bool sightObscured =
            context.Listener.HasModifier<InjectedLowVisionModifier>() ||
            context.Listener.HasModifier<InjectedVeryLowVisionModifier>();
        bool muffled =
            context.GetOption(MuffleInjectedHearing) &&
            (sightObscured ||
             context.Listener.HasModifier<InjectedConfusedModifier>() ||
             context.Listener.HasModifier<InjectedNauseaModifier>());
        if (muffled)
        {
            return (VoiceListenerFilterResult)(sightObscured
                ? ListenerMuffledAndSightObscured
                : ListenerMuffled);
        }

        return (VoiceListenerFilterResult)(sightObscured
            ? ListenerSightObscured
            : ListenerNormal);
    }

    private static VoiceManagedRadioChannelResult? ResolveLawyerRadio(VoiceRuleContext context)
    {
        if (context.IsDead || !context.GetOption(TeamRadioLawyer))
        {
            return null;
        }

        byte lawyerId;
        byte clientId;
        if (context.Player.Data?.Role is LawyerRole lawyerRole)
        {
            var client = lawyerRole.Client ?? LawyerUtils.FindClientForLawyer(context.Player.PlayerId);
            if (client == null || client.HasDied())
            {
                return null;
            }

            lawyerId = context.Player.PlayerId;
            clientId = client.PlayerId;
        }
        else if (context.Player.GetModifier<LawyerTargetModifier>() is { } target)
        {
            var lawyerPlayer = LawyerUtils.GetLawyerForClient(context.Player, target.OwnerId)?.Player;
            if (lawyerPlayer == null || lawyerPlayer.HasDied())
            {
                return null;
            }

            lawyerId = target.OwnerId;
            clientId = context.Player.PlayerId;
        }
        else
        {
            return null;
        }

        ushort pairId = (ushort)((Math.Min(lawyerId, clientId) << 8) | Math.Max(lawyerId, clientId));
        if (!LawyerRadios.TryGetValue(pairId, out var cached))
        {
            cached = new VoiceManagedRadioChannelResult($"lawyer:{lawyerId}:{clientId}", "Lawyer", "L");
            LawyerRadios[pairId] = cached;
        }

        return (VoiceManagedRadioChannelResult)cached;
    }

    private static VoiceOverlaySpeakerResult ResolveOverlaySpeaker(VoiceOverlaySpeakerContext context)
    {
        if (context.Phase != VoicePhaseKind.Tasks)
        {
            return VoiceOverlaySpeakerResult.Pass;
        }

        return context.Speaker.HasModifier<WraithLanternInvisibilityModifier>()
            ? VoiceOverlaySpeakerResult.HideSource
            : VoiceOverlaySpeakerResult.Pass;
    }
}
