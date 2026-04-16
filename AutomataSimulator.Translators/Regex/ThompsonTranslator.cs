using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutomataSimulator.Core.Enums;
using AutomataSimulator.Core.Models;
using AutomataSimulator.Core.Models.Automata;
using AutomataSimulator.Core.Models.Transitions;
using AutomataSimulator.Translators.Interfaces;

namespace AutomataSimulator.Translators.Regex;

public class ThompsonTranslator : ITranslator<FiniteAutomaton>
{
    private class Fragment
    {
        public State Start { get; init; } = null!;
        public State End { get; init; } = null!;
    }

    public FiniteAutomaton Translate(string regex)
    {
        if (string.IsNullOrEmpty(regex)) throw new ArgumentException("Regex cannot be empty");

        string prepared = PrepareRegex(regex);
        string postfix = ToPostfix(prepared);

        var stack = new Stack<Fragment>();
        var nfa = new FiniteAutomaton(isDeterministic: false)
        {
            Origin = CreationOrigin.Regex,
            OriginSource = regex
        };

        foreach (char c in postfix)
        {
            if (c == '*') stack.Push(ProcessStar(nfa, stack.Pop()));
            else if (c == '|') stack.Push(ProcessUnion(nfa, stack.Pop(), stack.Pop()));
            else if (c == '.') stack.Push(ProcessConcat(nfa, stack.Pop(), stack.Pop()));
            else stack.Push(ProcessLiteral(nfa, c));
        }

        var final = stack.Pop();
        final.Start.IsStart = true;
        final.End.IsFinal = true;
        nfa.Alphabet = nfa.Transitions
            .Where(t => t.Symbol.HasValue)
            .Select(t => t.Symbol!.Value)
            .ToHashSet();
        return nfa;
    }

    private string PrepareRegex(string regex)
    {
        var result = new StringBuilder();
        for (int i = 0; i < regex.Length; i++)
        {
            char c1 = regex[i];
            result.Append(c1);
            if (i + 1 < regex.Length)
            {
                char c2 = regex[i + 1];
                if (IsOperand(c1) && IsOperand(c2) || c1 == '*' && IsOperand(c2) || IsOperand(c1) && c2 == '(' || c1 == ')' && IsOperand(c2))
                    result.Append('.');
            }
        }
        return result.ToString();
    }

    private bool IsOperand(char c) => char.IsLetterOrDigit(c) || c == 'ε';

    private string ToPostfix(string regex)
    {
        var output = new StringBuilder();
        var operators = new Stack<char>();
        var precedence = new Dictionary<char, int> { { '*', 3 }, { '.', 2 }, { '|', 1 } };

        foreach (char c in regex)
        {
            if (IsOperand(c)) output.Append(c);
            else if (c == '(') operators.Push(c);
            else if (c == ')')
            {
                while (operators.Peek() != '(') output.Append(operators.Pop());
                operators.Pop();
            }
            else
            {
                while (operators.Count > 0 && operators.Peek() != '(' && precedence[operators.Peek()] >= precedence[c])
                    output.Append(operators.Pop());
                operators.Push(c);
            }
        }
        while (operators.Count > 0) output.Append(operators.Pop());
        return output.ToString();
    }

    private Fragment ProcessLiteral(FiniteAutomaton nfa, char s)
    {
        var start = new State { Name = $"q{nfa.States.Count}" };
        var end = new State { Name = $"q{nfa.States.Count + 1}" };
        nfa.States.AddRange(new[] { start, end });
        nfa.Transitions.Add(new FiniteTransition { FromStateId = start.Id, ToStateId = end.Id, Symbol = s == 'ε' ? null : s });
        return new Fragment { Start = start, End = end };
    }

    private Fragment ProcessConcat(FiniteAutomaton nfa, Fragment right, Fragment left)
    {
        nfa.Transitions.Add(new FiniteTransition { FromStateId = left.End.Id, ToStateId = right.Start.Id, Symbol = null });
        return new Fragment { Start = left.Start, End = right.End };
    }

    private Fragment ProcessUnion(FiniteAutomaton nfa, Fragment right, Fragment left)
    {
        var s = new State { Name = $"q{nfa.States.Count}" };
        var e = new State { Name = $"q{nfa.States.Count + 1}" };
        nfa.States.AddRange(new[] { s, e });
        nfa.Transitions.Add(new FiniteTransition { FromStateId = s.Id, ToStateId = left.Start.Id, Symbol = null });
        nfa.Transitions.Add(new FiniteTransition { FromStateId = s.Id, ToStateId = right.Start.Id, Symbol = null });
        nfa.Transitions.Add(new FiniteTransition { FromStateId = left.End.Id, ToStateId = e.Id, Symbol = null });
        nfa.Transitions.Add(new FiniteTransition { FromStateId = right.End.Id, ToStateId = e.Id, Symbol = null });
        return new Fragment { Start = s, End = e };
    }

    private Fragment ProcessStar(FiniteAutomaton nfa, Fragment inner)
    {
        var s = new State { Name = $"q{nfa.States.Count}" };
        var e = new State { Name = $"q{nfa.States.Count + 1}" };
        nfa.States.AddRange(new[] { s, e });
        nfa.Transitions.Add(new FiniteTransition { FromStateId = s.Id, ToStateId = inner.Start.Id, Symbol = null });
        nfa.Transitions.Add(new FiniteTransition { FromStateId = s.Id, ToStateId = e.Id, Symbol = null });
        nfa.Transitions.Add(new FiniteTransition { FromStateId = inner.End.Id, ToStateId = inner.Start.Id, Symbol = null });
        nfa.Transitions.Add(new FiniteTransition { FromStateId = inner.End.Id, ToStateId = e.Id, Symbol = null });
        return new Fragment { Start = s, End = e };
    }
}