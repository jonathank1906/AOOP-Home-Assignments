using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.Generic;
using HW3_Data_Visualization.Models;
using System.Linq;

namespace HW3_Data_Visualization.ViewModels;
public class BarChartViewModel : ChartViewModelBase
{
    // Make Title writable by adding a set accessor
    public override string Title { get; set; }

    public override IEnumerable<ISeries> SeriesCollection { get; }

    public override IEnumerable<Axis> XAxes { get; }

    public override IEnumerable<Axis> YAxes { get; }

    public BarChartViewModel(IEnumerable<FoodWasteData> data, string title = "Bar Chart")
    {
        // Set the title from the constructor parameter
        Title = title;

        // Initialize the series collection
        SeriesCollection = new[]
        {
                new ColumnSeries<double>
                {
                    Values = data.Select(d => d.TotalWaste).ToArray(),
                    Name = "Waste"
                }
            };

        // Initialize the X-axis
        XAxes = new[]
        {
                new Axis { Labels = data.Select(d => d.FoodCategory).ToArray() }
            };

        // Initialize the Y-axis
        YAxes = new[]
        {
                new Axis { Labeler = value => $"{value:N0} Tons" }
            };
    }
}