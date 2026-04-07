using AliLib.Core.Events;
using System.Collections.Generic;
using ThunderRoad;

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
    public Ability GetAbility<T>() where T : Ability => abilities.Find(a => a is T);

    /// <summary> Register all <see cref="Ability"/>s. </summary>
    public abstract List<Ability> RegisterAbilities();

    /// <inheritdoc/>
    public override void Load(SpellCaster spellCaster)
    {
        base.Load(spellCaster);

        OnAbilitySpellLoad.Invoke(this);

        abilities.AddRange(RegisterAbilities());

        foreach (var ability in Abilities)
            ability.Load();
    }

    /// <inheritdoc/>
    public override void Unload()
    {
        base.Unload();

        foreach (var ability in Abilities)
            ability.Unload();

        abilities.Clear();
    }

    /// <inheritdoc/>
    public override void Fire(bool active)
    {
        base.Fire(active);

        if (active)
        {
            OnStartCast.Invoke();
        }
        else
        {
            OnStopCast.Invoke();
        }
    }

    /// <inheritdoc/>
    public override void UpdateCaster()
    {
        base.UpdateCaster();

        OnUpdateCast.Invoke();
    }
}
