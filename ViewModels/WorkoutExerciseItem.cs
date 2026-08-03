using FitZoneGymScheduler.Models;
using System.Collections.ObjectModel;
using System.Linq;
using FitZoneGymScheduler.Commands;
using FitZoneGymScheduler.Views;
using System.Windows.Input;

namespace FitZoneGymScheduler.ViewModels
{
    public class WorkoutExerciseItem : BaseViewModel
    {
        // =========================================
        // AVAILABLE EXERCISES
        // =========================================

        public ObservableCollection<ExerciseLibrary> AvailableExercises
        {
            get;
            set;
        }

        // =========================================
        // ALL EXERCISES
        // =========================================

        public ObservableCollection<ExerciseLibrary> AllExercises
        {
            get;
            set;
        }

        // =========================================
        // COMMANDS
        // =========================================

        public ICommand OpenExerciseSelectorCommand
        {
            get;
        }

        // =========================================
        // SELECTED EXERCISE
        // =========================================

        private ExerciseLibrary _selectedExercise;

        public ExerciseLibrary SelectedExercise
        {
            get => _selectedExercise;

            set
            {
                _selectedExercise = value;

                if (_selectedExercise != null)
                {
                    ExerciseName =
                        _selectedExercise.ExerciseName;
                }

                OnPropertyChanged();
            }
        }

        // =========================================
        // EXERCISE DISPLAY NAME
        // =========================================

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

        // =========================================
        // SEARCH TEXT
        // =========================================

        private string _exerciseSearchText;

        public string ExerciseSearchText
        {
            get => _exerciseSearchText;

            set
            {
                _exerciseSearchText = value;

                ExerciseName = value;

                OnPropertyChanged();

                FilterExercises();
            }
        }

        // =========================================
        // SETS
        // =========================================

        private int _sets;

        public int Sets
        {
            get => _sets;

            set
            {
                _sets = value;

                OnPropertyChanged();
            }
        }

        // =========================================
        // REPS / DURATION
        // =========================================

        private string _repsOrDuration;

        public string RepsOrDuration
        {
            get => _repsOrDuration;

            set
            {
                _repsOrDuration = value;

                OnPropertyChanged();
            }
        }

        // =========================================
        // NOTES
        // =========================================

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

        // =========================================
        // CONSTRUCTOR
        // =========================================

        public WorkoutExerciseItem()
        {
            AvailableExercises =
                new ObservableCollection<ExerciseLibrary>();

            AllExercises =
                new ObservableCollection<ExerciseLibrary>();

            OpenExerciseSelectorCommand =
                new RelayCommand(OpenExerciseSelector);
        }

        // =========================================
        // OPEN EXERCISE SELECTOR
        // =========================================

        private void OpenExerciseSelector(object obj)
        {
            var window = new ExerciseSelectorWindow();

            if (window.ShowDialog() == true)
            {
                SelectedExercise = window.SelectedExercise;

                if (SelectedExercise != null)
                {
                    ExerciseName =
                        SelectedExercise.ExerciseName;
                }
            }
        }

        // =========================================
        // FILTER EXERCISES
        // =========================================

        private void FilterExercises()
        {
            AvailableExercises.Clear();

            var filtered =
                AllExercises;

            if (!string.IsNullOrWhiteSpace(
                ExerciseSearchText))
            {
                filtered =
                    new ObservableCollection<ExerciseLibrary>(
                        AllExercises.Where(x =>
                            x.ExerciseName
                             .ToLower()
                             .Contains(
                                 ExerciseSearchText
                                 .ToLower())));
            }

            foreach (var exercise in filtered)
            {
                AvailableExercises.Add(exercise);
            }
        }
    }
}