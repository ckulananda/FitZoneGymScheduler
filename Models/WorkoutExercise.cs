namespace FitZoneGymScheduler.Models
{
    public class WorkoutExercise
    {
        public int Id { get; set; }

        public int WorkoutSectionId { get; set; }

        // EXERCISE REFERENCE
        public int ExerciseLibraryId { get; set; }

        public int Sets { get; set; }

        public string RepsOrDuration { get; set; }

        public string Notes { get; set; }

        // NAVIGATION
        public WorkoutSection WorkoutSection { get; set; }

        public ExerciseLibrary ExerciseLibrary { get; set; }
    }
}