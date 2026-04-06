using AliLib.Core.Assets;
using AliLib.Core.Portability;
using System;
using System.Linq;
using System.Reflection;
using ThunderRoad;
using UnityEngine;
using static ThunderRoad.BrainModuleStance;

namespace AliLib.Core;

/// <summary>
/// The Main <see cref="ThunderScript"/> responsible for management of AliLib systems.
/// </summary>
internal class CoreScript : ThunderScript
{
    /// <inheritdoc/>
    public override void ScriptEnable()
    {
        base.ScriptEnable();

        AddressableLibrary.LoadAddressableAssetAttributes();

        ModOptionNomadOnlyAttribute.Setup();
        ModOptionPCVROnlyAttribute.Setup();

        // Setup default mod options
        // TODO: Move this to a method. I'd love to have this in a ModOptionsExtension but its a static method
        foreach (var mod in ModManager.loadedMods)
        {
            foreach (var modOption in mod.modOptions)
            {
                // Hacky
                if (modOption.parameterValues == null)
                    modOption.LoadModOptionParameters();

                object? value = GetModOptionValue(modOption);
                if (value == null)
                    continue;

                int index = Array.FindIndex(modOption.parameterValues, p => p.value.Equals(value));

                if (index == -1)
                {
                    Debug.LogWarning($"[AliLib] Value {value} not found in ModOption: {modOption.name}");
                    return;
                }

                modOption.Apply(index);
            }
        }

    }

    private static object? GetModOptionValue(ModOption option)
    {
        return option.member switch
        {
            FieldInfo field => field.GetValue(null),
            PropertyInfo property => property.GetValue(null),
            MethodInfo method => method.Invoke(null, null),
            _ => throw new NotSupportedException()
        };
    }
}
