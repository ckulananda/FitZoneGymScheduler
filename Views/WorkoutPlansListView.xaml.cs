using FitZoneGymScheduler.ViewModels;
using System.Windows.Controls;

namespace FitZoneGymScheduler.Views
{
    public partial class WorkoutPlansListView : UserControl
    {
        public WorkoutPlansListView()
        {
            InitializeComponent();

            DataContext =
                new WorkoutPlansListViewModel();
        }
    }
}