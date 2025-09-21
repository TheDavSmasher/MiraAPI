using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MiraAPI.LocalSettings;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MiraAPI.Patches.LocalSettings;

[HarmonyPatch(typeof(OptionsMenuBehaviour))]
public static class OptionsMenuPatches
{
    internal static OptionsMenuBehaviour? Instance { get; private set; }
    internal static BoxCollider2D MaskCollider;
    private static SpriteRenderer? background;

    private static int currentPage = 1;
    private static Dictionary<int, List<GameObject>> tabButtons = new();

    /// <summary>
    /// Creates the tabs and their content.
    /// </summary>
    [HarmonyPostfix]
    [HarmonyPatch(nameof(OptionsMenuBehaviour.Start))]
    public static void StartPostfix(OptionsMenuBehaviour __instance)
    {
        Instance = __instance;
        // Fix for tabs not being clickable in the main menu
        if (!AmongUsClient.Instance.IsInGame)
        {
            __instance.Background.GetComponent<BoxCollider2D>().enabled = false;
            __instance.transform.FindChild("Tint").SetLocalZ(5.6f);
        }

        background = __instance.Background;

        var maskObj = new GameObject
        {
            layer = 5,
            name = "SpriteMask",
        };
        maskObj.transform.SetParent(__instance.transform);
        maskObj.transform.localPosition = new Vector3(0, -0.3f, 0);
        maskObj.transform.localScale = new Vector3(500, 120, 1);

        var blank = Sprite.Create(
            Texture2D.whiteTexture,
            new Rect(0, 0, Texture2D.whiteTexture.width, Texture2D.whiteTexture.height),
            new Vector2(0.5f, 0.5f));

        var mask = maskObj.AddComponent<SpriteMask>();
        mask.sprite = blank;
        mask.isCustomRangeActive = true;
        mask.sortingLayerName = "Default";
        mask.backSortingOrder = 100;
        mask.frontSortingOrder = 500;

        MaskCollider = maskObj.AddComponent<BoxCollider2D>();
        MaskCollider.size = new Vector2(0.01f, 0.1f);
        MaskCollider.isTrigger = true;
        MaskCollider.enabled = true;

        currentPage = 1;
        tabButtons.Clear();
        float yOffset = 0;
        int i = 0;
        int tabIdx = 0;
        int page = 1;
        foreach (var settings in LocalSettingsManager.Tabs)
        {
            var tab = settings.CreateTab(__instance);
            if (settings.ShouldCreateButton)
            {
                var button = settings.CreateTabButton(__instance, ref tabIdx, ref yOffset);
                button.GetComponent<TabGroup>().Content = tab;
                button.SetActive(page == currentPage);

                if (!tabButtons.TryGetValue(page, out var list))
                {
                    tabButtons.Add(page, new());
                    list = tabButtons[page];
                }
                list.Add(button);
                yOffset += 0.6f;
                tabIdx++;
                i++;
            }
            if (i > 7)
            {
                i = 0;
                yOffset = 0;
                page++;
            }
        }

        void ChangePage(int increase)
        {
            currentPage += increase;
            if (currentPage > tabButtons.Count)
            {
                currentPage = 1;
            }
            if (currentPage < 1)
            {
                currentPage = tabButtons.Count;
            }
        }

        // Create page buttons
        // TODO: In-game menu has no close button. I tried making one from scratch but failed. Add one later plz
        if (tabButtons.Count <= 1)
        {
            return;
        }

        var nextButton = Object.Instantiate(__instance.BackButton, __instance.transform).GetComponent<PassiveButton>();
        nextButton.transform.localPosition = new Vector3(3.2f, 2.6f, -10);
        nextButton.transform.localScale = new Vector2(0.25f, 0.25f);
        var nextRend = nextButton.GetComponent<SpriteRenderer>();
        nextRend.sprite = MiraAssets.NextButton.LoadAsset();
        nextButton.gameObject.GetComponent<CloseButtonConsoleBehaviour>().DestroyImmediate();

        var nextRollover = nextButton.gameObject.AddComponent<ButtonRolloverHandler>();
        nextRollover.Target = nextRend;
        nextRollover.OverColor = Palette.AcceptedGreen;

        nextButton.OnClick = new Button.ButtonClickedEvent();
        nextButton.OnClick.AddListener(
            (UnityAction)(() =>
            {
                ChangePage(1);
                UpdatePages();
            }));

        var backButton = Object.Instantiate(__instance.BackButton, __instance.transform).GetComponent<PassiveButton>();
        backButton.transform.localPosition = new Vector3(2.9f, 2.6f, -10);
        backButton.transform.localScale = new Vector2(0.25f, 0.25f);
        var backRend = backButton.GetComponent<SpriteRenderer>();
        backRend.sprite = MiraAssets.NextButton.LoadAsset();
        backRend.flipX = true;
        backButton.gameObject.GetComponent<CloseButtonConsoleBehaviour>().DestroyImmediate();

        var backRollover = backButton.gameObject.AddComponent<ButtonRolloverHandler>();
        backRollover.Target = backRend;
        backRollover.OverColor = Palette.AcceptedGreen;

        backButton.OnClick = new Button.ButtonClickedEvent();
        backButton.OnClick.AddListener(
            (UnityAction)(() =>
            {
                ChangePage(-1);
                UpdatePages();
            }));
    }

    /// <summary>
    /// Modifies how opening tabs is handled for the custom ones to work.
    /// </summary>
    [HarmonyPrefix]
    [HarmonyPatch(nameof(OptionsMenuBehaviour.OpenTabGroup))]
    public static bool OpenTabGroupPrefix(OptionsMenuBehaviour __instance, ref int index)
    {
        __instance.Tabs.ToList().ForEach(x => x.Close()); // Close all vanilla tabs
        LocalSettingsManager.Tabs.ForEach(CustomClose); // Close all mod tabs

        // Tabs with index 10 and above are the mod settings tabs
        if (index >= 10)
        {
            CustomOpen(LocalSettingsManager.Tabs[index - 10]);
            return false;
        }

        return true;
    }

    /// <summary>
    /// Makes the custom tabs close when opening the options menu and resets pages.
    /// </summary>
    [HarmonyPostfix]
    [HarmonyPatch(nameof(OptionsMenuBehaviour.Open))]
    public static void OpenPostfix(OptionsMenuBehaviour __instance)
    {
        LocalSettingsManager.Tabs.ForEach(CustomClose);
        currentPage = 1;
        UpdatePages();
    }

    private static void UpdatePages()
    {
        foreach (var pages in tabButtons)
        {
            pages.Value?
                .Where(x => x != null)
                .Do(
                   x => x?.SetActive(
                       pages.Key == currentPage));
        }
    }

    private static void CustomOpen(LocalSettingsTab tab)
    {
        if (tab.TabButton?.Button)
            tab.TabButton!.Button.color = tab.TabAppearance.TabButtonHoverColor;

        if (tab.TabButton?.Rollover)
            tab.TabButton!.Rollover.OutColor = tab.TabAppearance.TabButtonHoverColor;

        if (tab.TabButton?.Content)
            tab.TabButton!.Content.SetActive(true);

        if (background != null)
            background.color = tab.TabAppearance.TabColor;
    }
    private static void CustomClose(LocalSettingsTab tab)
    {
        if (tab.TabButton?.Button)
            tab.TabButton!.Button.color = tab.TabAppearance.TabButtonColor;

        if (tab.TabButton?.Rollover)
            tab.TabButton!.Rollover.OutColor = tab.TabAppearance.TabButtonColor;

        if (tab.TabButton?.Content)
            tab.TabButton!.Content.SetActive(false);

        if (background != null)
            background.color = Color.white;
    }
}
