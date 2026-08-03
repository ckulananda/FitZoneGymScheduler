using FitZoneGymScheduler.ViewModels;
using System.Windows.Controls;

namespace FitZoneGymScheduler.Views
{
    public partial class DashboardView : UserControl
    {


        public DashboardView()
        {
            InitializeComponent();



            DataContext = new DashboardViewModel();
        }
    }
}