using System.Globalization;
using Rewired;

namespace MiraAPI.Keybinds;

/// <summary>
/// Represents a registered keybind.
/// </summary>
public class MiraKeybind : BaseKeybind
{
    /// <summary>
    /// Gets name of the keybind
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the default keycode of this keybind.
    /// </summary>
    public KeyboardKeyCode DefaultKey { get; }

    /// <summary>
    /// Gets the modifier keys assigned to this keybind.
    /// Due to Rewired limitations, there can only be 3 modifier keys.
    /// </summary>
    public ModifierKey[] ModifierKeys { get; }

    /// <summary>
    /// Gets a value indicating whether this keybind should be checked for conflicts with other exclusive keybinds using the same key.
    /// </summary>
    public bool Exclusive { get; }

    internal string? SourcePluginName { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MiraKeybind"/> class.
    /// </summary>
    /// <param name="name">The name of the keybind.</param>
    /// <param name="defaultKeycode">The default keycode.</param>
    /// <param name="modifierKeys">Up to 3 optional modifier keys.</param>
    /// <param name="exclusive">Is exclusive.</param>
    public MiraKeybind(
        string name,
        KeyboardKeyCode? defaultKeycode,
        ModifierKey[]? modifierKeys = null,
        bool exclusive = true) : base(name.ToLower(CultureInfo.InvariantCulture).Replace(' ', '_'))
    {
        Name = name;
        DefaultKey = defaultKeycode ?? KeyboardKeyCode.None;
        ModifierKeys = modifierKeys ?? [];
        Exclusive = exclusive;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{Name} ({Id})";
    }
}
