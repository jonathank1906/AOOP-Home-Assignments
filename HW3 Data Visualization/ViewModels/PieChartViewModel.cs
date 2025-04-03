using CommunityToolkit.Mvvm.ComponentModel;
using HW3_Data_Visualization.Models;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System.Collections.Generic;
using System.Linq;

namespace HW3_Data_Visualization.ViewModels;

public class PieChartViewModel : ChartViewModelBase
{
    public override string Title { get; set; }
    public override IEnumerable<ISeries> SeriesCollection { get; }
    public override IEnumerable<Axis> XAxes => null; // Pie chart doesn't use axes
    public override IEnumerable<Axis> YAxes => null;

    public PieChartViewModel(IEnumerable<FoodWasteData> data, string title = "Household Waste % by Category")
    {
        Title = title;

        SeriesCollection = data.Select(d => new PieSeries<double>
        {
            Values = new[] { d.HouseholdWastePercentage },
            Name = d.FoodCategory,
            DataLabelsSize = 14,
            DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle,
            DataLabelsFormatter = point => $"{point.Model:F1}%"
        }).ToList();
    }
}
