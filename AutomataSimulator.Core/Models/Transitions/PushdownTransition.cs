namespace AutomataSimulator.Core.Models.Transitions;

public class PushdownTransition : Transition
{
    public char? InputSymbol { get; set; }   // Что читаем из строки
    public char? PopSymbol { get; set; }     // Что снимаем со стека
    public string? PushSymbols { get; set; } // Что кладем в стек (строка символов)
}