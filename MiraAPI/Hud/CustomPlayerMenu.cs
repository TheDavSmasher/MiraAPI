using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Il2CppInterop.Runtime.Attributes;
using Reactor.Utilities.Attributes;
using UnityEngine;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace MiraAPI.Hud;

/// <summary>
/// Custom Player Menu using the <see cref="ShapeshifterPanel"/> as a base.
/// </summary>
/// <param name="il2CppPtr">Used by Il2Cpp. Do not use constructor, this is a <see cref="MonoBehaviour"/>.</param>
[RegisterInIl2Cpp]
[SuppressMessage("Design", "CA1051:Do not declare visible instance fields", Justification = "Unity Convention")]
[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1307:Accessible fields should begin with upper-case letter", Justification = "Unity Convention")]
[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "Unity Convention")]
public class CustomPlayerMenu(IntPtr il2CppPtr) : CustomMultiSelectMenu<PlayerControl>(il2CppPtr)
{
    public List<ShapeshifterPanel> potentialVictims;

    /// <summary>
    /// Creates a <see cref="CustomPlayerMenu"/>.
    /// </summary>
    /// <returns>New <see cref="CustomPlayerMenu"/> object.</returns>
    public static CustomPlayerMenu Create()
    {
        return Create<CustomPlayerMenu>();
    }

    /// <summary>
    /// Begins/opens the custom player menu.
    /// </summary>
    /// <param name="playerMatch">Function to determine if player should show in the custom menu.</param>
    /// <param name="onClick"><see cref="PassiveButton.OnClick"/> action for player.</param>
    /// <param name="shouldConfirm">Whether the set of both selections should be confirmed manually.</param>
    [HideFromIl2Cpp]
    public void Begin(Func<PlayerControl, bool> playerMatch, Action<PlayerControl?> onClick, bool shouldConfirm = false)
    {
        Begin(PlayerControl.AllPlayerControls.ToArray().Where(playerMatch), onClick, shouldConfirm);
        potentialVictims = EntryPanels;
    }

    /// <summary>
    /// Begins/opens the custom player menu.
    /// </summary>
    /// <param name="playerMatch">Function to determine if player should show in the custom menu.</param>
    /// <param name="onClick"><see cref="PassiveButton.OnClick"/> action for the two-player selection.</param>
    /// <param name="shouldConfirm">Whether the set of both selections should be confirmed manually.</param>
    /// <param name="canRepeat">If the same entry can be selected both times, else unselect entry on click.</param>
    [HideFromIl2Cpp]
    public void Begin(Func<PlayerControl, bool> playerMatch, Action<PlayerControl?, PlayerControl?> onClick, bool shouldConfirm = false, bool canRepeat = false)
    {
        Begin(PlayerControl.AllPlayerControls.ToArray().Where(playerMatch), onClick, shouldConfirm, canRepeat);
        potentialVictims = EntryPanels;
    }

    /// <inheritdoc/>
    protected override void SetupPanelEntry(ShapeshifterPanel panel, int i, PlayerControl entry, Action onClick)
    {
        var flag = PlayerControl.LocalPlayer.Data.Role.NameColor == entry.Data.Role.NameColor;
        panel.SetPlayer(i, entry.Data, (Il2CppSystem.Action)onClick);
        panel.NameText.color = flag ? entry.Data.Role.NameColor : Color.white;
    }
}
