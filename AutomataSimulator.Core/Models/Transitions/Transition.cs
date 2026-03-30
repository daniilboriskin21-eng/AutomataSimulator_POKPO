using AutomataSimulator.Core.Interfaces;

namespace AutomataSimulator.Core.Models.Transitions;

public abstract class Transition : ITransition
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid FromStateId { get; set; }
    public Guid ToStateId { get; set; }
}