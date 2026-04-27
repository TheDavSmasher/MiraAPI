using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Patches.Menu;
using Reactor.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace MiraAPI.Utilities.Assets;

/// <summary>
/// The component for loading Addressables, and cosmetics. This is deprecated in favor of Corsac Cosmetics.
/// </summary>
public static class AddressablesLoader
{
    /// <summary>
    /// Registers a specific addressables package to load asynchronously at the start of the game, when possible.
    /// </summary>
    /// <param name="location">The location, remote or otherwise, of the addressables.</param>
    /// <param name="providerSuffix">The suffix of the provider for an addressables package.</param>
    public static void RegisterCatalog(string location, string providerSuffix = "")
    {
        Error("Catalogs are no longer possible to load in MiraAPI! Please use Corsac Cosmetics instead.");
    }

    /// <summary>
    /// Registers a specific addressables key as only containing <see cref="HatData"/>'s to load.
    /// </summary>
    /// <param name="addressablesKey">The key/label/group for a List <see cref="HatData"/>.</param>
    public static void RegisterHats(string addressablesKey)
    {
        Error("Hats are no longer possible to load in MiraAPI! Please use Corsac Cosmetics instead.");
    }

    /// <summary>
    /// Registers a specific addressables key as only containing <see cref="SkinData"/>'s to load.
    /// </summary>
    /// <param name="addressablesKey">The key/label/group for a List <see cref="SkinData"/>.</param>
    public static void RegisterSkins(string addressablesKey)
    {
        Error("Skins are no longer possible to load in MiraAPI! Please use Corsac Cosmetics instead.");
    }

    /// <summary>
    /// Registers a specific addressables key as only containing <see cref="NamePlateData"/>'s to load.
    /// </summary>
    /// <param name="addressablesKey">The key/label/group for a List <see cref="NamePlateData"/>.</param>
    /// /// <param name="groupTitle">The title of the group for visors.</param>
    public static void RegisterNameplates(string addressablesKey, string groupTitle)
    {
        Error("Nameplates are no longer possible to load in MiraAPI! Please use Corsac Cosmetics instead.");
    }

    /// <summary>
    /// Registers a specific addressables key as only containing <see cref="VisorData"/>'s to load.
    /// </summary>
    /// <param name="addressablesKey">The key/label/group for a List <see cref="VisorData"/>.</param>
    /// <param name="groupTitle">The title of the group for visors.</param>
    public static void RegisterVisors(string addressablesKey, string groupTitle)
    {
        Error("Visors are no longer possible to load in MiraAPI! Please use Corsac Cosmetics instead.");
    }
}
