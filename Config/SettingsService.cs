using FitZoneGymScheduler.Models;
using System.IO;
using System.Text.Json;

namespace FitZoneGymScheduler.Services
{
    public static class SettingsService
    {
        private static readonly string SettingsPath =
            Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Config",
                "Settings.json");

        public static AppSettings Load()
        {
            if (!File.Exists(SettingsPath))
            {
                return new AppSettings
                {
                    PdfFolderPath =
                        @"D:\FitZone\WorkoutPlans"
                };
            }

            string json =
                File.ReadAllText(SettingsPath);

            return JsonSerializer.Deserialize<AppSettings>(json);
        }

        public static void Save(AppSettings settings)
        {
            var folder =
                Path.GetDirectoryName(SettingsPath);

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            string json =
                JsonSerializer.Serialize(
                    settings,
                    new JsonSerializerOptions
                    {
                        WriteIndented = true
                    });

            File.WriteAllText(SettingsPath, json);
        }
    }
}