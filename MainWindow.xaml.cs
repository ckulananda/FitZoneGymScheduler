using FitZoneGymScheduler.ViewModels;
using System.Windows;

namespace FitZoneGymScheduler
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            WindowState = WindowState.Maximized;

            DataContext = new MainViewModel();
        }
    }
}