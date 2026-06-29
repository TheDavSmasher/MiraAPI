using System.Linq;

namespace MiraAPI.GameOptions;

/// <summary>
/// Singleton for <see cref="AbstractOptionGroup"/>s.
/// </summary>
/// <typeparam name="T">The option group type.</typeparam>
public static class OptionGroupSingleton<T> where T : AbstractOptionGroup
{
    private static T? _instance;

    /// <summary>
    /// Gets the instance of the <typeparamref name="T"/> group.
    /// </summary>
#pragma warning disable CA1000
    public static T Instance => _instance ??= ModdedOptionsManager.Groups.OfType<T>().Single();
#pragma warning restore CA1000
}
