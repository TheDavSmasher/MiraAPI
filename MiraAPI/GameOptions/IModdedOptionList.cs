using System;
using System.Collections.Generic;
using BepInEx.Configuration;
using MiraAPI.Networking;
using MiraAPI.PluginLoading;
using UnityEngine;

namespace MiraAPI.GameOptions;

/// <summary>
/// Interface for list of modded options.
/// </summary>
public interface IModdedOptionList
{
    /// <summary>
    /// Gets the number of options in the list.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Gets the unique identifiers for the options.
    /// </summary>
    IReadOnlyList<uint> Ids { get; }

    /// <summary>
    /// Gets or sets the titles of the options.
    /// The end title will equal this value with its index appended (e.g., Title1).
    /// </summary>
    string Title { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the first option's title has a 0, else starts with 1.
    /// </summary>
    bool ZeroIndexTitle { get; set; }

    /// <summary>
    /// Gets the StringName for the options, used for localization.
    /// </summary>
    IReadOnlyList<StringNames> StringNames { get; }

    /// <summary>
    /// Gets or sets the MiraPlugin that created these options.
    /// </summary>
    IMiraPlugin? ParentMod { get; set; }

    /// <summary>
    /// Gets the game setting data for the options.
    /// </summary>
    IReadOnlyList<BaseGameSetting> Data { get; }

    /// <summary>
    /// Gets the OptionBehaviour object of the options.
    /// </summary>
    IReadOnlyList<OptionBehaviour?> OptionBehaviours { get; }

    /// <summary>
    /// Gets or sets the visibility function for the options.
    /// </summary>
    Func<int, bool> Visible { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the options should be included with presets.
    /// </summary>
    bool IncludeInPreset { get; set; }

    /// <summary>
    /// Gets the array of ConfigDefinition for the options, used for BepInEx configuration.
    /// </summary>
    ConfigDefinition?[] ConfigDefinitions { get; }

    /// <summary>
    /// Creates the option behaviour for the modded option at index <paramref name="idx"/>.
    /// </summary>
    /// <param name="idx">The option's index.</param>
    /// <param name="toggleOpt">The ToggleOption template.</param>
    /// <param name="numberOpt">The NumberOption template.</param>
    /// <param name="stringOpt">The StringOption template.</param>
    /// <param name="playerOpt">The PlayerOption template.</param>
    /// <param name="container">>The Transform container for the option.</param>
    /// <returns>The created OptionBehaviour object.</returns>
    OptionBehaviour CreateOption(int idx, ToggleOption toggleOpt, NumberOption numberOpt, StringOption stringOpt, PlayerOption playerOpt, Transform container);

    /// <summary>
    /// Gets the value at index <paramref name="idx"/> as a float.
    /// </summary>
    /// <param name="idx">The option's index.</param>
    /// <returns>The value of the option as a float.</returns>
    float GetFloatData(int idx);

    /// <summary>
    /// Gets the NetData for the option at index <paramref name="idx"/>, used for network synchronization.
    /// </summary>
    /// <param name="idx">The option's index.</param>
    /// <returns>Returns the NetData object for the option.</returns>
    NetData GetNetData(int idx);

    /// <summary>
    /// Handles incoming network data for the option at index <paramref name="idx"/>.
    /// </summary>
    /// <param name="idx">The option's index.</param>
    /// <param name="data">The byte array representing the network data.</param>
    void HandleNetData(int idx, byte[] data);

    /// <summary>
    /// Saves the options to a preset configuration file.
    /// </summary>
    /// <param name="presetConfig">The ConfigFile representing the preset configuration.</param>
    /// <param name="saveDefault">Indicates whether to save the default value instead of the current value.</param>
    void SaveToPreset(ConfigFile presetConfig, bool saveDefault = false);

    /// <summary>
    /// Binds the options to a configuration file.
    /// </summary>
    /// <param name="config">The ConfigFile to bind the option to.</param>
    void Bind(ConfigFile config);

    /// <summary>
    /// Loads the options from a preset configuration file, applying the values to the options' configuration.
    /// </summary>
    /// <param name="presetConfig">The ConfigFile representing the preset configuration.</param>
    void LoadFromPreset(ConfigFile presetConfig);
}
