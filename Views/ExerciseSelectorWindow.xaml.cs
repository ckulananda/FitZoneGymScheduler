using FitZoneGymScheduler.DAL;
using FitZoneGymScheduler.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace FitZoneGymScheduler.Views
{
    public partial class ExerciseSelectorWindow : Window
    {
        private readonly AppDbContext _context;

        private List<ExerciseLibrary> _allExercises;

        public ExerciseLibrary SelectedExercise
        {
            get;
            private set;
        }

        public ExerciseSelectorWindow()
        {
            InitializeComponent();

            _context = new AppDbContext();

            LoadExercises();
        }

        private void LoadExercises()
        {
            _allExercises =
                _context.ExerciseLibraries.ToList();

            ExerciseGrid.ItemsSource =
                _allExercises;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string search =
                SearchBox.Text.ToLower();

            var filtered =
                _allExercises
                .Where(x =>
                    x.ExerciseName.ToLower().Contains(search) ||
                    x.TargetArea.ToLower().Contains(search))
                .ToList();

            ExerciseGrid.ItemsSource =
                filtered;
        }

        private void ExerciseGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ExerciseGrid.SelectedItem is ExerciseLibrary exercise)
            {
                SelectedExercise = exercise;

                ExerciseNameText.Text =
                    exercise.ExerciseName;

                TargetAreaText.Text =
                    exercise.TargetArea;

                DifficultyText.Text =
                    exercise.Difficulty;

                DescriptionText.Text =
                    exercise.Description;

                NotesText.Text =
                    exercise.Notes;
            }
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedExercise == null)
            {
                MessageBox.Show("Please select an exercise.");

                return;
            }

            DialogResult = true;

            Close();
        }
    }
}