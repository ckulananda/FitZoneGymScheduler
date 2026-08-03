using System;

namespace FitZoneGymScheduler.Models
{
    public class UserLoginHistory
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public DateTime LoginTime { get; set; }

        public DateTime? LogoutTime { get; set; }

        public double? DurationMinutes { get; set; }

        public User User { get; set; }
    }
}