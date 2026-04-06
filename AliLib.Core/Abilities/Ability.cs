
namespace AliLib.Core.Abilities;

/// <summary>
/// Base class for a modular spell ability.
/// </summary>
public abstract class Ability
{
    /// <summary> The owning <see cref="AbilitySpell"/>. </summary>
    public AbilitySpell Spell { get; set; } = null!;

    /// <summary> Initializes a new instance of the <see cref="Ability"/> class. </summary>
    public Ability(AbilitySpell spell) => Spell = spell;

    public virtual void Load() { }
    public virtual void Unload() { }
}
