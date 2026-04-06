using AliLib.Core.Events;
using System.Collections.Generic;
using ThunderRoad;

namespace AliLib.Core.Abilities;

/// <summary>
/// Base class for <see cref="SpellCastCharge"/>s that use the modular <see cref="Ability"/> system.
/// </summary>
public abstract class AbilitySpell : SpellCastCharge
{
    public ModEvent OnStartCast {  get; set; } = new ModEvent();

    public ModEvent OnStopCast { get; set; } = new ModEvent();

    /// <summary> All registered <see cref="Ability"/>s. </summary>
    public IReadOnlyList<Ability> Abilities => abilities;

    private List<Ability> abilities = new List<Ability>();

    /// <summary> Register all <see cref="Ability"/>s. </summary>
    public abstract List<Ability> RegisterAbilities();

    /// <inheritdoc/>
    public override void Load(SpellCaster spellCaster)
    {
        base.Load(spellCaster);

        abilities.AddRange(RegisterAbilities());

        foreach (var ability in Abilities)
            ability.Load();
    }

    /// <inheritdoc/>
    public override void Unload()
    {
        base.Unload();

        abilities.Clear();

        foreach (var ability in Abilities)
            ability.Unload();
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
}
