using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities.Extensions;
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
/// Defines an entry in a <see cref="CustomPhoneMenu"/> with a Panel.
/// </summary>
public interface IMenuEntry
{
    /// <summary>
    /// Gets the base Panel of the menu entry.
    /// </summary>
    ShapeshifterPanel Panel { get; }
}

/// <summary>
/// Defines a custom menu with a list of <see cref="ICustomMenu"/> entries.
/// </summary>
public interface ICustomMenu
{
    /// <summary>
    /// Gets all registered <see cref="IMenuEntry"/>s.
    /// </summary>
    List<IMenuEntry> MenuEntries { get; }
}

/// <summary>
/// Defines a custom menu with a list of only <typeparamref name="TMenu"/> entries.
/// <para/>
/// Only implement this if you want to define a single new type of menu entries explicitly.
/// Must reference the member of the <see cref="CustomPhoneMenu"/> superclass that is being hidden by this one to work.
/// </summary>
/// <typeparam name="TMenu">The type of menu entry.</typeparam>
public interface ICustomMenu<TMenu> : ICustomMenu where TMenu : IMenuEntry
{
    /// <summary>
    /// Gets all registered <typeparamref name="TMenu"/>s.
    /// </summary>
    new List<TMenu> MenuEntries { get; }
}

/// <summary>
/// Custom Phone Menu using the <see cref="ShapeshifterPanel"/> as a base.
/// </summary>
/// <param name="il2CppPtr">Used by Il2Cpp. Do not use constructor, this is a <see cref="MonoBehaviour"/>.</param>
[SuppressMessage("Design", "CA1051:Do not declare visible instance fields", Justification = "Unity Convention")]
[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1307:Accessible fields should begin with upper-case letter", Justification = "Unity Convention")]
[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "Unity Convention")]
public abstract class CustomPhoneMenu(IntPtr il2CppPtr) : Minigame(il2CppPtr), ICustomMenu
{
    /// <summary>
    /// Menu Entry used when specifically only the Panel itself is required.
    /// </summary>
    /// <param name="Panel">The <see cref="ShapeshifterPanel"/> instance.</param>
    protected sealed record class BasicEntry(ShapeshifterPanel Panel) : IMenuEntry
    {
        public static implicit operator ShapeshifterPanel(BasicEntry entry) => entry.Panel;

        public static implicit operator BasicEntry(ShapeshifterPanel panel) => new(panel);
    }

    public List<IMenuEntry> MenuEntries { get; protected set; } = [];

    public List<ShapeshifterPanel> EntryPanels => MenuEntries.Select(e => e.Panel).ToList();

    public float xStart = -0.8f;
    public float yStart = 2.15f;
    public float xOffset = 1.95f;
    public float yOffset = -0.65f;

    public ShapeshifterPanel panelPrefab;
    public UiElement backButton;
    public UiElement defaultButtonSelected;

    public Transform PhoneUI => transform.FindChild("PhoneUI");

    public delegate void PanelButtonOnMouse(SpriteRenderer highlight, SpriteRenderer icon, bool isSelected);

    protected PanelButtonOnMouse? onMouseOverAction;
    protected PanelButtonOnMouse? onMouseOutAction;

    protected virtual float MenuDepth => -50f;

    /// <summary>
    /// Creates a <typeparamref name="TMenu"/>.
    /// </summary>
    /// <typeparam name="TMenu">The type of <see cref="CustomPhoneMenu"/>.</typeparam>
    /// <param name="onMouseOut">Function that can optionally be run when the mouse is moved outside a menu panel.</param>
    /// <param name="onMouseOver">Function that can optionally be run when the mouse is moved over a menu panel.</param>
    /// <returns>New <typeparamref name="TMenu"/> object.</returns>
    protected static TMenu Create<TMenu>(
        PanelButtonOnMouse? onMouseOut = null,
        PanelButtonOnMouse? onMouseOver = null
        ) where TMenu : CustomPhoneMenu
    {
        var shapeShifterRole = RoleManager.Instance.GetRole(RoleTypes.Shapeshifter);

        var ogMenu = shapeShifterRole.TryCast<ShapeshifterRole>()!.ShapeshifterMenu;
        var newMenu = Instantiate(ogMenu);
        var customMenu = newMenu.gameObject.AddComponent<TMenu>();

        customMenu.panelPrefab = newMenu.PanelPrefab;
        customMenu.xStart = newMenu.XStart;
        customMenu.yStart = newMenu.YStart;
        customMenu.xOffset = newMenu.XOffset;
        customMenu.yOffset = newMenu.YOffset;
        customMenu.defaultButtonSelected = newMenu.DefaultButtonSelected;
        customMenu.backButton = newMenu.BackButton;

        var back = customMenu.backButton.GetComponent<PassiveButton>();
        back.OnClick.RemoveAllListeners();
        back.OnClick.AddListener((UnityAction)customMenu.Close);

        customMenu.CloseSound = newMenu.CloseSound;
        customMenu.logger = newMenu.logger;
        customMenu.OpenSound = newMenu.OpenSound;

        newMenu.DestroyImmediate();

        customMenu.transform.SetParent(Camera.main!.transform, false);
        customMenu.transform.localPosition = new Vector3(0f, 0f, customMenu.MenuDepth);

        customMenu.onMouseOverAction = onMouseOver;
        customMenu.onMouseOutAction = onMouseOut;

        return customMenu;
    }

    private void OnDisable()
    {
        ControllerManager.Instance.CloseOverlayMenu(name);
    }

#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member
    [Obsolete("Will always fail. Call or define another Begin method that calls MinigameStubs.Begin", true)]
    [SuppressMessage("Info Code Smell", "S1133:Deprecated code should be removed", Justification = "Overrides an unusable method.")]
    public override sealed void Begin(PlayerTask task)
    {
        throw new NotImplementedException("Use the other Begin method.");
    }
#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member

    protected void RegisterPanels<TEntry>(
        IEnumerable<TEntry> entries,
        Action<ShapeshifterPanel, int, TEntry> entryPanelConfig,
        Func<ShapeshifterPanel, TEntry, IMenuEntry>? menuEntryMaker = null)
    {
        int currentEntries = MenuEntries.Count;

        var list = entries.ToList();

        for (var i = 0; i < list.Count; i++)
        {
            var index = currentEntries + i;
            var entry = list[i];

            var shapeshifterPanel = Instantiate(panelPrefab, transform);
            shapeshifterPanel.transform.localPosition = new Vector3(0f, 0f, -1f);
            entryPanelConfig(shapeshifterPanel, index, entry);

            menuEntryMaker ??= (s, _) => new BasicEntry(s);
            var menuEntry = menuEntryMaker(shapeshifterPanel, entry);
            MenuEntries.Add(menuEntry);

            var button = shapeshifterPanel.Button;
            var nameplate = shapeshifterPanel.gameObject.transform.FindChild("Nameplate");
            var highlight = nameplate.FindChild("Highlight").GetComponent<SpriteRenderer>();
            var icon = highlight.transform.GetChild(0).GetComponent<SpriteRenderer>();

            if (onMouseOverAction != null)
            {
                button.OnMouseOver.RemoveAllListeners();
                button.OnMouseOver = new UnityEvent();
                button.OnMouseOver.AddListener((UnityAction)
                    (() => onMouseOverAction(highlight, icon, IsEntrySelected(menuEntry))));
            }
            if (onMouseOutAction != null)
            {
                button.OnMouseOut.RemoveAllListeners();
                button.OnMouseOut = new UnityEvent();
                button.OnMouseOut.AddListener((UnityAction)
                    (() => onMouseOutAction(highlight, icon, IsEntrySelected(menuEntry))));
            }
        }
    }

    protected void RegisterPanels<TEntry>(
        IEnumerable<TEntry> entries,
        Action<TEntry?> onEntryClick,
        Action<ShapeshifterPanel, int, TEntry, Action> entryPanelConfig,
        Func<ShapeshifterPanel, TEntry, IMenuEntry>? menuEntryMaker = null)
    {
        RegisterPanels(entries, (p, i, e) => entryPanelConfig(p, i, e, () => onEntryClick(e)), menuEntryMaker);
    }

    protected virtual bool IsEntrySelected(IMenuEntry entry) => false;

    [HideFromIl2Cpp]
    protected static void SetNameplateAppearance(
        IMenuEntry menuEntry, LoadableAsset<Sprite>? sprite, Color? overColor, Color? unselectedColor)
    {
        ShapeshifterPanel panel = menuEntry.Panel;

        var nameplate = panel.gameObject.transform.FindChild("Nameplate");
        var highlight = nameplate.FindChild("Highlight").GetComponent<SpriteRenderer>();
        var icon = highlight.transform.GetChild(0).GetComponent<SpriteRenderer>();
        if (sprite != null)
        {
            icon.sprite = sprite.LoadAsset();
        }
        var button = nameplate.GetComponent<ButtonRolloverHandler>();
        if (overColor is { } oColor)
        {
            button.OverColor = oColor;
        }
        if (unselectedColor is { } uColor)
        {
            button.UnselectedColor = uColor;
        }
    }
}

/// <inheritdoc cref="CustomPhoneMenu(IntPtr)"/>
/// <typeparam name="TMenu">The type of menu entries.</typeparam>
public abstract class CustomPhoneMenu<TMenu>(IntPtr il2CppPtr) : CustomPhoneMenu(il2CppPtr), ICustomMenu<TMenu> where TMenu : IMenuEntry
{
    public new List<TMenu> MenuEntries
    {
        get => base.MenuEntries.Cast<TMenu>().ToList();
        protected set => base.MenuEntries = value.Cast<IMenuEntry>().ToList();
    }
}
