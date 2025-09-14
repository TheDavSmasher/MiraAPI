using System;
using System.Linq;
using BepInEx.Configuration;
using UnityEngine;

namespace MiraAPI.LocalSettings.SettingTypes;

/// <inheritdoc />
public abstract class LocalSettingBase<T> : ILocalSetting
{
    /// <inheritdoc />
    public string Name { get; }

    /// <inheritdoc />
    public string Description { get; }

    /// <inheritdoc />
    public GameObject? Setting { get; } = null!;

    /// <inheritdoc />
    public ConfigEntryBase ConfigEntry { get; }

    internal LocalSettingsTab? Tab => LocalSettingsManager.Tabs.FirstOrDefault(x => x.Settings.Contains(this));

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalSettingBase{T}"/> class.
    /// </summary>
    /// <param name="tab">The tab to create the setting in.</param>
    /// <param name="configEntry">The config entry.</param>
    /// <param name="name">The name of the setting.</param>
    /// <param name="description">The description of the setting.</param>
    protected LocalSettingBase(Type tab, ConfigEntryBase configEntry, string? name = null, string? description = null)
    {
        ConfigEntry = configEntry;
        Name = name ?? ConfigEntry.Definition.Key;
        Description = description ?? ConfigEntry.Description.Description;
        LocalSettingsManager.TypeToTab[tab].Settings.Add(this);
    }

    /// <summary>
    /// Creates an instance of the setting.
    /// </summary>
    /// <returns>The created setting.</returns>
    public abstract GameObject CreateOption(ToggleButtonBehaviour toggle, SlideBar slider, Transform parent, ref float offset, ref int order, bool last);

    /// <summary>
    /// Returns the formated string to use in the text of the setting.
    /// </summary>
    /// <returns>The value text.</returns>
    protected virtual string GetValueText()
    {
        return string.Empty;
    }

    /// <summary>
    /// Gets the value of the config entry, casted to the setting type.
    /// </summary>
    /// <returns>The value.</returns>
    public virtual T GetValue()
    {
        return (T)ConfigEntry.BoxedValue;
    }

    /// <summary>
    /// Sets the value of the config entry.
    /// </summary>
    /// <param name="value">The value to set to.</param>
    public virtual void SetValue(T value)
    {
        ConfigEntry.BoxedValue = value;
        Tab?.OnOptionChanged(ConfigEntry);
    }
}
