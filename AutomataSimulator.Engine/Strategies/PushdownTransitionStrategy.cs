using AutomataSimulator.Core.Interfaces;
using AutomataSimulator.Core.Models.Transitions;
using AutomataSimulator.Engine.Interfaces;
using AutomataSimulator.Engine.Models;
using System.Collections.Immutable;

namespace AutomataSimulator.Engine.Strategies;

public class PushdownTransitionStrategy : ITransitionStrategy
{
    public ExecutionState NextStep(ExecutionState current, IEnumerable<ITransition> transitions)
    {
        char? input = current.RemainingInput.Length > 0 ? current.RemainingInput[0] : null;
        char? top = current.Stack.IsEmpty ? null : current.Stack.Peek();

        var valid = transitions.Cast<PushdownTransition>().FirstOrDefault(t =>
            current.ActiveStateIds.Contains(t.FromStateId) &&
            t.InputSymbol == input &&
            t.PopSymbol == top);

        if (valid == null) return current with { RemainingInput = "" }; // Блокировка

        var nextStack = current.Stack;
        if (valid.PopSymbol.HasValue) nextStack = nextStack.Pop();
        if (!string.IsNullOrEmpty(valid.PushSymbols))
        {
            foreach (var c in valid.PushSymbols.Reverse()) nextStack = nextStack.Push(c);
        }

        return current with
        {
            ActiveStateIds = ImmutableHashSet.Create(valid.ToStateId),
            RemainingInput = input.HasValue ? current.RemainingInput[1..] : current.RemainingInput,
            ReadPosition = input.HasValue ? current.ReadPosition + 1 : current.ReadPosition,
            Stack = nextStack,
            IsEpsilonStep = !input.HasValue
        };
    }

    public ExecutionState ApplyEpsilonClosure(ExecutionState current, IEnumerable<ITransition> transitions)
    {
        // В PDA эпсилон-переходы часто зависят от стека. 
        // Данная реализация ищет все цепочки переходов, не потребляющих входной символ.
        var pdaTransitions = transitions.Cast<PushdownTransition>().ToList();
        var queue = new Queue<ExecutionState>();
        queue.Enqueue(current);
        var visited = new HashSet<(Guid, string)>(); // Состояние + строка стека для детекции циклов

        ExecutionState finalState = current;

        while (queue.Count > 0)
        {
            var curr = queue.Dequeue();
            char? top = curr.Stack.IsEmpty ? null : curr.Stack.Peek();

            var epsTransitions = pdaTransitions.Where(t =>
                curr.ActiveStateIds.Contains(t.FromStateId) &&
                t.InputSymbol == null &&
                t.PopSymbol == top);

            foreach (var t in epsTransitions)
            {
                var nextStack = curr.Stack;
                if (t.PopSymbol.HasValue) nextStack = nextStack.Pop();
                if (!string.IsNullOrEmpty(t.PushSymbols))
                {
                    foreach (var c in t.PushSymbols.Reverse()) nextStack = nextStack.Push(c);
                }

                var nextState = curr with { ActiveStateIds = ImmutableHashSet.Create(t.ToStateId), Stack = nextStack };
                var stateKey = (t.ToStateId, string.Join("", nextState.Stack));

                if (visited.Add(stateKey))
                {
                    queue.Enqueue(nextState);
                    finalState = nextState;
                }
            }
        }
        return finalState;
    }
}