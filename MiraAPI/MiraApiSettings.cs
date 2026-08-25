using System.Collections;
using BepInEx.Configuration;
using MiraAPI.Hud;
using MiraAPI.LocalSettings;
using MiraAPI.LocalSettings.Attributes;
using MiraAPI.Patches;
using MiraAPI.Translation;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using UnityEngine;

namespace MiraAPI;

/// <summary>
/// Mira API <see cref="ConfigFile"/> Handler.
/// </summary>
public class MiraApiSettings(ConfigFile config) : LocalSettingsTab(config)
{
    /// <inheritdoc />
    public override string TabName => "miraApi.name".Translate();

    /// <inheritdoc />
    public override LocalSettingTabAppearance TabAppearance => new()
    {
        TabButtonHoverColor = MiraApiPlugin.MiraColor,
        TabIcon = MiraAssets.SettingsIcon,
        HideIconOnHover = false,
    };

    /// <summary>
    /// Gets or sets the value stored for scaling ability buttons properly.
    /// </summary>
    public static float OldButtonScaleFactor { get; set; }

    /// <summary>
    /// Gets or sets the value stored for scaling UI buttons properly.
    /// </summary>
    public static float OldUiButtonScaleFactor { get; set; }

    /// <inheritdoc />
    public override void Open()
    {
        base.Open();
        OldButtonScaleFactor = ButtonUIFactorSlider.Value;
        OldUiButtonScaleFactor = TopRightButtonsFactorSlider.Value;
    }

    /// <inheritdoc />
    public override void OnOptionChanged(ConfigEntryBase configEntry)
    {
        base.OnOptionChanged(configEntry);
        if (configEntry == ButtonUIFactorSlider)
        {
            if (HudManager.InstanceExists)
            {
                HudManagerPatches.ResizeUI(1f / OldButtonScaleFactor);
                HudManagerPatches.ResizeUI(ButtonUIFactorSlider.Value);
            }
            OldButtonScaleFactor = ButtonUIFactorSlider.Value;
        }
        else if (configEntry == TopRightButtonsFactorSlider)
        {
            if (MiraHudHelper.Instance)
            {
                ResizeUI(TopRightButtonsFactorSlider.Value);
            }
            OldUiButtonScaleFactor = TopRightButtonsFactorSlider.Value;
        }
        else if (configEntry == WikiOnBottomRow)
        {
            SetUpButtonPositions();
        }
        else if (configEntry == SetFpsSlider)
        {
            Application.targetFrameRate = (int)SetFpsSlider.Value;
        }
    }

    public static void SetUpButtonPositions()
    {
        var topUi = MiraHudHelper.UiTopRight;
        var extraTopUi = MiraHudHelper.ExtraUiTopRight;
        var wikiButton = MiraHudHelper.VanillaMatchInfoButton;
        var subButton = MiraHudHelper.SubmergedFloorButton;
        var modDisplay = MiraHudHelper.ModifierDisplayOnRight ? MiraHudHelper.ModifierDisplayObject : null!;
        ResetButtonPositions();
        if (topUi && extraTopUi)
        {
            var opts = LocalSettingsTabSingleton<MiraApiSettings>.Instance;
            if (wikiButton)
            {
                wikiButton.transform.SetParent(opts.WikiOnBottomRow.Value ? extraTopUi.transform : topUi.transform);
            }
            if (subButton)
            {
                subButton.transform.SetParent(extraTopUi.transform);
            }
            if (modDisplay)
            {
                modDisplay.transform.SetParent(extraTopUi.transform);
            }
            MiraHudHelper.UiGrid.ArrangeChilds();
            MiraHudHelper.ExtraUiGrid.ArrangeChilds();
        }
    }

    public static void ResetButtonPositions()
    {
        var topUi = MiraHudHelper.UiTopRight;
        var extraTopUi = MiraHudHelper.ExtraUiTopRight;
        var subButton = MiraHudHelper.SubmergedFloorButton;
        var modDisplay = MiraHudHelper.ModifierDisplayOnRight ? MiraHudHelper.ModifierDisplayObject : null!;
        if (topUi && extraTopUi)
        {
            var wikiButton = MiraHudHelper.VanillaMatchInfoButton;
            if (wikiButton)
            {
                wikiButton.transform.SetParent(null);
            }
            if (subButton)
            {
                subButton.transform.SetParent(null);
            }
            if (modDisplay)
            {
                modDisplay.transform.SetParent(null);
            }
        }
    }

    public static IEnumerator CoResizeSettingsUI()
    {
        while (!HudManager.Instance || !MiraHudHelper.UiGrid || !MiraHudHelper.ExtraUiGrid)
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.01f);
        ResizeUI(LocalSettingsTabSingleton<MiraApiSettings>.Instance.TopRightButtonsFactorSlider.Value);
    }

    public static void ResizeUI(float scaleFactor)
    {
        var alteredScale = scaleFactor * 0.85f;
        var actualScaleVal = scaleFactor * 1.176470588235294f;
        var actualScale = new Vector3(actualScaleVal, actualScaleVal, 1);
        var baseAspect = MiraHudHelper.UiAspectPos;
        var baseGrid = MiraHudHelper.UiGrid;
        var baseUi = MiraHudHelper.UiTopRight;
        if (baseUi && baseAspect && baseGrid)
        {
            baseAspect.DistanceFromEdge = new Vector2(0.435f * scaleFactor, 0.475f * scaleFactor);

            foreach (var button in baseUi.GetComponentsInChildren<PassiveButton>(true))
            {
                if (button.gameObject == null)
                {
                    continue;
                }
                if (button.transform.name.Contains("Friends List Button"))
                {
                    button.gameObject.transform.localScale = new Vector3(0.2675f * actualScaleVal, 0.2675f * actualScaleVal, 1);
                    continue;
                }

                button.gameObject.transform.localScale = actualScale;
            }

            baseGrid.CellSize = new Vector2(alteredScale, alteredScale);
            if (baseGrid.gameObject.transform.childCount != 0)
            {
                baseGrid.ArrangeChilds();
            }
            baseAspect.AdjustPosition();
        }

        var extraAspect = MiraHudHelper.ExtraUiAspectPos;
        var extraGrid = MiraHudHelper.ExtraUiGrid;
        var extraUi = MiraHudHelper.ExtraUiTopRight;
        if (extraUi && extraAspect && extraGrid)
        {
            extraAspect.DistanceFromEdge = new Vector3(0.435f * scaleFactor, 1.25f * scaleFactor, 0f);

            foreach (var button in extraUi.GetAllChildren())
            {
                if (button.gameObject == null)
                {
                    continue;
                }
                if (button.transform.name.Contains("Modifiers"))
                {
                    button.gameObject.transform.localScale = new Vector3(0.65f * scaleFactor, 0.65f * scaleFactor, 1);
                    continue;
                }

                button.gameObject.transform.localScale = actualScale;
            }

            extraGrid.CellSize = new Vector2(alteredScale, alteredScale);
            if (extraGrid.gameObject.transform.childCount != 0)
            {
                extraGrid.ArrangeChilds();
            }
            extraAspect.AdjustPosition();
        }
    }

    /// <summary>
    /// Gets whether the modifiers hud should be on the left side of the screen (under roles/task tab). Recommended for streamers.
    /// </summary>
    [LocalToggleSetting]
    public ConfigEntry<bool> ModifiersHudLeftSide { get; private set; } = config.Bind("Visuals/UI", "Show Modifiers HUD on Left Side", false);

    /// <summary>
    /// Gets whether to show keybinds on buttons.
    /// </summary>
    [LocalToggleSetting]
    public ConfigEntry<bool> ShowKeybinds { get; private set; } = config.Bind("Visuals/UI", "Show Keybinds on Buttons", true);

    /// <summary>
    /// Gets the scale of the ability buttons.
    /// </summary>
    [LocalSliderSetting(min: 0.5f, max: 1.5f, suffixType: MiraNumberSuffixes.Multiplier, formatString: "0.00", displayValue: true)]
    public ConfigEntry<float> ButtonUIFactorSlider { get; private set; } =
        config.Bind("Visuals/UI", "Button Scale Factor", 0.75f);

    /// <summary>
    /// Gets the scale of the UI buttons.
    /// </summary>
    [LocalSliderSetting(min: 0.3f, max: 2f, suffixType: MiraNumberSuffixes.Multiplier, formatString: "0.00", displayValue: true)]
    public ConfigEntry<float> TopRightButtonsFactorSlider { get; private set; } =
        config.Bind("Visuals/UI", "Top-Right UI Scale Factor", 1f);

    /// <summary>
    /// Gets the fps specified by the player.
    /// </summary>
    [LocalSliderSetting(min: 60f, max: 240f, suffixType: MiraNumberSuffixes.None, formatString: "0", displayValue: true, roundValue: true)]
    public ConfigEntry<float> SetFpsSlider { get; private set; } =
        config.Bind("Visuals/UI", "Max FPS", 120f);

    [LocalToggleSetting]
    public ConfigEntry<bool> WikiOnBottomRow { get; private set; } =
        config.Bind("Visuals/UI", "Display Wiki on Bottom Row", true);

    /// <summary>
    /// Gets whether to apply cosmetic changes to the TaskAdder.
    /// </summary>
    [LocalToggleSetting]
    public ConfigEntry<bool> PrettyTaskAdder { get; private set; } = config.Bind("Freeplay", "Pretty Task Laptop", true);

    /// <summary>
    /// Gets whether to show the red flash from sabotages.
    /// </summary>
    [LocalToggleSetting]
    public ConfigEntry<bool> EnableSabotageFlashes { get; private set; } = config.Bind("Accessibility", "Enable Sabotage Flashes", true);

    /// <summary>
    /// Gets whether to enable the sabotage sound effects or not.
    /// </summary>
    [LocalToggleSetting]
    public ConfigEntry<bool> EnableSabotageBlares { get; private set; } = config.Bind("Accessibility", "Enable Sabotage Blare", true);
}
