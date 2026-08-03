using System.Windows;
using FitZoneGymScheduler.ViewModels;

namespace FitZoneGymScheduler.Views
{
    public partial class SignUpWindow : Window
    {
        public SignUpWindow()
        {
            InitializeComponent();

            DataContext =
                new SignUpViewModel();
        }
    }
}