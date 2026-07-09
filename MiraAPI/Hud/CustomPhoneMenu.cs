using AmongUs.GameOptions;
using Reactor.Utilities.Extensions;
using System;
using System.Diagnostics.CodeAnalysis;
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
    public float xStart = -0.8f;
    public float yStart = 2.15f;
    public float xOffset = 1.95f;
    public float yOffset = -0.65f;

    public ShapeshifterPanel panelPrefab;
    public UiElement backButton;
    public UiElement defaultButtonSelected;

    /// <summary>
    /// Creates a <typeparamref name="TMenu"/>.
    /// </summary>
    /// <typeparam name="TMenu">The type of <see cref="CustomPhoneMenu"/>.</typeparam>
    /// <returns>New <typeparamref name="TMenu"/> object.</returns>
    protected static TMenu Create<TMenu>() where TMenu : CustomPhoneMenu
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
        customMenu.transform.localPosition = new Vector3(0f, 0f, -50f);
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
}
