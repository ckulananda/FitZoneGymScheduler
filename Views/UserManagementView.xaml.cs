using System.Windows.Controls;
using FitZoneGymScheduler.ViewModels;

namespace FitZoneGymScheduler.Views
{
    public partial class UserManagementView : UserControl
    {
        public UserManagementView()
        {
            InitializeComponent();

            DataContext = new UserManagementViewModel();
        }
    }
}