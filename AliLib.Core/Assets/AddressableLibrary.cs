using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ThunderRoad;

namespace AliLib.Core.Assets;

/// <summary>
/// Utility class for Addressable Assets.
/// </summary>
public static class AddressableLibrary
{
    public enum LoadType
    {
        FromCache,
        FromCatalog
    }

    /// <summary> All cached adressables. </summary>
    public static IReadOnlyDictionary<string, UnityEngine.Object> AssetCache => assetCache;

    private static Dictionary<string, UnityEngine.Object> assetCache = new();

    /// <summary> Loads an asset from the cache or loads it asynchronously into the cache. </summary>
    /// <returns> The <see cref="LoadType"/> used to load the asset. </returns>
    public static LoadType LoadCachedAssetAsync<T>(string address, Action<T> callback) where T : UnityEngine.Object
    {
        if (assetCache.TryGetValue(address, out UnityEngine.Object result))
        {
            callback((T)result);
            return LoadType.FromCache;
        }

        Catalog.LoadAssetAsync<T>(address, result =>
        {
            assetCache[address] = result;
            callback(result);
        }, "AliLib.AddressableLibrary");

        return LoadType.FromCatalog;
    }

    /// <summary> Removes an asset from the cache. </summary>
    public static void RemoveCachedAsset(string address) => assetCache.Remove(address);

    /// <summary> Removes all assets from the cache. </summary>
    public static void ClearCachedAssets() => assetCache.Clear();

    /// <summary> Loads assets for all all properties marked with <see cref="AddressableAttribute"/>. </summary>
    public static void LoadAddressableAssetAttributes()
    {
        // This is a lot of LinQ, we could probably optimize it
        List<PropertyInfo> properties = ModManager.loadedMods
            .SelectMany(mod => mod.assemblies)
            .SelectMany(a => a.GetTypes())
            .SelectMany(t => t.GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
            .Where(p => p.IsDefined(typeof(AddressableAttribute), false))
            .ToList();

        foreach (PropertyInfo property in properties)
        {
            AddressableAttribute attribute = (AddressableAttribute)property.GetCustomAttributes(typeof(AddressableAttribute), false)[0];
            Type assetType = property.PropertyType;
            MethodInfo openMethod = typeof(Catalog).GetMethods(BindingFlags.Static | BindingFlags.Public)
                .FirstOrDefault(m =>
                        m.Name == "LoadAssetAsync" &&
                        m.IsGenericMethodDefinition &&
                        m.GetParameters() is { } p &&
                        p.Length == 3 &&
                        p[0].ParameterType == typeof(string) &&
                        p[2].ParameterType == typeof(string)
                );

            MethodInfo closedMethod = openMethod.MakeGenericMethod(assetType);
            MethodInfo createCallbackMethod = typeof(AddressableLibrary).GetMethod(nameof(CreateTypedCallback), BindingFlags.Static | BindingFlags.NonPublic)!.MakeGenericMethod(assetType);
            object callback = createCallbackMethod.Invoke(null, new object[] { property });
            closedMethod.Invoke(null, new object[] { attribute.Address, callback, "AliLib.AddressableLibrary" });
        }
    }

    private static Action<T> CreateTypedCallback<T>(PropertyInfo property) => (T result) => property.SetValue(null, result);
}
