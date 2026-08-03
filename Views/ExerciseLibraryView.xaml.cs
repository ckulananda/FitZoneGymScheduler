using FitZoneGymScheduler.ViewModels;
using System.Windows.Controls;

namespace FitZoneGymScheduler.Views
{
    public partial class ExerciseLibraryView : UserControl
    {
        public ExerciseLibraryView()
        {
            InitializeComponent();

            DataContext =
                new ExerciseLibraryViewModel();
        }
    }
}