using System;
using System.Collections.Generic;
using System.Linq;

namespace AliLib.Core.Events;

/// <inheritdoc/>
public class ModEvent : IModEvent<Action>
{
    /// <inheritdoc/>
    public bool Cancelled { get; set; } = false;

    /// <inheritdoc/>
    public IReadOnlyList<Action> Actions => actions.ToList();

    // A linked list has a slight performance advantage over a list since we only ever access sequentially
    private readonly LinkedList<Action> actions = new LinkedList<Action>();

    /// <inheritdoc/>
    public void AddFirst(Action action) => actions.AddFirst(action);

    /// <inheritdoc/>
    public void AddLast(Action action) => actions.AddLast(action);

    /// <inheritdoc/>
    public void Remove(Action action) => actions.Remove(action);

    public void Invoke()
    {
        foreach (Action action in actions)
        {
            if (Cancelled)
                break;

            action();
        }

        Cancelled = false;
    }

    public static ModEvent operator +(ModEvent modEvent, Action action)
    {
        modEvent.AddLast(action);
        return modEvent;
    }

    public static ModEvent operator -(ModEvent modEvent, Action action)
    {
        modEvent.Remove(action);
        return modEvent;
    }
}

/// <inheritdoc/>
public class ModEvent<T> : IModEvent<Action<T>>
{
    /// <inheritdoc/>
    public bool Cancelled { get; set; } = false;

    /// <inheritdoc/>
    public IReadOnlyList<Action<T>> Actions => actions.ToList();

    // A linked list has a slight performance advantage over a list since we only ever access sequentially
    private readonly LinkedList<Action<T>> actions = new LinkedList<Action<T>>();

    /// <inheritdoc/>
    public void AddFirst(Action<T> action) => actions.AddFirst(action);

    /// <inheritdoc/>
    public void AddLast(Action<T> action) => actions.AddLast(action);

    /// <inheritdoc/>
    public void Remove(Action<T> action) => actions.Remove(action);

    public void Invoke(T arg)
    {
        foreach (Action<T> action in actions)
        {
            if (Cancelled)
                break;

            action(arg);
        }

        Cancelled = false;
    }

    public static ModEvent<T> operator +(ModEvent<T> modEvent, Action<T> action)
    {
        modEvent.AddLast(action);
        return modEvent;
    }

    public static ModEvent<T> operator -(ModEvent<T> modEvent, Action<T> action)
    {
        modEvent.Remove(action);
        return modEvent;
    }
}
