using System;
using BepInEx.Configuration;

namespace MiraAPI.LocalSettings.Attributes;

/// <summary>
/// Attribute for local setting buttons.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class LocalSettingsButtonAttribute : Attribute
{
}
