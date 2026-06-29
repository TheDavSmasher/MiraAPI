using MiraAPI.Utilities.Assets;
using UnityEngine;

namespace MiraAPI.LocalSettings;

/// <summary>
/// Struct for modifying <see cref="LocalSettingsTab"/> appearance.
/// Defines how the settings look.
/// </summary>
public record struct LocalSettingTabAppearance()
{
    /// <summary>
    /// Gets or sets the main <see cref="Color"/> of the <see cref="LocalSettingsTab"/>.
    /// </summary>
    public Color TabColor { get; set; } = Color.white;

    /// <summary>
    /// Gets or sets the <see cref="LocalSettingsTab"/> button icon. No icon by default.
    /// </summary>
    public LoadableAsset<Sprite>? TabIcon { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="Color"/> of the <see cref="LocalSettingsTab"/> button.
    /// </summary>
    public Color TabButtonColor { get; set; } = Color.white;

    /// <summary>
    /// Gets or sets a value indicating whether the icon should be hidden when the <see cref="LocalSettingsTab"/> button is hovered.
    /// </summary>
    public bool HideIconOnHover { get; set; } = true;

    /// <summary>
    /// Gets or sets the hover <see cref="Color"/> of the <see cref="LocalSettingsTab"/> button.
    /// </summary>
    public Color TabButtonHoverColor { get; set; } = Palette.AcceptedGreen;

    /// <summary>
    /// Gets or sets the active <see cref="Color"/> of the <see cref="LocalSettingsTab"/> button.
    /// </summary>
    public Color TabButtonActiveColor { get; set; } = Palette.AcceptedGreen;

    /// <summary>
    /// Gets or sets the hover <see cref="Color"/> of <see cref="SettingTypes.LocalToggleSetting"/>s.
    /// </summary>
    public Color ToggleHoverColor { get; set; } = Palette.AcceptedGreen;

    /// <summary>
    /// Gets or sets the active <see cref="Color"/> of <see cref="SettingTypes.LocalToggleSetting"/>s.
    /// </summary>
    public Color ToggleActiveColor { get; set; } = Palette.AcceptedGreen;

    /// <summary>
    /// Gets or sets the inactive <see cref="Color"/> of <see cref="SettingTypes.LocalToggleSetting"/>s.
    /// </summary>
    public Color ToggleInactiveColor { get; set; } = Color.red;

    /// <summary>
    /// Gets or sets the <see cref="Color"/> of <see cref="SettingTypes.LocalNumberSetting"/> buttons.
    /// </summary>
    public Color NumberColor { get; set; } = Color.white;

    /// <summary>
    /// Gets or sets the hover <see cref="Color"/> of <see cref="SettingTypes.LocalNumberSetting"/> buttons.
    /// </summary>
    public Color NumberHoverColor { get; set; } = Palette.AcceptedGreen;

    /// <summary>
    /// Gets or sets the <see cref="Color"/> of <see cref="SettingTypes.LocalSliderSetting"/>s.
    /// </summary>
    public Color SliderColor { get; set; } = Color.white;

    /// <summary>
    /// Gets or sets the hover <see cref="Color"/> of <see cref="SettingTypes.LocalSliderSetting"/>s.
    /// </summary>
    public Color SliderHoverColor { get; set; } = Palette.AcceptedGreen;

    /// <summary>
    /// Gets or sets the <see cref="Color"/> of <see cref="SettingTypes.LocalEnumSetting"/> buttons.
    /// </summary>
    public Color EnumColor { get; set; } = Color.white;

    /// <summary>
    /// Gets or sets the hover <see cref="Color"/> of <see cref="SettingTypes.LocalEnumSetting"/> buttons.
    /// </summary>
    public Color EnumHoverColor { get; set; } = Palette.AcceptedGreen;

    /// <summary>
    /// Gets or sets the <see cref="Color"/> of buttons.
    /// </summary>
    public Color ButtonColor { get; set; } = Color.white;

    /// <summary>
    /// Gets or sets the hover <see cref="Color"/> of buttons.
    /// </summary>
    public Color ButtonHoverColor { get; set; } = Palette.AcceptedGreen;
}
