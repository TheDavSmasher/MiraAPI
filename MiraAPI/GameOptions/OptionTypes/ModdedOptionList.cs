using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    /// <summary>
    /// Initializes a new instance of the <see cref="ModdedOptionList{T}"/> class.
    /// </summary>
    /// <param name="count">The option list's length.</param>
    /// <param name="optionFactory">The option factory to instantiate the options from.</param>
    public ModdedOptionList(int count, Func<int, T> optionFactory)
    {
        Count = count;
        Options = Enumerable.Range(0, Count).Select(optionFactory).ToArray();
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
