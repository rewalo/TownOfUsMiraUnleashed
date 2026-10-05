using MiraUnleashed.Options.Roles.Impostor;
using TownOfUs.Events.Impostor;
using TownOfUs.Patches;
using TownOfUs.Utilities;
using TownOfUs.Utilities.Appearances;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MiraUnleashed.Modifiers;

public sealed class InjectedConfusedModifier : InjectedDisabledModifier
{
    public InjectedConfusedModifier(float duration, InjectorEffectDurationType durationType)
        : base(InjectorEffectType.Confused, duration, durationType)
    {
    }

    public override bool CanReport => false;

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
        if (DurationType != InjectorEffectDurationType.AllRound && Player.AmOwner)
        {
            RemoveHallucinatoryEffects();
        }

        base.OnMeetingStart();
    }

    protected override void OnEffectRemoved()
    {
        if (Player != null && Player.AmOwner)
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
}
