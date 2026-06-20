using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Configuration;
using MiraAPI.PluginLoading;

namespace MiraAPI.GameOptions.OptionTypes;

/// <summary>
/// Represents a modded option list.
/// </summary>
/// <typeparam name="T">The type of options.</typeparam>
public class ModdedOptionList<T> : IModdedOptionList, IReadOnlyList<T> where T : IModdedOption
{
    /// <inheritdoc/>
    public int Count { get; }

    /// <inheritdoc />
    public IMiraPlugin? ParentMod
    {
        get;
        set
        {
            if (field != null || value == null) return;
            field = value;

            foreach (var option in Options)
            {
                option.ParentMod = value;
            }
        }
    }

    /// <summary>
    /// Gets the list of options.
    /// </summary>
    public IReadOnlyList<T> Options { get; }

    /// <inheritdoc />
    public Func<int, bool> Visible
    {
        get;
        set
        {
            field = value;
            foreach (var (option, idx) in Options.Select((o, i) => (o, i)))
            {
                option.Visible = () => value(idx);
            }
        }
    }

    /// <inheritdoc />
    public bool IncludeInPreset
    {
        get;
        set
        {
            field = value;
            foreach (var option in Options)
            {
                option.IncludeInPreset = value;
            }
        }
    }

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

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator()
    {
        return ((IEnumerable<T>)Options).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Options.GetEnumerator();
    }

    /// <summary>
    /// Indexes the option of type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="idx">The option's index.</param>
    /// <returns>option of type <typeparamref name="T"/>.</returns>
    public T this[int idx] => Options[idx];
}
