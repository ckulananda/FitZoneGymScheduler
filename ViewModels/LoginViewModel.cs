using FitZoneGymScheduler.Commands;
using FitZoneGymScheduler.Services;
using FitZoneGymScheduler.Views;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FitZoneGymScheduler.DAL;


using FitZoneGymScheduler.Models;
using Microsoft.EntityFrameworkCore;



namespace FitZoneGymScheduler.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        // =====================================================
        // FIELDS
        // =====================================================

        private string _username;

        // =====================================================
        // PROPERTIES
        // =====================================================




        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }


        private bool _isPasswordVisible;

        public bool IsPasswordVisible
        {
            get => _isPasswordVisible;
            set
            {
                _isPasswordVisible = value;
                OnPropertyChanged();
            }
        }

        private string _password;

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        // =====================================================
        // COMMANDS
        // =====================================================

        public ICommand LoginCommand { get; }

        public ICommand OpenSignUpCommand { get; }

        // =====================================================
        // CONSTRUCTOR
        // =====================================================

        public LoginViewModel()
        {
            // Login button command
            LoginCommand =
                new RelayCommand(Login);

            // Sign Up button command
            OpenSignUpCommand =
                new RelayCommand(OpenSignUp);
        }

        // =====================================================
        // OPEN SIGN UP WINDOW
        // =====================================================

        private void OpenSignUp(object obj)
        {
            var signUpWindow =
                new SignUpWindow();

            signUpWindow.ShowDialog();
        }

        // =====================================================
        // LOGIN PROCESS
        // =====================================================



        private async void Login(object parameter)
        {


            if (parameter is not PasswordBox passwordBox)
                return;

            string password = passwordBox.Password;

            // =====================================================
            // VALIDATION
            // =====================================================

            if (string.IsNullOrWhiteSpace(Username))
            {
                DialogService.Warning(
                    "Username Required",
                    "Please enter your username.");
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                DialogService.Warning(
                    "Password Required",
                    "Please enter your password.");
                return;
            }

            try
            {
                App.LoadingService.Show("Signing In...");

                await Task.Delay(100);

                using var db = new AppDbContext();

                // =====================================================
                // GET USER
                // =====================================================

                var user = await Task.Run(() =>
                    db.Users.FirstOrDefault(u =>
                        u.Username == Username &&
                        u.IsActive));

                // =====================================================
                // USER NOT FOUND
                // =====================================================

                if (user == null)
                {
                    db.LoginAttempts.Add(new LoginAttempt
                    {
                        Username = Username,
                        IsSuccess = false,
                        Message = "User not found",
                        AttemptTime = DateTime.Now
                    });

                    await db.SaveChangesAsync();

                    App.LoadingService.Hide();

                    DialogService.Error(
                        "Login Failed",
                        "User account was not found.");

                    return;
                }

                // =====================================================
                // CHECK ACCOUNT LOCK
                // =====================================================

                if (user.IsLocked && user.LockedUntil > DateTime.Now)
                {
                    db.LoginAttempts.Add(new LoginAttempt
                    {
                        UserId = user.UserId,
                        Username = Username,
                        IsSuccess = false,
                        Message = "Account locked",
                        AttemptTime = DateTime.Now
                    });

                    await db.SaveChangesAsync();

                    App.LoadingService.Hide();

                    DialogService.Error(
                        "Account Locked",
                        "Too many failed attempts. Try again later.");

                    return;
                }

                // =====================================================
                // PASSWORD CHECK
                // =====================================================

                bool isValid =
              PasswordService.VerifyPassword(
    user.PasswordHash,
    password);

                var attempt = new LoginAttempt
                {
                    UserId = user.UserId,
                    Username = Username,
                    IsSuccess = isValid,
                    AttemptTime = DateTime.Now
                };

                // =====================================================
                // FAILED LOGIN
                // =====================================================

                if (!isValid)
                {
                    user.FailedLoginAttempts++;

                    if (user.FailedLoginAttempts >= 5)
                    {
                        user.IsLocked = true;
                        user.LockedUntil = DateTime.Now.AddMinutes(15);

                        attempt.Message =
                            "Account locked after 5 failed attempts";

                        db.LoginAttempts.Add(attempt);

                        await db.SaveChangesAsync();

                        App.LoadingService.Hide();

                        DialogService.Error(
                            "Account Locked",
                            "Too many failed attempts. Try again after 15 minutes.");
                    }
                    else
                    {
                        attempt.Message = "Invalid password";

                        db.LoginAttempts.Add(attempt);

                        await db.SaveChangesAsync();

                        App.LoadingService.Hide();

                        DialogService.Error(
                            "Login Failed",
                            $"Incorrect password. Attempt {user.FailedLoginAttempts}/5");
                    }

                    return;
                }

                // =====================================================
                // SUCCESS LOGIN
                // =====================================================

                user.FailedLoginAttempts = 0;
                user.IsLocked = false;
                user.LockedUntil = null;

                db.LoginAttempts.Add(new LoginAttempt
                {
                    UserId = user.UserId,
                    Username = Username,
                    IsSuccess = true,
                    Message = "Login successful",
                    AttemptTime = DateTime.Now
                });

                // =====================================================
                // SESSION CREATION
                // =====================================================

                var loginHistory = new UserLoginHistory
                {
                    UserId = user.UserId,
                    LoginTime = DateTime.Now
                };

                db.UserLoginHistories.Add(loginHistory);

                App.LoadingService.Show("Log In...");

                await db.SaveChangesAsync();

              

                // =====================================================
                // SESSION STORAGE
                // =====================================================

                UserSession.CurrentUser = user;
                UserSession.LoginTime = DateTime.Now;
                UserSession.CurrentLoginHistoryId = loginHistory.Id;

                App.LoadingService.Hide();

                // =====================================================
                // NAVIGATE TO MAIN WINDOW
                // =====================================================

                DialogService.Success(
                    "Welcome Back",
                    $"Welcome, {user.FullName}!");

                var mainWindow = new MainWindow();

                Application.Current.MainWindow = mainWindow;

                mainWindow.Show();

                foreach (Window window in Application.Current.Windows)
                {
                    if (window is Views.LoginWindow)
                    {
                        window.Close();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                App.LoadingService.Hide();

                DialogService.Error(
                    "System Error",
                    ex.Message);
            }
        }
    }
}