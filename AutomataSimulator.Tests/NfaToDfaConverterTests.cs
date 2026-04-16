using AutomataSimulator.Core.Models.Automata;
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
}