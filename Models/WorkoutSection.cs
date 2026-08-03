using System.Collections.Generic;

namespace FitZoneGymScheduler.Models
{
    public class WorkoutSection
    {
        public int Id { get; set; }

        // DAY CONNECTION
        public int WorkoutDayId { get; set; }

        // SECTION
        public string SectionName { get; set; }

        // NAVIGATION
        public WorkoutDay WorkoutDay { get; set; }

        public ICollection<WorkoutExercise> WorkoutExercises { get; set; }
    }
}