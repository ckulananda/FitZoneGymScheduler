using FitZoneGymScheduler.ViewModels;
using System.Windows.Controls;

namespace FitZoneGymScheduler.Views
{
    public partial class MembersView : UserControl
    {
        public MembersView()
        {
            InitializeComponent();

            DataContext = new MembersViewModel();
        }
    }
}