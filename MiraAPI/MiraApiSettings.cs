using BepInEx.Configuration;
using MiraAPI.LocalSettings;
using MiraAPI.LocalSettings.Attributes;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;

namespace MiraAPI;

/// <summary>
/// Mira API Config File Handler
/// </summary>
public class MiraApiSettings(ConfigFile config) : LocalSettingsTab(config)
{
    /// <inheritdoc />
    public override string TabName => "Mira API";

    /// <inheritdoc />
    public override LocalSettingTabAppearance TabAppearance => new()
    {
        TabButtonHoverColor = MiraApiPlugin.MiraColor,
        TabIcon = MiraAssets.SettingsIcon,
    };

    /// <summary>
    /// Gets whether the modifiers hud should be on the left side of the screen (under roles/task tab). Recommended for streamers.
    /// </summary>
    [LocalToggleSetting]
    public ConfigEntry<bool> ModifiersHudLeftSide { get; private set; } = config.Bind("Displays", "Show Modifiers HUD on Left Side", false);

    /// <summary>
    /// Gets whether vents and their connections appear on the map.
    /// </summary>
    [LocalToggleSetting]
    public ConfigEntry<bool> ShowVentsOnMap { get; private set; } = config.Bind("Displays", "Show Vent Layout on Map", true);

    /// <summary>
    /// Gets whether cooldowns for buttons below 10 seconds appear with decimals.
    /// </summary>
    [LocalToggleSetting]
    public ConfigEntry<bool> PreciseCooldowns { get; private set; } = config.Bind("Buttons", "Precise Button Cooldowns Under 10s", false);

    /// <summary>
    /// Gets the size of ability buttons in-game.
    /// </summary>
    [LocalSliderSetting(min: 0.5f, max: 1.5f, displayValue: true, formatString: "0.00", suffixType: MiraNumberSuffixes.Multiplier)]
    public ConfigEntry<float> ButtonUiScale { get; private set; } = config.Bind("Buttons", "Button Scale Factor", 0.8f);

    /// <summary>
    /// Gets whether to show keybinds in the control mapper.
    /// </summary>
    public ConfigEntry<bool> ShowKeybinds { get; private set; } = config.Bind("Keybinds", "Show Keybinds in Control Mapper", true);
}
