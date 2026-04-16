using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ThunderRoad;
using UnityEngine;

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
        var properties = new List<PropertyInfo>();

        foreach (var mod in ModManager.loadedMods)
        {
            if (mod?.assemblies == null)
                continue;

            foreach (var assembly in mod.assemblies)
            {
                if (assembly == null)
                    continue;

                Type[] types;

                try
                {
                    types = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    types = ex.Types.Where(t => t != null).ToArray();
                }
                catch
                {
                    continue;
                }

                foreach (var type in types)
                {
                    if (type == null) continue;

                    PropertyInfo[] props;

                    try
                    {
                        props = type.GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                    }
                    catch
                    {
                        continue;
                    }

                    foreach (var prop in props)
                    {
                        if (prop == null) continue;

                        if (!prop.IsDefined(typeof(AddressableAttribute), false))
                            continue;

                        properties.Add(prop);
                    }
                }
            }
        }

        MethodInfo openMethod = typeof(Catalog).GetMethods(BindingFlags.Static | BindingFlags.Public)
            .FirstOrDefault(m =>
                m.Name == "LoadAssetAsync" &&
                m.IsGenericMethodDefinition &&
                m.GetParameters() is { Length: 3 } p &&
                p[0].ParameterType == typeof(string) &&
                p[2].ParameterType == typeof(string)
            );

        if (openMethod == null)
            return;

        MethodInfo createCallbackGeneric = typeof(AddressableLibrary)
            .GetMethod(nameof(CreateTypedCallback), BindingFlags.Static | BindingFlags.NonPublic);

        foreach (PropertyInfo property in properties)
        {
            try
            {
                var attribute = (AddressableAttribute)property.GetCustomAttributes(typeof(AddressableAttribute), false)[0];

                Type assetType = property.PropertyType;

                MethodInfo closedMethod = openMethod.MakeGenericMethod(assetType);
                MethodInfo callbackMethod = createCallbackGeneric.MakeGenericMethod(assetType);

                object callback = callbackMethod.Invoke(null, new object[] { property });

                closedMethod.Invoke(null, new object[]
                {
                    attribute.Address,
                    callback,
                    "AliLib.AddressableLibrary"
                });
            }
            catch
            {
                Debug.LogError("Failed to load addressable asset: " + property.Name);
                continue;
            }
        }
    }

    private static Action<T> CreateTypedCallback<T>(PropertyInfo property) => (T result) => property.SetValue(null, result);
}
