using System;
using System.Linq;
using ThunderRoad;

namespace AliLib.Core.Portability;

/// <summary>
/// Marks a <see cref="ModOption"/> as PCVR-Only.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property)]
public class ModOptionPCVROnlyAttribute : Attribute
{
    /// <summary> Clears any mod option not matching this attribute. </summary>
    public static void Setup()
    {
        if (Platform.IsPCVR)
            return;

        foreach (var mod in ModManager.loadedMods)
            mod.modOptions.RemoveAll(modOption => modOption.member.CustomAttributes.Any(attr => attr.AttributeType == typeof(ModOptionPCVROnlyAttribute)));
    }
}