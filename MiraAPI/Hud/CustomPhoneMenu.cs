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

namespace MiraAPI.Hud;

/// <summary>
/// Custom Phone Menu using the <see cref="ShapeshifterPanel"/> as a base.
/// </summary>
/// <param name="il2CppPtr">Used by Il2Cpp. Do not use constructor, this is a <see cref="MonoBehaviour"/>.</param>
[SuppressMessage("Design", "CA1051:Do not declare visible instance fields", Justification = "Unity Convention")]
[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1307:Accessible fields should begin with upper-case letter", Justification = "Unity Convention")]
[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "Unity Convention")]
public abstract class CustomPhoneMenu(IntPtr il2CppPtr) : Minigame(il2CppPtr)
{
    public interface IMenuEntry
    {
        ShapeshifterPanel Panel { get; }
    }

    /// <summary>
    /// Menu Entry used when specifically only the Panel itself is required.
    /// </summary>
    /// <param name="Panel">The <see cref="ShapeshifterPanel"/> instance.</param>
    protected record class BasicEntry(ShapeshifterPanel Panel) : IMenuEntry
    {
        public static implicit operator ShapeshifterPanel(BasicEntry entry) => entry.Panel;

        public static implicit operator BasicEntry(ShapeshifterPanel panel) => new(panel);
    }

    public List<IMenuEntry> menuEntries;

    public float xStart = -0.8f;
    public float yStart = 2.15f;
    public float xOffset = 1.95f;
    public float yOffset = -0.65f;

    public ShapeshifterPanel panelPrefab;
    public UiElement backButton;
    public UiElement defaultButtonSelected;

    // These are the Highlight, Icon, and IsSelected variable respectively.
    protected Action<SpriteRenderer, SpriteRenderer, bool>? onMouseOverAction;
    protected Action<SpriteRenderer, SpriteRenderer, bool>? onMouseOutAction;

    protected virtual float MenuDepth => -50f;

    /// <summary>
    /// Creates a <typeparamref name="TMenu"/>.
    /// </summary>
    /// <typeparam name="TMenu">The type of <see cref="CustomPhoneMenu"/>.</typeparam>
    /// <param name="onMouseOut">Function that can optionally be run when the mouse is moved outside a menu panel.</param>
    /// <param name="onMouseOver">Function that can optionally be run when the mouse is moved over a menu panel.</param>
    /// <returns>New <typeparamref name="TMenu"/> object.</returns>
    protected static TMenu Create<TMenu>(
        Action<SpriteRenderer, SpriteRenderer, bool>? onMouseOut = null,
        Action<SpriteRenderer, SpriteRenderer, bool>? onMouseOver = null
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

    /// <inheritdoc />
    public override sealed void Begin(PlayerTask task)
    {
        throw new NotImplementedException("Use the other Begin method.");
    }

    protected void RegisterPanels<TEntry>(
        IEnumerable<TEntry> entries,
        Action<ShapeshifterPanel, int, TEntry> entryPanelConfig,
        Func<ShapeshifterPanel, TEntry, IMenuEntry>? menuEntryMaker = null)
    {
        menuEntries ??= [];

        int currentEntries = menuEntries.Count;

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
            menuEntries.Add(menuEntry);

            var nameplate = shapeshifterPanel.gameObject.transform.FindChild("Nameplate");
            var highlight = nameplate.FindChild("Highlight").GetComponent<SpriteRenderer>();
            var icon = highlight.transform.GetChild(0).GetComponent<SpriteRenderer>();

            if (onMouseOverAction != null)
            {
                shapeshifterPanel.Button.OnMouseOver.RemoveAllListeners();
                shapeshifterPanel.Button.OnMouseOver = new UnityEvent();
                shapeshifterPanel.Button.OnMouseOver.AddListener((UnityAction)
                    (() => onMouseOverAction(highlight, icon, IsEntrySelected(menuEntry))));
            }
            if (onMouseOutAction != null)
            {
                shapeshifterPanel.Button.OnMouseOut.RemoveAllListeners();
                shapeshifterPanel.Button.OnMouseOut = new UnityEvent();
                shapeshifterPanel.Button.OnMouseOut.AddListener((UnityAction)
                    (() => onMouseOutAction(highlight, icon, IsEntrySelected(menuEntry))));
            }
        }
    }

    protected void RegisterPanels<TEntry>(
        IEnumerable<TEntry> entries,
        Action<TEntry?> onEntryClick,
        Action<ShapeshifterPanel, int, TEntry, Action> entryPanelConfig,
        Func<ShapeshifterPanel, TEntry, IMenuEntry>? menuEntryConfig = null)
    {
        RegisterPanels(entries, (p, i, e) => entryPanelConfig(p, i, e, () => onEntryClick(e)), menuEntryConfig);
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
