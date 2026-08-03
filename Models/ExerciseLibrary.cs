using System.Collections.Generic;

namespace FitZoneGymScheduler.Models
{
    public class ExerciseLibrary
    {
        public int Id { get; set; }

        // BASIC INFO
        public string ExerciseName { get; set; }

        public string TargetArea { get; set; }

        public string Difficulty { get; set; }

        // OPTIONAL DETAILS
        public string Description { get; set; }

        public string Notes { get; set; }

        // NAVIGATION
        public ICollection<WorkoutExercise> WorkoutExercises { get; set; }
    }
}