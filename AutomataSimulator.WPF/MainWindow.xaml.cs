using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using AutomataSimulator.ViewModels;
using AutomataSimulator.Core.Models.Automata;
using AutomataSimulator.Core.Models.Transitions;
using AutomataSimulator.WPF.Models;
using QuikGraph;

namespace AutomataSimulator.WPF;

public partial class MainWindow : Window
{
    private MainViewModel? _vm;
    private Dictionary<Guid, VisualVertex> _vertexMap = new();

    public MainWindow()
    {
        InitializeComponent();

        _vm = new MainViewModel();
        DataContext = _vm;

        _vm.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(MainViewModel.CurrentAutomaton))
            {
                Dispatcher.Invoke(() => BuildVisualGraph(_vm.CurrentAutomaton));
            }
        };

        _vm.Simulation.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == "ExecutionUpdated")
            {
                Dispatcher.Invoke(UpdateVisualHighlighting);
            }
        };
    }

    private void BuildVisualGraph(object? automaton)
    {
        if (automaton == null) return;

        var visualGraph = new BidirectionalGraph<object, IEdge<object>>();
        _vertexMap.Clear();

        if (automaton is FiniteAutomaton fa)
        {
            foreach (var s in fa.States)
            {
                var v = new VisualVertex { Name = s.Name, IsStart = s.IsStart, IsFinal = s.IsFinal };
                _vertexMap[s.Id] = v;
                visualGraph.AddVertex(v);
            }

            foreach (var t in fa.Transitions.Cast<FiniteTransition>())
            {
                var label = t.Symbol?.ToString() ?? "ε";
                visualGraph.AddEdge(new VisualEdge(_vertexMap[t.FromStateId], _vertexMap[t.ToStateId], label));
            }
        }
        else if (automaton is PushdownAutomaton pda)
        {
            foreach (var s in pda.States)
            {
                var v = new VisualVertex { Name = s.Name, IsStart = s.IsStart, IsFinal = s.IsFinal };
                _vertexMap[s.Id] = v;
                visualGraph.AddVertex(v);
            }

            foreach (var t in pda.Transitions.Cast<PushdownTransition>())
            {
                var label = $"{t.InputSymbol ?? 'ε'}, {t.PopSymbol ?? 'ε'} → {t.PushSymbols ?? "ε"}";
                visualGraph.AddEdge(new VisualEdge(_vertexMap[t.FromStateId], _vertexMap[t.ToStateId], label));
            }
        }

        GraphLayout.Graph = visualGraph;

        Dispatcher.BeginInvoke(new Action(() =>
        {
            GraphLayout.UpdateLayout();
            GraphLayout.Relayout();
        }), DispatcherPriority.Background);

        UpdateVisualHighlighting();
    }

    private void UpdateVisualHighlighting()
    {
        if (_vm?.Simulation == null) return;

        var activeIds = _vm.Simulation.GetActiveStateIds()?.ToList() ?? new List<Guid>();

        //if (activeIds.Count == 0)
        //{
        //    foreach (var pair in _vertexMap)
        //    {
        //        pair.Value.IsActive = pair.Value.IsStart;
        //    }
        //    return;
        //}

        foreach (var pair in _vertexMap)
        {
            pair.Value.IsActive = activeIds.Contains(pair.Key);
        }
    }

    private void StepForward_Click(object sender, RoutedEventArgs e)
    {
        if (_vm?.Simulation?.StepForwardCommand.CanExecute(null) == true)
        {
            _vm.Simulation.StepForwardCommand.Execute(null);
            UpdateVisualHighlighting();
        }
    }

    // --- НОВЫЙ ОБРАБОТЧИК ---
    private void StepBackward_Click(object sender, RoutedEventArgs e)
    {
        if (_vm?.Simulation?.StepBackwardCommand.CanExecute(null) == true)
        {
            _vm.Simulation.StepBackwardCommand.Execute(null);
            UpdateVisualHighlighting();
        }
    }

    private void Reset_Click(object sender, RoutedEventArgs e)
    {
        if (_vm?.Simulation?.ResetCommand.CanExecute(null) == true)
        {
            _vm.Simulation.ResetCommand.Execute(null);
            UpdateVisualHighlighting();
        }
    }
}