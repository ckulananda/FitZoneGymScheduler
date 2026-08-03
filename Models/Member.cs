using System;
using System.Collections.Generic;

namespace FitZoneGymScheduler.Models
{
    public class Member
    {
        public int Id { get; set; }

        public string FullName { get; set; }

        public int Age { get; set; }

        public string Gender { get; set; }

        public string Country { get; set; }

        // HEIGHT
        public double Height { get; set; }

        public string HeightUnit { get; set; }

        // WEIGHT
        public double Weight { get; set; }

        public string WeightUnit { get; set; }

        // BMI
        public double BMI { get; set; }

        public string BMIWord { get; set; }

        // FITNESS
        public string FitnessLevel { get; set; }

        public string FitnessGoal { get; set; }

        // CONTACT
        public string PhoneNumber { get; set; }

        // SYSTEM
        public DateTime JoinDate { get; set; }

        public string Notes { get; set; }

        public ICollection<WorkoutPlan> WorkoutPlans { get; set; }
    }
}