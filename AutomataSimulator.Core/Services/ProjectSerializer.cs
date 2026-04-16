using System.Text.Json;
using System.Text.Json.Serialization;
using AutomataSimulator.Core.Models.Automata;
using AutomataSimulator.Core.Models.Transitions;
using AutomataSimulator.Core.Models.DTOs;
using AutomataSimulator.Core.Enums;

namespace AutomataSimulator.Core.Services;

public static class ProjectSerializer
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true, // Красивое форматирование JSON с отступами
        Converters = { new JsonStringEnumConverter() } // Сохраняем Enum как строки ("DFA" вместо 0)
    };

    public static string Serialize(object automaton)
    {
        var dto = new AutomatonDto();

        if (automaton is FiniteAutomaton fa)
        {
            dto.Type = fa.Type;
            dto.Name = fa.Name;
            dto.Origin = fa.Origin;
            dto.OriginSource = fa.OriginSource;
            dto.Alphabet = fa.Alphabet;
            dto.States = fa.States;
            dto.Transitions = fa.Transitions.Select(t => new TransitionDto
            {
                Id = t.Id,
                FromStateId = t.FromStateId,
                ToStateId = t.ToStateId,
                Symbol = t.Symbol
            }).ToList();
        }
        else if (automaton is PushdownAutomaton pda)
        {
            dto.Type = AutomatonType.PDA;
            dto.Name = pda.Name;
            dto.Origin = pda.Origin;
            dto.OriginSource = pda.OriginSource;
            dto.Alphabet = pda.Alphabet;
            dto.StackAlphabet = pda.StackAlphabet;
            dto.InitialStackSymbol = pda.InitialStackSymbol;
            dto.States = pda.States;
            dto.Transitions = pda.Transitions.Select(t => new TransitionDto
            {
                Id = t.Id,
                FromStateId = t.FromStateId,
                ToStateId = t.ToStateId,
                InputSymbol = t.InputSymbol,
                PopSymbol = t.PopSymbol,
                PushSymbols = t.PushSymbols
            }).ToList();
        }

        return JsonSerializer.Serialize(dto, Options);
    }

    public static object Deserialize(string json)
    {
        var dto = JsonSerializer.Deserialize<AutomatonDto>(json, Options)
            ?? throw new Exception("Файл поврежден или имеет неверный формат.");

        if (dto.Type == AutomatonType.DFA || dto.Type == AutomatonType.NFA)
        {
            return new FiniteAutomaton(dto.Type == AutomatonType.DFA)
            {
                Name = dto.Name,
                Origin = dto.Origin,
                OriginSource = dto.OriginSource,
                Alphabet = dto.Alphabet,
                States = dto.States,
                Transitions = dto.Transitions.Select(t => new FiniteTransition
                {
                    Id = t.Id,
                    FromStateId = t.FromStateId,
                    ToStateId = t.ToStateId,
                    Symbol = t.Symbol
                }).ToList()
            };
        }
        else // PDA
        {
            return new PushdownAutomaton
            {
                Name = dto.Name,
                Origin = dto.Origin,
                OriginSource = dto.OriginSource,
                Alphabet = dto.Alphabet,
                StackAlphabet = dto.StackAlphabet,
                InitialStackSymbol = dto.InitialStackSymbol,
                States = dto.States,
                Transitions = dto.Transitions.Select(t => new PushdownTransition
                {
                    Id = t.Id,
                    FromStateId = t.FromStateId,
                    ToStateId = t.ToStateId,
                    InputSymbol = t.InputSymbol,
                    PopSymbol = t.PopSymbol,
                    PushSymbols = t.PushSymbols
                }).ToList()
            };
        }
    }
}