
using AliLib.Core.Abilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;

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
    private static Stack<(string contextKey, ManagedOwner owner)>? contexts = new();

    /// <summary> The current <see cref="ManagedOwner"/> <see cref="SmartObject{T}"/>s will register with or null if none. </summary>
    public static ManagedOwner? CurrentOwner => contexts?.Count > 0 ? contexts.Peek().owner : null;

    /// <summary> The current <see cref="ManagedOwner"/> context key <see cref="SmartObject{T}"/>s will register with or null if none. </summary>
    public static string? CurrentContext => contexts?.Count > 0 ? contexts.Peek().contextKey : null;

    /// <summary> Pushes a new <see cref="ManagedOwner"/> context. </summary>
    public static IDisposable PushContext(string contextKey, ManagedOwner owner)
    {
        contexts?.Push((contextKey, owner));
        return new ContextScope();
    }

    /// <summary> Pops the current <see cref="ManagedOwner"/> context. </summary>
    public static void PopContext() => contexts?.Pop();

    /// <summary> Helper class that allows for <see cref="PushContext(string, ManagedOwner)"/> to be used as a <see cref="IDisposable"/>. </summary>
    public sealed class ContextScope : IDisposable
    {
        /// <inheritdoc/>
        public void Dispose() => PopContext();
    }

    public class ManagedOwner
    {
        private readonly Dictionary<string, Queue<ISmartObject>> queues = new();

        public Queue<ISmartObject> GetQueue(string contextKey)
        {
            if (!queues.TryGetValue(contextKey, out var q))
                queues[contextKey] = q = new Queue<ISmartObject>();

            return q;
        }

        /// <summary> Dispose every object registered under <paramref name="contextKey"/>. </summary>
        public void DisposeContext(string contextKey)
        {
            if (!queues.TryGetValue(contextKey, out var q))
                return;

            while (q.Count > 0)
                q.Dequeue().Dispose();
        }

        /// <summary> Forces disposal of all queues. </summary>
        public void DisposeAll()
        {
            foreach (var q in queues.Values)
                while (q.Count > 0) q.Dequeue().Dispose();

            queues.Clear();
        }
    }
}
