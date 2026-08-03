using System.IO;

namespace FitZoneGymScheduler.Helpers
{
    public static class SetupState
    {
        private static string FlagFile =>
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "setup.flag");

        public static bool IsSetupCompleted()
        {
            return File.Exists(FlagFile);
        }

        public static void MarkCompleted()
        {
            File.WriteAllText(FlagFile, "done");
        }
    }
}