using System;
using System.Collections.Generic;
using System.Linq;

namespace AliLib.Core;

/// <summary>
/// Sequential Event that allows easier third-party event hooking.
/// </summary>
public class ModEvent<T>
{
    /// <summary> Whether the event has been cancelled. </summary>
    public volatile bool Cancelled { get; set; } = false;

    /// <summary> The internal list of <see cref="Action"/> handlers. </summary>
    public IReadOnlyList<Action<T>> Actions => actions.ToList();

    // A linked list has a slight performance advantage over a list since we only ever access sequentially
    private readonly LinkedList<Action<T>> actions = new LinkedList<Action<T>>();

    /// <summary> Adds an <see cref="Action"/> to the start of the execution list. </summary>
    public void AddFirst(Action<T> action) => actions.AddFirst(action);

    /// <summary> Adds an <see cref="Action"/> to the end of the execution list. </summary>
    public void AddLast(Action<T> action) => actions.AddLast(action);

    /// <summary> Removes an <see cref="Action"/> from the execution list. </summary>
    public void Remove(Action<T> action) => actions.Remove(action);

    public void Invoke(T arg)
    {
        foreach (Action<T> action in actions)
        {
            if (Cancelled)
                break;

            action(arg);
        }
    }
}
