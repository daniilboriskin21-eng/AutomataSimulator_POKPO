using System.Collections.Immutable;

namespace AutomataSimulator.Engine.Models;

public record StateConfiguration(Guid StateId, IImmutableStack<char> Stack)
{
    // Переопределяем Equals для корректной работы HashSet
    public virtual bool Equals(StateConfiguration? other)
    {
        if (other == null) return false;
        if (StateId != other.StateId) return false;
        return Stack.SequenceEqual(other.Stack);
    }
    public override int GetHashCode() => HashCode.Combine(StateId, string.Join("", Stack));
}