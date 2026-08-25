using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using MiraAPI.PluginLoading;
using MiraAPI.Translation;
using MiraAPI.Utilities.Assets;
using Reactor;
using Reactor.Networking;
using Reactor.Networking.Attributes;
using UnityEngine;

namespace MiraAPI.Example;

[BepInAutoPlugin("mira.example", "MiraExampleMod")]
[BepInProcess("Among Us.exe")]
[BepInDependency(ReactorPlugin.Id)]
[BepInDependency(MiraApiPlugin.Id)]
[ReactorModFlags(ModFlags.RequireOnAllClients)]
public partial class ExamplePlugin : BasePlugin, IMiraPlugin
{
    public ExamplePlugin()
    {
        MiraLocaleManager.Register("mira.example");
    }
    public Harmony Harmony { get; } = new(Id);
    public string OptionsTitleText => "Mira API\nExample Mod";
    public string CustomOptionMenuNameTwo => "angxl's Options";
    public ConfigFile GetConfigFile() => Config;
    public override void Load()
    {
        ExampleEventHandlers.Initialize();
        Harmony.PatchAll();
    }
}
