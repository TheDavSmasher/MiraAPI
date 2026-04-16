using System;
using System.Collections.Generic;
using Reactor.Utilities.Extensions;
using UnityEngine;

namespace MiraAPI.Utilities.Assets;

/// <summary>
/// A utility class for loading assets from an asset bundle.
/// </summary>
/// <param name="name">The name of the asset.</param>
/// <param name="bundle">The AssetBundle that contains the asset.</param>
/// <typeparam name="T">The type of the asset to be loaded.</typeparam>
public class LoadableBundleAsset<T>(string name, AssetBundle bundle) : LoadableAsset<T> where T : UnityEngine.Object
{
    private static readonly object CacheLock = new();
    private static readonly Dictionary<(int BundleId, Type AssetType, string Name), UnityEngine.Object> BundleAssetCache = [];

    /// <summary>
    /// Loads the asset from the asset bundle.
    /// </summary>
    /// <returns>The asset.</returns>
    /// <exception cref="Exception">The asset did not load properly.</exception>
    public override T LoadAsset()
    {
        if (LoadedAsset != null)
        {
            return LoadedAsset;
        }

        var cacheKey = (bundle.GetInstanceID(), typeof(T), name);
        lock (CacheLock)
        {
            if (BundleAssetCache.TryGetValue(cacheKey, out var cachedAsset) && cachedAsset is T typedAsset)
            {
                LoadedAsset = typedAsset;
                return typedAsset;
            }
        }

        LoadedAsset = bundle.LoadAsset<T>(name);

        if (LoadedAsset == null)
        {
            throw new InvalidOperationException($"INVALID ASSET: {name}");
        }

        lock (CacheLock)
        {
            BundleAssetCache[cacheKey] = LoadedAsset;
        }

        return LoadedAsset;
    }
}
