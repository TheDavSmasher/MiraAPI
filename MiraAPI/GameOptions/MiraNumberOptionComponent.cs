using System;
using MiraAPI.GameOptions.OptionTypes;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace MiraAPI.GameOptions;

/// <summary>
/// Component added onto options generated via MiraAPI. This allows developers to hook patches onto options in general, without using a list.
/// </summary>
[RegisterInIl2Cpp]
public class MiraNumberOptionComponent(IntPtr cppPtr) : MonoBehaviour(cppPtr)
{
    /// <summary>
    /// Gets or sets the modded option associated with the object.
    /// </summary>
    public ModdedNumberOption NumberOption { get; set; }

    /// <summary>
    /// Gets or sets the default increment.
    /// </summary>
    public float DefaultIncrement { get; set; }

    /// <summary>
    /// Gets or sets the value of whether or not holding shift will split the increment in half.
    /// </summary>
    public bool ShiftIncrementToggle { get; set; }
}
