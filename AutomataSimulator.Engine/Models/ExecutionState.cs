using System.Collections.Immutable;

namespace AutomataSimulator.Engine.Models;

public record ExecutionState
{
    // ТЕПЕРЬ МЫ ХРАНИМ НАБОР КОНФИГУРАЦИЙ (Состояние + Стек)
    public IImmutableSet<StateConfiguration> ActiveConfigurations { get; init; } = ImmutableHashSet<StateConfiguration>.Empty;

    public string RemainingInput { get; init; } = string.Empty;
    public int ReadPosition { get; init; }
    public bool IsEpsilonStep { get; init; }
    public bool IsTerminal => string.IsNullOrEmpty(RemainingInput);
}