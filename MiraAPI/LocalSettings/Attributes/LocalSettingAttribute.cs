using System;
using BepInEx.Configuration;

namespace MiraAPI.LocalSettings.Attributes;

/// <summary>
/// Base class for all local settings attributes.
/// </summary>
/// <param name="name">The name of the setting. Defaults to entry key.</param>
/// <param name="description">The description of the setting. Defalts to entry description.</param>
[AttributeUsage(AttributeTargets.Property)]
public abstract class LocalSettingAttribute(
    string? name = null,
    string? description = null
    ) : Attribute
{
    /// <summary>
    /// Returns the created setting object.
    /// </summary>
    /// <param name="tab">Gets the tab where the setting is located.</param>
    /// <param name="configEntryBase">Gets the config entry the setting is attached to.</param>
    /// <returns>The created class.</returns>
    public abstract ILocalSetting CreateSetting(Type tab, ConfigEntryBase configEntryBase);
}
