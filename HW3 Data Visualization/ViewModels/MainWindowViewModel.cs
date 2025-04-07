using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using HW3_Data_Visualization.Models;
using HW3_Data_Visualization.Services;
using LiveChartsCore.SkiaSharpView;
using System.Linq;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System;

namespace HW3_Data_Visualization.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly CsvService _csvService;

    public ObservableCollection<ChartViewModelBase> Charts { get; } = new();
    public ObservableCollection<FoodWasteData> FoodWasteRecords { get; set; } = new();

    public IRelayCommand<ChartViewModelBase> RemoveChartCommand { get; }
    public IRelayCommand UndoCommand { get; }
    public IRelayCommand RedoCommand { get; }

    private readonly Stack<IUndoableAction> _undoStack = new();
    private readonly Stack<IUndoableAction> _redoStack = new();

    public MainWindowViewModel()
    {
        _csvService = new CsvService();
        LoadCsvData();

        RemoveChartCommand = new RelayCommand<ChartViewModelBase>(chart =>
        {
            if (chart != null) RemoveChart(chart);
        });

        UndoCommand = new RelayCommand(Undo, () => _undoStack.Count > 0);
        RedoCommand = new RelayCommand(Redo, () => _redoStack.Count > 0);
    }

    private interface IUndoableAction
    {
        void Execute();
        void Undo();
    }

    private class ChartAction : IUndoableAction
    {
        private readonly MainWindowViewModel _vm;
        private readonly ChartViewModelBase _chart;
        private readonly bool _isAddAction;

        public ChartAction(MainWindowViewModel vm, ChartViewModelBase chart, bool isAddAction)
        {
            _vm = vm;
            _chart = chart;
            _isAddAction = isAddAction;
        }

        public void Execute()
        {
            if (_isAddAction)
            {
                _vm.Charts.Add(_chart);
            }
            else
            {
                _vm.Charts.Remove(_chart);
            }
        }

        public void Undo()
        {
            if (_isAddAction)
            {
                _vm.Charts.Remove(_chart);
            }
            else
            {
                _vm.Charts.Add(_chart);
            }
        }
    }

    public void AddChart(ChartViewModelBase chart)
    {
        var action = new ChartAction(this, chart, true);
        action.Execute();
        _undoStack.Push(action);
        _redoStack.Clear();
        UpdateCommandStates();
    }

    public void RemoveChart(ChartViewModelBase chart)
    {
        var action = new ChartAction(this, chart, false);
        action.Execute();
        _undoStack.Push(action);
        _redoStack.Clear();
        UpdateCommandStates();
    }

    private void Undo()
    {
        if (_undoStack.Count == 0) return;

        var action = _undoStack.Pop();
        action.Undo();
        _redoStack.Push(action);
        UpdateCommandStates();
    }

    private void Redo()
    {
        if (_redoStack.Count == 0) return;

        var action = _redoStack.Pop();
        action.Execute();
        _undoStack.Push(action);
        UpdateCommandStates();
    }

    private void UpdateCommandStates()
    {
        UndoCommand.NotifyCanExecuteChanged();
        RedoCommand.NotifyCanExecuteChanged();
    }

    private void LoadCsvData()
    {
        var filePath = "Assets/global_food_wastage_dataset.csv";
        var data = _csvService.LoadData(filePath);
        FoodWasteRecords = new ObservableCollection<FoodWasteData>(data);

        Console.WriteLine($"Loaded {FoodWasteRecords.Count} records.");
    }

    [RelayCommand]
    private void ShowWasteByFoodCategoryOverTime()
    {
        var wasteByCategoryOverTime = FoodWasteRecords
            .GroupBy(f => new { f.FoodCategory, f.Year }) // Group by FoodCategory and Year
            .Select(g => new FoodWasteData
            {
                FoodCategory = g.Key.FoodCategory,
                Year = g.Key.Year,
                TotalWaste = g.Sum(f => f.TotalWaste)
            })
            .OrderBy(f => f.Year) // Sort by Year for better visualization
            .ToList();

        // Debug: Check if wasteByCategoryOverTime is empty
        if (wasteByCategoryOverTime.Count == 0)
        {
            Console.WriteLine("No data found for Waste by Food Category Over Time.");
            return;
        }

        var chart = new LineChartViewModel(wasteByCategoryOverTime, "Waste by Food Category Over Time")
        {
            RemoveChartCommand = RemoveChartCommand // Pass the command explicitly
        };
        AddChart(chart);
    }

    [RelayCommand]
    private void ShowWasteByCountry()
    {
        var groupedByCountry = FoodWasteRecords
            .GroupBy(f => f.Country)
            .Select(g => new FoodWasteData
            {
                FoodCategory = g.Key, // Map Country to FoodCategory for X-axis labels
                TotalWaste = g.Sum(f => f.TotalWaste)
            })
            .ToList();

        var chart = new BarChartViewModel(groupedByCountry, "Total Waste by Country")
        {
            RemoveChartCommand = RemoveChartCommand // Pass the command explicitly
        };
        AddChart(chart);
    }

    [RelayCommand]
    private void ShowFoodWaste()
    {
        var foodWasteData = FoodWasteRecords
            .GroupBy(f => f.FoodCategory)
            .Select(g => new FoodWasteData
            {
                FoodCategory = g.Key,
                TotalWaste = g.Sum(f => f.TotalWaste)
            })
            .ToList();

        var chart = new BarChartViewModel(foodWasteData, "Total Waste by Food Category")
        {
            RemoveChartCommand = RemoveChartCommand // Pass the command explicitly
        };
        AddChart(chart);
    }

    [RelayCommand]
    private void ShowTotalWasteOverTime()
    {
        var totalWasteOverTime = FoodWasteRecords
            .GroupBy(f => f.Year) // Group by Year
            .Select(g => new FoodWasteData
            {
                Year = g.Key,
                TotalWaste = g.Sum(f => f.TotalWaste)
            })
            .OrderBy(f => f.Year) // Sort by Year for better visualization
            .ToList();

        // Debug: Check if totalWasteOverTime is empty
        if (totalWasteOverTime.Count == 0)
        {
            Console.WriteLine("No data found for Total Waste Over Time.");
            return;
        }

        var chart = new LineChartViewModel(totalWasteOverTime, "Total Waste Over Time")
        {
            RemoveChartCommand = RemoveChartCommand // Pass the command explicitly
        };
        AddChart(chart);
    }

    [RelayCommand]
    private void ShowEconomicLossByCountry()
    {
        var economicLossByCountry = FoodWasteRecords
            .GroupBy(f => f.Country)
            .Select(g => new FoodWasteData
            {
                FoodCategory = g.Key, // Using Country as label
                EconomicLoss = g.Sum(f => f.EconomicLoss)
            })
            .ToList();

        var chart = new BarChartViewModel(economicLossByCountry, "Economic Loss by Country")
        {
            RemoveChartCommand = RemoveChartCommand
        };
        AddChart(chart);
    }

    [RelayCommand]
    private void ShowHouseholdWastePieChart()
    {
        var categoryAverages = FoodWasteRecords
            .GroupBy(f => f.FoodCategory)
            .Select(g => new FoodWasteData
            {
                FoodCategory = g.Key,
                HouseholdWastePercentage = g.Average(f => f.HouseholdWastePercentage)
            })
            .Where(f => f.HouseholdWastePercentage > 0)
            .ToList();

        var chart = new PieChartViewModel(categoryAverages, "Avg Household Waste % by Category")
        {
            RemoveChartCommand = RemoveChartCommand
        };
        AddChart(chart);
    }
}