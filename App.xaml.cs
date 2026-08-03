using FitZoneGymScheduler.DAL;
using FitZoneGymScheduler.Helpers;
using FitZoneGymScheduler.Models;
using FitZoneGymScheduler.Services;
using FitZoneGymScheduler.Views;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Windows;

namespace FitZoneGymScheduler
{
    public partial class App : Application
    {
        // GLOBAL LOADING SERVICE
        public static LoadingService LoadingService { get; }
            = new LoadingService();

       

        // =========================================
        // APP STARTUP
        // =========================================


        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);


            ShutdownMode = ShutdownMode.OnMainWindowClose;

            var splash = new SplashScreenWindow();
            Current.MainWindow = splash;
            splash.Show();

            try
            {
                string configPath =
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");

                // =====================================================
                // FIRST RUN CHECK (NO CRASH)
                // =====================================================
                if (!File.Exists(configPath))
                {
                    var setup = new DatabaseSetupWindow();

                    bool? result = setup.ShowDialog();

                    if (result != true)
                    {
                        Shutdown();
                        return;
                    }
                }

                // =====================================================
                // DATABASE MIGRATION
                // =====================================================
                using (var db = new AppDbContext())
                {
                    db.Database.Migrate();
                }

                SeedAdministrator();

                // =====================================================
                // OPEN LOGIN
                // =====================================================
               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Startup Error");
                Shutdown();
            }
        }
        // =========================================
        // DEFAULT ADMIN CREATION
        // =========================================
        private void SeedAdministrator()
        {
            using var db = new AppDbContext();

            var admin = db.Users.FirstOrDefault(u => u.Username == "admin");

            if (admin == null)
            {
                admin = new User
                {
                    FullName = "System Administrator",
                    Username = "admin",
                    Email = "admin@fitzone.com",
                    PhoneNumber = "0000000000",
                    CreatedDate = new DateTime(2026, 1, 1),
                    IsActive = true
                };

                db.Users.Add(admin);
            }

            // 🔥 FORCE CORRECT VALUES EVERY STARTUP
            admin.Role = "Administrator";   // <- THIS is what your DB currently uses
            admin.PasswordHash = PasswordService.HashPassword("admin123");

            db.SaveChanges();

        }




        // =========================================
        // APP EXIT HANDLER
        // =========================================
        protected override void OnExit(ExitEventArgs e)
        {
            try
            {
                AuthService.Logout();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"Logout Error: {ex.Message}");
            }

            base.OnExit(e);
        }
    }
}