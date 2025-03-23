using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using HW3_Data_Visualization.Models;
using HW3_Data_Visualization.Services;
using LiveChartsCore.SkiaSharpView;
using System.Linq;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System;

namespace HW3_Data_Visualization.ViewModels
{
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
        }

        private void AddToUndoStack(Action undoAction)
        {
            // Push the new undo action onto the stack
            _undoStack.Push(undoAction);

            // Clear the redo stack only when a new action is performed
            _redoStack.Clear();
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
            }
        }

        [RelayCommand]
        private void ShowHouseholdWaste()
        {
            var householdData = FoodWasteRecords.Where(f => f.FoodCategory == "Household").ToList();
            var chart = new BarChartViewModel(householdData)
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
                    FoodCategory = g.Key,
                    TotalWaste = g.Sum(f => f.TotalWaste)
                })
                .ToList();
            var chart = new BarChartViewModel(groupedByCountry)
            {
                RemoveChartCommand = RemoveChartCommand // Pass the command explicitly
            };
            AddChart(chart);
        }

        [RelayCommand]
        private void ShowFoodWaste()
        {
            var chart = new BarChartViewModel(FoodWasteRecords.ToList())
            {
                RemoveChartCommand = RemoveChartCommand // Pass the command explicitly
            };
            AddChart(chart);
        }

        [RelayCommand]
        private void ShowYearlyWasteTrend()
        {
            var yearlyData = FoodWasteRecords
                .GroupBy(f => f.Year)
                .Select(g => new FoodWasteData
                {
                    FoodCategory = g.Key.ToString(),
                    TotalWaste = g.Sum(f => f.TotalWaste)
                })
                .ToList();
            var chart = new LineChartViewModel(yearlyData) // Create Line Chart
            {
                RemoveChartCommand = RemoveChartCommand
            };
            AddChart(chart);
        }
    }
}