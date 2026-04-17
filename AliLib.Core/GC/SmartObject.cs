using AliLib.Core.Events;
using System;
using System.Diagnostics;
using System.Linq;

namespace AliLib.Core.GC;

/// <summary>
/// Non-Generic version of <see cref="SmartObject{T}"/>.
/// </summary>
public interface ISmartObject
{
    /// <summary> Disposes the <see cref="SmartObject{T}"/>. </summary>
    void Dispose();
}

/// <summary>
/// A <see cref="UnityEngine.Object"/> instance that is tracked by AliLib's <see cref="AliGC"/>.
/// </summary>
/// <typeparam name="T">The type of <see cref="UnityEngine.Object"/>.</typeparam>
public class SmartObject<T> : ISmartObject where T : UnityEngine.Object
{
    /// <summary> The underlying <typeparamref name="T"/>. </summary>
    public T Object { get; private set; }

    /// <summary> The key of the context that owns this <see cref="SmartObject{T}"/>. </summary>
    public string ContextKey { get; private set; } = string.Empty;

    /// <summary> Called before the <see cref="SmartObject{T}"/> is disposed. </summary>
    /// <remarks> This can be cancelled via <see cref="ModEvent.Cancelled"/> to bypass default disposal logic. </remarks>
    public ModEvent OnDisposed { get; set; } = new ModEvent();

    private SmartObject(T obj)
    {
        Object = obj;

        var owner = AliGC.CurrentOwner;
        var key = AliGC.CurrentContext;

        if (owner == null || key == null)
            throw new InvalidOperationException("SmartObject<T> cannot be created outside an AliGC context. Use AliGC.PushContext() or one of the lifecycle wrappers.");

        owner.GetQueue(key).Enqueue(this);

        ContextKey = key;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        // We want to allow the typical disposal pattern to be cancelled via ModEvents
        OnDisposed += ForceDispose;

        OnDisposed.Invoke();
    }

    /// <summary> Bypasses <see cref="OnDisposed"/> and forces disposal. </summary>
    public void ForceDispose()
    {
        UnityEngine.Object.Destroy(Object);
        Object = null;
    }

    public static implicit operator SmartObject<T>(T obj) => new SmartObject<T>(obj);
    public static implicit operator T(SmartObject<T> obj) => obj.Object;
}
