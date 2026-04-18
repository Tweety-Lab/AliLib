using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ThunderRoad;
using UnityEngine;

namespace AliLib.Core.Assets;

/// <summary>
/// Extensions for <see cref="Catalog"/>.
/// </summary>
public static class CatalogExtensions
{
    public enum LoadType
    {
        FromCache,
        FromCatalog
    }

    private static Dictionary<string, UnityEngine.Object> assetCache = new();
    private static Dictionary<string, HashSet<Type>> addressableComponents = new();

    extension(Catalog)
    {
        /// <summary> All cached adressables. </summary>
        public static IReadOnlyDictionary<string, UnityEngine.Object> AssetCache => assetCache;

        /// <summary> All registered Addressable Components. </summary>
        public static IReadOnlyDictionary<string, HashSet<Type>> AddressableComponents => addressableComponents;

        /// <summary> Loads an asset from the cache or loads it asynchronously into the cache. </summary>
        /// <returns> The <see cref="LoadType"/> used to load the asset. </returns>
        public static LoadType LoadCachedAssetAsync<T>(string address, Action<T> callback) where T : UnityEngine.Object
        {
            void ApplyComponents(T asset)
            {
                if (asset is GameObject go && addressableComponents.TryGetValue(address, out var components))
                    foreach (var componentType in components)
                        go.AddComponent(componentType);

                callback(asset);
            }

            if (assetCache.TryGetValue(address, out UnityEngine.Object result))
            {
                ApplyComponents((T)result);
                return LoadType.FromCache;
            }

            Catalog.LoadAssetAsync<T>(address, result =>
            {
                assetCache[address] = result;
                ApplyComponents(result);
            }, "AliLib.CatalogExtensions");

            return LoadType.FromCatalog;
        }

        /// <summary> Removes an asset from the cache. </summary>
        public static void RemoveCachedAsset(string address) => assetCache.Remove(address);

        /// <summary> Removes all assets from the cache. </summary>
        public static void ClearCachedAssets() => assetCache.Clear();

        /// <summary> Loads assets for all all properties marked with <see cref="AddressableComponentAttribute"/>. </summary>
        public static void LoadAddressableComponents()
        {
            var currentName = Assembly.GetExecutingAssembly().GetName().Name;
            ModManager.ModData[] dependantMods = ModManager.loadedMods.Where(m => m?.assemblies != null && m.assemblies.Any(a => a?.GetReferencedAssemblies().Any(r => r.Name == currentName) == true)).ToArray();
            foreach (var mod in dependantMods)
            {
                if (mod?.assemblies == null)
                    continue;

                foreach (var  assembly in mod.assemblies)
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
                        if (type == null)
                            continue;

                        AddressableComponentAttribute? componentAttribute = type.GetCustomAttributes(typeof(AddressableComponentAttribute), false).FirstOrDefault() as AddressableComponentAttribute;
                        if (componentAttribute == null)
                            continue;

                        addressableComponents[componentAttribute.Address] ??= new();
                        addressableComponents[componentAttribute.Address].Add(type);
                    }
                }
            }
        }

        /// <summary> Loads assets for all all properties marked with <see cref="AddressableAttribute"/>. </summary>
        public static void LoadAddressableAssetAttributes()
        {
            var properties = new List<PropertyInfo>();

            var currentName = Assembly.GetExecutingAssembly().GetName().Name;
            ModManager.ModData[] dependantMods = ModManager.loadedMods.Where(m => m?.assemblies != null && m.assemblies.Any(a => a?.GetReferencedAssemblies().Any(r => r.Name == currentName) == true)).ToArray();
            foreach (var mod in dependantMods)
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
                        if (type == null)
                            continue;

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
                            if (prop == null)
                                continue;

                            if (!prop.IsDefined(typeof(AddressableAttribute), false))
                                continue;

                            properties.Add(prop);
                        }
                    }
                }
            }

            MethodInfo openMethod = typeof(CatalogExtensions)
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .FirstOrDefault(m =>
                    m.Name == "LoadCachedAssetAsync" &&
                    m.IsGenericMethodDefinition &&
                    m.GetParameters() is { Length: 2 } p &&
                    p[0].ParameterType == typeof(string)
                );

            if (openMethod == null)
                return;

            MethodInfo createCallbackGeneric = typeof(CatalogExtensions).GetMethod(nameof(CreateTypedCallback), BindingFlags.Static | BindingFlags.NonPublic);

            foreach (PropertyInfo property in properties)
            {
                try
                {
                    var attribute = (AddressableAttribute)property.GetCustomAttributes(typeof(AddressableAttribute), false)[0];

                    Type assetType = property.PropertyType;

                    MethodInfo closedMethod = openMethod.MakeGenericMethod(assetType);
                    MethodInfo callbackMethod = createCallbackGeneric.MakeGenericMethod(assetType);

                    object callback = callbackMethod.Invoke(null, new object[] { property });

                    closedMethod.Invoke(null, new object[] { attribute.Address, callback });
                }
                catch
                {
                    Debug.LogError($"[AliLib] Failed to load addressable asset: {property.Name}");
                }
            }
        }
    }


    private static Action<T> CreateTypedCallback<T>(PropertyInfo property) => (T result) => property.SetValue(null, result);
}
