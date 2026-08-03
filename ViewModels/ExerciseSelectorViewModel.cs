using FitZoneGymScheduler.DAL;
using FitZoneGymScheduler.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace FitZoneGymScheduler.ViewModels
{
    public class ExerciseSelectorViewModel : BaseViewModel
    {
        private readonly AppDbContext _context;

        // =========================================
        // ALL EXERCISES
        // =========================================

        public ObservableCollection<ExerciseLibrary> AllExercises
        {
            get;
            set;
        }

        // =========================================
        // FILTERED EXERCISES
        // =========================================

        public ObservableCollection<ExerciseLibrary> FilteredExercises
        {
            get;
            set;
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

                OnPropertyChanged();
            }
        }

        // =========================================
        // SEARCH TEXT
        // =========================================

        private string _searchText;

        public string SearchText
        {
            get => _searchText;

            set
            {
                _searchText = value;

                OnPropertyChanged();

                FilterExercises();
            }
        }

        // =========================================
        // CONSTRUCTOR
        // =========================================

        public ExerciseSelectorViewModel()
        {
            _context = new AppDbContext();

            AllExercises =
                new ObservableCollection<ExerciseLibrary>();

            FilteredExercises =
                new ObservableCollection<ExerciseLibrary>();

            LoadExercises();
        }

        // =========================================
        // LOAD EXERCISES
        // =========================================

        private void LoadExercises()
        {
            var exercises =
                _context.ExerciseLibraries.ToList();

            foreach (var exercise in exercises)
            {
                AllExercises.Add(exercise);

                FilteredExercises.Add(exercise);
            }
        }

        // =========================================
        // FILTER EXERCISES
        // =========================================

        private void FilterExercises()
        {
            FilteredExercises.Clear();

            var filtered = AllExercises;

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                filtered =
                    new ObservableCollection<ExerciseLibrary>(
                        AllExercises.Where(x =>
                            x.ExerciseName
                            .ToLower()
                            .Contains(SearchText.ToLower()))
                    );
            }

            foreach (var exercise in filtered)
            {
                FilteredExercises.Add(exercise);
            }
        }
    }
}