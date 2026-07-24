using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Patches.Stubs;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities;
using Reactor.Utilities.Attributes;
using Reactor.Utilities.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
#pragma warning disable S2365 // Properties should not make collection or array copies

namespace MiraAPI.Hud;

/// <summary>
/// Paginable <see cref="CustomPhoneMenu"/> using the <see cref="ShapeshifterPanel"/> as a base.
/// </summary>
/// <param name="il2CppPtr">Used by Il2Cpp. Do not use constructor, this is a <see cref="MonoBehaviour"/>.</param>
[RegisterInIl2Cpp]
[SuppressMessage("Design", "CA1051:Do not declare visible instance fields", Justification = "Unity Convention")]
[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1307:Accessible fields should begin with upper-case letter", Justification = "Unity Convention")]
[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "Unity Convention")]
public abstract class CustomPaginableMenu(IntPtr il2CppPtr) : CustomPhoneMenu<CustomPaginableMenu.MenuEntry>(il2CppPtr)
{
    protected abstract string Name { get; }

    protected int currentPage;

    protected abstract TextBoxTMP? PrefabTextbox { get; }

    private TextBoxTMP? searchTextbox;
    private string searchText = string.Empty;
    private TextMeshPro? noResultsText;

    private const int ItemsPerPage = 15;

    protected override float MenuDepth => -60f;

    public record MenuEntry(ShapeshifterPanel Panel, string SortKey) : IMenuEntry;

    /// <summary>
    /// Creates a <typeparamref name="TMenu"/>.
    /// </summary>
    /// <typeparam name="TMenu">The type of <see cref="CustomPaginableMenu"/>.</typeparam>
    /// <param name="onMouseOut">Function that can optionally be run when the mouse is moved outside a menu panel.</param>
    /// <param name="onMouseOver">Function that can optionally be run when the mouse is moved over a menu panel.</param>
    /// <returns>New <typeparamref name="TMenu"/> object.</returns>
    protected static new TMenu Create<TMenu>(
        PanelButtonOnMouse? onMouseOut = null,
        PanelButtonOnMouse? onMouseOver = null
        ) where TMenu : CustomPaginableMenu
    {
        TMenu customMenu = CustomPhoneMenu.Create<TMenu>(onMouseOut, onMouseOver);

        var nextButton = Instantiate(customMenu.backButton, customMenu.transform).gameObject;
        nextButton.transform.localPosition = new Vector3(1.85f, -2.185f, customMenu.MenuDepth);
        nextButton.transform.localScale = new Vector3(0.65f, 0.65f, 1);
        nextButton.name = "RightArrowButton";
        nextButton.GetComponent<SpriteRenderer>().sprite = MiraAssets.NextButton.LoadAsset();
        nextButton.gameObject.GetComponent<CloseButtonConsoleBehaviour>().DestroyImmediate();

        var passiveButton = nextButton.gameObject.GetComponent<PassiveButton>();
        passiveButton.OnClick = new Button.ButtonClickedEvent();
        passiveButton.OnClick.AddListener((UnityAction)customMenu.NextPage);

        var backButton = Instantiate(nextButton, customMenu.transform).gameObject;
        backButton.transform.localPosition = new Vector3(-1.85f, -2.185f, customMenu.MenuDepth);
        backButton.name = "LeftArrowButton";
        backButton.gameObject.GetComponent<CloseButtonConsoleBehaviour>().Destroy();
        backButton.GetComponent<SpriteRenderer>().flipX = true;
        var prevPassive = backButton.gameObject.GetComponent<PassiveButton>();
        prevPassive.OnClick.AddListener((UnityAction)customMenu.PreviousPage);

        customMenu.PhoneUI.GetChild(0).GetComponent<SpriteRenderer>().material =
            PlayerControl.LocalPlayer.cosmetics.currentBodySprite.BodySprite.material;
        customMenu.PhoneUI.GetChild(1).GetComponent<SpriteRenderer>().material =
            PlayerControl.LocalPlayer.cosmetics.currentBodySprite.BodySprite.material;

        return customMenu;
    }

    private static string NormalizeForSearch(string? text)
    {
        return string.IsNullOrWhiteSpace(text)
            ? string.Empty
            : text.Trim().ToLowerInvariant();
    }

    [HideFromIl2Cpp]
    private List<MenuEntry> GetFilteredEntries()
    {
        var query = NormalizeForSearch(searchText);
        if (string.IsNullOrEmpty(query))
        {
            return MenuEntries;
        }

        return MenuEntries
            .Where(e => e.SortKey.Contains(query, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(e => e.SortKey.Equals(query, StringComparison.OrdinalIgnoreCase))
            .ThenByDescending(e => e.SortKey.StartsWith(query, StringComparison.OrdinalIgnoreCase))
            .ThenByDescending(e => e.SortKey.Contains(query, StringComparison.OrdinalIgnoreCase))
            .ThenBy(e => e.SortKey, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static int GetTotalPages(int itemCount)
    {
        return Mathf.Max(1, Mathf.CeilToInt(itemCount / (float)ItemsPerPage));
    }

    private void RefreshControllerOverlay(Il2CppSystem.Collections.Generic.List<UiElement> list)
    {
        if (ControllerManager.Instance && backButton != null)
        {
            ControllerManager.Instance.OpenOverlayMenu(name, backButton, defaultButtonSelected, list);
        }
    }

    [HideFromIl2Cpp]
    private IEnumerator CoRestoreFocus()
    {
        yield return null;
        searchTextbox?.GiveFocus();
    }

    private void NextPage()
    {
        var filtered = GetFilteredEntries();
        var pages = GetTotalPages(filtered.Count);
        currentPage = (currentPage + 1) % pages;
        var list = ShowPage();
        RefreshControllerOverlay(list);
    }

    private void PreviousPage()
    {
        var filtered = GetFilteredEntries();
        var pages = GetTotalPages(filtered.Count);
        currentPage = (currentPage - 1 + pages) % pages;
        var list = ShowPage();
        RefreshControllerOverlay(list);
    }

    public Il2CppSystem.Collections.Generic.List<UiElement> ShowPage()
    {
        foreach (var entry in MenuEntries)
        {
            entry.Panel.gameObject.SetActive(false);
        }

        var filtered = GetFilteredEntries();
        noResultsText?.gameObject.SetActive(filtered.Count == 0 && !string.IsNullOrWhiteSpace(searchText));
        var totalPages = GetTotalPages(filtered.Count);
        currentPage = Mathf.Clamp(currentPage, 0, totalPages - 1);

        var list = filtered.Skip(currentPage * ItemsPerPage).Take(ItemsPerPage).ToList();
        var list2 = new Il2CppSystem.Collections.Generic.List<UiElement>();

        for (var i = 0; i < list.Count; i++)
        {
            var entry = list[i];
            var num = i % 3;
            var num2 = i / 3 % 5;
            entry.Panel.transform.localPosition =
                new Vector3(xStart + num * xOffset, yStart + num2 * yOffset, -1f);
            entry.Panel.gameObject.SetActive(true);
            list2.Add(entry.Panel.Button);
        }

        return list2;
    }

    [HideFromIl2Cpp]
    protected void EnsureSearchUi()
    {
        if (searchTextbox != null)
        {
            return;
        }

        var gridCenterX = xStart + xOffset;
        var desiredSearchBarCenterY = yStart + 0.55f;

        if (PrefabTextbox == null)
        {
            return;
        }

        var searchRoot = PrefabTextbox.transform.parent != null ? PrefabTextbox.transform.parent.gameObject : PrefabTextbox.gameObject;
        var searchObj = Instantiate(searchRoot, transform);
        searchObj.name = $"{Name}SearchBar";

        foreach (var aspect in searchObj.GetComponentsInChildren<AspectPosition>(true))
        {
            aspect.DestroyImmediate();
        }

        searchTextbox = searchObj.GetComponentInChildren<TextBoxTMP>(true);
        if (searchTextbox == null)
        {
            return;
        }

        try
        {
            var placeholder = searchTextbox.transform.parent.GetChild(2).GetComponent<TextMeshPro>();
            placeholder?.gameObject.SetActive(false);
        }
        catch
        {
            foreach (var tmp in searchObj.GetComponentsInChildren<TextMeshPro>(true))
            {
                if (tmp != searchTextbox.outputText &&
                    (tmp.text.Contains("Search", StringComparison.OrdinalIgnoreCase) ||
                     tmp.text.Contains("Here", StringComparison.OrdinalIgnoreCase)))
                {
                    tmp.gameObject.SetActive(false);
                }
            }
        }

        searchObj.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
        searchObj.transform.localPosition = new Vector3(gridCenterX, desiredSearchBarCenterY, -1f);

        var searchBounds = CalcSpriteBoundsInParentSpace(transform, searchObj);
        var deltaX = gridCenterX - searchBounds.center.x;
        var deltaY = desiredSearchBarCenterY - searchBounds.center.y;
        searchObj.transform.localPosition += new Vector3(deltaX, deltaY, 0f);
        searchBounds = CalcSpriteBoundsInParentSpace(transform, searchObj);

        var wikiClickSound = HudManager.Instance?.MapButton?.ClickSound;

        var searchFocusButton = searchTextbox.gameObject.GetComponent<PassiveButton>()
                             ?? searchTextbox.gameObject.AddComponent<PassiveButton>();
        if (wikiClickSound != null)
        {
            searchFocusButton.ClickSound = wikiClickSound;
        }
        searchFocusButton.OnClick.RemoveAllListeners();
        searchFocusButton.OnClick.AddListener((UnityAction)(Action)(() =>
        {
            searchTextbox.GiveFocus();
        }));

        searchTextbox.SetText(string.Empty);
        searchTextbox.OnChange.RemoveAllListeners();
        searchTextbox.OnChange.AddListener((UnityAction)(Action)(() =>
        {
            searchText = searchTextbox.outputText.text ?? string.Empty;
            currentPage = 0;
            var list = ShowPage();
            RefreshControllerOverlay(list);

            if (searchTextbox != null)
            {
                Coroutines.Start(CoRestoreFocus());
            }
        }));

        var label = Instantiate(HudManager.Instance?.TaskPanel.taskText, transform);
        if (label != null)
        {
            label.name = $"{Name}SearchLabel";
            label.text = "Search";
            label.alignment = TextAlignmentOptions.Center;
            label.fontSize = label.fontSizeMin = label.fontSizeMax = 2.1f;
            label.color = Color.white;
            label.transform.localPosition = new Vector3(gridCenterX, searchBounds.max.y + 0.18f, -1f);

            if (searchTextbox.outputText != null)
            {
                label.font = searchTextbox.outputText.font;
                label.fontMaterial = searchTextbox.outputText.fontMaterial;
            }
        }

        noResultsText = Instantiate(HudManager.Instance?.TaskPanel.taskText, transform);
        if (noResultsText != null)
        {
            noResultsText.name = $"{Name}NoResultsText";
            noResultsText.text = "No results";
            noResultsText.alignment = TextAlignmentOptions.Center;
            noResultsText.fontSize = noResultsText.fontSizeMin = noResultsText.fontSizeMax = 2.25f;
            noResultsText.color = Color.white;
            noResultsText.transform.localPosition = new Vector3(gridCenterX, yStart + 0.1f, -1f);
            noResultsText.gameObject.SetActive(false);
        }

        if (backButton != null && searchTextbox != null)
        {
            var clearButtonX = searchBounds.max.x + 0.2f;
            var clearButtonY = searchBounds.center.y;

            var clearObj = Instantiate(backButton.gameObject, transform);
            clearObj.name = "ClearSearchButton";
            clearObj.transform.localScale = new Vector3(0.35f, 0.35f, 1f);
            clearObj.transform.localPosition = new Vector3(clearButtonX, clearButtonY, -1f);

            clearObj.GetComponent<CloseButtonConsoleBehaviour>()?.DestroyImmediate();
            clearObj.GetComponent<AspectPosition>()?.DestroyImmediate();

            var clearSearchButton = clearObj.GetComponent<PassiveButton>();
            if (clearSearchButton != null)
            {
                if (wikiClickSound != null)
                {
                    clearSearchButton.ClickSound = wikiClickSound;
                }

                clearSearchButton.OnClick.RemoveAllListeners();
                clearSearchButton.OnClick = new Button.ButtonClickedEvent();
                clearSearchButton.OnClick.AddListener((UnityAction)(() =>
                {
                    if (searchTextbox == null)
                    {
                        return;
                    }

                    searchTextbox.SetText(string.Empty);
                    searchText = string.Empty;
                    currentPage = 0;
                    var list = ShowPage();
                    RefreshControllerOverlay(list);
                }));
            }
        }
    }

    [HideFromIl2Cpp]
    private static Bounds CalcSpriteBoundsInParentSpace(Transform parent, GameObject root)
    {
        var first = true;
        var bounds = default(Bounds);

        foreach (var r in root.GetComponentsInChildren<SpriteRenderer>(true))
        {
            if (r == null || r.sprite == null) continue;

            var b = r.bounds;
            var c = b.center;
            var e = b.extents;

            var p1 = parent.InverseTransformPoint(new Vector3(c.x - e.x, c.y - e.y, c.z));
            var p2 = parent.InverseTransformPoint(new Vector3(c.x - e.x, c.y + e.y, c.z));
            var p3 = parent.InverseTransformPoint(new Vector3(c.x + e.x, c.y - e.y, c.z));
            var p4 = parent.InverseTransformPoint(new Vector3(c.x + e.x, c.y + e.y, c.z));

            if (first)
            {
                bounds = new Bounds(p1, Vector3.zero);
                first = false;
            }

            bounds.Encapsulate(p1);
            bounds.Encapsulate(p2);
            bounds.Encapsulate(p3);
            bounds.Encapsulate(p4);
        }

        return bounds;
    }

    /// <summary>
    /// Begins/opens the custom player menu. After registering panels, it will prepare the search, pages, and open the menu.
    /// </summary>
    /// <param name="registerEntryPanels">Function where all panels should be registered.</param>
    [HideFromIl2Cpp]
    protected void Begin(Action registerEntryPanels)
    {
        MinigameStubs.Begin(this, null);

        searchText = string.Empty;
        currentPage = 0;

        registerEntryPanels();

        EnsureSearchUi();

        var list2 = ShowPage();

        ControllerManager.Instance.OpenOverlayMenu(name, backButton, defaultButtonSelected, list2);
    }
}
