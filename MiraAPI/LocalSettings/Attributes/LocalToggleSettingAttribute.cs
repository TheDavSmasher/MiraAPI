using System;
using BepInEx.Configuration;
using MiraAPI.LocalSettings.SettingTypes;

namespace MiraAPI.LocalSettings.Attributes;

/// <summary>
/// Creates a toggle setting for the <see cref="ConfigEntry{T}"/>.
/// </summary>
/// <inheritdoc/>
[AttributeUsage(AttributeTargets.Property)]
public class LocalToggleSettingAttribute(
    string? name = null,
    string? description = null
    ) : LocalSettingAttribute(name, description)
{
    private readonly string? _name = name;
    private readonly string? _description = description;

    /// <inheritdoc/>
    internal override LocalToggleSetting CreateSetting(Type tab, ConfigEntryBase configEntryBase)
    {
        return new LocalToggleSetting(tab, configEntryBase, _name, _description);
    }
}
