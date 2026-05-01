using System;
using System.Collections.Generic;
using MiraAPI.PluginLoading;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;

namespace MiraAPI.GameModes;

/// <summary>
/// Manages custom gamemodes.
/// </summary>
public static class CustomGameModeManager
{
    public static readonly Dictionary<uint, AbstractGameMode> IdToModeMap = [];

    private static uint GetNextId() => ++LastId;

    internal static uint LastId { get; private set; }

    /// <summary>
    /// Register gamemode from type.
    /// </summary>
    /// <param name="gameModeType">Type of gamemode class, should inherit from <see cref="AbstractGameMode"/>.</param>
    /// <param name="pluginInfo">The custom plugin info of the mod.</param>
    internal static void RegisterGameMode(Type gameModeType, MiraPluginInfo pluginInfo)
    {
        if (!typeof(AbstractGameMode).IsAssignableFrom(gameModeType))
        {
            Warning($"{gameModeType.Name} does not inherit CustomGameMode!");
            return;
        }

        var instance = Activator.CreateInstance(gameModeType);

        if (instance is not AbstractGameMode mode)
        {
            Error($"Failed to create instance of {gameModeType.Name}");
            return;
        }

        IdToModeMap.Add(GetNextId(), mode);
        pluginInfo.GameModes.Add(LastId, mode);
        GameModeOption.AddOption(mode);
        mode.ID = LastId;
    }

    /// <summary>
    /// Checks to see if the default game mode is on.
    /// </summary>
    /// <returns>True if the default mode is the current one.</returns>
    public static bool IsDefault() => ActiveMode is DefaultMode;
    public static bool IsHideNSeek() => ActiveMode is HideAndSeekMode;

    /// <summary>
    /// Gets the current gamemode.
    /// </summary>
    public static AbstractGameMode? ActiveMode { get; internal set; }

    internal static void RegisterDefaultMode()
    {
        var defaultMode = new DefaultMode();
        IdToModeMap.Add(0, defaultMode);
        // no need to add to game mode option as it already contains it
        // because we cannot have the option be created with no values
        defaultMode.ID = 0;
        var hnsMode = new HideAndSeekMode();
        IdToModeMap.Add(1, hnsMode);
        hnsMode.ID = 1;
        GameModeOption.AddOption(hnsMode);
        LastId++;
    }

    internal static void SetGameMode(uint id)
    {
        if (IdToModeMap.TryGetValue(id, out var mode))
        {
            ActiveMode = mode;
        }
        else if (id != 0)
        {
            ActiveMode = IdToModeMap[0];
            GameModeOption.Value = 0;
            Logger<MiraApiPlugin>.Warning($"Unable to find game mode of id {id}!");
        }
    }

    internal static void GetAndSetGameMode()
    {
        var id = (uint)GameModeOption.Value;

        if (IdToModeMap.TryGetValue(id, out var mode))
        {
            ActiveMode = mode;
            return;
        }

        ActiveMode = IdToModeMap[0];
        Logger<MiraApiPlugin>.Warning($"Unable to find game mode of id {id}!");
    }
}