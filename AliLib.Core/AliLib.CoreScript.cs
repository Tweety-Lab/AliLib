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

        AddressableLibrary.LoadAddressableAssetAttributes();

        ModOptionNomadOnlyAttribute.Setup();
        ModOptionPCVROnlyAttribute.Setup();
    }
}
