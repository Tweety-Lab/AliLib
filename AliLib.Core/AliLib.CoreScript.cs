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
    /// <inheritdoc/>
    public override void ScriptEnable()
    {
        base.ScriptEnable();

        // Multiple AliLibs can link to multiple mods but we only want to init once
        if (AppDomain.CurrentDomain.GetData("AliLibInitialized") != null)
            return;

        AppDomain.CurrentDomain.SetData("AliLibInitialized", true);

        AddressableLibrary.LoadAddressableAssetAttributes();

        ModOptionNomadOnlyAttribute.Setup();
        ModOptionPCVROnlyAttribute.Setup();
    }
}
