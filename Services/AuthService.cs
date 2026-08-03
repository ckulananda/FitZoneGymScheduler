using System;
using System.Linq;
using FitZoneGymScheduler.DAL;
using FitZoneGymScheduler.Models;

namespace FitZoneGymScheduler.Services
{
    public static class AuthService
    {
        public static void Logout()
        {
            try
            {
                if (UserSession.CurrentUser == null)
                    return;

                using var db = new AppDbContext();

                var session = db.UserLoginHistories
                    .Where(x =>
                        x.UserId == UserSession.CurrentUser.UserId &&
                        x.LogoutTime == null)
                    .OrderByDescending(x => x.LoginTime)
                    .FirstOrDefault();

                if (session != null)
                {
                    session.LogoutTime = DateTime.Now;

                    session.DurationMinutes =
                        (session.LogoutTime - session.LoginTime)?.TotalMinutes;

                    db.SaveChanges();
                }

                // clear session
                UserSession.CurrentUser = null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
    }
}