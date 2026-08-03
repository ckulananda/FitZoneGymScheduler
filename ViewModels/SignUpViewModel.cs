using FitZoneGymScheduler.Commands;
using FitZoneGymScheduler.Services;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using FitZoneGymScheduler.DAL;
using FitZoneGymScheduler.Models;
using System;
using System.Linq;
using Microsoft.Win32;

namespace FitZoneGymScheduler.ViewModels
{
    public class SignUpViewModel : BaseViewModel
    {
        // =====================================================
        // FIELDS
        // =====================================================

        private string _fullName;
        private string _username;
        private string _email;
        private string _phoneNumber;
        private string _password;
        private string _confirmPassword;
        private string _selectedRole;

        // =====================================================
        // PROPERTIES
        // =====================================================

        public string FullName
        {
            get => _fullName;
            set
            {
                _fullName = value;
                OnPropertyChanged();
            }
        }

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }

        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                _phoneNumber = value;
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set { _confirmPassword = value; OnPropertyChanged(); }
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


   

        public string SelectedRole
        {
            get => _selectedRole;
            set
            {
                _selectedRole = value;
                OnPropertyChanged();
            }
        }


        private string _profilePicturePath;

        public string ProfilePicturePath
        {
            get => _profilePicturePath;
            set
            {
                _profilePicturePath = value;
                OnPropertyChanged();
            }
        }

        // =====================================================
        // ROLE LIST
        // =====================================================

        public ObservableCollection<string> Roles { get; }
            = new ObservableCollection<string>
            {
                "Trainer",
                "Receptionist"
            };

        // =====================================================
        // COMMANDS
        // =====================================================

        public ICommand CreateAccountCommand { get; }

        public ICommand BackCommand { get; }

        public ICommand BrowsePictureCommand { get; }

        // =====================================================
        // CONSTRUCTOR
        // =====================================================

        public SignUpViewModel()
        {
            CreateAccountCommand =
                new RelayCommand(CreateAccount);

            BackCommand =
                new RelayCommand(Back);


            BrowsePictureCommand =
    new RelayCommand(BrowsePicture);
        }



        // =====================================================
        // CREATE ACCOUNT
        // =====================================================
        private void CreateAccount(object obj)
        {
            // =====================================================
            // FULL NAME VALIDATION
            // =====================================================

            if (string.IsNullOrWhiteSpace(FullName))
            {
                DialogService.Warning(
                    "Full Name Required",
                    "Please enter your full name.");

                return;
            }

            // =====================================================
            // USERNAME VALIDATION
            // =====================================================

            if (string.IsNullOrWhiteSpace(Username))
            {
                DialogService.Warning(
                    "Username Required",
                    "Please choose a username.");

                return;
            }

            // =====================================================
            // EMAIL VALIDATION
            // =====================================================

            if (string.IsNullOrWhiteSpace(Email))
            {
                DialogService.Warning(
                    "Email Required",
                    "Please enter your email address.");

                return;
            }

            // =====================================================
            // PHONE VALIDATION
            // =====================================================

            if (string.IsNullOrWhiteSpace(PhoneNumber))
            {
                DialogService.Warning(
                    "Phone Number Required",
                    "Please enter your phone number.");

                return;
            }

            if (!PhoneNumber.All(char.IsDigit))
            {
                DialogService.Warning(
                    "Invalid Phone Number",
                    "Phone number can only contain numbers.");

                return;
            }

            if (PhoneNumber.Length != 10)
            {
                DialogService.Warning(
                    "Invalid Phone Number",
                    "Phone number must contain exactly 10 digits.");

                return;
            }

            // =====================================================
            // PASSWORD VALIDATION
            // =====================================================

            if (string.IsNullOrWhiteSpace(Password))
            {
                DialogService.Warning(
                    "Password Required",
                    "Please enter a password.");

                return;
            }

            if (Password.Length < 6)
            {
                DialogService.Warning(
                    "Weak Password",
                    "Password must contain at least 6 characters.");

                return;
            }

            // =====================================================
            // CONFIRM PASSWORD VALIDATION
            // =====================================================

            if (Password != ConfirmPassword)
            {
                DialogService.Warning(
                    "Password Mismatch",
                    "Passwords do not match.");

                return;
            }

            // =====================================================
            // ROLE VALIDATION
            // =====================================================

            if (string.IsNullOrWhiteSpace(SelectedRole))
            {
                DialogService.Warning(
                    "Role Required",
                    "Please select a role.");

                return;
            }

            // =====================================================
            // SAVE USER
            // =====================================================

            try
            {
                using var db = new AppDbContext();

                // -----------------------------------------
                // DUPLICATE USERNAME CHECK
                // -----------------------------------------

                bool usernameExists =
                    db.Users.Any(x =>
                        x.Username == Username);

                if (usernameExists)
                {
                    DialogService.Warning(
                        "Username Exists",
                        "Please choose a different username.");

                    return;
                }

                // -----------------------------------------
                // DUPLICATE EMAIL CHECK
                // -----------------------------------------

                bool emailExists =
                    db.Users.Any(x =>
                        x.Email == Email);

                if (emailExists)
                {
                    DialogService.Warning(
                        "Email Already Used",
                        "This email address is already registered.");

                    return;
                }

                // -----------------------------------------
                // CREATE USER OBJECT
                // -----------------------------------------

                var user = new User
                {
                    FullName = FullName.Trim(),
                    Username = Username.Trim(),
                    PasswordHash = PasswordService.HashPassword(Password),
                    Email = Email.Trim(),
                    PhoneNumber = PhoneNumber.Trim(),
                    Role = SelectedRole,
                    ProfilePicturePath = ProfilePicturePath,
                    CreatedDate = DateTime.Now,
                    IsActive = true
                };

                // -----------------------------------------
                // SAVE TO DATABASE
                // -----------------------------------------

                db.Users.Add(user);

                db.SaveChanges();

                // -----------------------------------------
                // SUCCESS MESSAGE
                // -----------------------------------------

                DialogService.Success(
                    "Account Created",
                    $"Welcome {FullName}! Your account has been created successfully.");

                // -----------------------------------------
                // CLEAR FORM
                // -----------------------------------------

                ClearForm();

                // -----------------------------------------
                // CLOSE SIGNUP WINDOW
                // -----------------------------------------

                foreach (Window window in Application.Current.Windows)
                {
                    if (window.GetType().Name == "SignUpWindow")
                    {
                        window.Close();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                DialogService.Error(
                    "Database Error",
                    ex.Message);
            }
        }



        private void ClearForm()
        {
            FullName = string.Empty;
            Username = string.Empty;
            Email = string.Empty;
            PhoneNumber = string.Empty;
            Password = string.Empty;
            ConfirmPassword = string.Empty;
            SelectedRole = null;
            ProfilePicturePath = null;
        }

        // =====================================================
        // BACK TO LOGIN
        // =====================================================

        private void Back(object obj)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.GetType().Name == "SignUpWindow")
                {
                    window.Close();
                    break;
                }
            }
        }

        private void BrowsePicture(object obj)
        {
            var dialog = new OpenFileDialog();

            dialog.Title = "Select Profile Picture";

            dialog.Filter =
                "Image Files|*.jpg;*.jpeg;*.png;*.bmp";

            if (dialog.ShowDialog() == true)
            {
                ProfilePicturePath =
                    dialog.FileName;
            }
        }
    }
}