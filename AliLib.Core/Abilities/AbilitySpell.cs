using AliLib.Core.Events;
using AliLib.Core.GC;
using System.Collections.Generic;
using ThunderRoad;
using UnityEngine;

namespace AliLib.Core.Abilities;

/// <summary>
/// Base class for <see cref="SpellCastCharge"/>s that use the modular <see cref="Ability"/> system.
/// </summary>
public abstract class AbilitySpell : SpellCastCharge
{
    /// <summary> Called when a spell is loaded. </summary>
    public static ModEvent<AbilitySpell> OnAbilitySpellLoad { get; set; } = new ModEvent<AbilitySpell>();

    /// <summary> Called when the spell is cast. </summary>
    public ModEvent OnStartCast { get; set; } = new ModEvent();

    /// <summary> Called when the spell is stopped being casted. </summary>
    public ModEvent OnStopCast { get; set; } = new ModEvent();

    /// <summary> Called when the spell is updated. </summary>
    public ModEvent OnUpdateCast { get; set; } = new ModEvent();

    /// <summary> All registered <see cref="Ability"/>s. </summary>
    public IReadOnlyList<Ability> Abilities => abilities;

    private List<Ability> abilities = new List<Ability>();

    /// <summary> Get an <see cref="Ability"/> by type. </summary>
    public T? GetAbility<T>() where T : Ability => abilities.Find(a => a is T) as T;

    /// <summary> Register all <see cref="Ability"/>s. </summary>
    public abstract List<Ability> RegisterAbilities();

    /// <inheritdoc/>
    public override void Load(SpellCaster spellCaster)
    {
        base.Load(spellCaster);

        foreach (var ability in abilities)
        {
            // HACK: We have to reassign this for... reasons?? Spell getting cloned between Init() and Load() or something akin to that
            ability.Spell = this;
            ability.InternalEquip();
        }

        OnAbilitySpellLoad.Invoke(this);
    }

    /// <inheritdoc/>
    public override void Unload()
    {
        base.Unload();

        foreach (var ability in abilities)
            ability.OnUnequip();
    }

    /// <inheritdoc/>
    public override void Init()
    {
        base.Init();

        abilities.Clear();
        abilities.AddRange(RegisterAbilities());

        foreach (var ability in abilities)
            ability.Init();
    }

    /// <inheritdoc/>
    public override void Fire(bool active)
    {
        base.Fire(active);

        if (active)
            OnStartCast.Invoke();
        else
            OnStopCast.Invoke();
    }

    /// <inheritdoc/>
    public override void UpdateCaster()
    {
        base.UpdateCaster();

        OnUpdateCast.Invoke();
    }
}
