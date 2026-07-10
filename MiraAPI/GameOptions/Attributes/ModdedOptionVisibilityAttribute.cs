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

    internal Func<bool>? GetVisibility(AbstractOptionGroup group, PropertyInfo property)
    {
        MemberInfo? member = GetVisibilityMember(group, property, out Type type);

        if (member is MethodInfo vMethod)
        {
            if (vMethod.ReturnType != typeof(bool))
            {
                Error($"Method {memberName} does not return a bool.");
                return null;
            }
            var paramList = vMethod.GetParameters();
            if (paramList.Length > 0)
            {
                Error($"Method {memberName} has too many parameters.");
                return null;
            }

            return () => (bool)vMethod.Invoke(group, null)!;
        }
        // TODO: Properties and/or fields

        Error($"Valid member {memberName} does not exist in group {type.FullName}.");
        return null;
    }

    internal Func<int, bool>? GetListVisibility(AbstractOptionGroup group, PropertyInfo property)
    {
        MemberInfo? member = GetVisibilityMember(group, property, out Type type);

        if (member is MethodInfo vMethod)
        {
            if (vMethod.ReturnType != typeof(bool))
            {
                Error($"Method {memberName} does not return a bool.");
                return null;
            }
            var paramList = vMethod.GetParameters();
            if (paramList.Length > 1)
            {
                Error($"Method {memberName} has too many parameters.");
                return null;
            }
            if (paramList.Length == 0)
            {
                return _ => (bool)vMethod.Invoke(group, null)!;
            }

            if (paramList[0].ParameterType != typeof(int))
            {
                Error($"Method {memberName}'s parameter is not an int.");
                return null;
            }
            return i => (bool)vMethod.Invoke(group, [i])!;
        }
        // TODO: Properties and/or fields

        Error($"Valid member {memberName} does not exist in group {type.FullName}.");
        return null;
    }

#pragma warning disable S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields
    private MemberInfo? GetVisibilityMember(AbstractOptionGroup group, PropertyInfo property, out Type type)
    {
        type = holderType ?? group.GetType();
        memberName ??= $"{property.Name}Visible";
        BindingFlags flags = BindingFlags.Static | BindingFlags.Public;
        if (holderType == null)
        {
            flags |= BindingFlags.Instance | BindingFlags.NonPublic;
        }
        return type.GetMember(memberName, flags).FirstOrDefault();
    }
#pragma warning restore S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields
}
