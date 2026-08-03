using System;
using System.Collections.Generic;

namespace FitZoneGymScheduler.Models
{
    public class User
    {
        public int UserId { get; set; }

        public string FullName { get; set; }

        public string Username { get; set; }

        public string PasswordHash { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public string Role { get; set; }

        public string? ProfilePicturePath { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool IsActive { get; set; }

        public ICollection<UserLoginHistory> LoginHistory { get; set; }
            = new List<UserLoginHistory>();

        public int FailedLoginAttempts { get; set; }

        public bool IsLocked { get; set; }

        public DateTime? LockedUntil { get; set; }
    }
}