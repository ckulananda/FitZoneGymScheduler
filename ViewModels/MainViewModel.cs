using FitZoneGymScheduler.Commands;
using FitZoneGymScheduler.DAL;
using FitZoneGymScheduler.Services;
using FitZoneGymScheduler.Views;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FitZoneGymScheduler.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private UserControl _currentView;

        public UserControl CurrentView
        {
            get => _currentView;

            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        // COMMANDS

        public ICommand DashboardCommand { get; }

        public ICommand MembersCommand { get; }

        public ICommand ExerciseLibraryCommand { get; }

        public ICommand WorkoutPlansCommand { get; }

        public ICommand SavedPlansCommand { get; }

        public ICommand LogoutCommand { get; }
        public ICommand UserManagementCommand { get; }

        // CONSTRUCTOR

        public MainViewModel()
        {


            DashboardCommand =
                new RelayCommand(OpenDashboard);

            MembersCommand =
                new RelayCommand(OpenMembers);

            ExerciseLibraryCommand =
                new RelayCommand(OpenExerciseLibrary);

            WorkoutPlansCommand =
                new RelayCommand(OpenWorkoutPlans);

            SavedPlansCommand =
    new RelayCommand(OpenSavedPlans);

            UserManagementCommand =
    new RelayCommand(OpenUserManagement);

            IsAdministrator =
    UserSession.CurrentUser?.Role == "Administrator";

            // DEFAULT PAGE

            CurrentView =
                new DashboardView();

            LogoutCommand =
    new RelayCommand(Logout);
        }

        // DASHBOARD

        private void OpenDashboard(object obj)
        {
            CurrentView =
                new DashboardView();
        }

        // MEMBERS

        private void OpenMembers(object obj)
        {
            CurrentView =
                new MembersView();
        }

        // EXERCISE LIBRARY

        private void OpenExerciseLibrary(object obj)
        {
            CurrentView =
                new ExerciseLibraryView();
        }

        // WORKOUT PLANS

        private void OpenWorkoutPlans(object obj)
        {
            CurrentView =
                new WorkoutPlansView();
        }

        private void OpenSavedPlans(object obj)
        {
            CurrentView =
                new WorkoutPlansListView();
        }


        private bool _isAdministrator;

        public bool IsAdministrator
        {
            get => _isAdministrator;
            set
            {
                _isAdministrator = value;
                OnPropertyChanged();
            }
        }

        private void Logout(object obj)
        {
            if (!DialogService.Confirm(
                "Logout",
                "Are you sure you want to logout?"))
            {
                return;
            }

            SaveLogoutTime();

            var loginWindow = new LoginWindow();

            Application.Current.MainWindow = loginWindow;

            loginWindow.Show();

            foreach (Window window in Application.Current.Windows)
            {
                if (window is MainWindow)
                {
                    window.Close();
                    break;
                }
            }
        }

        private void SaveLogoutTime()
        {
            try
            {
                using var db = new AppDbContext();

                var history = db.UserLoginHistories
                    .FirstOrDefault(x =>
                        x.Id ==
                        UserSession.CurrentLoginHistoryId);

                if (history == null)
                    return;

                history.LogoutTime = DateTime.Now;

                history.DurationMinutes =
                    (history.LogoutTime.Value -
                     history.LoginTime)
                    .TotalMinutes;

                db.SaveChanges();
            }
            catch
            {
                // Ignore logout tracking errors
            }
        }

        private void OpenUserManagement(object obj)
        {
            if (UserSession.CurrentUser?.Role != "Administrator")
            {
                DialogService.Warning(
                    "Access Denied",
                    "Only Administrators can access User Management.");

                return;
            }

            CurrentView = new UserManagementView();
        }
    }
}