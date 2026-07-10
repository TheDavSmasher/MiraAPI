using System;
using System.Linq;
using System.Reflection;

namespace MiraAPI.GameOptions.Attributes;

/// <summary>
/// Attribute to modify a property with a <see cref="ModdedOptionAttribute"/> or <see cref="ModdedOptionListAttribute"/>.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ModdedOptionVisiblityAttribute"/> class.
/// </remarks>
/// <param name="holderType">The type the member is in.</param>
/// <param name="memberName">The member to get the visibility function from.</param>
[AttributeUsage(AttributeTargets.Property)]
public sealed class ModdedOptionVisiblityAttribute(Type? holderType = null, string? memberName = null) : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ModdedOptionVisiblityAttribute"/> class.
    /// </summary>
    /// <param name="memberName">The member to get the visibility function from.</param>
    public ModdedOptionVisiblityAttribute(string memberName)
        : this(null, memberName)
    {
    }

#pragma warning disable S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields
    internal Func<bool>? GetVisibility(AbstractOptionGroup group, PropertyInfo property)
    {
        return null;
    }

    internal Func<int, bool>? GetListVisibility(AbstractOptionGroup group, PropertyInfo property)
    {
        return null;
    }
#pragma warning restore S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields
}
