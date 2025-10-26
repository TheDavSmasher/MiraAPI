using MiraAPI.Utilities.Assets;
using UnityEngine;

namespace MiraAPI.LocalSettings;

/// <summary>
/// Struct for modifying tab appearance.
/// Defines how the settings look.
/// </summary>
public record struct LocalSettingTabAppearance()
{
    /// <summary>
    /// Gets or sets main color of the tab.
    /// </summary>
    public Color TabColor { get; set; } = Color.white;

    /// <summary>
    /// Gets or sets tab button icon. No icon by default.
    /// </summary>
    public LoadableAsset<Sprite>? TabIcon { get; set; }

    /// <summary>
    /// Gets or sets color of the tab button.
    /// </summary>
    public Color TabButtonColor { get; set; } = Color.white;

    /// <summary>
    /// Gets or sets a value indicating whether the icon should be hidden when tab button is hovered.
    /// </summary>
    public bool HideIconOnHover { get; set; } = true;

    /// <summary>
    /// Gets or sets hover color of the tab button.
    /// </summary>
    public Color TabButtonHoverColor { get; set; } = Palette.AcceptedGreen;

    /// <summary>
    /// Gets or sets active color of the tab button.
    /// </summary>
    public Color TabButtonActiveColor { get; set; } = Palette.AcceptedGreen;

    /// <summary>
    /// Gets or sets hover color of toggles.
    /// </summary>
    public Color ToggleHoverColor { get; set; } = Palette.AcceptedGreen;

    /// <summary>
    /// Gets or sets active color of toggles.
    /// </summary>
    public Color ToggleActiveColor { get; set; } = Palette.AcceptedGreen;

    /// <summary>
    /// Gets or sets inactive color of toggles.
    /// </summary>
    public Color ToggleInactiveColor { get; set; } = Color.red;

    /// <summary>
    /// Gets or sets color of number buttons.
    /// </summary>
    public Color NumberColor { get; set; } = Color.white;

    /// <summary>
    /// Gets or sets hover color of number buttons.
    /// </summary>
    public Color NumberHoverColor { get; set; } = Palette.AcceptedGreen;

    /// <summary>
    /// Gets or sets color of sliders.
    /// </summary>
    public Color SliderColor { get; set; } = Color.white;

    /// <summary>
    /// Gets or sets hover color of sliders.
    /// </summary>
    public Color SliderHoverColor { get; set; } = Palette.AcceptedGreen;

    /// <summary>
    /// Gets or sets color of enum buttons.
    /// </summary>
    public Color EnumColor { get; set; } = Color.white;

    /// <summary>
    /// Gets or sets hover color of enum buttons.
    /// </summary>
    public Color EnumHoverColor { get; set; } = Palette.AcceptedGreen;

    /// <summary>
    /// Gets or sets color of buttons.
    /// </summary>
    public Color ButtonColor { get; set; } = Color.white;

    /// <summary>
    /// Gets or sets hover color of buttons.
    /// </summary>
    public Color ButtonHoverColor { get; set; } = Palette.AcceptedGreen;
}
