using System;

namespace MiraAPI.Colors;

/// <summary>
/// Used to mark a class for <see cref="CustomColor"/> registration.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class RegisterCustomColorsAttribute : Attribute;
