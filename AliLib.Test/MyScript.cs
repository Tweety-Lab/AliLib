using UnityEngine;
using AliLib.Core;
using ThunderRoad;
using AliLib.Core.Assets;

namespace AliLib.Test;

public class MyScript : ThunderScript
{
    [Addressable("ProjectionSorcery.GlassSpawn")]
    public static AudioClip? MyClip { get; set; }

    [ExportedString("Test/MyText.txt")]
    public const string MyAsset = "Hello, World!";

    /// <inheritdoc />
    public override void ScriptUpdate()
    {
        base.ScriptUpdate();

        if (MyClip != null && Platform.IsPCVR)
        {
            Debug.Log($"[AliLib] {MyClip.length}");
        }
    }
}
