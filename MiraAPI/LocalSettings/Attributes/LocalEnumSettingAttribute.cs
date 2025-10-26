using System;
using BepInEx.Configuration;
using MiraAPI.LocalSettings.SettingTypes;

namespace MiraAPI.LocalSettings.Attributes;

/// <summary>
/// Creates an enum setting for the <see cref="ConfigEntry{T}"/>.
/// </summary>
/// <param name="names">Optional custom enum names.</param>
/// <inheritdoc/>
[AttributeUsage(AttributeTargets.Property)]
public class LocalEnumSettingAttribute(
    string? name = null,
    string? description = null,
    string[]? names = null
    ) : LocalSettingAttribute(name, description)
{
    private readonly string? _name = name;
    private readonly string? _description = description;

    /// <inheritdoc/>
    public override LocalEnumSetting CreateSetting(Type tab, ConfigEntryBase configEntryBase)
    {
        return new LocalEnumSetting(tab, configEntryBase, configEntryBase.SettingType, _name, _description, names);
    }
}
