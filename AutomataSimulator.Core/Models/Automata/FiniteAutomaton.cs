using AutomataSimulator.Core.Enums;
using AutomataSimulator.Core.Models.Transitions;

namespace AutomataSimulator.Core.Models.Automata;

public class FiniteAutomaton : Automaton<FiniteTransition>
{
    public FiniteAutomaton(bool isDeterministic = true)
    {
        Type = isDeterministic ? AutomatonType.DFA : AutomatonType.NFA;
    }

    public bool IsDeterministic()
    {
        // Логика проверки на DFA: 
        // 1. Нет $\varepsilon$-переходов.
        // 2. Для каждого состояния и символа алфавита ровно один переход.
        if (Transitions.Any(t => t.Symbol == null)) return false;

        var grouped = Transitions
            .GroupBy(t => new { t.FromStateId, t.Symbol })
            .Any(g => g.Count() > 1);

        return !grouped;
    }
}