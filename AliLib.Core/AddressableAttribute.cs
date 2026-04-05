using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ThunderRoad;

namespace AliLib.Core;

/// <summary>
/// Marks a static property as an Addressable Asset.
/// </summary>
/// <remarks>
/// Static properties marked with this attribute will asynchronously be loaded when the SDK loads <see cref="ThunderScript"/>s. Due to the asynchronous nature of this,
/// the property should be nullable and should be assumed to be <see langword="null"/>.
/// </remarks>
[AttributeUsage(AttributeTargets.Property)]
public class AddressableAttribute : Attribute
{
    /// <summary> The path to the addressable asset. </summary>
    public string Address { get; }

    /// <summary> Initializes a new instance of the <see cref="AddressableAttribute"/> class. </summary>
    public AddressableAttribute(string address) => Address = address;

    /// <summary> Loads assets for all all properties marked with <see cref="AddressableAttribute"/>. </summary>
    public static void LoadAddressableAssets()
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
            AddressableAttribute attribute = (AddressableAttribute)property.GetCustomAttributes(typeof(AliLib.Core.AddressableAttribute), false)[0];
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
            MethodInfo createCallbackMethod = typeof(AddressableAttribute).GetMethod(nameof(CreateTypedCallback), BindingFlags.Static | BindingFlags.NonPublic)!.MakeGenericMethod(assetType);
            object callback = createCallbackMethod.Invoke(null, new object[] { property });
            closedMethod.Invoke(null, new object[] { attribute.Address, callback, "LoadAddressableAssets" });
        }
    }

    private static Action<T> CreateTypedCallback<T>(PropertyInfo property) => (T result) => property.SetValue(null, result);
}
