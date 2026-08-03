using FitZoneGymScheduler.Commands;
using FitZoneGymScheduler.DAL;
using FitZoneGymScheduler.Models;
using FitZoneGymScheduler.Views;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

using FitZoneGymScheduler.Services;
using FitZoneGymScheduler.Helpers;


namespace FitZoneGymScheduler.ViewModels
{
    public class WorkoutPlansViewModel : BaseViewModel
    {




        private readonly AppDbContext _context;

        // MEMBERS

        public ObservableCollection<Member> Members
        {
            get;
            set;
        }

        // DAYS

        public ObservableCollection<WorkoutDayItem> WorkoutDays
        {
            get;
            set;
        }

        // SELECTED MEMBER

        private Member _selectedMember;

        public Member SelectedMember
        {
            get => _selectedMember;

            set
            {
                _selectedMember = value;

                OnPropertyChanged();
            }
        }

        // PLAN INFO

        private string _planName;

        public string PlanName
        {
            get => _planName;

            set
            {
                _planName = value;
                OnPropertyChanged();
            }
        }

        private string _goal;

        public string Goal
        {
            get => _goal;

            set
            {
                _goal = value;
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

        public ObservableCollection<string> DifficultyLevels { get; } =
    new ObservableCollection<string>
    {
        "Beginner",
        "Intermediate",
        "Advanced"
    };





        // COMMANDS

        public ICommand AddDayCommand { get; }

        public ICommand SavePlanCommand { get; }

        public ICommand ViewPdfPreviewCommand { get; }

        public ICommand SendWhatsAppCommand { get; }

        public ICommand OpenMemberSelectorCommand { get; }
        public ICommand OpenExerciseSelectorCommand { get; }







        public WorkoutPlansViewModel()
        {
            _context = new AppDbContext();

            Members =
                new ObservableCollection<Member>();

            WorkoutDays =
                new ObservableCollection<WorkoutDayItem>();

            AddDayCommand =
                new RelayCommand(AddDay);

            SavePlanCommand =
                new RelayCommand(SavePlan);

            ViewPdfPreviewCommand =
                new RelayCommand(ViewPdfPreview);

            SendWhatsAppCommand =
                new RelayCommand(SendWhatsApp);

            OpenMemberSelectorCommand =
                new RelayCommand(OpenMemberSelector);

            LoadMembers();
        }

        private void OpenMemberSelector()
        {
            var window =
                new MemberSelectorWindow(
                    Members.ToList());

            if (window.ShowDialog() == true)
            {
                SelectedMember = window.SelectedMember;

                OnPropertyChanged(nameof(SelectedMember));
            }
        }


        public void LoadPlan(int planId)
        {
            EditingPlanId = planId;

            WorkoutDays.Clear();

            var plan =
                _context.WorkoutPlans

                .Include(x => x.WorkoutDays)
                    .ThenInclude(d => d.WorkoutSections)
                        .ThenInclude(s => s.WorkoutExercises)
                            .ThenInclude(e => e.ExerciseLibrary)

                .FirstOrDefault(x => x.Id == planId);

            if (plan == null)
                return;

            PlanName = plan.PlanName;
            Goal = plan.Goal;
            Difficulty = plan.Difficulty;
            Notes = plan.Notes;

            SelectedMember =
                Members.FirstOrDefault(
                    x => x.Id == plan.MemberId);

            // LOAD DAYS

            foreach (var dbDay in plan.WorkoutDays.OrderBy(x => x.DayNumber))
            {
                var dayVm =
                    new WorkoutDayItem
                    {
                        DayNumber = dbDay.DayNumber,
                        IsRestDay = dbDay.IsRestDay,
                        ParentCollection = WorkoutDays
                    };

                // LOAD SECTIONS

                foreach (var dbSection in dbDay.WorkoutSections)
                {
                    var sectionVm =
                        new WorkoutSectionItem
                        {
                            SectionName =
                                dbSection.SectionName
                        };

                    // LOAD EXERCISES

                    foreach (var dbExercise in dbSection.WorkoutExercises)
                    {
                        sectionVm.Exercises.Add(
                            new WorkoutExerciseItem
                            {
                                SelectedExercise =
                                    dbExercise.ExerciseLibrary,

                                ExerciseName =
                                    dbExercise.ExerciseLibrary?.ExerciseName,

                                Sets =
                                    dbExercise.Sets,

                                RepsOrDuration =
                                    dbExercise.RepsOrDuration,

                                Notes =
                                    dbExercise.Notes
                            });
                    }

                    dayVm.Sections.Add(sectionVm);
                }

                WorkoutDays.Add(dayVm);
            }
        }


        // validation


        
       private bool ValidateWorkoutPlan()
        {
            if (SelectedMember == null)
            {
                DialogService.Warning(
                    "Member Required",
                    "Please select a member before saving the workout plan.");

                return false;
            }

            if (string.IsNullOrWhiteSpace(PlanName))
            {
                DialogService.Warning(
                    "Plan Name Required",
                    "Please enter a workout plan name.");

                return false;
            }

            if (string.IsNullOrWhiteSpace(Goal))
            {
                DialogService.Warning(
                    "Goal Required",
                    "Please specify the training goal.");

                return false;
            }

            if (string.IsNullOrWhiteSpace(Difficulty))
            {
                DialogService.Warning(
                    "Difficulty Required",
                    "Please select a difficulty level.");

                return false;
            }

            if (WorkoutDays.Count == 0)
            {
                DialogService.Warning(
                    "Workout Days Required",
                    "Please add at least one workout day.");

                return false;
            }

            foreach (var day in WorkoutDays)
            {
                if (!day.IsRestDay && day.Sections.Count == 0)
                {
                    DialogService.Warning(
                        "Incomplete Day",
                        $"Day {day.DayNumber} does not contain any workout sections.");

                    return false;
                }

                foreach (var section in day.Sections)
                {
                    if (string.IsNullOrWhiteSpace(section.SectionName))
                    {
                        DialogService.Warning(
                            "Section Name Required",
                            $"Day {day.DayNumber} contains a section without a name.");

                        return false;
                    }

                    if (section.Exercises.Count == 0)
                    {
                        DialogService.Warning(
                            "Exercises Required",
                            $"Section '{section.SectionName}' in Day {day.DayNumber} contains no exercises.");

                        return false;
                    }

                    foreach (var exercise in section.Exercises)
                    {
                        if (exercise.SelectedExercise == null)
                        {
                            DialogService.Warning(
                                "Exercise Required",
                                $"Please select an exercise in section '{section.SectionName}'.");

                            return false;
                        }

                        if (exercise.Sets <= 0)
                        {
                            DialogService.Warning(
                                "Invalid Sets",
                                $"'{exercise.ExerciseName}' must have at least 1 set.");

                            return false;
                        }

                        if (string.IsNullOrWhiteSpace(exercise.RepsOrDuration))
                        {
                            DialogService.Warning(
                                "Reps / Duration Required",
                                $"Please enter reps or duration for '{exercise.ExerciseName}'.");

                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private void ClearForm()
        {
            SelectedMember = null;
            PlanName = string.Empty;
            Goal = string.Empty;
            Difficulty = string.Empty;
            Notes = string.Empty;

            WorkoutDays.Clear();
            EditingPlanId = null;
        }

        private bool ConfirmDiscardChanges()
        {
            return DialogService.Confirm(
                "Unsaved Changes",
                "You have unsaved changes. Are you sure you want to leave this page?");
        }





        private void LoadMembers()
        {
            var members =
                _context.Members.ToList();

            foreach (var member in members)
            {
                Members.Add(member);
            }
        }

        private void AddDay(object obj)
        {
            if (WorkoutDays.Count >= 4)
            {
                DialogService.Warning(
                    "Maximum Days Reached",
                    "You can add a maximum of 4 workout days to a plan.");

                return;
            }

            var day = new WorkoutDayItem
            {
                DayNumber = WorkoutDays.Count + 1,
                ParentCollection = WorkoutDays
            };

            WorkoutDays.Add(day);

            DialogService.Success(
                "Day Added",
                $"Workout Day {day.DayNumber} has been added successfully.");
        }



        private void SavePlan(object obj)
        {
            try
            {
                if (!ValidateWorkoutPlan())
                    return;

                WorkoutPlan workoutPlan;

                if (EditingPlanId.HasValue)
                {
                    workoutPlan = _context.WorkoutPlans
                        .FirstOrDefault(x => x.Id == EditingPlanId.Value);

                    if (workoutPlan == null)
                    {
                        DialogService.Error(
                            "Error",
                            "Workout plan not found.");

                        return;
                    }

                    var oldDays = _context.WorkoutDays
                        .Include(d => d.WorkoutSections)
                            .ThenInclude(s => s.WorkoutExercises)
                        .Where(x => x.WorkoutPlanId == workoutPlan.Id)
                        .ToList();

                    foreach (var day in oldDays)
                    {
                        foreach (var section in day.WorkoutSections)
                        {
                            _context.WorkoutExercises.RemoveRange(
                                section.WorkoutExercises);
                        }

                        _context.WorkoutSections.RemoveRange(
                            day.WorkoutSections);
                    }

                    _context.WorkoutDays.RemoveRange(oldDays);
                }
                else
                {
                    workoutPlan = new WorkoutPlan();

                    _context.WorkoutPlans.Add(workoutPlan);
                }

                workoutPlan.MemberId = SelectedMember.Id;
                workoutPlan.PlanName = PlanName.Trim();
                workoutPlan.Goal = Goal.Trim();
                workoutPlan.Difficulty = Difficulty;
                workoutPlan.Notes = string.IsNullOrWhiteSpace(Notes)
                    ? "No notes"
                    : Notes.Trim();

                workoutPlan.CreatedDate = DateTime.Now;

                foreach (var dayVm in WorkoutDays)
                {
                    var workoutDay = new WorkoutDay
                    {
                        WorkoutPlan = workoutPlan,
                        DayNumber = dayVm.DayNumber,
                        IsRestDay = dayVm.IsRestDay,
                        WorkoutSections = new List<WorkoutSection>()
                    };

                    foreach (var sectionVm in dayVm.Sections)
                    {
                        var workoutSection = new WorkoutSection
                        {
                            SectionName = sectionVm.SectionName,
                            WorkoutExercises = new List<WorkoutExercise>()
                        };

                        foreach (var exerciseVm in sectionVm.Exercises)
                        {
                            if (exerciseVm.SelectedExercise == null)
                                continue;

                            workoutSection.WorkoutExercises.Add(
                                new WorkoutExercise
                                {
                                    ExerciseLibraryId =
                                        exerciseVm.SelectedExercise.Id,

                                    Sets =
                                        exerciseVm.Sets,

                                    RepsOrDuration =
                                        exerciseVm.RepsOrDuration,

                                    Notes =
                                        string.IsNullOrWhiteSpace(exerciseVm.Notes)
                                            ? "No notes"
                                            : exerciseVm.Notes.Trim()
                                });
                        }

                        workoutDay.WorkoutSections.Add(workoutSection);
                    }

                    _context.WorkoutDays.Add(workoutDay);
                }

                _context.SaveChanges();

                DialogService.Success(
                    EditingPlanId.HasValue
                        ? "Workout Plan Updated"
                        : "Workout Plan Created",

                    EditingPlanId.HasValue
                        ? "Workout plan updated successfully."
                        : "Workout plan created successfully.");

                ClearForm();
            }
            catch (DbUpdateException ex)
            {
                DialogService.Error(
                    "Database Error",
                    ex.InnerException?.Message ?? ex.Message);
            }
            catch (Exception ex)
            {
                DialogService.Error(
                    "Unexpected Error",
                    ex.Message);
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



        private void ViewPdfPreview(object obj)
        {
            var preview =
                new PdfPreviewWindow(this);

            preview.ShowDialog();
        }

        private void SendWhatsApp(object obj)
        {
            DialogService.Warning(
                "Coming Soon",
                "WhatsApp Export feature will be available in a future update.");
        }


        private void ChangeExercise_Click(object sender, RoutedEventArgs e)
        {
            var selector = new ExerciseSelectorWindow();

            if (selector.ShowDialog() == true)
            {
                // update selected exercise here
            }
        }
    }




}