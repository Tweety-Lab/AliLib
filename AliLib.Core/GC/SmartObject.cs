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
    /// <summary> Forces disposal of the <see cref="SmartObject{T}"/>. </summary>
    void Dispose();
}

/// <summary>
/// A <see cref="UnityEngine.Object"/> instance that is tracked by AliLib's <see cref="AliGC"/>.
/// </summary>
/// <typeparam name="T">The type of <see cref="UnityEngine.Object"/>.</typeparam>
public class SmartObject<T> : ISmartObject where T : UnityEngine.Object
{
    /// <summary> The underlying <typeparamref name="T"/> or null if already disposed. </summary>
    public T? Object { get; private set; }

    /// <summary> Called before the <see cref="SmartObject{T}"/> is disposed. </summary>
    public ModEvent OnDisposed { get; set; } = new ModEvent();

    private SmartObject(T obj)
    {
        Object = obj;

        var owner = AliGC.CurrentOwner;
        var key = AliGC.CurrentContext;

        if (owner == null || key == null)
            throw new InvalidOperationException("SmartObject<T> cannot be created outside an AliGC context. Use AliGC.PushContext() or one of the lifecycle wrappers.");

        owner.GetQueue(key).Enqueue(this);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        OnDisposed.Invoke();

        UnityEngine.Object.Destroy(Object);
        Object = null;
    }

    public static implicit operator SmartObject<T>(T obj) => new SmartObject<T>(obj);
    public static implicit operator T?(SmartObject<T> obj) => obj.Object;
}
