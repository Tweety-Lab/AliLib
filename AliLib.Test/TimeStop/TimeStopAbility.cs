using AliLib.Core.Abilities;
using UnityEngine;

namespace AliLib.Test.TimeStop;

public class TimeStopAbility : Ability
{
    /// <inheritdoc/>
    public TimeStopAbility(AbilitySpell spell) : base(spell) { }

    /// <inheritdoc />
    public override void Load()
    {
        base.Load();

        Spell.OnStartCast += StartCast;
    }

    public void StartCast()
    {
        Debug.Log("TimeStopAbility.StartCast");
    }
}
