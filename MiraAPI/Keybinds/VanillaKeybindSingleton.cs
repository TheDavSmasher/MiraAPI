namespace MiraAPI.Keybinds;

/// <summary>
/// Singleton for getting a vanilla keybind by <see cref="ActionButton"/>.
/// </summary>
/// <typeparam name="T">ActionButton type.</typeparam>
public static class VanillaKeybinding<T> where T : ActionButton
{
    private static VanillaKeybind? _instance;

    /// <summary>
    /// Gets the instance of the option group.
    /// </summary>
#pragma warning disable CA1000
    public static VanillaKeybind Instance => _instance ??= KeybindManager.VanillaKeybinds[typeof(T)];
#pragma warning restore CA1000
}
