using System;
using System.Reflection;

namespace MiraAPI.GameOptions.Attributes;

/// <summary>
/// Represents an attribute that is used to define an <see cref="IModdedOption"/>.
/// </summary>
/// <param name="title">The option title.</param>
/// <param name="roleType">Optional parameter to specify a role Type.</param>
[AttributeUsage(AttributeTargets.Property)]
public abstract class ModdedOptionAttribute(string title, Type? roleType = null) : PropertyOptionAttribute
{
    internal IModdedOption? HolderOption { get; set; }

    /// <summary>
    /// Gets the title of the option.
    /// </summary>
    public string Title => title;

    /// <summary>
    /// Gets the role type of the option.
    /// </summary>
    protected Type? RoleType => roleType;

    internal abstract IModdedOption? CreateOption(object? value, PropertyInfo property);
}
