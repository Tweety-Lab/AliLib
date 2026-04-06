using System;
using System.Collections.Generic;
using System.Text;

namespace AliLib.Core.Events;

/// <summary>
/// Sequential Event that allows easier third-party event hooking.
/// </summary>
public interface IModEvent<T> where T : Delegate
{
    /// <summary> Whether the event has been cancelled. </summary>
    public bool Cancelled { get; set; }

    /// <summary> The internal list of <see cref="Action"/> handlers. </summary>
    public IReadOnlyList<T> Actions { get; }

    /// <summary> Adds an <see cref="Action"/> to the start of the execution list. </summary>
    public void AddFirst(T action);

    /// <summary> Adds an <see cref="Action"/> to the end of the execution list. </summary>
    public void AddLast(T action);

    /// <summary> Removes an <see cref="Action"/> from the execution list. </summary>
    public void Remove(T action);
}
