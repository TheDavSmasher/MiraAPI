using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem;
using MiraAPI.GameOptions;
using MiraAPI.Patches.GameModes;
using MiraAPI.Patches.Options;
using MiraAPI.Presets;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using Reactor.Localization.Utilities;
using Reactor.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.ProBuilder;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace MiraAPI.GameModes;

/// <summary>
/// The game mode option.
/// </summary>
[HarmonyPatch]
public static class GameModeOption
{
    /// <summary>
    /// Gets or Sets the current index of the Game Mode Option
    /// For the value as an AbstractGameMode, see CustomGameModeManager.ActiveMode
    /// </summary>
    public static int Value
    {
        get =>
            OptionBehaviour != null
                ? OptionBehaviour.GetInt()
                : 0;
        set
        {
            if (OptionBehaviour == null)
                return;
            OptionBehaviour.Value = value;
            OptionBehaviour.UpdateValue();
            _lastValue = value;
        }
    }
    internal static StringOption OptionBehaviour { get; private set; } = null!;

    private static int _lastValue;
    private static readonly StringNames GamemodeName = CustomStringName.CreateAndRegister("Gamemode");
    private static readonly StringNames CustomName = CustomStringName.CreateAndRegister("Custom");
    private static readonly Dictionary<uint, StringNames> Values = new()
    {
        [0] = CustomStringName.CreateAndRegister("Classic"),
    };

    internal static void AddOption(AbstractGameMode mode)
    {
        if (!Values.ContainsKey(mode.ID))
            Values.Add(mode.ID, CustomStringName.CreateAndRegister(mode.GetColoredName()));
    }

    private static readonly List<CategoryHeaderMasked> VanillaCategories = new();
    private static readonly List<OptionBehaviour> VanillaOptions = new();
    private static readonly List<CategoryHeaderMasked> BaseCategories = new();
    private static readonly List<OptionBehaviour> BaseOptions = new();
    private static readonly List<CategoryHeaderMasked> ModeCategories = new();
    private static readonly List<OptionBehaviour> ModeOptions = new();
    [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.CreateSettings))]
    [HarmonyPostfix]
    private static void CreateSettingsPatch(GameOptionsMenu __instance)
    {
        if (GameSettingMenuPatches.SelectedModIdx != 0 || GameManager.Instance.IsHideAndSeek() || CustomGameModeManager.ActiveMode == null)
        {
            return;
        }

        var num = 0.713f;

        ModeCategories.Clear();
        ModeOptions.Clear();
        VanillaCategories.Clear();
        VanillaOptions.Clear();
        BaseCategories.Clear();
        BaseOptions.Clear();
        foreach (var category in __instance.settingsContainer.GetComponentsInChildren<CategoryHeaderMasked>())
        {
            if (category)
                category.gameObject.transform.localPosition -= new Vector3(0, 1.3f, 0);
        }

        var showOpts = CustomGameModeManager.ActiveMode.ShowNormalGameSettings;

        var newList = __instance.Children.ToArray();
        BaseOptions.Add(newList[0]);
        foreach (var obj in newList.Skip(1))
        {
            VanillaOptions.Add(obj);
            obj.gameObject.SetActive(showOpts);
        }
        foreach (var category in __instance.settingsContainer.GetComponentsInChildren<CategoryHeaderMasked>())
        {
            VanillaCategories.Add(category);
            category.gameObject.SetActive(showOpts);
        }

        CategoryHeaderMasked categoryHeaderMasked = Object.Instantiate(__instance.categoryHeaderOrigin, Vector3.zero, Quaternion.identity, __instance.settingsContainer);
        categoryHeaderMasked.SetHeader(CustomName, 20);
        categoryHeaderMasked.transform.localScale = Vector3.one * 0.63f;
        categoryHeaderMasked.transform.localPosition = new Vector3(-0.903f, num, -2f);
        OptionBehaviour = Object.Instantiate(
            __instance.stringOptionOrigin,
            Vector3.zero,
            Quaternion.identity,
            __instance.settingsContainer);
        num -= 0.63f;
        OptionBehaviour.transform.localPosition = new Vector3(0.952f, num, -2f);
        OptionBehaviour.SetClickMask(__instance.ButtonClickMask);
        StringGameSetting setting = ScriptableObject.CreateInstance<StringGameSetting>();
        setting.Type = OptionTypes.MultipleChoice;
        setting.Title = GamemodeName;
        setting.Index = _lastValue;
        setting.Values = new Il2CppStructArray<StringNames>([Values[0]]);
        OptionBehaviour.SetUpFromData(setting, 20);
        OptionBehaviour.TitleText.fontSize = 3;
        OptionBehaviour.OnValueChanged = (Action<OptionBehaviour>) ((OptionBehaviour opt) =>
        {
            _lastValue = opt.GetInt();
            CustomGameModeManager.SetGameMode((uint)_lastValue);
            HudPatches.SetGameModeText(CustomGameModeManager.GetMode(Values.ElementAt(_lastValue).Key).Name);
            // could make Values a dict of AbstractGameMode to 
            __instance.RefreshOptions(CustomGameModeManager.ActiveMode);
        });
        foreach (var optionBehaviour in __instance.Children.ToArray().Skip(1))
        {
            optionBehaviour.gameObject.transform.localPosition -= new Vector3(0, 1.3f, 0);
        }
        __instance.Children.Add(OptionBehaviour);
        BaseCategories.Add(categoryHeaderMasked);
        BaseOptions.Add(OptionBehaviour);
        for (var i = 1; i < Values.Count; i++)
            OptionBehaviour.Values = (Il2CppStructArray<StringNames>)OptionBehaviour.Values.Add(Values.ElementAt(i).Value);
        __instance.RefreshOptions(CustomGameModeManager.ActiveMode);
    }

    private static void RefreshOptions(this GameOptionsMenu instance, AbstractGameMode mode)
    {
        foreach (var option in ModeOptions)
        {
            option.gameObject.Destroy();
        }
        foreach (var header in ModeCategories)
        {
            header.gameObject.Destroy();
        }
        ModeOptions.Clear();
        ModeCategories.Clear();
        instance.Children.Clear();
        foreach (var group in BaseOptions)
        {
            instance.Children.Add(group);
        }

        var showOpts = mode.ShowNormalGameSettings;
        var num = 0.713f - (0.63f * BaseCategories.Count) - (0.45f * BaseOptions.Count);
        foreach (var category in VanillaCategories)
        {
            category.gameObject.SetActive(showOpts);
        }
        var filteredGroups =
            ModdedOptionsManager.GameModeOptionGroups
                .Where(x => x.Key == mode.GetType()).Select(y => y.Value).SelectMany(y => y) ?? [];

        foreach (var group in filteredGroups)
        {
            CreateGroup(instance, group);
        }
        if (showOpts)
        {
            foreach (var obj in VanillaOptions)
            {
                obj.gameObject.SetActive(true);
                instance.Children.Add(obj);
            }
            num -= 0.63f * VanillaCategories.Count;
            num -= 0.45f * VanillaOptions.Count;
        }
        else
        {
            foreach (var obj in VanillaOptions)
            {
                obj.gameObject.SetActive(false);
            }
        }

        if (filteredGroups.Any())
        {
            num += 0.225f;
            foreach (var group in filteredGroups)
            {
                GameOptionsMenuPatch.UpdateGroup(group, ref num);
            }
        }
        instance.ControllerSelectable.Clear();
        foreach (var obj in instance.scrollBar.GetComponentsInChildren<UiElement>())
        {
            instance.ControllerSelectable.Add(obj);
        }
        instance.scrollBar.SetYBoundsMax(-num - 1.65f);
    }

    [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.ValueChanged))]
    [HarmonyPrefix]
    private static bool ValueChanged(GameOptionsMenu __instance, OptionBehaviour option)
    {
        return !OptionBehaviour.Equals(option);
    }

    private static void CreateGroup(GameOptionsMenu menu, AbstractOptionGroup group)
    {
        var categoryHeaderMasked = Object.Instantiate(
            menu.categoryHeaderOrigin,
            Vector3.zero,
            Quaternion.identity,
            menu.settingsContainer);

        categoryHeaderMasked.SetHeader(CustomStringName.CreateAndRegister(group.GroupName), 20);
        categoryHeaderMasked.Background.color = group.GroupColor;
        categoryHeaderMasked.Divider.color = group.GroupColor;
        categoryHeaderMasked.Title.color = group.GroupColor.Equals(MiraApiPlugin.DefaultHeaderColor)
            ? Color.white
            : group.GroupColor.FindAlternateColor();

        categoryHeaderMasked.Background.sprite = MiraAssets.CategoryHeader.LoadAsset();
        categoryHeaderMasked.Background.sprite.texture.filterMode = FilterMode.Bilinear;
        categoryHeaderMasked.Background.sprite.texture.wrapMode = TextureWrapMode.Clamp;

        categoryHeaderMasked.Background.transform.localPosition = new Vector3(0.5f, -0.1833f, 0);
        categoryHeaderMasked.Background.size = new Vector2(
            categoryHeaderMasked.Background.size.x + 1.5f,
            categoryHeaderMasked.Background.size.y);

        categoryHeaderMasked.gameObject.SetActive(false);
        group.Header = categoryHeaderMasked;
        categoryHeaderMasked.transform.localPosition += Vector3.down;

        var newText = Object.Instantiate(categoryHeaderMasked.Title, categoryHeaderMasked.transform);
        newText.text = group.AllOptionsHidden
            ? "<size=70%>(Click to open)</size>"
            : "<size=70%>(Click to close)</size>";
        newText.transform.localPosition = new Vector3(2.6249f, -0.165f, 0f);
        newText.gameObject.GetComponent<TextTranslatorTMP>().Destroy();

        var options = group.Options.Select(opt => opt.CreateOption(
            menu.checkboxOrigin,
            menu.numberOptionOrigin,
            menu.stringOptionOrigin,
            menu.playerOptionOrigin,
            menu.settingsContainer));
        ModeCategories.Add(categoryHeaderMasked);

        /*OptionPreset? defaultPreset = null;
        if (GameSettingMenuPatches.SelectedMod != null && PresetManager.DefaultPresets.TryGetValue(
                GameSettingMenuPatches.SelectedMod,
                out var preset))
        {
            defaultPreset = preset;
        }*/

        foreach (var newOpt in options)
        {
            newOpt.SetClickMask(menu.ButtonClickMask);

            SpriteRenderer[] componentsInChildren = newOpt.GetComponentsInChildren<SpriteRenderer>(true);
            foreach (var renderer in componentsInChildren)
            {
                if (group.GroupColor != MiraApiPlugin.DefaultHeaderColor)
                {
                    renderer.color = group.GroupColor.FindAlternateColor();
                    if (renderer.transform.parent.TryGetComponent<GameOptionButton>(out var btn))
                    {
                        btn.interactableColor = group.GroupColor.FindAlternateColor();
                        btn.interactableHoveredColor = Color.white;
                    }
                }

                renderer.material.SetInt(PlayerMaterial.MaskLayer, 20);
            }

            foreach (var textMeshPro in newOpt.GetComponentsInChildren<TextMeshPro>(true))
            {
                if (group.GroupColor != MiraApiPlugin.DefaultHeaderColor)
                {
                    textMeshPro.color = group.GroupColor;
                }

                textMeshPro.fontMaterial.SetFloat(ShaderID.StencilComp, 3f);
                textMeshPro.fontMaterial.SetFloat(ShaderID.Stencil, 20);
            }

            if (newOpt is ToggleOption toggle)
            {
                toggle.CheckMark.sprite = MiraAssets.Checkmark.LoadAsset();
                toggle.CheckMark.color = group.GroupColor != MiraApiPlugin.DefaultHeaderColor
                    ? group.GroupColor
                    : MiraAssets.AcceptedTeal;
                var rend = toggle.CheckMark.transform.parent.FindChild("ActiveSprite")
                    .GetComponent<SpriteRenderer>();
                rend.sprite = MiraAssets.CheckmarkBox.LoadAsset();
                rend.color = group.GroupColor != MiraApiPlugin.DefaultHeaderColor
                    ? group.GroupColor
                    : MiraAssets.AcceptedTeal;
            }

            menu.Children.Add(newOpt);

            /*var resetBtn = new GameObject("ResetOption");
            resetBtn.transform.parent = newOpt.transform;
            resetBtn.transform.localScale = new(.5f, .5f, 1);
            resetBtn.layer = LayerMask.NameToLayer("UI");
            resetBtn.transform.localPosition = new Vector3(-3.1f, 0f, -2f);
            var resetRend = resetBtn.AddComponent<SpriteRenderer>();
            resetRend.sprite = MiraAssets.ResetButton.LoadAsset();
            resetRend.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            resetRend.color = group.GroupColor.Equals(MiraApiPlugin.DefaultHeaderColor)
                ? Color.white
                : group.GroupColor.FindAlternateColor();
            var resetBoxCol = resetBtn.gameObject.AddComponent<BoxCollider2D>();
            resetBoxCol.size = new Vector2(1f, 1f);
            resetBoxCol.offset = new Vector2(0, 0);
            var passiveButton = resetBtn.AddComponent<PassiveButton>();
            passiveButton.OnClick = new Button.ButtonClickedEvent();
            passiveButton.ClickSound = menu.BackButton.GetComponent<PassiveButton>().ClickSound;
            passiveButton.OnMouseOver = new UnityEvent();
            passiveButton.OnMouseOver.AddListener(
                (UnityAction)(() =>
                {
                    resetRend.color = group.GroupColor != MiraApiPlugin.DefaultHeaderColor
                        ? group.GroupColor
                        : MiraAssets.AcceptedTeal;
                }));
            passiveButton.OnMouseOut = new UnityEvent();
            passiveButton.OnMouseOut.AddListener(
                (UnityAction)(() =>
                {
                    resetRend.color = group.GroupColor.Equals(MiraApiPlugin.DefaultHeaderColor)
                        ? Color.white
                        : group.GroupColor.FindAlternateColor();
                }));
            if (newOpt is ToggleOption toggleOpt)
            {
                passiveButton.OnClick.AddListener(
                    (UnityAction)(() =>
                    {
                        defaultPreset!.ResetOption(toggleOpt);
                    }));
            }
            else if (newOpt is NumberOption numOpt)
            {
                passiveButton.OnClick.AddListener(
                    (UnityAction)(() =>
                    {
                        defaultPreset!.ResetOption(numOpt);
                    }));
            }
            else if (newOpt is StringOption strOpt)
            {
                passiveButton.OnClick.AddListener(
                    (UnityAction)(() =>
                    {
                        defaultPreset!.ResetOption(strOpt);
                    }));
            }
            if (!defaultPreset!.IsOptionInPreset(newOpt))
            {
                resetBtn.Destroy();
            }*/

            newOpt.Initialize();
            newOpt.gameObject.SetActive(false);
            ModeOptions.Add(newOpt);
        }

        var boxCol = categoryHeaderMasked.gameObject.AddComponent<BoxCollider2D>();
        boxCol.size = new Vector2(7, 0.7f);
        boxCol.offset = new Vector2(1.5f, -0.3f);

        var headerBtn = categoryHeaderMasked.gameObject.AddComponent<PassiveButton>();
        headerBtn.ClickSound = menu.BackButton.GetComponent<PassiveButton>().ClickSound;
        headerBtn.OnMouseOver = new UnityEvent();
        headerBtn.OnMouseOut = new UnityEvent();
        headerBtn.OnClick.AddListener(
            (UnityAction)(() =>
            {
                group.AllOptionsHidden = !group.AllOptionsHidden;
                newText.text = group.AllOptionsHidden
                    ? "<size=70%>(Click to open)</size>"
                    : "<size=70%>(Click to close)</size>";
                menu.RefreshOptions(CustomGameModeManager.ActiveMode!);
            }));
        headerBtn.SetButtonEnableState(true);
    }
}
