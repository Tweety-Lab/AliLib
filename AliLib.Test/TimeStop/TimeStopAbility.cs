using AliLib.Core;
using AliLib.Core.Abilities;
using AliLib.Core.Portability;
using ThunderRoad;
using UnityEngine;

namespace AliLib.Test.TimeStop;

public class TimeStopAbility : Ability
{
    [ModOptionNomadOnly]
    [ModOption]
    public static bool TestNomadOption;
    
    [ModOptionPCVROnly]
    [ModOption]
    public static bool TestPCVROption;

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
