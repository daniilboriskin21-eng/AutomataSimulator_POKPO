namespace AutomataSimulator.Core.Interfaces;

public interface IState
{
    Guid Id { get; }
    string Name { get; set; }
    bool IsStart { get; set; }
    bool IsFinal { get; set; }
}