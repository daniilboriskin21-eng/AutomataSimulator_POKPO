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
        Assert.False(engine.IsAccepted);
        Assert.True(engine.CanStepForward);

        engine.StepForward();

        Assert.True(engine.IsAccepted);
        Assert.False(engine.CanStepForward);
        Assert.True(engine.CanStepBackward);

        engine.StepBackward();
        Assert.False(engine.IsAccepted);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Engine_RunAndBreakpoints_StopsAtBreakpoint()
    {
        // Arrange: (q0) --a--> (q1) --b--> (q2 final)
        var dfa = new FiniteAutomaton();
        var q0 = new State { Name = "q0", IsStart = true };
        var q1 = new State { Name = "q1" };
        var q2 = new State { Name = "q2", IsFinal = true };
        dfa.States.AddRange(new[] { q0, q1, q2 });
        dfa.Transitions.Add(new FiniteTransition { FromStateId = q0.Id, ToStateId = q1.Id, Symbol = 'a' });
        dfa.Transitions.Add(new FiniteTransition { FromStateId = q1.Id, ToStateId = q2.Id, Symbol = 'b' });

        var engine = new ExecutionEngine<FiniteAutomaton, FiniteTransition>(dfa, "ab");

        // Ставим брейкпоинт на промежуточное состояние q1
        engine.ToggleBreakpoint(q1.Id);

        // Act
        engine.Run(); // Должен остановиться после прочтения 'a' на состоянии q1

        // Assert
        Assert.Contains(q1.Id, engine.GetActiveStateIds());
        Assert.False(engine.IsAccepted); // Строка еще не дочитана

        // Act 2: Запускаем дальше
        engine.ToggleBreakpoint(q1.Id); // Снимаем брейкпоинт (покрытие логики Toggle)
        engine.Run();

        // Assert 2: Должен дойти до конца
        Assert.Contains(q2.Id, engine.GetActiveStateIds());
        Assert.True(engine.IsAccepted);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Engine_SetInputAndReset_RestoresInitialState()
    {
        var dfa = new FiniteAutomaton();
        var q0 = new State { Name = "q0", IsStart = true };
        dfa.States.Add(q0);

        var engine = new ExecutionEngine<FiniteAutomaton, FiniteTransition>(dfa, "abc");
        engine.StepForward(); // Делаем фейковый шаг, чтобы изменить состояние

        // Act
        engine.SetInput("xyz");

        // Assert
        Assert.Equal("xyz", engine.CurrentState.RemainingInput);
        Assert.Equal(0, engine.CurrentState.ReadPosition);
        Assert.False(engine.CanStepBackward); // История должна была очиститься
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Engine_NfaEpsilonClosure_ExploresAllPaths()
    {
        // Arrange: Проверяем эпсилон-замыкание
        // q0 --eps--> q1 --a--> q2
        var nfa = new FiniteAutomaton(isDeterministic: false);
        var q0 = new State { IsStart = true };
        var q1 = new State();
        var q2 = new State { IsFinal = true };
        nfa.States.AddRange(new[] { q0, q1, q2 });

        nfa.Transitions.Add(new FiniteTransition { FromStateId = q0.Id, ToStateId = q1.Id, Symbol = null });
        nfa.Transitions.Add(new FiniteTransition { FromStateId = q1.Id, ToStateId = q2.Id, Symbol = 'a' });

        var engine = new ExecutionEngine<FiniteAutomaton, FiniteTransition>(nfa, "a");

        // Act
        engine.StepForward(); // Делает эпсилон-замыкание до q1, читает 'a', переходит в q2

        // Assert
        Assert.True(engine.IsAccepted);
        Assert.Contains(q2.Id, engine.GetActiveStateIds());
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Engine_PdaStackPop_WorksCorrectly()
    {
        // Arrange: Простой PDA, который проверяет снятие (Pop) со стека
        var pda = new PushdownAutomaton { InitialStackSymbol = 'Z' };
        var q0 = new State { Name = "q0", IsStart = true };
        var q1 = new State { Name = "q1", IsFinal = true };
        pda.States.AddRange(new[] { q0, q1 });

        // q0 --(читаем 'a', снимаем 'Z', кладем ничего)--> q1
        pda.Transitions.Add(new PushdownTransition
        {
            FromStateId = q0.Id,
            ToStateId = q1.Id,
            InputSymbol = 'a',
            PopSymbol = 'Z',
            PushSymbols = ""
        });

        var engine = new ExecutionEngine<PushdownAutomaton, PushdownTransition>(pda, "a");

        // Act
        engine.StepForward();

        // Assert
        Assert.True(engine.IsAccepted);
        var config = engine.CurrentState.ActiveConfigurations.First();
        Assert.True(config.Stack.IsEmpty); // Стек должен стать пустым
    }
}