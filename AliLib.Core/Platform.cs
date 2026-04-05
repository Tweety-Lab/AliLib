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
}
