using FitZoneGymScheduler.Commands;
using FitZoneGymScheduler.DAL;
using FitZoneGymScheduler.Models;
using FitZoneGymScheduler.Views;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace FitZoneGymScheduler.ViewModels
{
    public class WorkoutSectionItem : BaseViewModel
    {
        private string _sectionName;

        public string SectionName
        {
            get => _sectionName;

            set
            {
                _sectionName = value;
                OnPropertyChanged();
            }
        }

        // AVAILABLE SECTION TYPES

        public ObservableCollection<string> SectionTypes
        {
            get;
            set;
        }

        // EXERCISES

        public ObservableCollection<WorkoutExerciseItem> Exercises
        {
            get;
            set;
        }

        // COMMAND

        public ICommand AddExerciseCommand { get; }

        public ICommand OpenExerciseSelectorCommand { get; }


        public WorkoutSectionItem()
        {
            Exercises =
                new ObservableCollection<WorkoutExerciseItem>();

            // SECTION TYPES

            SectionTypes =
                new ObservableCollection<string>
                {
                    "Cardio",
                    "Upper Body",
                    "Lower Body",
                    "Push",
                    "Pull",
                    "Legs",
                    "Core",
                    "Mobility"
                };

            AddExerciseCommand =
                new RelayCommand(AddExercise);

            OpenExerciseSelectorCommand =
    new RelayCommand(OpenExerciseSelector);

        }

        private void OpenExerciseSelector(object obj)
        {
            var selectorWindow =
                new ExerciseSelectorWindow();

            var result =
                selectorWindow.ShowDialog();

            if (result == true)
            {
                if (selectorWindow.SelectedExercise != null)
                {
                    var selected =
                        selectorWindow.SelectedExercise;

                    Exercises.Add(
                        new WorkoutExerciseItem
                        {
                            SelectedExercise = selected,

                            ExerciseName =
                                selected.ExerciseName,

                            Sets = 3,

                            RepsOrDuration = "12",

                            Notes = ""
                        });
                }
            }
        }



        // ADD EXERCISE

        private void AddExercise(object obj)
        {
            var context =
                new AppDbContext();

            var exerciseItem =
                new WorkoutExerciseItem();

            // LOAD EXERCISES

            var exercises =
                context.ExerciseLibraries.ToList();

            // FILTER BY SECTION TYPE

            if (!string.IsNullOrEmpty(SectionName))
            {
                exercises =
                    exercises
                    .Where(x => x.TargetArea == SectionName)
                    .ToList();
            }

            // ADD TO DROPDOWN

            foreach (var exercise in exercises)
            {
                exerciseItem.AllExercises.Add(exercise);

                exerciseItem.AvailableExercises.Add(exercise);
            }

            Exercises.Add(exerciseItem);
        }
    }
}