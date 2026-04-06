using AliLib.Core.Assets;
using System;
using System.Linq;
using ThunderRoad;

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

        // When static linking we dont want to run this script twice
        if (AppDomain.CurrentDomain.GetAssemblies().Any(a => a.GetName().Name == "AliLib.Core"))
            return;

        AddressableLibrary.LoadAddressableAssetAttributes();
    }
}
