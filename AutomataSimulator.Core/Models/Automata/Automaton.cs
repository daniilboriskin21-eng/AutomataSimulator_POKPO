using AutomataSimulator.Core.Enums;
using AutomataSimulator.Core.Interfaces;
using AutomataSimulator.Core.Models.Transitions;

namespace AutomataSimulator.Core.Models.Automata;

public abstract class Automaton<TTransition> where TTransition : Transition
{
    public string Name { get; set; } = "New Automaton";
    public AutomatonType Type { get; protected set; }
    public CreationOrigin Origin { get; set; } = CreationOrigin.Manual;
    public string? OriginSource { get; set; } // Исходный Regex или Грамматика

    public HashSet<char> Alphabet { get; set; } = new();
    public List<State> States { get; set; } = new();
    public List<TTransition> Transitions { get; set; } = new();

    public State? GetStartState() => States.FirstOrDefault(s => s.IsStart);
    public IEnumerable<State> GetFinalStates() => States.Where(s => s.IsFinal);
}