using MahApps.Metro.Controls;
using Converter.App.ViewModels;

namespace Converter.App;

public partial class MainWindow : MetroWindow
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }
}
