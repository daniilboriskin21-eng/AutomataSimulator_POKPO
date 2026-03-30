namespace AutomataSimulator.Core.Interfaces;

public interface ITransition
{
    Guid FromStateId { get; }
    Guid ToStateId { get; }
}