using FitZoneGymScheduler.Helpers;
using FitZoneGymScheduler.Views;

namespace FitZoneGymScheduler.Services
{
    public static class DialogService
    {
        public static bool Confirm(
            string title,
            string message)
        {
            var dialog =
                new ModernDialog(
                    title,
                    message,
                    DialogType.Question);

            return dialog.ShowDialog() == true;
        }

        public static void Success(
            string title,
            string message)
        {
            new ModernDialog(
                title,
                message,
                DialogType.Success)
            .ShowDialog();
        }

        public static void Warning(
            string title,
            string message)
        {
            new ModernDialog(
                title,
                message,
                DialogType.Warning)
            .ShowDialog();
        }

        public static void Error(
            string title,
            string message)
        {
            new ModernDialog(
                title,
                message,
                DialogType.Error)
            .ShowDialog();
        }
    }
}