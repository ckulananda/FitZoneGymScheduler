using System.Diagnostics;
using System.Windows;

namespace FitZoneGymScheduler.Views
{
    public partial class AboutDeveloperWindow : Window
    {
        public AboutDeveloperWindow()
        {
            InitializeComponent();
        }

        private void GitHub_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/ckulananda",
                UseShellExecute = true
            });
        }

        private void LinkedIn_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://linkedin.com/in/dadallage-chamodha-dhananjana-kulananda-5515b040a",
                UseShellExecute = true
            });
        }
    }
}