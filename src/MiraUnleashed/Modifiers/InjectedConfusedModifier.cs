using MiraAPI.Modifiers;
using MiraAPI.Utilities.Assets;
using MiraUnleashed.Events.Impostor;
using MiraUnleashed.Options.Roles.Impostor;
using TownOfUs.Events.Impostor;
using TownOfUs.Modifiers;
using TownOfUs.Patches;
using TownOfUs.Utilities;
using TownOfUs.Utilities.Appearances;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MiraUnleashed.Modifiers;

public sealed class InjectedConfusedModifier : DisabledModifier, IInjectedModifier
{
    public override string ModifierName => "Injected (Confused)";
    public override bool HideOnUi => true;
    public override LoadableAsset<Sprite>? ModifierIcon => null;
    public override bool CanReport => false;

    private readonly float _duration;
    private readonly InjectorEffectDurationType _durationType;

    public InjectedConfusedModifier(float duration, InjectorEffectDurationType durationType)
    {
        _duration = duration;
        _durationType = durationType;
    }

    public Guid InjectionId { get; set; }

    public override float Duration => _durationType switch
            {
                InjectorEffectDurationType.AllRound => -1f,
                InjectorEffectDurationType.AllGame => -1f,
                InjectorEffectDurationType.SetTime => _duration,
                _ => _duration
            };

    public override bool AutoStart => true;
    public override bool CanUseAbilities => true;

    public override void OnActivate()
    {
        if (!Player.AmOwner)
        {
            return;
        }

        ApplyHallucinatoryEffects();
    }

    public override void OnMeetingStart()
    {
        if (_durationType == InjectorEffectDurationType.AllRound)
        {
            Player.RemoveModifier(this);
        }
        else if (Player.AmOwner)
        {
            RemoveHallucinatoryEffects();
        }
    }

    private void ApplyHallucinatoryEffects()
    {
        var players = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.HasDied() && x != Player).ToList();

        foreach (var player in players)
        {
            var hidden = Random.RandomRangeInt(0, 3);
            if (hidden == 0)
            {
                var seeker = players[Random.RandomRangeInt(0, players.Count)];
                if (seeker != null && seeker != player)
                {
                    var seekerAppearance = seeker.GetDefaultModifiedAppearance();
                    player.RawSetAppearance(seekerAppearance);
                }
            }
            else if (hidden == 1)
            {
                player.SetCamouflage();
            }
            else
            {
                var swoop = new VisualAppearance(player.GetDefaultModifiedAppearance(), TownOfUsAppearances.Swooper)
                {
                    HatId = string.Empty,
                    SkinId = string.Empty,
                    VisorId = string.Empty,
                    PlayerName = string.Empty,
                    PetId = string.Empty,
                    RendererColor = new Color(0f, 0f, 0f, 0.1f),
                    NameColor = Color.clear,
                    ColorBlindTextColor = Color.clear
                };

                player.RawSetAppearance(swoop);
            }

            player?.cosmetics.ToggleNameVisible(false);
        }
    }

    private void RemoveHallucinatoryEffects()
    {
        foreach (var player in PlayerControl.AllPlayerControls.ToArray().Where(x => !x.HasDied()))
        {
            player.MyPhysics.SetForcedBodyType(PlayerControl.LocalPlayer.BodyType);
            if (HudManagerPatches.CamouflageCommsEnabled)
            {
                continue;
            }

            player.RawSetAppearance(player.GetDefaultModifiedAppearance());
            player.cosmetics.ToggleNameVisible(true);
        }
    }

    public override void OnDeactivate()
    {
        base.OnDeactivate();
        if (Player != null && Player.AmOwner)
        {
            RemoveHallucinatoryEffects();
            InjectorEvents.ShowEffectWoreOffNotification(Player, "MiraUnleashed.Injector.Notification.WoreOffConfused");
        }
    }

    public string GetEffectDescription()
    {
        return MiraLocaleManager.Get("MiraUnleashed.Injector.EffectDescription.Confused", "Hallucinations, cannot report");
    }
}
