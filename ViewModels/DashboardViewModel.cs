using FitZoneGymScheduler.DAL;
using FitZoneGymScheduler.Models;
using FitZoneGymScheduler.Services;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.EntityFrameworkCore;
using SkiaSharp;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using FitZoneGymScheduler.Services;

namespace FitZoneGymScheduler.ViewModels
{


    public class DashboardViewModel : BaseViewModel
    {

        public string CurrentUserName => UserSession.CurrentUser?.FullName;

        public string CurrentUserRole => UserSession.CurrentUser?.Role;

        public string ProfilePicture => UserSession.CurrentUser?.ProfilePicturePath;

        public DateTime LoginTime => UserSession.LoginTime;
        private readonly AppDbContext _context;


        private readonly System.Timers.Timer _sessionTimer;
        private string _sessionDuration;

        public string SessionDuration
        {
            get => _sessionDuration;
            set => SetProperty(ref _sessionDuration, value);
        }




        public DashboardViewModel()
        {


            _context = new AppDbContext();

            RecentMembers = new ObservableCollection<Member>();
            RecentWorkoutPlans = new ObservableCollection<WorkoutPlan>();

            _sessionTimer = new System.Timers.Timer(1000);
            _sessionTimer.Elapsed += UpdateSessionTimer;
            _sessionTimer.Start();

            _ = LoadDashboardAsync();
        }

        // ======================================================
        // LOADING STATE
        // ======================================================

        public string MembersTrendText { get; set; }
        public string ActiveTrendText { get; set; }
        public string PlansTrendText { get; set; }
        public string ExerciseTrendText { get; set; }

        public string InsightText { get; set; }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        // ======================================================
        // DASHBOARD STATISTICS
        // ======================================================

        private int _totalMembers;
        public int TotalMembers
        {
            get => _totalMembers;
            set => SetProperty(ref _totalMembers, value);
        }

        private int _activeMembers;
        public int ActiveMembers
        {
            get => _activeMembers;
            set => SetProperty(ref _activeMembers, value);
        }

        private int _totalWorkoutPlans;
        public int TotalWorkoutPlans
        {
            get => _totalWorkoutPlans;
            set => SetProperty(ref _totalWorkoutPlans, value);
        }

        private int _totalExercises;
        public int TotalExercises
        {
            get => _totalExercises;
            set => SetProperty(ref _totalExercises, value);
        }

        // ======================================================
        // RECENT MEMBERS
        // ======================================================

        private ObservableCollection<Member> _recentMembers;
        public ObservableCollection<Member> RecentMembers
        {
            get => _recentMembers;
            set => SetProperty(ref _recentMembers, value);
        }

        // ======================================================
        // RECENT WORKOUT PLANS
        // ======================================================

        private ObservableCollection<WorkoutPlan> _recentWorkoutPlans;
        public ObservableCollection<WorkoutPlan> RecentWorkoutPlans
        {
            get => _recentWorkoutPlans;
            set => SetProperty(ref _recentWorkoutPlans, value);
        }

        // ======================================================
        // LIVECHARTS SERIES
        // ======================================================

        private ISeries[] _memberGrowthSeries;
        public ISeries[] MemberGrowthSeries
        {
            get => _memberGrowthSeries;
            set => SetProperty(ref _memberGrowthSeries, value);
        }

        private ISeries[] _workoutPlanSeries;
        public ISeries[] WorkoutPlanSeries
        {
            get => _workoutPlanSeries;
            set => SetProperty(ref _workoutPlanSeries, value);
        }

        private ISeries[] _exerciseSeries;
        public ISeries[] ExerciseSeries
        {
            get => _exerciseSeries;
            set => SetProperty(ref _exerciseSeries, value);
        }

        private Axis[] _xAxis;
        public Axis[] XAxis
        {
            get => _xAxis;
            set => SetProperty(ref _xAxis, value);
        }

        private Axis[] _yAxis;
        public Axis[] YAxis
        {
            get => _yAxis;
            set => SetProperty(ref _yAxis, value);
        }



        // ======================================================
        // LOAD DASHBOARD
        // ======================================================

        private async Task LoadDashboardAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = null;

                await LoadStatisticsAsync();
                await LoadRecentDataAsync();
                await LoadChartsAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            finally
            {
                IsLoading = false;
            }
        }

        // ======================================================
        // LOAD STATISTICS
        // ======================================================

        private async Task LoadStatisticsAsync()
        {
            TotalMembers =
                await _context.Members.CountAsync();

            ActiveMembers =
                await _context.Members.CountAsync();

            TotalWorkoutPlans =
                await _context.WorkoutPlans.CountAsync();

            TotalExercises =
                await _context.ExerciseLibraries.CountAsync();
        }

        // ======================================================
        // LOAD RECENT DATA
        // ======================================================

        private async Task LoadRecentDataAsync()
        {
            var members = await _context.Members
                .OrderByDescending(m => m.JoinDate)
                .Take(5)
                .ToListAsync();

            var plans = await _context.WorkoutPlans
                .OrderByDescending(p => p.CreatedDate)
                .Take(5)
                .ToListAsync();

            RecentMembers =
                new ObservableCollection<Member>(members);

            RecentWorkoutPlans =
                new ObservableCollection<WorkoutPlan>(plans);
        }

        // ======================================================
        // LOAD CHARTS
        // ======================================================

        private async Task LoadChartsAsync()
        {
            await Task.Run(() =>
            {
                LoadMemberGrowthChart();
                LoadWorkoutPlanChart();
                LoadExerciseChart();

                XAxis = new Axis[]
                {
                    new Axis
                    {
                        LabelsRotation = 15
                    }
                };

                YAxis = new Axis[]
                {
                    new Axis()
                };
            });
        }

        // ======================================================
        // MEMBER GROWTH CHART (LINE)
        // ======================================================

        private void LoadMemberGrowthChart()
        {
            var data = _context.Members
                .ToList()
                .GroupBy(x => x.JoinDate.ToString("MMM"))
                .Select(g => new
                {
                    Month = g.Key,
                    Count = g.Count()
                })
                .ToList();

            MemberGrowthSeries = new ISeries[]
            {
                new LineSeries<int>
                {
                    Values = data.Select(x => x.Count).ToList(),
                    Name = "Members",
                    Stroke = new SolidColorPaint(SKColors.Red, 3),
                    Fill = null
                }
            };
        }

        // ======================================================
        // WORKOUT PLAN DISTRIBUTION (PIE)
        // ======================================================

        private void LoadWorkoutPlanChart()
        {
            var data = _context.WorkoutPlans
                .ToList()
                .GroupBy(x => x.Goal)
                .Select(g => new
                {
                    Goal = g.Key,
                    Count = g.Count()
                })
                .ToList();

            WorkoutPlanSeries = data
                .Select(x => new PieSeries<int>
                {
                    Name = x.Goal,
                    Values = new[] { x.Count }
                })
                .ToArray();
        }

        // ======================================================
        // EXERCISE DISTRIBUTION (COLUMN)
        // ======================================================

        private void LoadExerciseChart()
        {
            var data = _context.ExerciseLibraries
                .ToList()
                .GroupBy(x => x.TargetArea)
                .Select(g => new
                {
                    Area = g.Key,
                    Count = g.Count()
                })
                .ToList();

            ExerciseSeries = new ISeries[]
            {
                new ColumnSeries<int>
                {
                    Values = data.Select(x => x.Count).ToList(),
                    Name = "Exercises"
                }
            };
        }

        // ======================================================
        // REFRESH DASHBOARD
        // ======================================================

        public async Task RefreshAsync()
        {
            await LoadDashboardAsync();
        }


        private void UpdateSessionTimer(object sender, System.Timers.ElapsedEventArgs e)
        {
            var duration = DateTime.Now - UserSession.LoginTime;

            SessionDuration =
                $"{duration.Hours:D2}:{duration.Minutes:D2}:{duration.Seconds:D2}";
        }



    }


}