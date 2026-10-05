using MiraAPI.Utilities.Assets;
using UnityEngine;

namespace MiraUnleashed.Assets;

public static class MiraUnleashedAudio
{
    private const string AudioPath = "MiraUnleashed.Resources.Audio";

    public static LoadableAsset<AudioClip> WitchLaugh { get; } = new LoadableAudioResourceAsset($"{AudioPath}.witch_laugh.wav");
    public static LoadableAsset<AudioClip> ObjectionSound { get; } = new LoadableAudioResourceAsset($"{AudioPath}.objection.wav");
    public static LoadableAsset<AudioClip> WraithDashSound { get; } = new LoadableAudioResourceAsset($"{AudioPath}.wraith_dash.wav");
    public static LoadableAsset<AudioClip> LanternBreakSound { get; } = new LoadableAudioResourceAsset($"{AudioPath}.lantern_break.wav");
    public static LoadableAsset<AudioClip> DecoyPlaceSound { get; } = new LoadableAudioResourceAsset($"{AudioPath}.decoy_place.wav");
    public static LoadableAsset<AudioClip> DecoyDestroySound { get; } = new LoadableAudioResourceAsset($"{AudioPath}.decoy_destroy.wav");
    public static LoadableAsset<AudioClip> HackerJamSound { get; } = new LoadableAudioResourceAsset($"{AudioPath}.hacker_jam.wav");
    public static LoadableAsset<AudioClip> ScavengerEatSound { get; } = new LoadableAudioResourceAsset($"{AudioPath}.scavenger_eat.wav");
}
