using AliLib.Core.Assets;
using AliLib.Core.Portability;
using System;
using System.Linq;
using ThunderRoad;

namespace AliLib.Core;

/// <summary>
/// The Main <see cref="ThunderScript"/> responsible for management of AliLib systems.
/// </summary>
internal class CoreScript : ThunderScript
{
    public static bool Initialized { get; private set; } = false;

    /// <inheritdoc/>
    public override void ScriptEnable()
    {
        base.ScriptEnable();

        // Multiple AliLibs can link to multiple mods but we only want to init once
        if (ModManager.loadedMods.SelectMany(mod => mod.assemblies).SelectMany(a => a.GetTypes()).Count(t => t == typeof(CoreScript)) > 1)
        {
            if (Initialized)
                return;
        }

        if (Initialized)
            return;

        Initialized = true;

        AddressableLibrary.LoadAddressableAssetAttributes();

        ModOptionNomadOnlyAttribute.Setup();
        ModOptionPCVROnlyAttribute.Setup();
    }
}
