using System;
using System.Collections.Generic;
using System.Reflection;
using BepInEx.Unity.IL2CPP;
using MiraAPI.GameOptions;
using Reactor.Utilities;

namespace MiraAPI.LocalSettings;

/// <summary>
/// Manages local settings.
/// </summary>
public static class LocalSettingsManager
{
    internal static readonly Dictionary<Type, LocalSettingsTab> TypeToTab = [];
    internal static readonly List<LocalSettingsTab> Tabs = [];

    internal static bool RegisterTab(Type type, BasePlugin pluginInfo)
    {
        if (Activator.CreateInstance(type, pluginInfo.Config) is not LocalSettingsTab tab)
        {
            return false;
        }

        if (TypeToTab.ContainsKey(type))
        {
            Error($"Local settings tab {type.Name} already exists.");
            return false;
        }

        Tabs.Add(tab);
        TypeToTab.Add(type, tab);

        typeof(LocalSettingsTabSingleton<>).MakeGenericType(type)
#pragma warning disable S3011
            .GetField("_instance", BindingFlags.Static | BindingFlags.NonPublic)!
#pragma warning restore S3011
            .SetValue(null, tab);

        return true;
    }
}
