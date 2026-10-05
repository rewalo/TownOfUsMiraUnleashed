using MiraAPI.Utilities.Assets;
using UnityEngine;

namespace MiraUnleashed.Assets;

public static class MiraUnleashedAssets
{
    public static LoadableAsset<Sprite> HexedSprite { get; } = new LoadableResourceAsset("MiraUnleashed.Resources.Hexed.png");
    public static LoadableAsset<Sprite> ObjectionButtonSprite { get; } = new LoadableResourceAsset("MiraUnleashed.Resources.Buttons.Object.png");
    public static LoadableAsset<Sprite> ObjectionAnimationSprite { get; } = new LoadableResourceAsset("MiraUnleashed.Resources.Objection!.png");
    public static LoadableAsset<Sprite> LanternSprite { get; } = new LoadableResourceAsset("MiraUnleashed.Resources.Lantern.png");
    public static LoadableAsset<Sprite> BrokenLanternSprite { get; } = new LoadableResourceAsset("MiraUnleashed.Resources.BrokenLantern.png");
    public static LoadableAsset<Sprite> DollPlainSprite { get; } = new LoadableResourceAsset("MiraUnleashed.Resources.Doll_Plain.png");
    public static LoadableAsset<Sprite> DollPinsSprite { get; } = new LoadableResourceAsset("MiraUnleashed.Resources.Doll_Pins.png");
    public static LoadableAsset<Sprite> MirageRoleIcon { get; } = new LoadableResourceAsset("MiraUnleashed.Resources.Mirage_Role_Icon.png", 200f);
    public static LoadableAsset<Sprite> TrapperRoleIcon { get; } = new LoadableResourceAsset("MiraUnleashed.Resources.Trapper_Role_Icon.png", 200f);
    public static LoadableAsset<Sprite> ForestallerRoleIcon { get; } = new LoadableResourceAsset("MiraUnleashed.Resources.Forestaller_Role_Icon.png", 200f);
    public static LoadableAsset<Sprite> CluelessModifierIcon { get; } = new LoadableResourceAsset("MiraUnleashed.Resources.Clueless_Modifier_Icon.png", 200f);
    public static LoadableAsset<Sprite> SpitefulModifierIcon { get; } = new LoadableResourceAsset("MiraUnleashed.Resources.Spiteful_Modifier_Icon.png", 200f);
    public static LoadableAsset<Sprite> ScavengerRoleIcon { get; } = new LoadableResourceAsset("MiraUnleashed.Resources.Scavenger_Icon.png", 200f);
    public static LoadableAsset<Sprite> ScavengerEatButtonSprite { get; } = new LoadableResourceAsset("MiraUnleashed.Resources.Buttons.Scavenger_Eat_Button.png");
    public static LoadableAsset<Sprite> ScavengerScavengeButtonSprite { get; } = new LoadableResourceAsset("MiraUnleashed.Resources.Buttons.Scavenger_FindBody_Button.png");
}
