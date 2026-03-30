using AutomataSimulator.Core.Enums;
using AutomataSimulator.Core.Models.Transitions;

namespace AutomataSimulator.Core.Models.Automata;

public class PushdownAutomaton : Automaton<PushdownTransition>
{
    public HashSet<char> StackAlphabet { get; set; } = new();
    public char? InitialStackSymbol { get; set; }

    public PushdownAutomaton()
    {
        Type = AutomatonType.PDA;
    }
}