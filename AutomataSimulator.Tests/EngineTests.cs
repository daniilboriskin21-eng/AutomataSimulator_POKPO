using AutomataSimulator.Core.Models;
using AutomataSimulator.Core.Models.Automata;
using AutomataSimulator.Core.Models.Transitions;
using AutomataSimulator.Engine;
using Xunit;

namespace AutomataSimulator.Tests;

public class EngineTests
{
    [Fact]
    [Trait("Category", "Unit")]
    public void Engine_StepForwardAndBackward_UpdatesStateCorrectly()
    {
        // Arrange: Создаем простой DFA: (q0) --a--> (q1)
        var dfa = new FiniteAutomaton();
        var q0 = new State { Name = "q0", IsStart = true };
        var q1 = new State { Name = "q1", IsFinal = true };
        dfa.States.AddRange(new[] { q0, q1 });
        dfa.Transitions.Add(new FiniteTransition { FromStateId = q0.Id, ToStateId = q1.Id, Symbol = 'a' });
        dfa.Alphabet.Add('a');

        var engine = new ExecutionEngine<FiniteAutomaton, FiniteTransition>(dfa, "a");

        // Act & Assert
        Assert.False(engine.IsAccepted); // Еще не прочитали строку
        Assert.True(engine.CanStepForward);

        engine.StepForward();

        Assert.True(engine.IsAccepted); // Прочитали 'a', попали в q1
        Assert.False(engine.CanStepForward); // Строка кончилась
        Assert.True(engine.CanStepBackward);

        engine.StepBackward();
        Assert.False(engine.IsAccepted); // Вернулись в q0
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Engine_PdaStack_WorksCorrectly()
    {
        // Arrange: Простой PDA, который кладет X в стек при 'a'
        var pda = new PushdownAutomaton { InitialStackSymbol = 'Z' };
        var q0 = new State { Name = "q0", IsStart = true, IsFinal = true };
        pda.States.Add(q0);

        pda.Transitions.Add(new PushdownTransition
        {
            FromStateId = q0.Id,
            ToStateId = q0.Id,
            InputSymbol = 'a',
            PopSymbol = 'Z',
            PushSymbols = "XZ"
        });

        var engine = new ExecutionEngine<PushdownAutomaton, PushdownTransition>(pda, "a");

        // Act
        engine.StepForward();

        // Assert
        var config = engine.CurrentState.ActiveConfigurations.First();
        Assert.Equal('X', config.Stack.Peek()); // Проверяем, что на вершине стека теперь X
    }
}