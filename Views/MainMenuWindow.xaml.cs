using Microsoft.VisualBasic.Logging;
using System.Windows;
using System.Windows.Input;

namespace FitZoneGymScheduler.Views
{
    public partial class MainMenuWindow : Window
    {
        public MainMenuWindow()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState =
                WindowState == WindowState.Maximized
                ? WindowState.Normal
                : WindowState.Maximized;
        }

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                WindowState =
                    WindowState == WindowState.Maximized
                    ? WindowState.Normal
                    : WindowState.Maximized;

                return;
            }

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void OpenLogin_Click(object sender, RoutedEventArgs e)
        {

            var login = new LoginWindow();
            Application.Current.MainWindow = login;
            login.Show();
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            
                var window = new DatabaseSetupWindow(); // your existing window name
                window.ShowDialog();
            
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(
                "Are you sure you want to exit FitZone?",
                "Exit",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {

            var window = new AboutDeveloperWindow(); // your existing window name
            window.ShowDialog();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}