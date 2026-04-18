using System;

namespace AliLib.Core.Assets;

/// <summary>
/// Links a <see cref="UnityEngine.MonoBehaviour"/> to an addressable prefab.
/// </summary>
/// <remarks>
/// When a <see cref="UnityEngine.MonoBehaviour"/> is marked with this attribute, it will automatically be added to the addressable when it's loaded.
/// </remarks>
[AttributeUsage(AttributeTargets.Class)]
public class AddressableComponentAttribute : Attribute
{
    /// <summary> The path to the addressable asset. </summary>
    public string Address { get; }

    /// <summary> Initializes a new instance of the <see cref="AddressableComponentAttribute"/> class. </summary>
    public AddressableComponentAttribute(string address) => Address = address;
}
