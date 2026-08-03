using FitZoneGymScheduler.Commands;
using FitZoneGymScheduler.DAL;
using FitZoneGymScheduler.Models;
using FitZoneGymScheduler.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using FitZoneGymScheduler.Helpers;
using FitZoneGymScheduler.Views;

namespace FitZoneGymScheduler.ViewModels
{
    public class ExerciseLibraryViewModel : BaseViewModel
    {
        private readonly AppDbContext _context;

        // COLLECTION

        public ObservableCollection<ExerciseLibrary> Exercises
        {
            get;
            set;
        }

        // SELECTED ITEM

        private ExerciseLibrary _selectedExercise;

        public ExerciseLibrary SelectedExercise
        {
            get => _selectedExercise;

            set
            {
                _selectedExercise = value;

                if (_selectedExercise != null)
                {
                    LoadSelectedExercise();
                }

                OnPropertyChanged();
            }
        }

        // FORM FIELDS

        private string _exerciseName;

        public string ExerciseName
        {
            get => _exerciseName;
            set
            {
                _exerciseName = value;
                OnPropertyChanged();
            }
        }

        private string _targetArea;

        public string TargetArea
        {
            get => _targetArea;
            set
            {
                _targetArea = value;
                OnPropertyChanged();
            }
        }

        private string _difficulty;

        public string Difficulty
        {
            get => _difficulty;
            set
            {
                _difficulty = value;
                OnPropertyChanged();
            }
        }

        private string _description;

        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }

        private string _notes;

        public string Notes
        {
            get => _notes;
            set
            {
                _notes = value;
                OnPropertyChanged();
            }
        }

        // SEARCH

        private string _searchText;

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();

                SearchExercises();
            }
        }

        // DROPDOWNS

        public ObservableCollection<string> TargetAreas
        {
            get;
            set;
        }

        public ObservableCollection<string> DifficultyLevels
        {
            get;
            set;
        }

        // COMMANDS

        public ICommand SaveExerciseCommand { get; }

        public ICommand UpdateExerciseCommand { get; }

        public ICommand DeleteExerciseCommand { get; }

        public ExerciseLibraryViewModel()
        {
            _context = new AppDbContext();

            Exercises =
                new ObservableCollection<ExerciseLibrary>();

            // TARGET AREAS

            TargetAreas =
                new ObservableCollection<string>
                {
                    "Cardio",
                    "Upper Body",
                    "Lower Body",
                    "Core",
                    "Push",
                    "Pull",
                    "Legs",
                    "Mobility"
                };

            // DIFFICULTY

            DifficultyLevels =
                new ObservableCollection<string>
                {
                    "Beginner",
                    "Intermediate",
                    "Advanced"
                };

            // COMMANDS

            SaveExerciseCommand =
                new RelayCommand(SaveExercise);

            UpdateExerciseCommand =
                new RelayCommand(UpdateExercise);

            DeleteExerciseCommand =
                new RelayCommand(DeleteExercise);

            LoadExercises();
        }





        // LOAD ALL

        private void LoadExercises()
        {
            Exercises.Clear();

            var exerciseList =
                _context.ExerciseLibraries.ToList();

            foreach (var exercise in exerciseList)
            {
                Exercises.Add(exercise);
            }
        }



        private bool ValidateExercise()
        {
            if (string.IsNullOrWhiteSpace(ExerciseName))
            {
                DialogService.Warning(
                    "Missing Information",
                    "Please enter an exercise name.");

                return false;
            }

            if (string.IsNullOrWhiteSpace(TargetArea))
            {
                DialogService.Warning(
                    "Missing Information",
                    "Please select a target area.");

                return false;
            }

            if (string.IsNullOrWhiteSpace(Difficulty))
            {
                DialogService.Warning(
                    "Missing Information",
                    "Please select a difficulty level.");

                return false;
            }

            return true;
        }


        // SAVE

        private void SaveExercise(object obj)
        {
            if (!ValidateExercise())
                return;

            bool confirmed =
                DialogService.Confirm(
                    "Add Exercise",
                    $"Do you want to add '{ExerciseName}' to the exercise library?");

            if (!confirmed)
                return;

            var exercise =
     new ExerciseLibrary
     {
         ExerciseName = ExerciseName,
         TargetArea = TargetArea,
         Difficulty = Difficulty,
         Description = string.IsNullOrWhiteSpace(Description)
    ? "No description is provided."
    : Description,
         Notes = string.IsNullOrWhiteSpace(Notes)
             ? "No additional notes provided."
             : Notes
     };
            _context.ExerciseLibraries.Add(exercise);

            _context.SaveChanges();

            LoadExercises();

            ClearFields();

            DialogService.Success(
                "Exercise Added",
                $"'{exercise.ExerciseName}' was added successfully.");
        }


        // UPDATE

        private void UpdateExercise(object obj)
        {
            if (SelectedExercise == null)
            {
                DialogService.Warning(
                    "No Selection",
                    "Please select an exercise to update.");

                return;
            }

            bool confirmed =
                DialogService.Confirm(
                    "Update Exercise",
                    $"Do you want to update '{ExerciseName}'?");

            if (!confirmed)
                return;

            SelectedExercise.ExerciseName = ExerciseName;
            SelectedExercise.TargetArea = TargetArea;
            SelectedExercise.Difficulty = Difficulty;
            SelectedExercise.Description = 
                string.IsNullOrWhiteSpace(Description)
                ?"No Description is Provided."
                : Description;

            SelectedExercise.Notes =
      string.IsNullOrWhiteSpace(Notes)
          ? "No additional notes provided."
          : Notes;

            _context.SaveChanges();

            LoadExercises();

            ClearFields();

            DialogService.Success(
                "Exercise Updated",
                "Exercise updated successfully.");
        }



        // DELETE
        private void DeleteExercise(object obj)
        {
            if (SelectedExercise == null)
            {
                DialogService.Warning(
                    "No Selection",
                    "Please select an exercise to delete.");

                return;
            }

            bool confirmed =
                DialogService.Confirm(
                    "Delete Exercise",
                    $"Are you sure you want to permanently delete '{SelectedExercise.ExerciseName}'?\n\nThis action cannot be undone.");

            if (!confirmed)
                return;

            _context.ExerciseLibraries.Remove(SelectedExercise);

            _context.SaveChanges();

            LoadExercises();

            ClearFields();

            DialogService.Success(
                "Exercise Deleted",
                "Exercise removed successfully.");
        }



        // LOAD SELECTED

        private void LoadSelectedExercise()
        {
            ExerciseName = SelectedExercise.ExerciseName;
            TargetArea = SelectedExercise.TargetArea;
            Difficulty = SelectedExercise.Difficulty;
            Description = SelectedExercise.Description;
            Notes = SelectedExercise.Notes;

            DialogService.Success(
                "Exercise Loaded",
                $"'{ExerciseName}' is ready to edit.");
        }

        // SEARCH

        private void SearchExercises()
        {
            Exercises.Clear();

            if (string.IsNullOrWhiteSpace(SearchText))
            {
                LoadExercises();
                return;
            }

            var filtered =
                _context.ExerciseLibraries
                .Where(x =>
                    x.ExerciseName.Contains(SearchText) ||
                    x.TargetArea.Contains(SearchText) ||
                    x.Difficulty.Contains(SearchText))
                .ToList();

            foreach (var item in filtered)
            {
                Exercises.Add(item);
            }
        }

        // CLEAR

        private void ClearFields()
        {
            ExerciseName = string.Empty;
            TargetArea = null;
            Difficulty = null;
            Description = string.Empty;
            Notes = string.Empty;

            SelectedExercise = null;
        }
    }
}