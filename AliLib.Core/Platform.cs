using System.Runtime.CompilerServices;
using UnityEngine;

namespace AliLib.Core;

public static class Platform
{
    /// <summary> Is Blade & Sorcery currently running on an Android device? </summary>
    public static bool IsNomad
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return Application.platform == RuntimePlatform.Android;
        }
    }

    /// <summary> Is Blade & Sorcery currently running on a PC device? </summary>
    public static bool IsPCVR
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXPlayer;
        }
    }

    /// <summary> Dumps info about the current platform to the console. </summary>
    /// <returns> The dumped string. </returns>
    public static string DumpInfo()
    {
        // StringBuilder is a bit overkill here but it scales
        var sb = new System.Text.StringBuilder();

        sb.AppendLine($"IsNomad: {IsNomad}");
        sb.AppendLine($"IsPCVR: {IsPCVR}");
        sb.AppendLine($"Application.platform: {Application.platform}");
        sb.AppendLine($"Application.systemLanguage: {Application.systemLanguage}");

        string dump = sb.ToString();
        Debug.Log(dump);
        return dump;
    }
}
