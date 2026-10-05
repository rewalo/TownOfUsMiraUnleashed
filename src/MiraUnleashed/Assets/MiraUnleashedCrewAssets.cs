using MiraAPI.Utilities.Assets;
using UnityEngine;

namespace MiraUnleashed.Assets;

public static class MiraUnleashedCrewAssets
{
    private const string ShortPath = "MiraUnleashed.Resources.Buttons";

    public static LoadableAsset<Sprite> DecoyButtonSprite { get; } = new LoadableResourceAsset($"{ShortPath}.Decoy_Button.png");
}
