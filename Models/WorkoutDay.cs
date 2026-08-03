using System.Collections.Generic;

namespace FitZoneGymScheduler.Models
{
    public class WorkoutDay
    {
        public int Id { get; set; }

        // PLAN CONNECTION
        public int WorkoutPlanId { get; set; }

        // DAY INFO
        public int DayNumber { get; set; }

        public bool IsRestDay { get; set; }

        // NAVIGATION
        public WorkoutPlan WorkoutPlan { get; set; }

        public ICollection<WorkoutSection> WorkoutSections { get; set; }
    }
}