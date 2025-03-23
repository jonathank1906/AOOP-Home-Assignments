using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System.Collections.Generic;
using HW3_Data_Visualization.Models;
using System.Linq;

namespace HW3_Data_Visualization.ViewModels
{
    public class LineChartViewModel : ChartViewModelBase
    {
        // Override the Title property with a set accessor
        public override string Title { get; set; }

        public override IEnumerable<ISeries> SeriesCollection { get; }

        public override IEnumerable<Axis> XAxes { get; }

        public override IEnumerable<Axis> YAxes { get; }

        public LineChartViewModel(IEnumerable<FoodWasteData> data, string title = "Line Chart")
        {
            // Set the title from the constructor parameter
            Title = title;

            // Group data by FoodCategory and create a series for each category
            var groupedData = data
                .GroupBy(f => f.FoodCategory)
                .Select(g => new
                {
                    FoodCategory = g.Key,
                    Values = g.OrderBy(f => f.Year).Select(f => f.TotalWaste).ToArray(),
                    Years = g.OrderBy(f => f.Year).Select(f => f.Year.ToString()).ToArray()
                })
                .ToList();

            // Initialize the series collection
            SeriesCollection = groupedData.Select(g => new LineSeries<double>
            {
                Values = g.Values,
                Name = g.FoodCategory // Use FoodCategory as the series name
            }).ToList();

            // Initialize the X-axis with Years as labels
            XAxes = new[]
            {
                new Axis
                {
                    Labels = groupedData.FirstOrDefault()?.Years, // Use Years for X-axis labels
                    Labeler = value => value.ToString("N0") // Format the labels as integers
                }
            };

            // Initialize the Y-axis
            YAxes = new[]
            {
                new Axis { Labeler = value => $"{value:N0} Tons" }
            };
        }
    }
}