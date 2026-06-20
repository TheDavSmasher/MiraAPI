using System;
using BepInEx.Configuration;
using MiraAPI.PluginLoading;

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
    /// Gets or sets the MiraPlugin that created these options.
    /// </summary>
    IMiraPlugin? ParentMod { get; set; }

    /// <summary>
    /// Gets or sets the visibility function for the options.
    /// </summary>
    Func<int, bool> Visible { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the options should be included with presets.
    /// </summary>
    bool IncludeInPreset { get; set; }

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
