using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Configuration;
using MiraAPI.PluginLoading;

namespace MiraAPI.GameOptions.OptionTypes;

/// <summary>
/// Represents a modded option list.
/// </summary>
/// <typeparam name="T">The type of options.</typeparam>
public class ModdedOptionList<T> : IModdedOptionList where T : IModdedOption
{
    private IMiraPlugin? _parentMod;

    /// <inheritdoc/>
    public int Count { get; }

    /// <inheritdoc />
    public IMiraPlugin? ParentMod
    {
        get => _parentMod;
        set
        {
            if (_parentMod != null || value == null) return;
            _parentMod = value;

            foreach (var option in Options)
            {
                option.ParentMod = _parentMod!;
            }
        }
    }

    /// <summary>
    /// Gets the list of options.
    /// </summary>
    public T[] Options { get; }

    /// <inheritdoc />
    public Func<int, bool> Visible { get; set; }

    /// <inheritdoc />
    public bool IncludeInPreset { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ModdedOptionList{T}"/> class.
    /// </summary>
    /// <param name="count">The option list's length.</param>
    /// <param name="optionFactory">The option factory to instantiate the options from.</param>
    /// <param name="includeInPreset">Whether to include the options in the preset.</param>
    public ModdedOptionList(int count, Func<int, T> optionFactory, bool includeInPreset = true)
    {
        Count = count;
        Visible = _ => true;
        IncludeInPreset = includeInPreset;
        Options = Enumerable.Range(0, Count).Select(optionFactory).ToArray();
    }

    /// <inheritdoc/>
    public void SaveToPreset(ConfigFile presetConfig, bool saveDefault = false)
    {
        foreach (var option in Options)
        {
            option.SaveToPreset(presetConfig, saveDefault);
        }
    }

    /// <inheritdoc/>
    public void Bind(ConfigFile config)
    {
        foreach (var option in Options)
        {
            option.Bind(config);
        }
    }

    /// <inheritdoc/>
    public void LoadFromPreset(ConfigFile presetConfig)
    {
        foreach (var option in Options)
        {
            option.LoadFromPreset(presetConfig);
        }
    }

    /// <summary>
    /// Indexes the option of type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="idx">The option's index.</param>
    /// <returns>option of type <typeparamref name="T"/>.</returns>
    public T this[int idx] => Options[idx];
}
