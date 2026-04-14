using AliLib.Core.GC;
using System;

namespace AliLib.Core.Abilities;

/// <summary>
/// Base class for a modular spell ability.
/// </summary>
public abstract class Ability : AliGC.ManagedOwner
{
    /// <summary> The owning <see cref="AbilitySpell"/>. </summary>
    public AbilitySpell Spell { get; set; } = null!;

    /// <summary> Initializes a new instance of the <see cref="Ability"/> class. </summary>
    public Ability(AbilitySpell spell) => Spell = spell;

    internal int RefCount { get; set; } = 0;

    // This is pretty fragile
    internal void InternalLoad()
    {
        using var _ = AliGC.PushContext("Load", this);
        Load();

        Spell.OnStartCast.AddFirst(() => AliGC.PushContext("StartCast", this));
        Spell.OnStopCast.AddFirst(() =>
        {
            AliGC.PopContext();
            DisposeContext("StartCast");
        });
    }

    public virtual void Load() { }
    public virtual void Unload() { DisposeContext("Load"); }
}