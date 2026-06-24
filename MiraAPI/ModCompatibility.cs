using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using BepInEx.Unity.IL2CPP;

namespace MiraAPI;

/// <summary>
/// Mod compatibility tools.
/// </summary>
public static class ModCompatibility
{
    /// <summary>
    /// The ID for the Submerged mod.
    /// </summary>
    public const string SubmergedId = "Submerged";

    /// <summary>
    /// Checks if the Submerged mod is loaded.
    /// </summary>
    /// <param name="submergedAssembly">Submerged mod assembly if loaded, <see langword="null"/> otherwise.</param>
    /// <returns><see langword="true"/> if the Submerged mod is loaded, <see langword="false"/> otherwise.</returns>
    public static bool SubmergedLoaded([NotNullWhen(true)] out Assembly? submergedAssembly)
    {
        var result = IL2CPPChainloader.Instance.Plugins.TryGetValue(SubmergedId, out var plugin);
        submergedAssembly = result ? plugin?.Instance.GetType().Assembly : null;
        return result;
    }
}
