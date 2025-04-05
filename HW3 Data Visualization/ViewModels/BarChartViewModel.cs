using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System.Collections.Generic;
using HW3_Data_Visualization.Models;
using System.Linq;

namespace HW3_Data_Visualization.ViewModels;

public class BarChartViewModel : ChartViewModelBase
{
    public override string Title { get; set; }
    public override IEnumerable<ISeries> SeriesCollection { get; }
    public override IEnumerable<Axis> XAxes { get; }
    public override IEnumerable<Axis> YAxes { get; }

    public BarChartViewModel(IEnumerable<FoodWasteData> data, string title = "Bar Chart")
    {
        Title = title;

        // Determine which property to visualize based on the title
        var values = data.Select(d =>
            title.Contains("Economic Loss") ? d.EconomicLoss :
            title.Contains("Per Capita") ? d.AvgWastePerCapita :
            d.TotalWaste).ToArray();

        // Use FoodCategory as label unless it's country-based data
        var labels = data.Select(d =>
            !string.IsNullOrWhiteSpace(d.FoodCategory) ? d.FoodCategory : d.Country).ToArray();

        // Name based on chart type
        var seriesName = title.Contains("Economic Loss") ? "Loss ($M)" :
                         title.Contains("Per Capita") ? "Kg/Capita" :
                         "Waste";

        SeriesCollection = new[]
        {
            new ColumnSeries<double>
            {
                Values = values,
                Name = seriesName
            }
        };

        XAxes = new[]
        {
            new Axis { Labels = labels }
        };

        YAxes = new[]
        {
            new Axis
            {
                Labeler = value =>
                    title.Contains("Economic Loss") ? $"${value:N1}M" :
                    title.Contains("Per Capita") ? $"{value:N1} Kg" :
                    $"{value:N0} Tons"
            }
        };
    }
}
