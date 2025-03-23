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

    private readonly Stack<Action> _undoStack = new();
    private readonly Stack<Action> _redoStack = new();

    private bool _lastOperationWasUndo = false;

    public MainWindowViewModel()
    {
        _csvService = new CsvService();
        LoadCsvData();

        // Initialize RemoveChartCommand to handle any ChartViewModelBase
        RemoveChartCommand = new RelayCommand<ChartViewModelBase>(chart =>
        {
            if (chart != null)
            {
                RemoveChart(chart);
            }
        });

        // Initialize Undo and Redo commands (always enabled)
        UndoCommand = new RelayCommand(Undo);
        RedoCommand = new RelayCommand(Redo);
    }

    private void LoadCsvData()
    {
        var filePath = "Assets/global_food_wastage_dataset.csv";  // The relative path to the CSV file
        var data = _csvService.LoadData(filePath);
        FoodWasteRecords = new ObservableCollection<FoodWasteData>(data);

        // Debug: Check the loaded data
        Console.WriteLine($"Loaded {FoodWasteRecords.Count} records.");
        foreach (var record in FoodWasteRecords.Take(5))
        {
            Console.WriteLine($"FoodCategory: {record.FoodCategory}, Country: {record.Country}, TotalWaste: {record.TotalWaste}, Year: {record.Year}");
        }
    }

    private void AddToUndoStack(Action undoAction)
    {
        // Push the new undo action onto the stack
        _undoStack.Push(undoAction);

        // Clear the redo stack only when a new action is performed after an undo
        if (!_lastOperationWasUndo)
        {
            _redoStack.Clear();
        }

        _lastOperationWasUndo = false;
    }

    private void AddToRedoStack(Action redoAction)
    {
        _redoStack.Push(redoAction);
    }

    public void AddChart(ChartViewModelBase chart)
    {
        Charts.Add(chart);

        // Store the undo action to remove the chart
        AddToUndoStack(() =>
        {
            Charts.Remove(chart);

            // Store the redo action to re-add the chart
            AddToRedoStack(() => AddChart(chart));
        });
    }

    public void RemoveChart(ChartViewModelBase chart)
    {
        Charts.Remove(chart);

        // Store the undo action to re-add the chart
        AddToUndoStack(() =>
        {
            Charts.Add(chart);

            // Store the redo action to remove the chart
            AddToRedoStack(() => RemoveChart(chart));
        });
    }

    private void Undo()
    {
        if (_undoStack.Count > 0)
        {
            // Pop the last undo action
            var undoAction = _undoStack.Pop();

            // Execute the undo action
            undoAction.Invoke();

            _lastOperationWasUndo = true;
        }
    }

    private void Redo()
    {
        if (_redoStack.Count > 0)
        {
            // Pop the last redo action
            var redoAction = _redoStack.Pop();

            // Execute the redo action
            redoAction.Invoke();

            _lastOperationWasUndo = false;
        }
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
}