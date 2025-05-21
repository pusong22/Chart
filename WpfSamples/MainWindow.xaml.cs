using System.Windows;
using WpfSamples.ViewModel;

namespace WpfSamples;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        DataContext = new ChartViewModel();
    }
}
