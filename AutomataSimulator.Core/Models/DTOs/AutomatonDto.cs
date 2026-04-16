using AutomataSimulator.Core.Enums;

namespace AutomataSimulator.Core.Models.DTOs;

public class AutomatonDto
{
    public string Name { get; set; } = string.Empty;
    public AutomatonType Type { get; set; }
    public CreationOrigin Origin { get; set; }
    public string? OriginSource { get; set; }

    public HashSet<char> Alphabet { get; set; } = new();
    public HashSet<char> StackAlphabet { get; set; } = new();
    public char? InitialStackSymbol { get; set; }

    // Состояния можно сериализовать напрямую, так как класс State очень простой
    public List<State> States { get; set; } = new();

    // А вот переходы сливаем в один универсальный DTO
    public List<TransitionDto> Transitions { get; set; } = new();
}

public class TransitionDto
{
    public Guid Id { get; set; }
    public Guid FromStateId { get; set; }
    public Guid ToStateId { get; set; }

    // Поля для конечного автомата (DFA/NFA)
    public char? Symbol { get; set; }

    // Поля для автомата с магазинной памятью (PDA)
    public char? InputSymbol { get; set; }
    public char? PopSymbol { get; set; }
    public string? PushSymbols { get; set; }
}