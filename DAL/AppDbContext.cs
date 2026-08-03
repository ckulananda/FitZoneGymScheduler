using FitZoneGymScheduler.Helpers;
using FitZoneGymScheduler.Models;
using FitZoneGymScheduler.Services;
using Microsoft.EntityFrameworkCore;

namespace FitZoneGymScheduler.DAL
{
    public class AppDbContext : DbContext
    {
        // MEMBERS TABLE
        public DbSet<Member> Members { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<WorkoutPlan> WorkoutPlans { get; set; }

        public DbSet<WorkoutDay> WorkoutDays { get; set; }

        public DbSet<WorkoutSection> WorkoutSections { get; set; }

        public DbSet<WorkoutExercise> WorkoutExercises { get; set; }
        public DbSet<ExerciseLibrary> ExerciseLibraries { get; set; }
        public DbSet<UserLoginHistory> UserLoginHistories { get; set; }

        public DbSet<LoginAttempt> LoginAttempts { get; set; }

        protected override void OnConfiguring(
     DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    DatabaseSettings.GetConnectionString());
            }
        }

       
        
    }
}