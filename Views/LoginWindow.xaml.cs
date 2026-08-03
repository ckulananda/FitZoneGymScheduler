using System.Windows;
using FitZoneGymScheduler.ViewModels;

namespace FitZoneGymScheduler.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();

            DataContext =
                new LoginViewModel();
        }
    }
}