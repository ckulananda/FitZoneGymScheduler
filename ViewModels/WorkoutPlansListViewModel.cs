using FitZoneGymScheduler.Commands;
using FitZoneGymScheduler.Views;
using FitZoneGymScheduler.DAL;
using FitZoneGymScheduler.Helpers;
using FitZoneGymScheduler.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace FitZoneGymScheduler.ViewModels
{
    public class WorkoutPlansListViewModel : BaseViewModel
    {
        private readonly AppDbContext _context;

        // =====================================
        // WORKOUT PLANS
        // =====================================

        public ObservableCollection<WorkoutPlan> WorkoutPlans
        {
            get;
            set;
        }

        // =====================================
        // SELECTED PLAN /Propeties
        // =====================================

        private WorkoutPlan _selectedPlan;

        public WorkoutPlan SelectedPlan
        {
            get => _selectedPlan;

            set
            {
                _selectedPlan = value;

                OnPropertyChanged();
            }
        }


        private int? _editingPlanId;

        public int? EditingPlanId
        {
            get => _editingPlanId;
            set
            {
                _editingPlanId = value;
                OnPropertyChanged();
            }
        }

        public bool IsEditMode
        {
            get => EditingPlanId.HasValue;
        }

        // =====================================
        // COMMANDS
        // =====================================

        public ICommand RefreshCommand { get; }

        public ICommand OpenPlanCommand { get; }

        public ICommand EditPlanCommand { get; }

        public ICommand DeletePlanCommand { get; }

        public ICommand DuplicatePlanCommand { get; }

        // =====================================
        // CONSTRUCTOR
        // =====================================

        public WorkoutPlansListViewModel()
        {
            _context =
                new AppDbContext();

            WorkoutPlans =
                new ObservableCollection<WorkoutPlan>();

            RefreshCommand =
                new RelayCommand(LoadPlans);

            LoadPlans(null);

            OpenPlanCommand =
    new RelayCommand(OpenPlan);

            EditPlanCommand =
                new RelayCommand(EditPlan);

            DeletePlanCommand =
                new RelayCommand(DeletePlan);

            DuplicatePlanCommand =
                new RelayCommand(DuplicatePlan);
        }

        // =====================================
        // LOAD PLANS
        // =====================================

        private void LoadPlans(object obj)
        {
            WorkoutPlans.Clear();

            var plans =
                _context.WorkoutPlans

                .Include(x => x.Member)

                .OrderByDescending(x => x.CreatedDate)

                .ToList();

            foreach (var plan in plans)
            {
                WorkoutPlans.Add(plan);
            }


        }

        private void OpenPlan(object obj)
        {
            if (obj is not WorkoutPlan plan)
                return;

            var workoutView =
                new WorkoutPlansView();

            if (workoutView.DataContext
                is WorkoutPlansViewModel vm)
            {
                vm.LoadPlan(plan.Id);
            }

            var window =
                System.Windows.Application
                .Current
                .MainWindow;

            if (window.DataContext
                is MainViewModel mainVm)
            {
                mainVm.CurrentView = workoutView;
            }
        }



        private void EditPlan(object obj)
        {

        }

        private void DeletePlan(object obj)
        {
            if (obj is not WorkoutPlan plan)
                return;

            var result =
                MessageBox.Show(
                    $"Delete '{plan.PlanName}' ?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            _context.WorkoutPlans.Remove(plan);

            _context.SaveChanges();

            WorkoutPlans.Remove(plan);

            MessageBox.Show(
                "Workout Plan Deleted Successfully.",
                "Success",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        //------------------------------------------

        private void DuplicatePlan(object obj)
        {
            if (obj is not WorkoutPlan plan)
                return;

            var original =
                _context.WorkoutPlans
                .Include(x => x.WorkoutDays)
                    .ThenInclude(d => d.WorkoutSections)
                        .ThenInclude(s => s.WorkoutExercises)
                .FirstOrDefault(x => x.Id == plan.Id);

            if (original == null)
                return;

            var copy =
                new WorkoutPlan
                {
                    MemberId = original.MemberId,
                    PlanName = original.PlanName + " Copy",
                    Goal = original.Goal,
                    Difficulty = original.Difficulty,
                    Notes = original.Notes,
                    CreatedDate = DateTime.Now,
                    WorkoutDays = new List<WorkoutDay>()
                };

            foreach (var day in original.WorkoutDays)
            {
                var newDay =
                    new WorkoutDay
                    {
                        DayNumber = day.DayNumber,
                        IsRestDay = day.IsRestDay,
                        WorkoutSections = new List<WorkoutSection>()
                    };

                foreach (var section in day.WorkoutSections)
                {
                    var newSection =
                        new WorkoutSection
                        {
                            SectionName = section.SectionName,
                            WorkoutExercises =
                                new List<WorkoutExercise>()
                        };

                    foreach (var exercise in section.WorkoutExercises)
                    {
                        newSection.WorkoutExercises.Add(
                            new WorkoutExercise
                            {
                                ExerciseLibraryId =
                                    exercise.ExerciseLibraryId,

                                Sets =
                                    exercise.Sets,

                                RepsOrDuration =
                                    exercise.RepsOrDuration,

                                Notes =
                                    exercise.Notes
                            });
                    }

                    newDay.WorkoutSections.Add(newSection);
                }

                copy.WorkoutDays.Add(newDay);
            }

            _context.WorkoutPlans.Add(copy);

            _context.SaveChanges();

            LoadPlans(null);
        }
    }
}