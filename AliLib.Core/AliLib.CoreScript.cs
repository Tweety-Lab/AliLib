using AliLib.Core.Assets;
using AliLib.Core.Portability;
using System;
using System.Linq;
using System.Reflection;
using ThunderRoad;
using UnityEngine;

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

        if (!ShouldInitialize())
            return;

        Debug.Log($"[AliLib v{Assembly.GetExecutingAssembly().GetName().Version}] Initializing AliLib Core Script...");

        Catalog.LoadAddressableAssetAttributes();
        Catalog.LoadAddressableComponents();

        ModOptionNomadOnlyAttribute.Setup();
        ModOptionPCVROnlyAttribute.Setup();
    }

    /// <summary> Returns whether or not this specific instance of AliLib should be initialized. </summary>
    public bool ShouldInitialize()
    {

        // Find all loaded AliLib assemblies and pick the most up to date one
        // We do this to ensure that the latest version of this script is used even if a mod depends on an older version
        // Implications of this are... risky

        var currentName = Assembly.GetExecutingAssembly().GetName().Name;
        var aliLibAssemblies = ModManager.loadedMods.Where(m => m?.assemblies != null).SelectMany(m => m.assemblies).Where(a => a != null && a.GetName().Name == currentName).OrderByDescending(a => a.GetName().Version).ToList();

        if (aliLibAssemblies.Count == 0)
            return false;

        Assembly latest = aliLibAssemblies[0];
        Version latestVersion = latest.GetName().Version;
        Version currentVersion = Assembly.GetExecutingAssembly().GetName().Version;

        if (currentVersion < latestVersion)
            return false;

        string domainKey = $"AliLib.Initialized.{currentVersion}";
        if (AppDomain.CurrentDomain.GetData(domainKey) is true)
            return false;

        AppDomain.CurrentDomain.SetData(domainKey, true);

        return true;
    }
}
