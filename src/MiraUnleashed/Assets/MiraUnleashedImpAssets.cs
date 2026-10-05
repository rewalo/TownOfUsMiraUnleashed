using MiraAPI.Utilities.Assets;
using UnityEngine;

namespace MiraUnleashed.Assets;

public static class MiraUnleashedImpAssets
{
    private const string ShortPath = "MiraUnleashed.Resources.Buttons";
    private const string HackerPath = "MiraUnleashed.Resources.Hacker";

    public static LoadableAsset<Sprite> SpellButtonSprite { get; } = new LoadableResourceAsset($"{ShortPath}.SpellButton.png");
    public static LoadableAsset<Sprite> LanternButtonSprite { get; } = new LoadableResourceAsset($"{ShortPath}.LanternButton.png");

    public static LoadableAsset<Sprite> HackerDownloadButtonSprite { get; } = new LoadableResourceAsset($"{HackerPath}.hackerdownload.png");
    public static LoadableAsset<Sprite> HackerJamButtonSprite { get; } = new LoadableResourceAsset($"{HackerPath}.HackerJam.png");
    public static LoadableAsset<Sprite> HackerDeviceGenericSprite { get; } = new LoadableResourceAsset($"{HackerPath}.evilcamera.png");
    public static LoadableAsset<Sprite> HackerR1EvilCameraSprite { get; } = new LoadableResourceAsset($"{HackerPath}.R1EvilCamera.png");
    public static LoadableAsset<Sprite> HackerRole { get; } = new LoadableResourceAsset($"{HackerPath}.Hacker_Role.png");
    public static LoadableAsset<Sprite> HackerAdminSkeldSprite { get; } = new LoadableResourceAsset($"{HackerPath}.eviladminskeld.png");
    public static LoadableAsset<Sprite> HackerAdminMiraSprite { get; } = new LoadableResourceAsset($"{HackerPath}.eviladminmira.png");
    public static LoadableAsset<Sprite> HackerAdminPolusSprite { get; } = new LoadableResourceAsset($"{HackerPath}.eviladminpolus.png");
    public static LoadableAsset<Sprite> HackerAdminAirshipSprite { get; } = new LoadableResourceAsset($"{HackerPath}.eviladminairship.png");
    public static LoadableAsset<Sprite> HackerAdminSubmergedSprite { get; } = new LoadableResourceAsset($"{HackerPath}.eviladminsubmerged.png");
    public static LoadableAsset<Sprite> HackerCamerasSprite { get; } = new LoadableResourceAsset($"{HackerPath}.evilcamera.png");
    public static LoadableAsset<Sprite> HackerDoorLogSprite { get; } = new LoadableResourceAsset($"{HackerPath}.evildoorlog.png");
    public static LoadableAsset<Sprite> HackerVitalsSprite { get; } = new LoadableResourceAsset($"{HackerPath}.evilvitals.png");

    public static LoadableAsset<Sprite> InjectorInjectButtonSprite { get; } = new LoadableResourceAsset($"{ShortPath}.Inject_Button.png");
    public static LoadableAsset<Sprite> InjectorRole { get; } = new LoadableResourceAsset("MiraUnleashed.Resources.Injector_Role_Icon.png", 200f);
    public static LoadableAsset<Sprite> CharlatanRole { get; } = new LoadableResourceAsset("MiraUnleashed.Resources.Charlatan_Role_Icon.png", 200f);
    public static LoadableAsset<Sprite> WraithRole { get; } = new LoadableResourceAsset("MiraUnleashed.Resources.Voodoo_Role_Icon.png", 200f);
    public static LoadableAsset<Sprite> DeceiveButtonSprite { get; } = new LoadableResourceAsset($"{ShortPath}.DecieveButton.png");
    public static LoadableAsset<Sprite> ConcealButtonSprite { get; } = new LoadableResourceAsset($"{ShortPath}.ConcealButton.png");
}
