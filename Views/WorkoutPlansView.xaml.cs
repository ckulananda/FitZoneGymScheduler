using FitZoneGymScheduler.ViewModels;
using System.Windows.Controls;

namespace FitZoneGymScheduler.Views
{
    public partial class WorkoutPlansView : UserControl
    {
        public WorkoutPlansView()
        {
            InitializeComponent();

            DataContext =
                new WorkoutPlansViewModel();
        }
    }
}