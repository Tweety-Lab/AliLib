
using AliLib.Core.Abilities;
using System;
using System.Collections.Generic;

namespace AliLib.Core.GC;

/// <summary>
/// A Type-Aware Garbage Collector for <see cref="SmartObject{T}"/>s.
/// </summary>
/// <remarks>
/// "Type-Aware" means it tracks lifetime of objects and disposes based on an associated type. In practice, this means we can make a <see cref="SmartObject{T}"/>
/// tied to the lifetime of an owning <see cref="Ability"/>.
/// </remarks>
public static class AliGC
{
    [ThreadStatic]
    internal static ManagedOwner? Current;

    public class ManagedOwner
    {
        public Queue<ISmartObject> DisposalQueue { get; private set; } = new();

        public void DisposeQueue()
        {
            while (DisposalQueue.Count > 0)
                DisposalQueue.Dequeue().Dispose();
        }
    }
}
