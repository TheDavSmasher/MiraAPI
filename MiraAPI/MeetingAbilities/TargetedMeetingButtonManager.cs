using System;
using System.Collections.Generic;
using MiraAPI.Modifiers;
using MiraAPI.Modifiers.Types;
using MiraAPI.PluginLoading;
using MiraAPI.Utilities.Assets;
using UnityEngine;

namespace MiraAPI.MeetingAbilities;

/// <summary>
/// Class for managing all registered meeting abilities.
/// </summary>
public static class TargetedMeetingButtonManager
{
    /// <summary>
    /// Gets a list if==of all registered <see cref="TargetedMeetingButton"/>.
    /// </summary>
    public static List<TargetedMeetingButton> Buttons { get; internal set; } = new();

    internal static bool RegisterMeetingAbility(Type type, MiraPluginInfo info)
    {
        if (!typeof(TargetedMeetingButton).IsAssignableFrom(type))
        {
            return false;
        }

        var button = Activator.CreateInstance(type) as TargetedMeetingButton;
        info.InternalMeetingAbilities.Add(button);
        Buttons.Add(button);
        return true;
    }
}
