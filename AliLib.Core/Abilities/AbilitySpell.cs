using System.Collections.Generic;
using ThunderRoad;

namespace AliLib.Core.Abilities;

/// <summary>
/// Base class for <see cref="SpellCastCharge"/>s that use the modular <see cref="Ability"/> system.
/// </summary>
public abstract class AbilitySpell : SpellCastCharge
{
    /// <summary> All registered <see cref="Ability"/>s. </summary>
    public abstract List<Ability> Abilities { get; set; }

    /// <inheritdoc/>
    public override void Fire(bool active)
    {
        base.Fire(active);

        if (active)
        {
            foreach (var ability in Abilities)
                ability.StartCast();
        }
        else
        {
            foreach (var ability in Abilities)
                ability.StopCast();
        }
    }
}
