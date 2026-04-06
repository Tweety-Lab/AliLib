using AliLib.Core.Abilities;
using UnityEngine;

namespace AliLib.Test.TimeStop;

public class TimeStopAbility : Ability
{
    /// <inheritdoc />
    public override void StartCast()
    {
        base.StartCast();

        Debug.Log("TimeStopAbility");
    }
}
