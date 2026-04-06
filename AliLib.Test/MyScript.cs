using UnityEngine;
using AliLib.Core;
using ThunderRoad;
using AliLib.Core.Assets;
using AliLib.Core.Events;

namespace AliLib.Test;

public class MyScript : ThunderScript
{
    [Addressable("ProjectionSorcery.GlassSpawn")]
    public static AudioClip? MyClip { get; set; }

    [ExportedString("Test/MyText.txt")]
    public const string MyAsset = "Hello, World!";

    public ModEvent MyEvent {  get; set; } = new ModEvent();

    /// <inheritdoc />
    public override void ScriptUpdate()
    {
        base.ScriptUpdate();

        MyEvent += () =>
        {
            MyEvent.Cancelled = true;

            if (MyClip != null)
            {
                Debug.Log($"[AliLib] {MyClip.length}");
            }
        };

        MyEvent.Invoke();
        MyEvent.Cancelled = true;

        if (MyClip != null && Platform.IsPCVR)
        {
            Debug.Log($"[AliLib] {MyClip.length}");
        }
    }
}
