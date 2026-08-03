using System.Threading.Tasks;
using System.Windows;

namespace FitZoneGymScheduler.Views
{
    public partial class SplashScreenWindow : Window
    {
        public SplashScreenWindow()
        {
            InitializeComponent();

            Loaded += SplashScreenWindow_Loaded;
        }

        private async void SplashScreenWindow_Loaded(
            object sender,
            RoutedEventArgs e)
        {
            await StartLoading();
        }

        private async Task StartLoading()
        {
            for (int i = 0; i <= 100; i++)
            {
                LoadingBar.Value = i;

                if (i < 25)
                    StatusText.Text = "Loading Database...";
                else if (i < 50)
                    StatusText.Text = "Loading Settings...";
                else if (i < 75)
                    StatusText.Text = "Loading Workout Plans...";
                else
                    StatusText.Text = "Starting Application...";

                await Task.Delay(30);
            }

            var mainMenu = new MainMenuWindow();

            Application.Current.MainWindow = mainMenu;
            mainMenu.Show();

            this.Close();
        }


    }
}