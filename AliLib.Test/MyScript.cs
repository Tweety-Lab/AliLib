using UnityEngine;
using AliLib;
using AliLib.Core;
using ThunderRoad;

namespace AliLib.Test;

public class MyScript : ThunderScript
{
    [Addressable("ProjectionSorcery.GlassSpawn")]
    public static AudioClip? MyClip { get; set; }

    /// <inheritdoc />
    public override void ScriptEnable()
    {
        base.ScriptEnable();
    }

    /// <inheritdoc />
    public override void ScriptUpdate()
    {
        base.ScriptUpdate();

        if (MyClip != null)
        {
            Debug.Log($"[AliLib] {MyClip.length}");
        }
    }
}
