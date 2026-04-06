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
        Spell.OnStartCast += StartCast2;
    }

    public void StartCast()
    {
        Debug.Log("TimeStopAbility.StartCast");

        Spell.OnStartCast.Cancelled = true;
    }

    public void StartCast2()
    {
        Debug.Log("TimeStopAbility.StartCast2");
    }
}
