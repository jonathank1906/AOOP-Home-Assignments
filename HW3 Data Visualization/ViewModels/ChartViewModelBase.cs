using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System.Collections.Generic;

namespace HW3_Data_Visualization.ViewModels;

public abstract partial class ChartViewModelBase : ObservableObject
{
    public abstract string Title { get; set; }
    public abstract IEnumerable<ISeries> SeriesCollection { get; }
    public abstract IEnumerable<Axis> XAxes { get; }
    public abstract IEnumerable<Axis> YAxes { get; }
    public IRelayCommand RemoveChartCommand { get; set; }
}