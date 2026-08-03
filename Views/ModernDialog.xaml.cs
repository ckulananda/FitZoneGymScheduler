using System.Windows;
using System.Windows.Input;
using FitZoneGymScheduler.Helpers;

namespace FitZoneGymScheduler.Views
{
    public partial class ModernDialog : Window
    {


        public string TitleText { get; set; }
        public string MessageText { get; set; }

        public ModernDialog(
            string title,
            string message,
            DialogType dialogType)
        {
            InitializeComponent();

            TitleText = title;
            MessageText = message;

            DataContext = this;

            switch (dialogType)
            {
                case DialogType.Success:
                    YesButton.Content = "OK";
                    NoButton.Visibility = Visibility.Collapsed;
                    break;

                case DialogType.Warning:
                    YesButton.Content = "OK";
                    NoButton.Visibility = Visibility.Collapsed;
                    break;

                case DialogType.Error:
                    YesButton.Content = "OK";
                    NoButton.Visibility = Visibility.Collapsed;
                    break;

                case DialogType.Question:
                    YesButton.Content = "Confirm";
                    NoButton.Content = "Cancel";
                    break;
            }
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }


        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                YesButton_Click(null, null);
            }
            else if (e.Key == Key.Escape)
            {
                NoButton_Click(null, null);
            }
        }


    }
}