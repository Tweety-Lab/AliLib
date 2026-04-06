
namespace AliLib.Core.Abilities;

/// <summary>
/// Base class for a modular spell ability.
/// </summary>
public abstract class Ability
{
    public virtual void StartCast() { }
    public virtual void StopCast() { }
}
