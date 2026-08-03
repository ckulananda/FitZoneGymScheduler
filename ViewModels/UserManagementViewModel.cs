using FitZoneGymScheduler.Commands;
using FitZoneGymScheduler.DAL;
using FitZoneGymScheduler.Models;
using FitZoneGymScheduler.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace FitZoneGymScheduler.ViewModels
{
    public class UserManagementViewModel : BaseViewModel
    {
        // =====================================================
        // FIELDS
        // =====================================================

        private ObservableCollection<User> _users;
        private User _selectedUser;
        private string _searchText;
        private string _selectedRole;

        // =====================================================
        // PROPERTIES
        // =====================================================

        public string EditFullName { get; set; }
        public string EditUsername { get; set; }
        public string EditEmail { get; set; }
        public string EditPhoneNumber { get; set; }
        public string EditRole { get; set; }
        public string EditPassword { get; set; }
        public string EditProfilePicture { get; set; }




        public ObservableCollection<User> Users
        {
            get => _users;
            set
            {
                _users = value;
                OnPropertyChanged();
            }
        }

        public User SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                OnPropertyChanged();

                LoadSelectedUserToEdit();
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                LoadUsers();
            }
        }

        public string SelectedRole
        {
            get => _selectedRole;
            set
            {
                _selectedRole = value;
                OnPropertyChanged();
                LoadUsers();
            }
        }

        public List<string> Roles { get; }

        // =====================================================
        // COMMANDS
        // =====================================================

        public ICommand RefreshCommand { get; }
        public ICommand DeactivateUserCommand { get; }
        public ICommand ActivateUserCommand { get; }
        public ICommand DeleteUserCommand { get; }
        public ICommand EditUserCommand { get; }
        public string Password { get; set; }
        public string ProfilePicturePath { get; set; }
        public ICommand BrowseImageCommand { get; }

        public ICommand SaveUserCommand { get; }

        // =====================================================
        // CONSTRUCTOR
        // =====================================================

        public UserManagementViewModel()
        {
            Roles = new List<string>
            {
                "All",
                "Administrator",
                "Trainer",
                "Receptionist"
            };

            SelectedRole = "All";

            RefreshCommand = new RelayCommand(_ => LoadUsers());
            DeactivateUserCommand = new RelayCommand(DeactivateUser);
            ActivateUserCommand = new RelayCommand(ActivateUser);
            DeleteUserCommand = new RelayCommand(DeleteUser);
            EditUserCommand = new RelayCommand(EditUser);
            BrowseImageCommand = new RelayCommand(BrowseImage);
            SaveUserCommand = new RelayCommand(SaveUser);

            // IMPORTANT: load after everything is initialized
            LoadUsers();
        }

        // =====================================================
        // LOAD USERS (SAFE VERSION)
        // =====================================================

        private void LoadUsers()
        {
            try
            {
                using var db = new AppDbContext();

                // ==============================
                // DEBUG CHECK (DB CONNECTION)
                // ==============================
                var allUsers = db.Users.ToList();
                System.Diagnostics.Debug.WriteLine($"TOTAL USERS IN DB: {allUsers.Count}");

                // ==============================
                // BASE QUERY
                // ==============================
                var query = db.Users
                    .AsNoTracking()
                    .AsQueryable();

                // ==============================
                // SEARCH FILTER
                // ==============================
                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    query = query.Where(x =>
                        x.FullName.Contains(SearchText) ||
                        x.Username.Contains(SearchText) ||
                        x.Email.Contains(SearchText));
                }

                // ==============================
                // ROLE FILTER
                // ==============================
                if (!string.IsNullOrWhiteSpace(SelectedRole) &&
                    SelectedRole != "All")
                {
                    query = query.Where(x => x.Role == SelectedRole);
                }

                // ==============================
                // EXECUTE QUERY
                // ==============================
                var result = query
                    .OrderBy(x => x.FullName)
                    .ToList();

                // ==============================
                // UPDATE UI
                // ==============================
                Users = new ObservableCollection<User>(result);
            }
            catch (Exception ex)
            {
                DialogService.Error("Load Failed", ex.Message);
            }
        }
        // =====================================================
        // DEACTIVATE USER
        // =====================================================

        private void DeactivateUser(object obj)
        {
            if (SelectedUser == null)
            {
                DialogService.Warning("Select User", "Please select a user first.");
                return;
            }

            if (SelectedUser.Role == "Administrator")
            {
                DialogService.Warning("Blocked", "Admin cannot be deactivated.");
                return;
            }

            if (!DialogService.Confirm("Deactivate", $"Deactivate {SelectedUser.FullName}?"))
                return;

            using var db = new AppDbContext();

            var user = db.Users.FirstOrDefault(x => x.UserId == SelectedUser.UserId);

            if (user == null) return;

            user.IsActive = false;
            db.SaveChanges();

            DialogService.Success("Done", "User deactivated");

            LoadUsers();
        }

        // =====================================================
        // ACTIVATE USER
        // =====================================================

        private void ActivateUser(object obj)
        {
            if (SelectedUser == null)
            {
                DialogService.Warning("Select User", "Please select a user first.");
                return;
            }

            using var db = new AppDbContext();

            var user = db.Users.FirstOrDefault(x => x.UserId == SelectedUser.UserId);

            if (user == null) return;

            user.IsActive = true;
            db.SaveChanges();

            DialogService.Success("Done", "User activated");

            LoadUsers();
        }


        private void DeleteUser(object obj)
        {
            if (SelectedUser == null)
            {
                DialogService.Warning("Select User", "Please select a user first.");
                return;
            }

            // 🚨 Prevent deleting admins
            if (SelectedUser.Role == "Administrator")
            {
                DialogService.Warning("Blocked", "Administrator accounts cannot be deleted.");
                return;
            }

            // 🔐 Confirm dialog
            bool confirm = DialogService.Confirm(
                "Delete User",
                $"Are you sure you want to permanently delete {SelectedUser.FullName}?");

            if (!confirm)
                return;

            try
            {
                using var db = new AppDbContext();

                var user = db.Users.FirstOrDefault(x =>
                    x.UserId == SelectedUser.UserId);

                if (user == null)
                    return;

                db.Users.Remove(user);
                db.SaveChanges();

                DialogService.Success("Deleted", "User removed successfully.");

                LoadUsers();
            }
            catch (Exception ex)
            {
                DialogService.Error("Error", ex.Message);
            }
        }

        private void EditUser(object obj)
        {
            if (SelectedUser == null)
            {
                DialogService.Warning("Select User", "Please select a user first.");
                return;
            }

            try
            {
                using var db = new AppDbContext();

                var user = db.Users.FirstOrDefault(x =>
                    x.UserId == SelectedUser.UserId);

                if (user == null)
                    return;

                // =========================
                // BASIC INFO
                // =========================
                user.FullName = SelectedUser.FullName;
                user.Username = SelectedUser.Username;
                user.Email = SelectedUser.Email;
                user.PhoneNumber = SelectedUser.PhoneNumber;
                user.Role = SelectedUser.Role;

                // =========================
                // PASSWORD (ONLY IF ENTERED)
                // =========================
                if (!string.IsNullOrWhiteSpace(Password))
                {
                    user.PasswordHash = PasswordService.HashPassword(Password);
                }

                // =========================
                // PROFILE PICTURE
                // =========================
                if (!string.IsNullOrWhiteSpace(ProfilePicturePath))
                {
                    user.ProfilePicturePath = ProfilePicturePath;
                }

                db.SaveChanges();

                DialogService.Success("Updated", "User updated successfully.");

                LoadUsers();
            }
            catch (Exception ex)
            {
                DialogService.Error("Error", ex.Message);
            }
        }

        private void BrowseImage(object obj)
        {
            try
            {
                // ✅ SAFETY CHECK
                if (SelectedUser == null)
                {
                    DialogService.Warning("No User Selected", "Please select a user first.");
                    return;
                }

                var dialog = new Microsoft.Win32.OpenFileDialog
                {
                    Filter = "Image Files|*.jpg;*.jpeg;*.png"
                };

                if (dialog.ShowDialog() == true)
                {
                    EditProfilePicture = dialog.FileName;

                    // update selected user preview instantly
                    SelectedUser.ProfilePicturePath = dialog.FileName;

                    OnPropertyChanged(nameof(EditProfilePicture));
                    OnPropertyChanged(nameof(SelectedUser));
                }
            }
            catch (Exception ex)
            {
                DialogService.Error("Image Error", ex.Message);
            }
        }


        private void LoadSelectedUserToEdit()
        {
            if (SelectedUser == null)
                return;

            EditFullName = SelectedUser.FullName;
            EditUsername = SelectedUser.Username;
            EditEmail = SelectedUser.Email;
            EditPhoneNumber = SelectedUser.PhoneNumber;
            EditRole = SelectedUser.Role;
            EditProfilePicture = SelectedUser.ProfilePicturePath;

            OnPropertyChanged(string.Empty);
        }


        private void SaveUser(object obj)
        {
            if (SelectedUser == null)
                return;

            using var db = new AppDbContext();

            var user = db.Users.FirstOrDefault(x =>
                x.UserId == SelectedUser.UserId);

            if (user == null)
                return;

            // =========================
            // BASIC INFO
            // =========================
            user.FullName = EditFullName;
            user.Username = EditUsername;
            user.Email = EditEmail;
            user.PhoneNumber = EditPhoneNumber;
            user.Role = EditRole;

            // =========================
            // PASSWORD (ONLY IF ENTERED)
            // =========================
            if (!string.IsNullOrWhiteSpace(EditPassword))
            {
                user.PasswordHash = PasswordService.HashPassword(EditPassword);
            }

            // =========================
            // PROFILE IMAGE
            // =========================
            if (!string.IsNullOrWhiteSpace(EditProfilePicture))
            {
                user.ProfilePicturePath = EditProfilePicture;
            }

            db.SaveChanges();

            DialogService.Success("Updated", "User updated successfully.");

            LoadUsers();
        }
    }
}