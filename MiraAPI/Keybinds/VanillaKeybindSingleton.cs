namespace MiraAPI.Keybinds;

/// <summary>
/// Singleton for getting a <see cref="VanillaKeybind"/> by <see cref="ActionButton"/>.
/// </summary>
/// <typeparam name="T"><see cref="AccountButton"/> type.</typeparam>
public static class VanillaKeybinding<T> where T : ActionButton
{
    private static VanillaKeybind? _instance;

    /// <summary>
    /// Gets the instance of the <see cref="VanillaKeybind"/>.
    /// </summary>
#pragma warning disable CA1000
    public static VanillaKeybind Instance => _instance ??= KeybindManager.VanillaKeybinds[typeof(T)];
#pragma warning restore CA1000
}
