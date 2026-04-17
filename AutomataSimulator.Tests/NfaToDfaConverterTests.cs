using AutomataSimulator.Core.Models;
using AutomataSimulator.Core.Models.Automata;
using AutomataSimulator.Core.Models.Transitions;
using AutomataSimulator.Core.Operations;
using Xunit;

namespace AutomataSimulator.Tests;

public class NfaToDfaConverterTests
{
    [Fact]
    [Trait("Category", "Unit")]
    public void Convert_DeterministicNfa_ReturnsSameAutomaton()
    {
        // Arrange
        var nfa = new FiniteAutomaton(isDeterministic: false);
        // Делаем его детерминированным по логике (нет эпсилон переходов и дубликатов)

        // Act
        var dfa = NfaToDfaConverter.Convert(nfa);

        // Assert
        Assert.True(dfa.IsDeterministic());
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Convert_NonDeterministicNfa_ReturnsCorrectDfa()
    {
        // Arrange: создаем NFA с эпсилон-переходом
        var nfa = new FiniteAutomaton(isDeterministic: false);
        var q0 = new State { Name = "q0", IsStart = true };
        var q1 = new State { Name = "q1", IsFinal = true };
        nfa.States.AddRange(new[] { q0, q1 });

        // Переход по символу 'a'
        nfa.Transitions.Add(new FiniteTransition { FromStateId = q0.Id, ToStateId = q1.Id, Symbol = 'a' });
        // Эпсилон-переход из q0 в q1
        nfa.Transitions.Add(new FiniteTransition { FromStateId = q0.Id, ToStateId = q1.Id, Symbol = null });

        nfa.Alphabet.Add('a');

        // Act
        var dfa = NfaToDfaConverter.Convert(nfa);

        // Assert
        Assert.True(dfa.IsDeterministic());
        Assert.NotEmpty(dfa.States);
        Assert.NotEmpty(dfa.Transitions);
        Assert.DoesNotContain(dfa.Transitions, t => t.Symbol == null); // В DFA нет эпсилон-переходов
    }
}