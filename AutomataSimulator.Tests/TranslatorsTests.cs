using AutomataSimulator.Translators.Grammar;
using AutomataSimulator.Translators.Regex;
using Xunit;

namespace AutomataSimulator.Tests;

public class TranslatorsTests
{
    [Fact]
    [Trait("Category", "Unit")]
    public void ThompsonTranslator_ValidRegex_CreatesFiniteAutomaton()
    {
        // Arrange
        var translator = new ThompsonTranslator();
        string regex = "a|b*";

        // Act
        var nfa = translator.Translate(regex);

        // Assert
        Assert.NotNull(nfa);
        Assert.False(nfa.IsDeterministic()); // Алгоритм Томпсона всегда выдает NFA
        Assert.NotEmpty(nfa.States);
        Assert.NotEmpty(nfa.Transitions);
        Assert.Contains('a', nfa.Alphabet);
        Assert.Contains('b', nfa.Alphabet);
        Assert.NotNull(nfa.GetStartState());
        Assert.NotEmpty(nfa.GetFinalStates());
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void CfgToPdaTranslator_ValidGrammar_CreatesPushdownAutomaton()
    {
        // Arrange
        var translator = new CfgToPdaTranslator();
        string grammar = "S -> aSb | ε";

        // Act
        var pda = translator.Translate(grammar);

        // Assert
        Assert.NotNull(pda);
        Assert.Equal('S', pda.InitialStackSymbol);
        Assert.NotEmpty(pda.Transitions);
        Assert.Contains('a', pda.Alphabet);
        Assert.Contains('b', pda.Alphabet);
        Assert.Contains('S', pda.StackAlphabet);
        Assert.NotNull(pda.GetStartState());
    }
}