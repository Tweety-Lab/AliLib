
using ThunderRoad;
using UnityEngine;

namespace AliLib.Core;

/// <summary>
/// Utility class for simple audio operations.
/// </summary>
/// <remarks>
/// For more advanced usage, see <see cref="ThunderRoad.EffectAudio"/>.
/// </remarks>
public static class Audio
{
    /// <summary> Plays an <see cref="AudioClip"/>. </summary>
    /// <param name="clip">The <see cref="AudioClip"/> to play.</param>
    /// <param name="mixer">The <see cref="AudioMixerName"/> to play the <see cref="AudioClip"/> on.</param>
    /// <param name="volume">The volume to play the <see cref="AudioClip"/> at.</param>
    /// <param name="spatialBlend">The spatial blend to play the <see cref="AudioClip"/> at.</param>
    /// <param name="position">The position to play the <see cref="AudioClip"/> at.</param>
    /// <returns>The <see cref="AudioSource"/> that played the <see cref="AudioClip"/> or null if the <see cref="AudioClip"/> has finished playing already.</returns>
    public static AudioSource? Play(AudioClip clip, AudioMixerName mixer, float volume = 1f, float spatialBlend = 1f, Vector3? position = null)
    {
        GameObject obj = new GameObject("AudioPlayer");
        obj.transform.position = position ?? Vector3.zero;

        AudioSource source = obj.AddComponent<AudioSource>();

        source.clip = clip;
        source.spatialBlend = spatialBlend;
        source.volume = volume;

        source.outputAudioMixerGroup = ThunderRoadSettings.GetAudioMixerGroup(mixer);

        source.Play();
        GameObject.Destroy(obj, clip.length);

        return source;
    }

    /// <summary> Plays an <see cref="AudioClip"/> with no spatial blend. </summary>
    /// <inheritdoc cref="Audio.Play(AudioClip, AudioMixerName, float, float, Vector3?)"/>
    public static AudioSource? PlayNoBlend(AudioClip clip, AudioMixerName mixer, float volume = 1f) => Play(clip, mixer, volume, 0f);

    /// <summary> Plays a random <see cref="AudioClip"/> from an array. </summary>
    /// <returns>The selected <see cref="AudioSource"/>, or null if <paramref name="clips"/> is empty.</returns>
    public static AudioSource? PlayRandom(AudioClip[] clips, AudioMixerName mixer, float volume = 1f, float spatialBlend = 1f)
    {
        if (clips == null || clips.Length == 0)
            return null;

        AudioClip clip = clips[UnityEngine.Random.Range(0, clips.Length)];

        return Play(clip, mixer, volume, spatialBlend);
    }

    /// <summary> Stops and destroys an <see cref="AudioSource"/> immediately. </summary>
    public static void Stop(AudioSource? source)
    {
        if (source == null)
            return;

        source.Stop();
        GameObject.Destroy(source.gameObject);
    }
}
