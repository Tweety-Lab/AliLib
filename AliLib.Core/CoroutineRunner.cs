
using UnityEngine;

namespace AliLib.Core;

/// <summary>
/// Utility class for running <see cref="Coroutine"/>s from non-monobehaviours.
/// </summary>
public class CoroutineRunner : MonoBehaviour
{
    /// <summary> Singleton instance. </summary>
    public static CoroutineRunner Instance
    {
        get
        {
            if (field == null)
                field = new GameObject("AliLib.CoroutineRunner").AddComponent<CoroutineRunner>();

            return field;
        }
    }
}
