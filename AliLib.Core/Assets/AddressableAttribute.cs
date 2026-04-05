using System;


namespace AliLib.Core.Assets;

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
}
