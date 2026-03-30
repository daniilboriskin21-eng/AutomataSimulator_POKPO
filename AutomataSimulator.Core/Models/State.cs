using AutomataSimulator.Core.Interfaces;

namespace AutomataSimulator.Core.Models;

public class State : IState
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public bool IsStart { get; set; }
    public bool IsFinal { get; set; }

    public override bool Equals(object? obj) => obj is State state && Id == state.Id;
    public override int GetHashCode() => Id.GetHashCode();
}