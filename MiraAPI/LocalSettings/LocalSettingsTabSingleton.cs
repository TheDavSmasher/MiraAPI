using System.Linq;

namespace MiraAPI.LocalSettings;

/// <summary>
/// Singleton for <see cref="LocalSettingsTab"/>s.
/// </summary>
/// <typeparam name="T">The settings tab type.</typeparam>
public static class LocalSettingsTabSingleton<T> where T : LocalSettingsTab
{
    private static T? _instance;

    /// <summary>
    /// Gets the instance of the <typeparamref name="T"/> setting tab.
    /// </summary>
#pragma warning disable CA1000
    public static T Instance => _instance ??= LocalSettingsManager.Tabs.OfType<T>().Single();
#pragma warning restore CA1000
}
