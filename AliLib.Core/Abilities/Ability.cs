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

    // This is pretty fragile
    internal void InternalEquip()
    {
        using var _ = AliGC.PushContext(nameof(OnEquip), this);
        OnEquip();

        Spell.OnStartCast.AddFirst(() => AliGC.PushContext("StartCast", this));
        Spell.OnStopCast.AddFirst(() =>
        {
            AliGC.PopContext();
            DisposeContext("StartCast");
        });
    }

    internal void InternalUnequip() => OnUnequip();

    /// <summary> Called once when the <see cref="AbilitySpell"/> is first created at mod load. </summary>
    public virtual void Init() { }

    /// <summary> Called when the <see cref="AbilitySpell"/> is selected/equipped. </summary>
    public virtual void OnEquip() { }

    /// <summary> Called when the <see cref="AbilitySpell"/> is deselected/unequipped. </summary>
    public virtual void OnUnequip() { DisposeContext(nameof(OnEquip)); }
}