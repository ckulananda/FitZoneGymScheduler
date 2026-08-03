using FitZoneGymScheduler.DAL;
using FitZoneGymScheduler.Helpers;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Data.Sql;

namespace FitZoneGymScheduler.Views
{
    public partial class DatabaseSetupWindow : Window
    {
        public DatabaseSetupWindow()
        {
            InitializeComponent();
            LoadServers();

        }

        // LOAD SQL SERVERS
        private void LoadServers()
        {
            ServerCombo.ItemsSource = new List<string>
    {
        Environment.MachineName + @"\SQLEXPRESS"
    };

            ServerCombo.SelectedIndex = 0;
        }

        // TEST CONNECTION
        private void TestConnection_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string conn = BuildConnectionString();

                using var connection = new Microsoft.Data.SqlClient.SqlConnection(conn);
                connection.Open();

                StatusText.Text = "Connection successful ✔";
            }
            catch (Exception ex)
            {
                StatusText.Text = "Connection failed ❌ " + ex.Message;
            }
        }


        private void CreateDatabase_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string dbName = DbNameText.Text.Trim();
                string masterConn =
                    BuildConnectionString().Replace($"Database={dbName}", "Database=master");

                using var connection = new Microsoft.Data.SqlClient.SqlConnection(masterConn);
                connection.Open();

                string query = $"IF DB_ID('{dbName}') IS NULL CREATE DATABASE [{dbName}]";

                using var command = new Microsoft.Data.SqlClient.SqlCommand(query, connection);
                command.ExecuteNonQuery();

                StatusText.Text = "Database created successfully ✔";
            }
            catch (Exception ex)
            {
                StatusText.Text = "DB creation failed ❌ " + ex.Message;
            }
        }

        // SAVE + INIT DB
        private void SaveInitialize_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string conn = BuildConnectionString();

                // Save config
                DatabaseSettings.SaveConnectionString(conn);

                using var db = new AppDbContext();

                // Run migrations
                db.Database.Migrate();

                StatusText.Text = "Initializing database ✔";

                MessageBox.Show(
                    "Setup completed successfully!",
                    "FitZone",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                StatusText.Text = ex.Message;
            }
        }


        // BUILD CONNECTION STRING
        private string BuildConnectionString()
        {
            return $"Server={ServerCombo.SelectedItem};" +
                   $"Database={DbNameText.Text};" +
                   $"Trusted_Connection=True;" +
                   $"TrustServerCertificate=True;";
        }

       

    }
}