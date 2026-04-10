
using System;
using System.Collections;
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
            {
                field = new GameObject("AliLib.CoroutineRunner").AddComponent<CoroutineRunner>();
                DontDestroyOnLoad(field.gameObject); // TODO: Do we want this?
            }

            return field;
        }
    }

    /// <summary> Plays an <see cref="Action"/> smoothly using <see cref="Coroutine"/>s. </summary>
    public void PlaySmooth(Action<float> action, float duration, float delay = 0f, AnimationCurve? curve = null, Action? onComplete = null)
    {
        curve ??= AnimationCurve.Linear(0f, 0f, 1f, 1f);
        StartCoroutine(PlaySmoothRoutine(action, duration, delay, curve, onComplete));
    }

    private IEnumerator PlaySmoothRoutine(Action<float> action, float duration, float delay, AnimationCurve curve, Action? onComplete)
    {
        if (delay > 0.0f)
            yield return new WaitForSeconds(delay);

        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            action(curve.Evaluate(t));

            elapsed += Time.deltaTime;
            yield return null;
        }

        action(curve.Evaluate(1f)); // Safety net
        onComplete?.Invoke();
    }
}
