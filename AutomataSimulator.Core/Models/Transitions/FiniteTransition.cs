namespace AutomataSimulator.Core.Models.Transitions;

public class FiniteTransition : Transition
{
    /// <summary>
    /// Символ перехода. null означает $\varepsilon$-переход.
    /// </summary>
    public char? Symbol { get; set; }
}