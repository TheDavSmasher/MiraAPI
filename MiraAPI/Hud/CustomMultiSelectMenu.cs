using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Patches.Stubs;
using MiraAPI.Utilities.Assets;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
#pragma warning disable S2365 // Properties should not make collection or array copies

namespace MiraAPI.Hud;

/// <summary>
/// Multi-select <see cref="CustomPhoneMenu"/> using the <see cref="ShapeshifterPanel"/> as a base.
/// </summary>
/// <param name="il2CppPtr">Used by Il2Cpp. Do not use constructor, this is a <see cref="MonoBehaviour"/>.</param>
/// <typeparam name="TEntry">The type of object each entry represents.</typeparam>
[SuppressMessage("Design", "CA1051:Do not declare visible instance fields", Justification = "Unity Convention")]
[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1307:Accessible fields should begin with upper-case letter", Justification = "Unity Convention")]
[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "Unity Convention")]
public abstract class CustomMultiSelectMenu<TEntry>(IntPtr il2CppPtr)
    : CustomPhoneMenu<CustomMultiSelectMenu<TEntry>.MenuEntry>(il2CppPtr) where TEntry : class
{
    private int totalSelections;
    private bool shouldConfirm;
    private bool canRepeat;
    private readonly List<MenuEntry> selectedEntries = [];

    private LoadableAsset<Sprite>? hoverSelectSprite;
    private LoadableAsset<Sprite>? hoverDeselectSprite;
    private Color? activeColor;
    private Color? hoverSelectColor;
    private Color? hoverDeselectColor;

    private Action<List<TEntry>>? onSelection;

    public record MenuEntry(ShapeshifterPanel Panel, TEntry Entry) : IMenuEntry;

    protected static TMenu Create<TMenu>(
        Color? activeColor = null,
        LoadableAsset<Sprite>? hoverSelectSprite = null,
        Color? hoverSelectColor = null,
        LoadableAsset<Sprite>? hoverDeselectSprite = null,
        Color? hoverDeselectColor = null,
        PanelButtonOnMouse? onMouseOut = null,
        PanelButtonOnMouse? onMouseOver = null
        ) where TMenu : CustomMultiSelectMenu<TEntry>
    {
        TMenu customMenu = Create<TMenu>(onMouseOut, onMouseOver);

        // TODO: create/add confirm button with click listener

        customMenu.activeColor = activeColor;

        customMenu.hoverSelectSprite = hoverSelectSprite;
        customMenu.hoverSelectColor = hoverSelectColor;

        customMenu.hoverDeselectSprite = hoverDeselectSprite ?? hoverSelectSprite;
        customMenu.hoverDeselectColor = hoverDeselectColor ?? hoverSelectColor;

        return customMenu;
    }

    protected abstract void SetupPanelEntry(ShapeshifterPanel panel, int i, TEntry entry, Action onClick);

    /// <summary>
    /// Begins/opens the custom multi-select menu.
    /// </summary>
    /// <param name="entries">All entries to give the custom menu.</param>
    /// <param name="onClick">Function called when all selections are made/confirmed.</param>
    /// <param name="totalSelections">The number of selections required.</param>
    /// <param name="shouldConfirm">Wheter the entire selection should be confirmed manually.</param>
    /// <param name="canRepeat">If the same entry can be selected multiple times, else unselect entry on click.</param>
    [HideFromIl2Cpp]
    protected void Begin(IEnumerable<TEntry> entries, Action<List<TEntry>?> onClick, int totalSelections, bool shouldConfirm, bool canRepeat = false)
    {
        MinigameStubs.Begin(this, null);

        this.totalSelections = totalSelections;
        this.shouldConfirm = shouldConfirm;
        this.canRepeat = canRepeat;
        this.onSelection = onClick;

        var back = backButton.GetComponent<PassiveButton>();
        back.OnClick.AddListener((UnityAction)(() =>
        {
            selectedEntries.Clear();
            onClick(null);
        }));

        DebugAnalytics.Instance.Analytics.MinigameOpened(PlayerControl.LocalPlayer.Data, TaskType);
        var list2 = new Il2CppSystem.Collections.Generic.List<UiElement>();
        RegisterPanels(
            entries,
            (shapeshifterPanel, i, entry) =>
            {
                var num = i % 3;
                var num2 = i / 3;
                shapeshifterPanel.transform.localPosition = new Vector3(xStart + num * xOffset, yStart + num2 * yOffset, -1f);
                SetupPanelEntry(shapeshifterPanel, i, entry, () => OnEntryClick(entry));
                list2.Add(shapeshifterPanel.Button);
            },
            (p, e) => new MenuEntry(p, e));
        ControllerManager.Instance.OpenOverlayMenu(name, backButton, defaultButtonSelected, list2);
    }

    /// <summary>
    /// Begins/opens the custom menu, only requiring a single selection.
    /// </summary>
    /// <param name="entries">All entries to give the custom menu.</param>
    /// <param name="onClick">Function called when the selection is made.</param>
    /// <param name="shouldConfirm">Wheter the set of both selections should be confirmed manually.</param>
    [HideFromIl2Cpp]
    protected void Begin(IEnumerable<TEntry> entries, Action<TEntry?> onClick, bool shouldConfirm)
    {
        Begin(entries, list => onClick(list?[0]), 1, shouldConfirm);
    }

    /// <summary>
    /// Begins/opens the custom menu, requiring two selections.
    /// </summary>
    /// <param name="entries">All entries to give the custom menu.</param>
    /// <param name="onClick">Function called when both selections are made.</param>
    /// <param name="shouldConfirm">Wheter the set of both selections should be confirmed manually.</param>
    /// <param name="canRepeat">If the same entry can be selected both times, else unselect entry on click.</param>
    [HideFromIl2Cpp]
    protected void Begin(IEnumerable<TEntry> entries, Action<TEntry?, TEntry?> onClick, bool shouldConfirm, bool canRepeat = false)
    {
        Begin(entries, list => onClick(list?[0], list?[1]), 2, shouldConfirm, canRepeat);
    }

    private void OnEntryClick(TEntry entry)
    {
        MenuEntry menuEntry = MenuEntries.First(e => e.Entry == entry);

        if (!canRepeat && selectedEntries.Remove(menuEntry)) // Unselect previous choice
        {
            // TODO: if confirm button is enabled, disable it
            SetNameplateAppearance(menuEntry, false);
            return;
        }

        selectedEntries.Add(menuEntry);
        if (selectedEntries.Count < totalSelections) // Add choice to list of selections
        {
            SetNameplateAppearance(menuEntry, true);
            return;
        }

        // Total selections reached
        if (shouldConfirm)
        {
            // TODO: enable confirm button with click listener to call onClick
            return;
        }

        onSelection!(selectedEntries.Select(se => se.Entry).ToList());
        selectedEntries.Clear();
    }

    protected override bool IsEntrySelected(IMenuEntry entry)
    {
        return selectedEntries.Any(e => e.Panel == entry.Panel);
    }

    [HideFromIl2Cpp]
    protected void SetNameplateAppearance(IMenuEntry menuEntry, bool isSelected)
    {
        SetNameplateAppearance(
            menuEntry,
            isSelected ? hoverDeselectSprite : hoverSelectSprite,
            isSelected ? hoverDeselectColor : hoverSelectColor,
            isSelected ? activeColor : Color.clear);
    }
}
