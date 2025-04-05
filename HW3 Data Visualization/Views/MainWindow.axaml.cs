using Avalonia.Controls;
using HW3_Data_Visualization.ViewModels;
namespace HW3_Data_Visualization.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }
}