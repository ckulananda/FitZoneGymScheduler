using System;
using System.Collections.Generic;

namespace FitZoneGymScheduler.Models
{
    public class WorkoutPlan
    {
        public int Id { get; set; }

        // MEMBER CONNECTION
        public int MemberId { get; set; }

        public string PlanName { get; set; }

        public string Goal { get; set; }

        public string Difficulty { get; set; }

        public string Notes { get; set; }

        public DateTime CreatedDate { get; set; }

        // NAVIGATION
        public Member Member { get; set; }

        public ICollection<WorkoutDay> WorkoutDays { get; set; }
    }
}