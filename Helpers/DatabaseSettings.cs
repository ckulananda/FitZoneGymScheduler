using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace FitZoneGymScheduler.Helpers
{
    public static class DatabaseSettings
    {
        private static readonly string ConfigPath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");

        // =====================================================
        // CHECK IF CONNECTION EXISTS
        // =====================================================
        public static bool HasConnectionString()
        {
            if (!File.Exists(ConfigPath))
                return false;

            try
            {
                var json = File.ReadAllText(ConfigPath);
                var root = JsonNode.Parse(json);

                if (root == null) return false;

                var conn = root["ConnectionStrings"]?["DefaultConnection"]?.ToString();

                return !string.IsNullOrWhiteSpace(conn);
            }
            catch
            {
                return false;
            }
        }

        // =====================================================
        // GET CONNECTION STRING (SAFE)
        // =====================================================
        public static string GetConnectionString()
        {
            if (!File.Exists(ConfigPath))
                throw new FileNotFoundException("Configuration file not found. Run setup first.");

            var json = File.ReadAllText(ConfigPath);
            var root = JsonNode.Parse(json);

            if (root == null)
                throw new Exception("Invalid configuration file.");

            var conn = root["ConnectionStrings"]?["DefaultConnection"]?.ToString();

            if (string.IsNullOrWhiteSpace(conn))
                throw new Exception("Connection string is empty.");

            return conn;
        }

        // =====================================================
        // SAVE CONNECTION STRING (AUTO-CREATE FILE IF NEEDED)
        // =====================================================
        public static void SaveConnectionString(string connectionString)
        {
            JsonNode root;

            // ⭐ CREATE FILE IF NOT EXISTS
            if (!File.Exists(ConfigPath))
            {
                root = new JsonObject
                {
                    ["ConnectionStrings"] = new JsonObject()
                };
            }
            else
            {
                var json = File.ReadAllText(ConfigPath);
                root = JsonNode.Parse(json) ?? new JsonObject();
            }

            if (root["ConnectionStrings"] == null)
                root["ConnectionStrings"] = new JsonObject();

            root["ConnectionStrings"]!["DefaultConnection"] = connectionString;

            File.WriteAllText(
                ConfigPath,
                root.ToJsonString(new JsonSerializerOptions
                {
                    WriteIndented = true
                }));
        }
    }
}