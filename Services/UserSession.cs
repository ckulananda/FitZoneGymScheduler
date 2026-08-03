using FitZoneGymScheduler.Models;

namespace FitZoneGymScheduler.Services
{
    public static class UserSession
    {
        public static User CurrentUser { get; set; }
        public static int CurrentLoginHistoryId { get; set; }

        public static DateTime LoginTime { get; set; }

        public static DateTime? LogoutTime { get; set; }
    }
}