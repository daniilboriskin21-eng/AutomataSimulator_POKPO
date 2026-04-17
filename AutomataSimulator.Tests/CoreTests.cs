using AutomataSimulator.Core.Models;
using AutomataSimulator.Core.Models.Automata;
using AutomataSimulator.Core.Models.Transitions;
using AutomataSimulator.Core.Services;
using Xunit;

namespace AutomataSimulator.Tests;

public class CoreTests
{
    [Fact]
    [Trait("Category", "Unit")]
    public void FiniteAutomaton_IsDeterministic_ReturnsCorrectResult()
    {
        // Arrange
        var fa = new FiniteAutomaton();
        var s1 = new State { IsStart = true };
        var s2 = new State { IsFinal = true };
        fa.States.AddRange(new[] { s1, s2 });

        // Детерминированный переход
        fa.Transitions.Add(new FiniteTransition { FromStateId = s1.Id, ToStateId = s2.Id, Symbol = 'a' });
        Assert.True(fa.IsDeterministic());

        // Добавляем эпсилон-переход -> становится недетерминированным
        fa.Transitions.Add(new FiniteTransition { FromStateId = s1.Id, ToStateId = s2.Id, Symbol = null });
        Assert.False(fa.IsDeterministic());
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void ProjectSerializer_SerializeDeserializeDfa_WorksCorrectly()
    {
        // Arrange
        var originalFa = new FiniteAutomaton(isDeterministic: true) { Name = "TestDFA" };
        var state = new State { Name = "Q0", IsStart = true, IsFinal = true };
        originalFa.States.Add(state);
        originalFa.Transitions.Add(new FiniteTransition { FromStateId = state.Id, ToStateId = state.Id, Symbol = 'a' });
        originalFa.Alphabet.Add('a');

        // Act
        string json = ProjectSerializer.Serialize(originalFa);
        var deserialized = ProjectSerializer.Deserialize(json) as FiniteAutomaton;

        // Assert
        Assert.NotNull(deserialized);
        Assert.Equal("TestDFA", deserialized.Name);
        Assert.Single(deserialized.States);
        Assert.Single(deserialized.Transitions);
        Assert.Contains('a', deserialized.Alphabet);
    }
}