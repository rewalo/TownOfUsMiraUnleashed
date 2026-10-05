using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using MiraAPI;
using MiraAPI.PluginLoading;
using MiraAPI.Translation;
using Reactor;
using Reactor.Networking;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using TownOfUs;

namespace MiraUnleashed;

[BepInAutoPlugin("rewalo.mira.unleashed", "Mira Unleashed")]
[BepInProcess("Among Us.exe")]
[BepInDependency(ReactorPlugin.Id)]
[BepInDependency(MiraApiPlugin.Id)]
[BepInDependency(TownOfUsPlugin.Id)]
[ReactorModFlags(ModFlags.RequireOnAllClients)]
public partial class MiraUnleashedPlugin : BasePlugin, IMiraPlugin
{
    public static MiraUnleashedPlugin Instance { get; private set; } = null!;

    /// <inheritdoc />
    public string OptionsTitleText => "Mira Unleashed";

    /// <inheritdoc />
    public string GetAbbreviatedModName() => "MU";

    /// <inheritdoc />
    public ConfigFile GetConfigFile() => Config;

    public Harmony Harmony { get; } = new(Id);

    public override void Load()
    {
        Instance = this;

        ReactorCredits.Register("Mira Unleashed", Version, false, ReactorCredits.AlwaysShow);
        MiraLocaleManager.Register(Id);
        Harmony.PatchAll();
    }
}
