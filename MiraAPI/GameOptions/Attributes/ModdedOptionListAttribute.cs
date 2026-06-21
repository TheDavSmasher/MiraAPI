using System;
using System.Reflection;

namespace MiraAPI.GameOptions.Attributes;

/// <summary>
/// Represents an attribute that is used to define a list of modded options.
/// </summary>
/// <param name="titler">A function to title of the options.</param>
[AttributeUsage(AttributeTargets.Property)]
public abstract class ModdedOptionListAttribute(Func<int, string> titler) : Attribute
{
    internal IModdedOptionList? HolderOptionList { get; set; }

    /// <summary>
    /// Gets the function to title of the options.
    /// </summary>
    public Func<int, string> Titler => titler;

    /// <summary>
    /// Sets the value of all the options.
    /// </summary>
    /// <param name="value">The new values as an object.</param>
    public abstract void SetValue(object value);

    /// <summary>
    /// Sets the value of the specific option.
    /// </summary>
    /// <param name="modOpt">The option to set.</param>
    /// <param name="value">The new value as an object.</param>
    public abstract void SetValue(IModdedOption modOpt, object value);

    /// <summary>
    /// Gets the value of all the options.
    /// </summary>
    /// <returns>The value of the options as an object.</returns>
    public abstract object GetValue();

    /// <summary>
    /// Gets the value of the specific option.
    /// </summary>
    /// <param name="modOpt">The option to set.</param>
    /// <returns>The value of the option as an object.</returns>
    public abstract object GetValue(IModdedOption modOpt);

    internal abstract IModdedOptionList? CreateOptionList(object? value, PropertyInfo property);
}
