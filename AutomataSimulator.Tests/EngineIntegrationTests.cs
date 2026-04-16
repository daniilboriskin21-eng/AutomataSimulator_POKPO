using AutomataSimulator.Core.Models.Automata;
using AutomataSimulator.Core.Models.Transitions;
using AutomataSimulator.Engine;
using AutomataSimulator.Translators.Regex;
using Xunit;

namespace AutomataSimulator.Tests;

public class EngineIntegrationTests
{
    [Theory]
    [Trait("Category", "Integration")]
    [InlineData("a|b", "a", true)]
    [InlineData("a|b", "b", true)]
    [InlineData("a|b", "c", false)]
    [InlineData("a*", "aaaa", true)]
    public void RegexToEngine_FullCycle_ReturnsExpectedResult(string regex, string input, bool expected)
    {
        // Arrange (Используем Translator для создания автомата)
        var translator = new ThompsonTranslator();
        var nfa = translator.Translate(regex);

        // Act (Используем Engine для симуляции)
        var engine = new ExecutionEngine<FiniteAutomaton, FiniteTransition>(nfa, input);
        engine.Run();

        // Assert
        Assert.Equal(expected, engine.IsAccepted);
    }
}