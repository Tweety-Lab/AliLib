using AliLib.Core.GC;

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

    internal void InternalLoad()
    {
        AliGC.Current = this;
        Load();
        AliGC.Current = null;
    }

    public virtual void Load() { }
    public virtual void Unload() { DisposeQueue(); }
}
