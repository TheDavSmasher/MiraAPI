using BepInEx.Configuration;
using UnityEngine;

namespace MiraAPI.LocalSettings;

/// <summary>
/// Interface for all local settings.
/// </summary>
public interface ILocalSetting
{
    /// <summary>
    /// Gets the preferred name of a setting.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the description of a setting.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Gets the setting's gameobject.
    /// </summary>
    GameObject? Setting { get; }

    /// <summary>
    /// Gets the setting's config entry.
    /// </summary>
    ConfigEntryBase ConfigEntry { get; }

    /// <summary>
    /// Used to create the setting.
    /// </summary>
    /// <param name="toggle">Toggle template.</param>
    /// <param name="slider">Slider template.</param>
    /// <param name="parent">The parent of the setting.</param>
    /// <param name="offset">The offset.</param>
    /// <param name="order">The order.</param>
    /// <param name="last">Whether it's the last on the row.</param>
    /// <returns>The created setting.</returns>
    GameObject? CreateOption(ToggleButtonBehaviour toggle, SlideBar slider, Transform parent, ref float offset, ref int order, bool last);
}
